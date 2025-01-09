//#define DEV_TESTING

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using SevenZipSpeedOptimizer.Properties;
using SevenZipSpeedOptimizer.WinFormsNative.Dialogs;
using SevenZipSpeedOptimizer.WinFormsNative.Taskbar;

namespace SevenZipSpeedOptimizer;

/*
TODO: Allow adding a context menu item, so the user can just right click and run it on a folder.
*/

public sealed partial class MainForm : Form, IEventDisabler
{
    private enum OperationType
    {
        None,
        CreateSingle,
        RepackBatch,
    }

    private OperationType _operationTypeInProgress = OperationType.None;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int EventsDisabled { get; set; }

    #region Testing

    private void Test1Button_Click(object sender, EventArgs e)
    {
    }

    #endregion

    public MainForm()
    {
        InitializeComponent();

        Text = "7-Zip Speed Optimizer " + Application.ProductVersion;

        Icon = Resources.App;

#if !DEV_TESTING
        Test1Button.Hide();
#endif

        CompressionMethodComboBox.Items.AddRange(CompressionMethodItems.ToFriendlyStrings());

        PopulateThreadsComboBox();

        MainProgressBar.CenterH(StatusGroupBox);
        SubProgressBar.CenterH(StatusGroupBox);
        Cancel_Button.CenterH(StatusGroupBox);
        ResetProgressMessage();
        ProgressSubMessageLabel.Text = "";
        ProgressMessageLabel.Location = ProgressMessageLabel.Location with { X = MainProgressBar.Left };
        ProgressSubMessageLabel.Location = ProgressSubMessageLabel.Location with { X = SubProgressBar.Left };

#if DEV_TESTING
        SourceFMDirectoryTextBox.Text = @"J:\_BlackParade_Blocks\TheBlackParade_1.0";
        OutputArchiveTextBox.Text = @"J:\Local Storage NVME\FMs\__solid_blocks_test\zz_test.7z";
#endif
    }

    #region Getters and setters

    public bool DragDropEnabled => _operationTypeInProgress == OperationType.None;

    public void SetMode(Mode mode)
    {
        ModeTabControl.SelectedTab = mode switch
        {
            Mode.Repack => RepackTabPage,
            _ => CreateTabPage,
        };
    }

    public string SourceFMPath => SourceFMDirectoryTextBox.Text;

    public string OutputArchive => OutputArchiveTextBox.Text;

    public string[] Repack_Archives => ArchivesToRepackListBox.ItemsAsStrings();

    public string Repack_OutputDirectory => RepackOutputDirectoryTextBox.Text;

    public void SetCompressionLevel(int value)
    {
        CompressionLevelComboBox.SelectedIndex = CompressionLevelComboBox.IndexIsInRange(value)
            ? value
            : 0;
    }

    public void SetCompressionMethod(CompressionMethod value)
    {
        int index = (int)value;
        using (new DisableEvents(this))
        {
            CompressionMethodComboBox.SelectedIndex =
                CompressionMethodComboBox.IndexIsInRange(index)
                    ? index
                    : 0;
        }
    }

    public void SetThreads(int value)
    {
        NumberOfCPUThreadsComboBox.SelectedIndex =
            value == -1
                ? 0
                : NumberOfCPUThreadsComboBox.IndexIsInRange(value)
                    ? value
                    : 0;
    }

    #endregion

    #region Progress reporting

