using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using SevenZipExtractor;
using static SevenZipSpeedOptimizer.Logger;

namespace SevenZipSpeedOptimizer;

internal static class Core
{
    internal static MainForm View = null!;

    private const string AL_Scan_Block_ListFileName = "list_al_scan.lst";
    private const string Rest_Block_ListFileName = "list_rest.lst";

    internal static void Init()
    {
        View = new MainForm();

        ConfigIni.ReadConfigData();

        View.SetMode(Config.Mode);
        View.SetCompressionLevel(Config.CompressionLevel);
        View.SetCompressionMethod(Config.CompressionMethod);
        View.SetThreads(Config.Threads);

        View.Show();
    }

    internal static void Shutdown()
    {
        ConfigIni.WriteConfigData();
        Application.Exit();
    }

    [MustUseReturnValue]
    private static RunProcessResult RunProcess(
    string sourcePath,
    string outputArchive,
    string listFile,
    int level,
    CompressionMethod method,
    CancellationToken cancellationToken)
    {
        Progress<Fen7z.ProgressReport> progress = new(ReportProgress);

        Fen7z.Result result = Fen7z.Compress(
            sevenZipPathAndExe: Paths.SevenZipExe,
            sourcePath: sourcePath,
            outputArchive: outputArchive,
            args: GetArgs(level, method),
            cancellationToken: cancellationToken,
            listFile: listFile,
            progress: progress
        );

        if (result.ErrorOccurred)
        {
            Log("7z.exe (Compress mode) returned an error:" + $"{NL}" + result);
            string msg = "Failed to create archive '" + outputArchive + "'. See log for details.";
            return new RunProcessResult(false, outputArchive, msg, result.Exception);
        }
        else if (result.Canceled)
        {
            throw new OperationCanceledException(_cts.Token);
        }

        return new RunProcessResult(true, outputArchive, "", null);

        static void ReportProgress(Fen7z.ProgressReport pr)
        {
            if (!pr.Canceling)
            {
                View.SetProgressPercent(pr.CompressPercent);
            }
        }
    }

    [MustUseReturnValue]
    private static RunProcessResult RunProcess_Rename(
        string sourcePath,
        string outputArchive,
        ListFileData listFileData,
        List<FileToRename> itemsToRename,
        int level,
        CompressionMethod method,
        CancellationToken cancellationToken)
    {
        foreach (var item in itemsToRename)
        {
            /*
            7z.exe doesn't report an error or any fail state when trying to rename a file that doesn't exist;
            it simply doesn't rename anything and reports success. So we need to run a verify pass afterwards to
            make sure we don't have our temp-named files still in there.
            */
            Fen7z.Result result = Fen7z.Rename(
                sevenZipPathAndExe: Paths.SevenZipExe,
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                originalFileName: item.TempSortedName,
                newFileName: item.Name,
                args: GetArgs(level, method),
                cancellationToken: cancellationToken
            );

            if (result.ErrorOccurred)
            {
                Log("7z.exe (Rename mode) returned an error:" + $"{NL}" + result);
                string msg = "Failed to finalize creation of archive '" + outputArchive +
                             "'. The archive may appear complete but certain file names may be wrong. See log for details.";
                return new RunProcessResult(false, outputArchive, msg, result.Exception);
            }
            else if (result.Canceled)
            {
                throw new OperationCanceledException(_cts.Token);
            }
        }

        View.SetProgressMessage("Verifying archive...");
        View.SetProgressPercent(100);

        if (!ArchiveVerificationSucceeded(outputArchive, listFileData, out List<string> filesInArchive, out List<string> filesOnDisk))
        {
            string filesStr = "Files on disk:" + $"{NL}" +
                              "--------------" + $"{NL}";
            foreach (string file in filesOnDisk)
            {
                filesStr += file + $"{NL}";
            }
            filesStr += $"{NL}";
            filesStr += "Files in archive:" + $"{NL}" +
                        "--------------" + $"{NL}";
            foreach (string file in filesInArchive)
            {
                filesStr += file + $"{NL}";
            }

            Log("Archive verification failed: file name set is different than source. Reverting of temp rename could possibly have failed. " +
                "The archive may appear complete but certain file names may be wrong." +
                $"{NL}" +
                filesStr);
            const string msg =
                "Archive verification failed: file name set is different than source. " +
                "Reverting of temp rename could possibly have failed. The archive may appear complete but certain file names may be wrong. See log for details.";
            return new RunProcessResult(false, outputArchive, msg, null);
        }

        return new RunProcessResult(true, outputArchive, "", null);

        // TODO: Also check explicitly for existence of un-renamed items from the renameable items list, so we can
        //  report that with certainly if it happens, rather than just saying "maybe it's this".
        static bool ArchiveVerificationSucceeded(string outputArchive, ListFileData listFileData, out List<string> filesInArchive, out List<string> filesOnDisk)
        {
            // Use SevenZipExtractor as it's a wrapper around 7z.dll so we can get the names straight out of the
            // dll to guarantee correctness as much as we can.
            using ArchiveFile archive = new(outputArchive, libraryFilePath: Paths.SevenZipDll);

            filesInArchive = new List<string>();
            foreach (Entry entry in archive.Entries)
            {
                filesInArchive.Add(entry.FileName.ToBackSlashes());
            }

            filesOnDisk = new List<string>();
            for (int i = 0; i < listFileData.OriginalFileNamesLeadingPathRemoved.Count; i++)
            {
                filesOnDisk.Add(listFileData.OriginalFileNamesLeadingPathRemoved[i].ToBackSlashes());
            }

            filesInArchive.Sort(StringComparer.OrdinalIgnoreCase);
            filesOnDisk.Sort(StringComparer.OrdinalIgnoreCase);

            if (filesInArchive.Count != filesOnDisk.Count)
            {
                return false;
            }
            for (int i = 0; i < filesInArchive.Count; i++)
            {
                string fileInArchive = filesInArchive[i];
                string fileOnDisk = filesOnDisk[i];
                if (!fileInArchive.PathEqualsI(fileOnDisk))
                {
                    return false;
                }
            }

            return true;
        }
    }

