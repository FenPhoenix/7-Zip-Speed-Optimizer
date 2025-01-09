using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SevenZipSpeedOptimizer;

// Same logic as AngelLoader, just adapted to fit the slightly different task at hand.
internal static class HtmlReference
{
    private static readonly string[] _imageFileExtensions =
    {
        ".gif", ".pcx", ".tga", ".dds", ".png", ".bmp", ".jpg", ".jpeg", ".tiff",
    };

    // Try to reject formats that don't make sense. Exclude instead of include for future-proofing.
    private static readonly string[] HtmlRefExcludes =
    {
        ".osm", ".exe", ".dll", ".ose", ".mis", ".gam", ".ibt", ".cbt", ".gmp", ".ned", ".unr", ".wav",
        ".mp3", ".ogg", ".aiff", ".aif", ".flac", ".bin", ".dlx", ".mc", ".mi", ".avi", ".mp4", ".mkv",
        ".flv", ".log", ".str", ".nut", ".db", ".obj",
    };

    // An html file might have other files it references, so do a recursive search through the archive to find
    // them all.
    internal static List<string> GetHtmlReferenceFiles(
        string fmSourcePath,
        List<string> readmeFilesFullPaths,
        string[] allFmFilesFullPaths)
    {
        List<Global.NameAndIndex> htmlRefFiles = new();

        foreach (string f in readmeFilesFullPaths)
        {
            if (!f.ExtIsHtml()) continue;

            string html = File.ReadAllText(f);

            for (int i = 0; i < allFmFilesFullPaths.Length; i++)
            {
                string e = allFmFilesFullPaths[i];
                string eName = Path.GetFileName(e);
                string eFullName = e.RemoveLeadingPath(fmSourcePath);
                AddHtmlRefFile(name: eName, fullName: eFullName, i, html, htmlRefFiles);
            }
        }

        if (htmlRefFiles.Count > 0)
        {
            for (int ri = 0; ri < htmlRefFiles.Count; ri++)
            {
                Global.NameAndIndex f = htmlRefFiles[ri];
                Trace.WriteLine(f.Name);
                string re = allFmFilesFullPaths[f.Index];

                if (RefFileExcluded(f.Name, new FileInfo(re).Length)) continue;

                string content;
                using (var sr = new StreamReader(Path.Combine(fmSourcePath, re)))
                {
                    content = sr.ReadToEnd();
                }

                for (int ei = 0; ei < allFmFilesFullPaths.Length; ei++)
                {
                    string e = allFmFilesFullPaths[ei];
                    string eName = Path.GetFileName(e);
                    string eFullName = e.RemoveLeadingPath(fmSourcePath);
                    AddHtmlRefFile(name: eName, fullName: eFullName, ei, content, htmlRefFiles);
                }
            }
        }

        var ret = new List<string>();

        if (htmlRefFiles.Count > 0)
        {
            foreach (Global.NameAndIndex f in htmlRefFiles)
            {
                ret.Add(f.Name);
            }
        }

        return ret;
    }

    private static void AddHtmlRefFile(string name, string fullName, int i, string content, List<Global.NameAndIndex> htmlRefFiles)
    {
        /*
        We just do a dumb string-match search through the whole file. While it's true that HTML files have their
        links in specific structures (href tags etc.), we don't attempt to narrow it down to these because a) we
        want to future-proof against any new ways to link that might come about, and b) HTML files can link out
        to other formats like CSS and who knows what else, and we don't want to write parsers for every format
        under the sun.
        */
        if (!name.IsEmpty() &&
            !name.EndsWithDirSep() &&
            name.Contains('.') &&
            !HtmlRefExcludes.Any(name.EndsWithI) &&
            content.ContainsI(name) &&
            htmlRefFiles.TrueForAll(x => x.Index != i))
        {
            htmlRefFiles.Add(new Global.NameAndIndex(fullName, i));
        }
    }

    private static bool RefFileExcluded(string name, long size) =>
        HtmlRefExcludes.Any(name.EndsWithI) ||
        _imageFileExtensions.Any(name.EndsWithI) ||
        // 128k is generous. Any text or markup sort of file should be WELL under that.
        size > Global.ByteSize.KB * 128;
}