    public void StartCreateSingleArchiveOperation() => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.Normal);
        _operationTypeInProgress = OperationType.CreateSingle;
        MainPanel.Enabled = false;
        ProgressMessageLabel.Text = "";
        MainProgressBar.Value = 0;
        MainProgressBar.Show();
        Cancel_Button.Show();
    });

    public void EndCreateSingleArchiveOperation(string? message = null) => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.NoProgress);
        MainProgressBar.Hide();
        Cancel_Button.Hide();
        if (message != null)
        {
            ProgressMessageLabel.Text = message;
        }
        else
        {
            ResetProgressMessage();
        }
        MainProgressBar.Value = 0;
        MainPanel.Enabled = true;
        _operationTypeInProgress = OperationType.None;
    });

    public void StartRepackBatchOperation() => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.Normal);
        _operationTypeInProgress = OperationType.RepackBatch;
        MainPanel.Enabled = false;
        ProgressMessageLabel.Text = "";
        ProgressSubMessageLabel.Text = "";
        ProgressSubMessageLabel.Show();
        MainProgressBar.Value = 0;
        SubProgressBar.Value = 0;
        MainProgressBar.Show();
        SubProgressBar.Show();
        Cancel_Button.Show();
    });

    public void EndRepackBatchOperation() => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.NoProgress);
        MainProgressBar.Hide();
        SubProgressBar.Hide();
        Cancel_Button.Hide();
        ResetProgressMessage();
        ProgressSubMessageLabel.Hide();
        MainProgressBar.Value = 0;
        SubProgressBar.Value = 0;
        MainPanel.Enabled = true;
        _operationTypeInProgress = OperationType.None;
    });

    public void SetProgressMessage(string message) => Invoke(() =>
    {
        if (_operationTypeInProgress == OperationType.CreateSingle)
        {
            ProgressMessageLabel.Text = message;
        }
        else
        {
            ProgressSubMessageLabel.Text = message;
        }
    });

    public void SetProgressBatchMainMessage(string message) => Invoke(() =>
    {
        ProgressMessageLabel.Text = message;
    });

    public void SetProgressPercent(int percent) => Invoke(() =>
    {
        if (_operationTypeInProgress == OperationType.CreateSingle)
        {
            TaskBarProgress.SetValue(Handle, percent.Clamp(0, 100), 100);
            MainProgressBar.Value = percent;
        }
        else
        {
            SubProgressBar.Value = percent;
        }
    });

    public void SetProgressBatchMainPercent(int percent) => Invoke(() =>
    {
        TaskBarProgress.SetValue(Handle, percent.Clamp(0, 100), 100);
        MainProgressBar.Value = percent;
    });

    private void ResetProgressMessage()
    {
        ProgressMessageLabel.Text = "No operation in progress.";
    }

    #endregion

    #region Directories

    private void FMDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        using VistaFolderBrowserDialog dialog = new();
        dialog.Title = "Choose source FM directory";

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        SourceFMDirectoryTextBox.Text = dialog.DirectoryName;
    }

    private void OutputArchiveBrowseButton_Click(object sender, EventArgs e)
    {
        using SaveFileDialog dialog = new();
        dialog.Title = "Create output archive";
        dialog.AddExtension = true;
        dialog.Filter = "7-Zip files (*.7z)|*.7z";
        dialog.DefaultExt = "7z";
        dialog.ValidateNames = true;
        dialog.OverwritePrompt = true;

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        OutputArchiveTextBox.Text = dialog.FileName;
    }

    #endregion

    #region Form events

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_operationTypeInProgress != OperationType.None)
        {
            MessageBox.Show(
                text: "An operation is in progress. Please cancel it first.",
                caption: "Alert",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Warning);
            e.Cancel = true;
        }
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        Core.Shutdown();
    }

    #endregion

    #region 7-Zip fields

    private void CompressionLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!CompressionLevelComboBox.SelectedIndexIsInRange()) return;
        Config.CompressionLevel = CompressionLevelComboBox.SelectedIndex;
        CompressionMethodComboBox.Visible = CompressionLevelComboBox.SelectedIndex > 0;
    }

    private void CompressionMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!CompressionMethodComboBox.SelectedIndexIsInRange()) return;
        Config.CompressionMethod = CompressionMethodItems[CompressionMethodComboBox.SelectedIndex].BackingValue;
        PopulateThreadsComboBox(switching: true);
    }

    private void PopulateThreadsComboBox(bool switching = false)
    {
        FriendlyStringAndBackingValue<int>[] items =
            Config.CompressionMethod == CompressionMethod.LZMA2
                ? Lzma2ThreadItems
                : LzmaThreadItems;

        NumberOfCPUThreadsComboBox.Items.Clear();
        NumberOfCPUThreadsComboBox.Items.AddRange(items.ToFriendlyStrings());

        if (switching && EventsDisabled <= 0)
        {
            SetThreads(-1);
            Config.Threads = -1;
        }
    }

    private void NumberOfCPUThreadsComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!NumberOfCPUThreadsComboBox.SelectedIndexIsInRange()) return;
        Config.Threads =
            NumberOfCPUThreadsComboBox.SelectedIndex == 0
                ? -1
                : Config.CompressionMethod == CompressionMethod.LZMA2
                    ? Lzma2ThreadItems[NumberOfCPUThreadsComboBox.SelectedIndex].BackingValue
                    : LzmaThreadItems[NumberOfCPUThreadsComboBox.SelectedIndex].BackingValue;
    }

    #endregion

    private async void CreateSingleArchiveButton_Click(object sender, EventArgs e)
    {
        await Core.CreateSingleArchive();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
        Core.CancelToken();
    }

    private void AddSourceArchiveButton_Click(object sender, EventArgs e)
    {
        using var d = new OpenFileDialog();

        d.Title = "Add source FM archive(s)";
        d.Filter =
            "All archive files (*.7z, *.zip, *.rar)|*.7z;*.zip;*.rar|" +
            "Zip files (*.zip)|*.zip|" +
            "7-Zip files (*.7z)|*.7z|" +
            "Rar files (*.rar)|*.rar|" +
            "All files (*.*)|*.*";
        d.Multiselect = true;

        ListBox lb = ArchivesToRepackListBox;
        string selectedItem =
            lb.SelectedIndex > -1 ? lb.SelectedItem.ToStringOrEmpty() :
            lb.Items.Count > 0 ? lb.ItemsAsStrings()[lb.Items.Count - 1] :
            "";

        if (!selectedItem.IsWhiteSpace())
        {
            try
            {
                d.InitialDirectory = Path.GetDirectoryName(selectedItem) ?? "";
            }
            catch
            {
                // ignore
            }
        }

        if (d.ShowDialog(this) == DialogResult.OK)
        {
            lb.AddUniqueItems(d.FileNames);
        }
    }

    private void RemoveSourceArchiveButton_Click(object sender, EventArgs e)
    {
        ArchivesToRepackListBox.RemoveAndSelectNearest();
    }

    private void ClearSourceArchivesButton_Click(object sender, EventArgs e)
    {
        ArchivesToRepackListBox.Items.Clear();
    }

    private void RepackOutputDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        using VistaFolderBrowserDialog dialog = new();
        dialog.Title = "Choose output directory";

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        RepackOutputDirectoryTextBox.Text = dialog.DirectoryName;
    }

    private void ModeTabControl_Selected(object sender, TabControlEventArgs e)
    {
        Config.Mode = ModeTabControl.SelectedTab == RepackTabPage ? Mode.Repack : Mode.Create;
    }

    private async void RepackButton_Click(object sender, EventArgs e)
    {
        await Core.RepackBatch();
    }

    private void ArchivesToRepackListBox_DragEnter(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out _))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void ArchivesToRepackListBox_DragDrop(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out string[]? droppedItems))
        {
            ArchivesToRepackListBox.AddUniqueItems(Core.GetStronglyCheckedFiles(droppedItems));
        }
    }

    /// <summary>
    /// This method is auto-invoked if <see cref="Core.View"/> is able to be invoked to.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public void ShowError(string message, string? title = null, MessageBoxIcon icon = MessageBoxIcon.Error)
    {
        Invoke(() =>
        {
            ShowError_Internal(message, this, title, icon);
        });
    }

    // Private method, not invoked because all calls are
    private static void ShowError_Internal(string message, IWin32Window? owner, string? title, MessageBoxIcon icon)
    {
        using var d = new DarkErrorDialog(message, title, icon);
        if (owner != null)
        {
            d.ShowDialog(owner);
        }
        else
        {
            d.ShowDialog();
        }
    }

    /// <summary>
    /// This method is auto-invoked if <see cref="Core.View"/> is able to be invoked to.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="yes"></param>
    /// <param name="no"></param>
    /// <param name="cancel"></param>
    /// <param name="yesIsDangerous"></param>
    /// <param name="noIsDangerous"></param>
    /// <param name="cancelIsDangerous"></param>
    /// <param name="checkBoxText"></param>
    /// <param name="defaultButton"></param>
    /// <param name="viewLogButtonVisible"></param>
    /// <returns></returns>
    public (MBoxButton ButtonPressed, bool CheckBoxChecked)
    ShowMultiChoiceDialog(string message,
        string title,
        MessageBoxIcon icon,
        string? yes,
        string? no,
        string? cancel = null,
        bool yesIsDangerous = false,
        bool noIsDangerous = false,
        bool cancelIsDangerous = false,
        string? checkBoxText = null,
        MBoxButton defaultButton = MBoxButton.Yes,
        bool viewLogButtonVisible = false) =>
        ((MBoxButton, bool))Invoke(() =>
        {
            using var d = new TaskDialogCustom(
                message: message,
                title: title,
                icon: icon,
                yesText: yes,
                noText: no,
                cancelText: cancel,
                yesIsDangerous: yesIsDangerous,
                noIsDangerous: noIsDangerous,
                cancelIsDangerous: cancelIsDangerous,
                checkBoxText: checkBoxText,
                defaultButton: defaultButton,
                viewLogButtonVisible: viewLogButtonVisible);

            DialogResult result = d.ShowDialog(this);

            return (DialogResultToMBoxButton(result), d.IsVerificationChecked);
        });

    private static MBoxButton DialogResultToMBoxButton(DialogResult dialogResult) => dialogResult switch
    {
        DialogResult.Yes => MBoxButton.Yes,
        DialogResult.No => MBoxButton.No,
        _ => MBoxButton.Cancel,
    };

    private void SourceFMDirectoryTextBox_DragEnter(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out _))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void SourceFMDirectoryTextBox_DragDrop(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out string[]? droppedItems) && droppedItems.Length > 0)
        {
            SourceFMDirectoryTextBox.Text = droppedItems[0];
        }
    }

    private void RepackOutputDirectoryTextBox_DragEnter(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out _))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void RepackOutputDirectoryTextBox_DragDrop(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out string[]? droppedItems) && droppedItems.Length > 0)
        {
            RepackOutputDirectoryTextBox.Text = droppedItems[0];
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {

    }
}