    /*
    -Put every file in its own block - that way AL can do extract cost calculations and really just get the first
     used .mis file.
    -Use a list file even when there's only one entry in it, to prevent any filename encoding issues.
    */
    [MustUseReturnValue]
    internal static RunProcessResult Run7z_ALScanFiles(
        string sourcePath,
        string outputArchive,
        ListFileData listFileData,
        int level,
        CompressionMethod method,
        CancellationToken cancellationToken)
    {
        string listFile = Path.Combine(Paths.Temp, AL_Scan_Block_ListFileName);

        RunProcessResult result;

        if (listFileData.Readmes.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding readmes to their own block...");

            File.WriteAllLines(listFile, listFileData.Readmes);

            result = RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
            if (!result.Success)
            {
                return result;
            }
        }

        if (listFileData.Thumbs.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding thumbs to their own block...");

            File.WriteAllLines(listFile, listFileData.Thumbs);

            result = RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
            if (!result.Success)
            {
                return result;
            }
        }

        if (listFileData.MainImages.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding \"main_\" images to their own block...");

            File.WriteAllLines(listFile, listFileData.MainImages);

            result = RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
            if (!result.Success)
            {
                return result;
            }
        }

        if (listFileData.GamFiles.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding .gam files to their own block...");

            File.WriteAllLines(listFile, listFileData.GamFiles);

            result = RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
            if (!result.Success)
            {
                return result;
            }
        }

        for (int i = 0; i < listFileData.OnePerBlockItems.Count; i++)
        {
            string fileName = listFileData.OnePerBlockItems[i];

            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding " + fileName + " to its own block...");

            using (var sw = new StreamWriter(listFile, false, UTF8NoBOM))
            {
                sw.WriteLine(fileName);
            }

            result = RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
            if (!result.Success)
            {
                return result;
            }
        }

        return new RunProcessResult(true, outputArchive, "", null);
    }

    [MustUseReturnValue]
    internal static RunProcessResult Run7z_Rest(
        string sourcePath,
        string outputArchive,
        string listFile,
        ListFileData listFileData,
        int level,
        CompressionMethod method,
        CancellationToken cancellationToken)
    {
        View.SetProgressMessage("Adding remaining files to archive...");
        View.SetProgressPercent(0);

        RunProcessResult result = RunProcess(
            sourcePath: sourcePath,
            outputArchive: outputArchive,
            listFile: listFile,
            level: level,
            method: method,
            cancellationToken: cancellationToken);
        if (!result.Success)
        {
            return result;
        }

        if (listFileData.FilesToRename.Count > 0)
        {
            View.SetProgressMessage("Finalizing archive...");
            View.SetProgressPercent(100);

            return RunProcess_Rename(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFileData,
                listFileData.FilesToRename,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
        }

        return new RunProcessResult(true, outputArchive, "", null);
    }

    internal static string GetArgs(int level, CompressionMethod method)
    {
        if (level is < 0 or > 9)
        {
            level = 5;
        }

        /*
        -mx   = Compression level 0-9
        -m0   = You put a number for poorly explained reasons (0 here), then you put the actual compression method
                as a string
        -myv  = Don't use filters not supported by this version and below. 4 digits, ex. 1600 for 16.00
        -mhc  = Enable or disable header compression
        -mqs  = Enable or disable sorting by file types in archive. Off = sort by name only.
        -y    = Say yes to all prompts automatically
        -r    = Recurse directories - basic thing required to put the files into the archive keeping the folder
                structure
        -bsp1 = Redirect progress information to stdout stream
        -bb1  = Show names of processed files in log (needed for smooth reporting of entries done count)
        -sas  = Add extension only if specified name has no extension. It's default option.
        -scs  = Set charset for list files (default is UTF-8, but let's be explicit)
        -t7z  = Explicitly set archive type to 7z
        */
        string args = "-mx=" + level;
        if (level == 0)
        {
            args += " -m0=Copy";
        }
        else
        {
            args += " -m0=" + GetCompressionMethodString(method);
        }

        /*
        Compatibility all the way back to FMSel with its v9.20. Of course it's highly unlikely any new filters
        will trigger as nobody will be putting ARM64 or RISC-V executables in their FMs, but hey.
        */
        args += " -myv=0920";

        int threads = Config.Threads;
        if (threads > -1)
        {
            Trace.WriteLine("********** threads non-default: " + threads);

            args += " -mmt=" + threads.ToStrInv();
        }
        args += " -mhc=off -mqs=off";

        args += " -y -r -bsp1 -bb1 -sas -t7z -scsUTF-8";

        return args;
    }

    private static CancellationTokenSource _cts = new();
    internal static void CancelToken()
    {
        View.SetProgressMessage("Canceling...");
        _cts.CancelIfNotDisposed();
    }

    internal static async Task CreateSingleArchive()
    {
        string sourcePath = View.SourceFMPath;
        string outputArchive = View.OutputArchive;

        if (sourcePath.IsEmpty() || outputArchive.IsEmpty())
        {
            MessageBox.Show(
                "You must specify both a source FM directory and an output archive.",
                "Alert",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        await Task.Run(() =>
        {
            if (File.Exists(sourcePath))
            {
                Log("Source path was a file.");
                View.ShowError("Source path was a file. Please set source path to a directory.");
                return;
            }

            try
            {
                View.StartCreateSingleArchiveOperation();

                _cts = _cts.Recreate();

                Paths.CreateOrClearTempPath(Paths.TempPaths.Base);

                if (File.Exists(outputArchive))
                {
                    (MBoxButton buttonPressed, _) = View.ShowMultiChoiceDialog(
                        message: "Output archive already exists. Overwrite it?",
                        title: "Alert",
                        icon: MessageBoxIcon.Warning,
                        yes: "Overwrite",
                        no: "Cancel",
                        yesIsDangerous: true,
                        defaultButton: MBoxButton.No);

                    if (buttonPressed != MBoxButton.Yes)
                    {
                        return;
                    }
                }

                try
                {
                    File.Delete(outputArchive);
                }
                catch (Exception ex)
                {
                    string msg = "Unable to overwrite '" + outputArchive + "'.";
                    Log(msg, ex);
                    View.ShowError(msg, "Error");
                    return;
                }

                int level = Config.CompressionLevel;
                CompressionMethod method = Config.CompressionMethod;

                ListFileData listFileData;
                string listFile_Rest;
                try
                {
                    (listFileData, listFile_Rest) = GetListFile(ref sourcePath, makeCopyOfFilesDir: true, _cts.Token);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Log(ex: ex);
                    View.ShowError("Failed to create FM. See log for details.");
                    return;
                }

                RunProcessResult result = Run7z_ALScanFiles(sourcePath, outputArchive, listFileData, level, method, _cts.Token);
                if (!result.Success)
                {
                    View.ShowError(result.Message);
                    return;
                }
                result = Run7z_Rest(sourcePath, outputArchive, listFile_Rest, listFileData, level, method, _cts.Token);
                if (!result.Success)
                {
                    View.ShowError(result.Message);
                    return;
                }

                View.SetProgressPercent(100);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Log(ex: ex);
                View.ShowError("Failed to create FM. See log for details.");
                return;
            }
            finally
            {
                View.SetProgressMessage("Cleaning up...");
                Paths.CreateOrClearTempPath(Paths.TempPaths.Base);
                View.EndCreateSingleArchiveOperation();
                _cts.Dispose();
            }
        });
    }

    internal sealed class RunProcessResult
    {
        internal readonly bool Success;
        internal readonly string FM;
        internal readonly string Message;
        internal readonly Exception? Exception;

        public RunProcessResult(bool success, string fm, string message, Exception? exception)
        {
            Success = success;
            FM = fm;
            Message = message;
            Exception = exception;
        }
    }

    private sealed class RepackBatchError
    {
        internal readonly string SourceArchiveName;
        internal readonly string Message;
        internal readonly Exception? Exception;

        internal RepackBatchError(string sourceArchiveName, string message, Exception? exception)
        {
            SourceArchiveName = sourceArchiveName;
            Message = message;
            Exception = exception;
        }
    }

    internal static async Task RepackBatch()
    {
        string[] sourceArchives = View.Repack_Archives;
        string outputDir = View.Repack_OutputDirectory;

        if (sourceArchives.Length == 0 || outputDir.IsEmpty())
        {
            MessageBox.Show(
                "You must specify an output directory and at least one archive to repack.",
                "Alert",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        await Task.Run(() =>
        {
            List<RepackBatchError> errors = new();

            try
            {
                View.StartRepackBatchOperation();

                _cts = _cts.Recreate();

                Paths.CreateOrClearTempPath(Paths.TempPaths.Base);

                Progress<Fen7z.ProgressReport> progress = new(ReportProgress);

                string tempExtractedDir = Paths.Temp_SourceCopy;
                int level = Config.CompressionLevel;
                CompressionMethod method = Config.CompressionMethod;


                if (File.Exists(outputDir))
                {
                    Log("Output directory was a file.");
                    View.ShowError("Output directory was a file. Please set output directory to a directory.");
                    return;
                }

                Directory.CreateDirectory(outputDir);

                for (int i = 0; i < sourceArchives.Length; i++)
                {
                    string archive = sourceArchives[i];

                    View.SetProgressBatchMainMessage("Repacking '" + archive + "' (" + (i + 1).ToStrInv() + " of" + sourceArchives.Length.ToStrInv() + ")...");

                    try
                    {
                        string extractedDirName = Path.GetFileNameWithoutExtension(archive).Trim();
                        string outputArchive = Path.Combine(outputDir, extractedDirName + ".7z");

                        outputArchive = Utils.GetNonConflictingFileName(outputArchive);

                        Paths.CreateOrClearTempPath(Paths.TempPaths.SourceCopy);

                        View.SetProgressMessage("Unpacking archive '" + archive + "'...");

                        var result = Fen7z.Extract(
                            sevenZipPathAndExe: Paths.SevenZipExe,
                            archivePath: archive,
                            outputPath: tempExtractedDir,
                            cancellationToken: _cts.Token,
                            progress: progress);

                        if (result.ErrorOccurred)
                        {
                            string msg = "7z.exe (Extract mode) returned an error:" + $"{NL}" + result;
                            errors.Add(new RepackBatchError(archive, msg, result.Exception));
                            Log("Archive: " + archive + $"{NL}" + msg);
                            return;
                        }
                        else if (result.Canceled)
                        {
                            throw new OperationCanceledException(_cts.Token);
                        }

                        (ListFileData listFileData, string listFile_Rest) = GetListFile(
                            ref tempExtractedDir,
                            makeCopyOfFilesDir: false,
                            _cts.Token);

                        RunProcessResult run7zResult = Run7z_ALScanFiles(tempExtractedDir, outputArchive, listFileData, level, method, _cts.Token);
                        if (!run7zResult.Success)
                        {
                            errors.Add(new RepackBatchError(archive, run7zResult.Message, run7zResult.Exception));
                            UpdateSubProgress(i, sourceArchives.Length);
                            continue;
                        }
                        run7zResult = Run7z_Rest(tempExtractedDir, outputArchive, listFile_Rest, listFileData, level, method, _cts.Token);
                        if (!run7zResult.Success)
                        {
                            errors.Add(new RepackBatchError(archive, run7zResult.Message, run7zResult.Exception));
                            UpdateSubProgress(i, sourceArchives.Length);
                            continue;
                        }
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        Log("Archive: " + archive, ex: ex);
                        errors.Add(new RepackBatchError(archive, ex.Message, ex));
                    }

                    UpdateSubProgress(i, sourceArchives.Length);
                }

                return;

                static void ReportProgress(Fen7z.ProgressReport pr)
                {
                    if (!pr.Canceling)
                    {
                        View.SetProgressPercent(pr.PercentOfBytes);
                    }
                }

                static void UpdateSubProgress(int i, int total)
                {
                    View.SetProgressBatchMainPercent(Utils.GetPercentFromValue_Int(i + 1, total));
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Log(ex: ex);
                View.ShowError("Failed to repack FM. See log for details.");
            }
            finally
            {
                View.SetProgressMessage("Cleaning up...");
                Paths.CreateOrClearTempPath(Paths.TempPaths.Base);
                View.EndRepackBatchOperation();
                _cts.Dispose();

                if (errors.Count > 0)
                {
                    View.ShowError("There were errors while attempting to repack one or more FMs. See log for details.");
                }
            }
        });
    }

    /*
    For certain types of files, we want to keep them in a multi-file block, but put them at the start of said
    block. The only way to force 7-Zip to do this is by making sure the file you want at the start of the block
    has a name that sorts to the top. So we temporarily rename the appropriate files to ensure this, then rename
    them back to their correct names once they're in the archive.
    */
    internal sealed class NameAndTempName
    {
        internal readonly string Name;
        internal string TempSortedName;
        internal readonly bool IsInBaseDir;
        internal string PhysicalName => !TempSortedName.IsEmpty() ? TempSortedName : Name;

        public NameAndTempName(string name, string tempSortedName, bool isInBaseDir)
        {
            Name = name;
            TempSortedName = tempSortedName;
            IsInBaseDir = isInBaseDir;
        }
    }

    internal sealed class FileToRename
    {
        internal readonly string Name;
        internal readonly string TempSortedName;

        internal static bool TryCreate(NameAndTempName item, [NotNullWhen(true)] out FileToRename? result)
        {
            if (!item.Name.IsEmpty() && !item.TempSortedName.IsEmpty())
            {
                result = new FileToRename(item.Name, item.TempSortedName);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        private FileToRename(string name, string tempSortedName)
        {
            Name = name;
            TempSortedName = tempSortedName;
        }
    }

    internal sealed class ListFileData
    {
        /*
        We get much better compression by putting as many binary files into the same block as possible. This
        includes both .mis and .gam files, and many other types of files in an FM. However, we need both smallest
        used .mis and smallest .gam files to be at the start of their respective blocks, so we can't put them
        both in with the big remaining-files block. We choose to put .mis files in with the remaining-files block
        because they're almost always larger (and usually MUCH larger) than .gam files, and there are more likely
        to be more of them. Thus, a larger amount of data gets packed into the block, resulting in much better
        compression ratios, and greatly lowering the outlier size increases.

        A happy side effect of this logic is that it also puts the smallest used .mis file as the first .mis file
        in the filenames list, so loaders that just run through the entries and grab the first valid .mis file
        they see will also get the one that has instant access. Friggin' score.
        */

        internal readonly List<string> GamFiles = new();
        internal readonly List<string> Readmes = new();
        internal readonly List<string> MainImages = new();
        internal readonly List<string> Thumbs = new();

        internal readonly List<string> OnePerBlockItems = new();

        internal readonly List<FileToRename> FilesToRename = new();

        internal readonly List<string> OriginalFileNamesLeadingPathRemoved = new();
    }

    internal static (ListFileData ListFileData, string RestListFile)
    GetListFile(ref string filesDir, bool makeCopyOfFilesDir, CancellationToken cancellationToken)
    {
        View.SetProgressMessage("Preparing...");

        ListFileData ret = new();

        List<NameAndTempName> misFiles = new();
        List<NameAndTempName> gamFiles = new();

        string[] files;
        try
        {
            files = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            Log("Couldn't get the list of files in '" + filesDir + "'.", ex);
            throw;
        }

        /*
        NOTE: This is slow. We can't just copy our to-be-renamed files either because they need to be in the
         stupid list file with paths relative to the main folder if they want to be in the same block as other
         stuff, which they do.
         We could:
         -Keep this slow copy.
         -Do some crazy garbage with symbolic links (assuming that works).
         -Compile our own version of 7-zip that can take a list of files to sort to the top of their block.
         -Renaming the files in-place is too dangerous no matter how carefully we backup and restore the originals,
          as we'd be messing with a user's actual working FM files, which to me is a hard no.
        */
        if (makeCopyOfFilesDir)
        {
            Paths.CreateOrClearTempPath(Paths.TempPaths.SourceCopy);

            View.SetProgressMessage("Making a copy of source FM directory...");
            View.SetProgressPercent(0);

            string[] sourceFiles = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);
            for (int i = 0; i < sourceFiles.Length; i++)
            {
                string file = sourceFiles[i];
                string destFile = Path.Combine(Paths.Temp_SourceCopy, file.RemoveLeadingPath(filesDir));
                string? dirName = Path.GetDirectoryName(destFile);
                if (!dirName.IsEmpty())
                {
                    Directory.CreateDirectory(dirName);

                    cancellationToken.ThrowIfCancellationRequested();
                }
                File.Copy(file, destFile);

                cancellationToken.ThrowIfCancellationRequested();

                View.SetProgressPercent(Utils.GetPercentFromValue_Int(i + 1, sourceFiles.Length));
            }

            filesDir = Paths.Temp_SourceCopy;
            files = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);
        }

        View.SetProgressMessage("Generating archive packaging logic...");
        View.SetProgressPercent(0);

        HashSet<string> dupePreventionHash = new(StringComparer.OrdinalIgnoreCase);

        List<string> readmeFullFilePaths = new();

        List<string> restFileNames = new();
        for (int i = 0; i < files.Length; i++)
        {
            string fn = files[i].RemoveLeadingPath(filesDir);

            ret.OriginalFileNamesLeadingPathRemoved.Add(fn);

            int dirSeps;
            long fileSize = new FileInfo(files[i]).Length;
            if ((fn.IsValidReadme() || fn.IsGlTitle()) &&
                fileSize > 0 &&
                (((dirSeps = fn.Rel_CountDirSepsUpToAmount(2)) == 1 &&
                  (fn.PathStartsWithI(FMDirs.T3FMExtras1S) ||
                   fn.PathStartsWithI(FMDirs.T3FMExtras2S))) ||
                 dirSeps == 0))
            {
                AddScanFile(fn.IsGlTitle() ? ret.Thumbs : ret.Readmes, fn);
                readmeFullFilePaths.Add(files[i]);
            }
            /*
            FMSel (both original and Sneaky Upgrade version) only look for this file in the root dir, not T3
            readme dir(s), tested.
            Also, FMSel only reads fmthumb.jpg, but there's at least one FM with an fmthumb.png, so why not just
            allow any image type.
            */
            else if (IsInBaseDir(fn) &&
                     Path.GetFileNameWithoutExtension(fn).EqualsI(FMFiles.FMThumb) &&
                     Utils.FileExtensionFound(fn, FMFileExtensions.ImageFileExtensions))
            {
                AddScanFile(ret.Thumbs, fn);
            }
            else if (fn.ExtIsMis())
            {
                AddScanFile_Renameable(misFiles, fn);
            }
            else if (fn.ExtIsGam())
            {
                AddScanFile_Renameable(gamFiles, fn);
            }
            else if (IsInBaseDir(fn) &&
                     (fn.EqualsI(FMFiles.FMInfoXml) ||
                      fn.EqualsI(FMFiles.FMIni) ||
                      fn.EqualsI(FMFiles.ModIni)))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.StringsS) &&
                     fn.PathEndsWithI(FMFiles.SMissFlag))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.IntrfaceS) &&
                     Regex.Match(Path.GetFileNameWithoutExtension(fn), "^main_[0-9]+$",
                         RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success &&
                     Utils.FileExtensionFound(fn, FMFileExtensions.ImageFileExtensions))
            {
                AddScanFile(ret.MainImages, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.IntrfaceS) &&
                     fn.PathEndsWithI(FMFiles.SNewGameStr))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.StringsS) &&
                     EndsWithTitleFile(fn))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else
            {
                restFileNames.Add(fn);
            }
        }

        /*
        Limit the values to the formats they're supposed to be, to keep their hands off any of our temp-renamed
        files
        */
        if (Utils.TryGetInfoFileFromFmIni(filesDir, out string infoFile) && infoFile.IsValidReadme())
        {
            AddIfNotInList(ret.Readmes, infoFile, ref filesDir);
        }

        var modIniValues = Utils.GetValuesFromModIni(filesDir);
        if (!modIniValues.Readme.IsEmpty() && modIniValues.Readme.IsValidReadme())
        {
            AddIfNotInList(ret.Readmes, modIniValues.Readme, ref filesDir);
        }
        if (!modIniValues.Icon.IsEmpty() && modIniValues.Icon.ExtIsIco())
        {
            AddIfNotInList(ret.Thumbs, modIniValues.Icon, ref filesDir);
        }

        List<string> htmlRefFiles = HtmlReference.GetHtmlReferenceFiles(filesDir, readmeFullFilePaths, files);
        foreach (string htmlRefFile in htmlRefFiles)
        {
            // If it's referencing a .mis or .gam file then it will probably be broken by our renaming, so just
            // leave those alone and take whatever hit we take
            if (!htmlRefFile.ExtIsMis() && !htmlRefFile.ExtIsGam())
            {
                AddIfNotInList(ret.OnePerBlockItems, htmlRefFile, ref filesDir);
            }
        }

        #region Temp rename files for sorting

        // If this name doesn't end up sorting at the top then someone's being a troll, so oh well.
        const string sortAtTopPrefix = "!!!!!!!!_";

        if (Utils.TryGetSmallestUsedMisFile(filesDir, out string smallestUsedMisFile))
        {
            AddRenameable(misFiles, smallestUsedMisFile, ref filesDir);
        }

        if (Utils.TryGetSmallestGamFile(filesDir, out string smallestGamFile))
        {
            AddRenameable(gamFiles, smallestGamFile, ref filesDir);
        }

        static void AddRenameable(List<NameAndTempName> potentialRenameables, string targetFileName, ref string filesDir)
        {
            int index = potentialRenameables.FindIndex(x => x.Name.EqualsI(targetFileName));
            Trace.Assert(index > -1);

            NameAndTempName item = potentialRenameables[index];
            Trace.Assert(IsInBaseDir(item.Name));

            string newName = sortAtTopPrefix + item.Name;

            // If there actually is already a file named like "!!!!!!!!_miss21.mis" then someone's being a troll
            // so just give up.
            if (potentialRenameables.Any(x => x.Name.EqualsI(newName)))
            {
                // Manual combine cause we don't want to deal with Path.Combine()'s might-throw bullshit
                string filesDirLeadingPathSafe = filesDir.TrimEnd('/', '\\') + Path.DirectorySeparatorChar;
                string msg = "Tried to rename '" + filesDirLeadingPathSafe + item.Name + "' to '" +
                             filesDirLeadingPathSafe + newName +
                             "' but '" + filesDirLeadingPathSafe + newName + "' already existed.";
                var ex = new InvalidOperationException("Rename failed.");
                Log(msg, ex);
                throw ex;
            }
            else
            {
                item.TempSortedName = newName;

                string origFullName;
                string newFullName;
                try
                {
                    origFullName = Path.Combine(filesDir, item.Name);
                    newFullName = Path.Combine(filesDir, item.TempSortedName);
                }
                catch (Exception ex)
                {
                    Log("Failed to combine paths.", ex);
                    Paths.CreateOrClearTempPath(Paths.TempPaths.Base);
                    throw;
                }

                try
                {
                    File.Move(origFullName, newFullName);
                }
                catch (Exception ex)
                {
                    Log("Failed to rename '" + origFullName + "' to '" + newFullName + "'.", ex);
                    Paths.CreateOrClearTempPath(Paths.TempPaths.Base);
                    throw;
                }
            }
        }

        #endregion

        List<string> finalRestFileNames = new();

        foreach (NameAndTempName item in gamFiles)
        {
            ret.GamFiles.Add(item.PhysicalName);
            if (FileToRename.TryCreate(item, out FileToRename? fileToRename))
            {
                ret.FilesToRename.Add(fileToRename);
            }
        }

        foreach (NameAndTempName item in misFiles)
        {
            finalRestFileNames.Add(item.PhysicalName);
            if (FileToRename.TryCreate(item, out FileToRename? fileToRename))
            {
                ret.FilesToRename.Add(fileToRename);
            }
        }

        finalRestFileNames.AddRange(restFileNames);

        string listFile_Rest = Path.Combine(Paths.Temp, Rest_Block_ListFileName);

        File.WriteAllLines(listFile_Rest, finalRestFileNames);

        return (ret, listFile_Rest);

        void AddScanFile(List<string> list, string fn)
        {
            string valNormalized = fn.ToBackSlashes();
            dupePreventionHash.Add(valNormalized);
            list.Add(valNormalized);
        }

        void AddScanFile_Renameable(List<NameAndTempName> list, string fn)
        {
            dupePreventionHash.Add(fn);
            list.Add(new NameAndTempName(fn, "", IsInBaseDir(fn)));
        }

        void AddIfNotInList(List<string> list, string value, ref string filesDir_)
        {
            string valNormalized = value.ToBackSlashes();
            if (!valNormalized.IsEmpty() &&
                File.Exists(Path.Combine(filesDir_, valNormalized)) &&
                dupePreventionHash.Add(valNormalized))
            {
                restFileNames.Remove(valNormalized);
                list.Add(valNormalized);
            }
        }

        static bool EndsWithTitleFile(string fileName)
        {
            return fileName.PathEndsWithI("/titles.str") ||
                   fileName.PathEndsWithI("/title.str");
        }

        static bool IsInBaseDir(string fn) => !fn.Rel_ContainsDirSep();
    }

    internal static string GetSevenZipVersion(string exe)
    {
        try
        {
            FileVersionInfo vi = FileVersionInfo.GetVersionInfo(exe);

            string versionString = vi.ProductMajorPart.ToStrInv() + "." +
                                   vi.ProductMinorPart.ToStrInv().PadLeft(2, '0');
            if (vi.ProductPrivatePart > 0)
            {
                versionString += "." + vi.ProductBuildPart.ToStrInv() + "." + vi.ProductPrivatePart.ToStrInv();
            }
            if (vi.ProductBuildPart > 0)
            {
                versionString += "." + vi.ProductBuildPart.ToStrInv();
            }

            return versionString;
        }
        catch
        {
            return "";
        }
    }

    internal static bool FilesDropped(object data, [NotNullWhen(true)] out string[]? droppedItems)
    {
        if (View.DragDropEnabled &&
            data is string[] droppedItems_Internal)
        {
            droppedItems = droppedItems_Internal;
            return true;
        }
        else
        {
            droppedItems = null;
            return false;
        }
    }

    internal static List<string> GetStronglyCheckedFiles(string[] droppedItems)
    {
        List<string> tempList = new(droppedItems.Length);
        foreach (string item in droppedItems)
        {
            if (!item.IsWhiteSpace() &&
                item.ExtIsExpectedFMArchiveFormat() &&
                !Directory.Exists(item))
            {
                tempList.Add(item);
            }
        }
        return tempList;
    }

    internal static void OpenLogFile()
    {
        try
        {
            Utils.ProcessStart_UseShellExecute(Paths.LogFile);
        }
        catch
        {
            MessageBox.Show("Unable to open the log file." + $"{NL}{NL}" + Paths.LogFile, "Error");
        }
    }
}
