namespace SevenZipSpeedOptimizer;

sealed partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.SourceFMDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.FMDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.SourceFMDirectoryLabel = new System.Windows.Forms.Label();
            this.OutputArchiveTextBox = new System.Windows.Forms.TextBox();
            this.OutputArchiveLabel = new System.Windows.Forms.Label();
            this.CreateSingleArchiveButton = new System.Windows.Forms.Button();
            this.CompressionLevelComboBox = new System.Windows.Forms.ComboBox();
            this.CompressionLevelLabel = new System.Windows.Forms.Label();
            this.CompressionMethodComboBox = new System.Windows.Forms.ComboBox();
            this.CompressionMethodLabel = new System.Windows.Forms.Label();
            this.OutputArchiveBrowseButton = new System.Windows.Forms.Button();
            this.Test1Button = new System.Windows.Forms.Button();
            this.NumberOfCPUThreadsLabel = new System.Windows.Forms.Label();
            this.NumberOfCPUThreadsComboBox = new System.Windows.Forms.ComboBox();
            this.ProgressMessageLabel = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.ModeTabControl = new System.Windows.Forms.TabControl();
            this.CreateTabPage = new System.Windows.Forms.TabPage();
            this.RepackTabPage = new System.Windows.Forms.TabPage();
            this.ClearSourceArchivesButton = new System.Windows.Forms.Button();
            this.RemoveSourceArchiveButton = new System.Windows.Forms.Button();
            this.AddSourceArchiveButton = new System.Windows.Forms.Button();
            this.RepackButton = new System.Windows.Forms.Button();
            this.ArchivesToRepackListBox = new System.Windows.Forms.ListBox();
            this.RepackOutputDirectoryLabel = new System.Windows.Forms.Label();
            this.RepackOutputDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.RepackOutputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.StatusGroupBox = new System.Windows.Forms.GroupBox();
            this.ProgressSubMessageLabel = new System.Windows.Forms.Label();
            this.SubProgressBar = new SevenZipSpeedOptimizer.ProgressBarCustom();
            this.MainProgressBar = new SevenZipSpeedOptimizer.ProgressBarCustom();
            this.IgnoreFMSelBakFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.MainPanel.SuspendLayout();
            this.ModeTabControl.SuspendLayout();
            this.CreateTabPage.SuspendLayout();
            this.RepackTabPage.SuspendLayout();
            this.StatusGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SourceFMDirectoryTextBox
            // 
            this.SourceFMDirectoryTextBox.AllowDrop = true;
            this.SourceFMDirectoryTextBox.Location = new System.Drawing.Point(12, 26);
            this.SourceFMDirectoryTextBox.Name = "SourceFMDirectoryTextBox";
            this.SourceFMDirectoryTextBox.Size = new System.Drawing.Size(640, 20);
            this.SourceFMDirectoryTextBox.TabIndex = 1;
            this.SourceFMDirectoryTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.SourceFMDirectoryTextBox_DragDrop);
            this.SourceFMDirectoryTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.SourceFMDirectoryTextBox_DragEnter);
            // 
            // FMDirectoryBrowseButton
            // 
            this.FMDirectoryBrowseButton.Location = new System.Drawing.Point(652, 25);
            this.FMDirectoryBrowseButton.Name = "FMDirectoryBrowseButton";
            this.FMDirectoryBrowseButton.Size = new System.Drawing.Size(81, 23);
            this.FMDirectoryBrowseButton.TabIndex = 2;
            this.FMDirectoryBrowseButton.Text = "Browse...";
            this.FMDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this.FMDirectoryBrowseButton.Click += new System.EventHandler(this.FMDirectoryBrowseButton_Click);
            // 
            // SourceFMDirectoryLabel
            // 
            this.SourceFMDirectoryLabel.AutoSize = true;
            this.SourceFMDirectoryLabel.Location = new System.Drawing.Point(12, 10);
            this.SourceFMDirectoryLabel.Name = "SourceFMDirectoryLabel";
            this.SourceFMDirectoryLabel.Size = new System.Drawing.Size(105, 13);
            this.SourceFMDirectoryLabel.TabIndex = 0;
            this.SourceFMDirectoryLabel.Text = "Source FM directory:";
            // 
            // OutputArchiveTextBox
            // 
            this.OutputArchiveTextBox.Location = new System.Drawing.Point(12, 74);
            this.OutputArchiveTextBox.Name = "OutputArchiveTextBox";
            this.OutputArchiveTextBox.Size = new System.Drawing.Size(640, 20);
            this.OutputArchiveTextBox.TabIndex = 4;
            // 
            // OutputArchiveLabel
            // 
            this.OutputArchiveLabel.AutoSize = true;
            this.OutputArchiveLabel.Location = new System.Drawing.Point(12, 58);
            this.OutputArchiveLabel.Name = "OutputArchiveLabel";
            this.OutputArchiveLabel.Size = new System.Drawing.Size(80, 13);
            this.OutputArchiveLabel.TabIndex = 3;
            this.OutputArchiveLabel.Text = "Output archive:";
            // 
            // CreateSingleArchiveButton
            // 
            this.CreateSingleArchiveButton.Location = new System.Drawing.Point(636, 320);
            this.CreateSingleArchiveButton.Name = "CreateSingleArchiveButton";
            this.CreateSingleArchiveButton.Size = new System.Drawing.Size(97, 23);
            this.CreateSingleArchiveButton.TabIndex = 6;
            this.CreateSingleArchiveButton.Text = "Create";
            this.CreateSingleArchiveButton.UseVisualStyleBackColor = true;
            this.CreateSingleArchiveButton.Click += new System.EventHandler(this.CreateSingleArchiveButton_Click);
            // 
            // CompressionLevelComboBox
            // 
            this.CompressionLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionLevelComboBox.FormattingEnabled = true;
            this.CompressionLevelComboBox.Items.AddRange(new object[] {
            "0 - Store",
            "1 - Fastest",
            "2",
            "3 - Fast",
            "4",
            "5 - Normal",
            "6",
            "7 - Maximum",
            "8",
            "9 - Ultra"});
            this.CompressionLevelComboBox.Location = new System.Drawing.Point(927, 28);
            this.CompressionLevelComboBox.Name = "CompressionLevelComboBox";
            this.CompressionLevelComboBox.Size = new System.Drawing.Size(132, 21);
            this.CompressionLevelComboBox.TabIndex = 2;
            this.CompressionLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.CompressionLevelComboBox_SelectedIndexChanged);
            // 
            // CompressionLevelLabel
            // 
            this.CompressionLevelLabel.AutoSize = true;
            this.CompressionLevelLabel.Location = new System.Drawing.Point(768, 32);
            this.CompressionLevelLabel.Name = "CompressionLevelLabel";
            this.CompressionLevelLabel.Size = new System.Drawing.Size(95, 13);
            this.CompressionLevelLabel.TabIndex = 1;
            this.CompressionLevelLabel.Text = "Compression level:";
            // 
            // CompressionMethodComboBox
            // 
            this.CompressionMethodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionMethodComboBox.FormattingEnabled = true;
            this.CompressionMethodComboBox.Location = new System.Drawing.Point(927, 63);
            this.CompressionMethodComboBox.Name = "CompressionMethodComboBox";
            this.CompressionMethodComboBox.Size = new System.Drawing.Size(132, 21);
            this.CompressionMethodComboBox.TabIndex = 4;
            this.CompressionMethodComboBox.SelectedIndexChanged += new System.EventHandler(this.CompressionMethodComboBox_SelectedIndexChanged);
            // 
            // CompressionMethodLabel
            // 
            this.CompressionMethodLabel.AutoSize = true;
            this.CompressionMethodLabel.Location = new System.Drawing.Point(768, 67);
            this.CompressionMethodLabel.Name = "CompressionMethodLabel";
            this.CompressionMethodLabel.Size = new System.Drawing.Size(108, 13);
            this.CompressionMethodLabel.TabIndex = 3;
            this.CompressionMethodLabel.Text = "Compression method:";
            // 
            // OutputArchiveBrowseButton
            // 
            this.OutputArchiveBrowseButton.Location = new System.Drawing.Point(652, 73);
            this.OutputArchiveBrowseButton.Name = "OutputArchiveBrowseButton";
            this.OutputArchiveBrowseButton.Size = new System.Drawing.Size(81, 23);
            this.OutputArchiveBrowseButton.TabIndex = 5;
            this.OutputArchiveBrowseButton.Text = "Browse...";
            this.OutputArchiveBrowseButton.UseVisualStyleBackColor = true;
            this.OutputArchiveBrowseButton.Click += new System.EventHandler(this.OutputArchiveBrowseButton_Click);
            // 
            // Test1Button
            // 
            this.Test1Button.Location = new System.Drawing.Point(984, 128);
            this.Test1Button.Name = "Test1Button";
            this.Test1Button.Size = new System.Drawing.Size(81, 23);
            this.Test1Button.TabIndex = 7;
            this.Test1Button.Text = "Test1";
            this.Test1Button.UseVisualStyleBackColor = true;
            this.Test1Button.Click += new System.EventHandler(this.Test1Button_Click);
            // 
            // NumberOfCPUThreadsLabel
            // 
            this.NumberOfCPUThreadsLabel.AutoSize = true;
            this.NumberOfCPUThreadsLabel.Location = new System.Drawing.Point(768, 102);
            this.NumberOfCPUThreadsLabel.Name = "NumberOfCPUThreadsLabel";
            this.NumberOfCPUThreadsLabel.Size = new System.Drawing.Size(122, 13);
            this.NumberOfCPUThreadsLabel.TabIndex = 5;
            this.NumberOfCPUThreadsLabel.Text = "Number of CPU threads:";
            // 
            // NumberOfCPUThreadsComboBox
            // 
            this.NumberOfCPUThreadsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NumberOfCPUThreadsComboBox.FormattingEnabled = true;
            this.NumberOfCPUThreadsComboBox.Location = new System.Drawing.Point(927, 98);
            this.NumberOfCPUThreadsComboBox.Name = "NumberOfCPUThreadsComboBox";
            this.NumberOfCPUThreadsComboBox.Size = new System.Drawing.Size(132, 21);
            this.NumberOfCPUThreadsComboBox.TabIndex = 6;
            this.NumberOfCPUThreadsComboBox.SelectedIndexChanged += new System.EventHandler(this.NumberOfCPUThreadsComboBox_SelectedIndexChanged);
            // 
            // ProgressMessageLabel
            // 
            this.ProgressMessageLabel.AutoSize = true;
            this.ProgressMessageLabel.Location = new System.Drawing.Point(16, 24);
            this.ProgressMessageLabel.Name = "ProgressMessageLabel";
            this.ProgressMessageLabel.Size = new System.Drawing.Size(97, 13);
            this.ProgressMessageLabel.TabIndex = 0;
            this.ProgressMessageLabel.Text = "[ProgressMessage]";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(488, 104);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(81, 23);
            this.Cancel_Button.TabIndex = 4;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Visible = false;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.ModeTabControl);
            this.MainPanel.Controls.Add(this.NumberOfCPUThreadsLabel);
            this.MainPanel.Controls.Add(this.CompressionLevelComboBox);
            this.MainPanel.Controls.Add(this.NumberOfCPUThreadsComboBox);
            this.MainPanel.Controls.Add(this.CompressionLevelLabel);
            this.MainPanel.Controls.Add(this.CompressionMethodComboBox);
            this.MainPanel.Controls.Add(this.CompressionMethodLabel);
            this.MainPanel.Controls.Add(this.Test1Button);
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1072, 400);
            this.MainPanel.TabIndex = 0;
            // 
            // ModeTabControl
            // 
            this.ModeTabControl.Controls.Add(this.CreateTabPage);
            this.ModeTabControl.Controls.Add(this.RepackTabPage);
            this.ModeTabControl.Location = new System.Drawing.Point(8, 8);
            this.ModeTabControl.Name = "ModeTabControl";
            this.ModeTabControl.SelectedIndex = 0;
            this.ModeTabControl.Size = new System.Drawing.Size(752, 384);
            this.ModeTabControl.TabIndex = 0;
            this.ModeTabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.ModeTabControl_Selected);
            // 
            // CreateTabPage
            // 
            this.CreateTabPage.Controls.Add(this.SourceFMDirectoryLabel);
            this.CreateTabPage.Controls.Add(this.OutputArchiveLabel);
            this.CreateTabPage.Controls.Add(this.CreateSingleArchiveButton);
            this.CreateTabPage.Controls.Add(this.OutputArchiveTextBox);
            this.CreateTabPage.Controls.Add(this.OutputArchiveBrowseButton);
            this.CreateTabPage.Controls.Add(this.FMDirectoryBrowseButton);
            this.CreateTabPage.Controls.Add(this.SourceFMDirectoryTextBox);
            this.CreateTabPage.Location = new System.Drawing.Point(4, 22);
            this.CreateTabPage.Name = "CreateTabPage";
            this.CreateTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.CreateTabPage.Size = new System.Drawing.Size(744, 358);
            this.CreateTabPage.TabIndex = 0;
            this.CreateTabPage.Text = "Create single FM";
            this.CreateTabPage.UseVisualStyleBackColor = true;
            // 
            // RepackTabPage
            // 
            this.RepackTabPage.Controls.Add(this.IgnoreFMSelBakFilesCheckBox);
            this.RepackTabPage.Controls.Add(this.ClearSourceArchivesButton);
            this.RepackTabPage.Controls.Add(this.RemoveSourceArchiveButton);
            this.RepackTabPage.Controls.Add(this.AddSourceArchiveButton);
            this.RepackTabPage.Controls.Add(this.RepackButton);
            this.RepackTabPage.Controls.Add(this.ArchivesToRepackListBox);
            this.RepackTabPage.Controls.Add(this.RepackOutputDirectoryLabel);
            this.RepackTabPage.Controls.Add(this.RepackOutputDirectoryBrowseButton);
            this.RepackTabPage.Controls.Add(this.RepackOutputDirectoryTextBox);
            this.RepackTabPage.Location = new System.Drawing.Point(4, 22);
            this.RepackTabPage.Name = "RepackTabPage";
            this.RepackTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.RepackTabPage.Size = new System.Drawing.Size(744, 358);
            this.RepackTabPage.TabIndex = 1;
            this.RepackTabPage.Text = "Batch repack";
            this.RepackTabPage.UseVisualStyleBackColor = true;
            // 
            // ClearSourceArchivesButton
            // 
            this.ClearSourceArchivesButton.Location = new System.Drawing.Point(510, 224);
            this.ClearSourceArchivesButton.Name = "ClearSourceArchivesButton";
            this.ClearSourceArchivesButton.Size = new System.Drawing.Size(75, 23);
            this.ClearSourceArchivesButton.TabIndex = 1;
            this.ClearSourceArchivesButton.Text = "Clear";
            this.ClearSourceArchivesButton.UseVisualStyleBackColor = true;
            this.ClearSourceArchivesButton.Click += new System.EventHandler(this.ClearSourceArchivesButton_Click);
            // 
            // RemoveSourceArchiveButton
            // 
            this.RemoveSourceArchiveButton.Location = new System.Drawing.Point(584, 224);
            this.RemoveSourceArchiveButton.Name = "RemoveSourceArchiveButton";
            this.RemoveSourceArchiveButton.Size = new System.Drawing.Size(75, 23);
            this.RemoveSourceArchiveButton.TabIndex = 2;
            this.RemoveSourceArchiveButton.Text = "Remove";
            this.RemoveSourceArchiveButton.UseVisualStyleBackColor = true;
            this.RemoveSourceArchiveButton.Click += new System.EventHandler(this.RemoveSourceArchiveButton_Click);
            // 
            // AddSourceArchiveButton
            // 
            this.AddSourceArchiveButton.Location = new System.Drawing.Point(658, 224);
            this.AddSourceArchiveButton.Name = "AddSourceArchiveButton";
            this.AddSourceArchiveButton.Size = new System.Drawing.Size(75, 23);
            this.AddSourceArchiveButton.TabIndex = 3;
            this.AddSourceArchiveButton.Text = "Add...";
            this.AddSourceArchiveButton.UseVisualStyleBackColor = true;
            this.AddSourceArchiveButton.Click += new System.EventHandler(this.AddSourceArchiveButton_Click);
            // 
            // RepackButton
            // 
            this.RepackButton.Location = new System.Drawing.Point(636, 320);
            this.RepackButton.Name = "RepackButton";
            this.RepackButton.Size = new System.Drawing.Size(97, 23);
            this.RepackButton.TabIndex = 7;
            this.RepackButton.Text = "Repack all";
            this.RepackButton.UseVisualStyleBackColor = true;
            this.RepackButton.Click += new System.EventHandler(this.RepackButton_Click);
            // 
            // ArchivesToRepackListBox
            // 
            this.ArchivesToRepackListBox.AllowDrop = true;
            this.ArchivesToRepackListBox.FormattingEnabled = true;
            this.ArchivesToRepackListBox.Location = new System.Drawing.Point(12, 10);
            this.ArchivesToRepackListBox.Name = "ArchivesToRepackListBox";
            this.ArchivesToRepackListBox.Size = new System.Drawing.Size(720, 212);
            this.ArchivesToRepackListBox.TabIndex = 0;
            this.ArchivesToRepackListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.ArchivesToRepackListBox_DragDrop);
            this.ArchivesToRepackListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.ArchivesToRepackListBox_DragEnter);
            // 
            // RepackOutputDirectoryLabel
            // 
            this.RepackOutputDirectoryLabel.AutoSize = true;
            this.RepackOutputDirectoryLabel.Location = new System.Drawing.Point(12, 260);
            this.RepackOutputDirectoryLabel.Name = "RepackOutputDirectoryLabel";
            this.RepackOutputDirectoryLabel.Size = new System.Drawing.Size(85, 13);
            this.RepackOutputDirectoryLabel.TabIndex = 4;
            this.RepackOutputDirectoryLabel.Text = "Output directory:";
            // 
            // RepackOutputDirectoryBrowseButton
            // 
            this.RepackOutputDirectoryBrowseButton.Location = new System.Drawing.Point(652, 275);
            this.RepackOutputDirectoryBrowseButton.Name = "RepackOutputDirectoryBrowseButton";
            this.RepackOutputDirectoryBrowseButton.Size = new System.Drawing.Size(81, 23);
            this.RepackOutputDirectoryBrowseButton.TabIndex = 6;
            this.RepackOutputDirectoryBrowseButton.Text = "Browse...";
            this.RepackOutputDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this.RepackOutputDirectoryBrowseButton.Click += new System.EventHandler(this.RepackOutputDirectoryBrowseButton_Click);
            // 
            // RepackOutputDirectoryTextBox
            // 
            this.RepackOutputDirectoryTextBox.AllowDrop = true;
            this.RepackOutputDirectoryTextBox.Location = new System.Drawing.Point(12, 276);
            this.RepackOutputDirectoryTextBox.Name = "RepackOutputDirectoryTextBox";
            this.RepackOutputDirectoryTextBox.Size = new System.Drawing.Size(640, 20);
            this.RepackOutputDirectoryTextBox.TabIndex = 5;
            this.RepackOutputDirectoryTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.RepackOutputDirectoryTextBox_DragDrop);
            this.RepackOutputDirectoryTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.RepackOutputDirectoryTextBox_DragEnter);
            // 
            // StatusGroupBox
            // 
            this.StatusGroupBox.Controls.Add(this.ProgressSubMessageLabel);
            this.StatusGroupBox.Controls.Add(this.ProgressMessageLabel);
            this.StatusGroupBox.Controls.Add(this.SubProgressBar);
            this.StatusGroupBox.Controls.Add(this.MainProgressBar);
            this.StatusGroupBox.Controls.Add(this.Cancel_Button);
            this.StatusGroupBox.Location = new System.Drawing.Point(8, 401);
            this.StatusGroupBox.Name = "StatusGroupBox";
            this.StatusGroupBox.Size = new System.Drawing.Size(1056, 135);
            this.StatusGroupBox.TabIndex = 1;
            this.StatusGroupBox.TabStop = false;
            this.StatusGroupBox.Text = "Status";
            // 
            // ProgressSubMessageLabel
            // 
            this.ProgressSubMessageLabel.AutoSize = true;
            this.ProgressSubMessageLabel.Location = new System.Drawing.Point(16, 64);
            this.ProgressSubMessageLabel.Name = "ProgressSubMessageLabel";
            this.ProgressSubMessageLabel.Size = new System.Drawing.Size(97, 13);
            this.ProgressSubMessageLabel.TabIndex = 2;
            this.ProgressSubMessageLabel.Text = "[ProgressMessage]";
            // 
            // SubProgressBar
            // 
            this.SubProgressBar.Location = new System.Drawing.Point(16, 80);
            this.SubProgressBar.Name = "SubProgressBar";
            this.SubProgressBar.Size = new System.Drawing.Size(1024, 16);
            this.SubProgressBar.TabIndex = 3;
            this.SubProgressBar.Visible = false;
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(16, 40);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(1024, 16);
            this.MainProgressBar.TabIndex = 1;
            this.MainProgressBar.Visible = false;
            // 
            // IgnoreFMSelBakFilesCheckBox
            // 
            this.IgnoreFMSelBakFilesCheckBox.AutoSize = true;
            this.IgnoreFMSelBakFilesCheckBox.Checked = true;
            this.IgnoreFMSelBakFilesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IgnoreFMSelBakFilesCheckBox.Location = new System.Drawing.Point(16, 224);
            this.IgnoreFMSelBakFilesCheckBox.Name = "IgnoreFMSelBakFilesCheckBox";
            this.IgnoreFMSelBakFilesCheckBox.Size = new System.Drawing.Size(129, 17);
            this.IgnoreFMSelBakFilesCheckBox.TabIndex = 8;
            this.IgnoreFMSelBakFilesCheckBox.Text = "Ignore FMSelBak files";
            this.IgnoreFMSelBakFilesCheckBox.UseVisualStyleBackColor = true;
            this.IgnoreFMSelBakFilesCheckBox.CheckedChanged += new System.EventHandler(this.IgnoreFMSelBakFilesCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 544);
            this.Controls.Add(this.StatusGroupBox);
            this.Controls.Add(this.MainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "7-Zip Speed Optimizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.ModeTabControl.ResumeLayout(false);
            this.CreateTabPage.ResumeLayout(false);
            this.CreateTabPage.PerformLayout();
            this.RepackTabPage.ResumeLayout(false);
            this.RepackTabPage.PerformLayout();
            this.StatusGroupBox.ResumeLayout(false);
            this.StatusGroupBox.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox SourceFMDirectoryTextBox;
    private System.Windows.Forms.Button FMDirectoryBrowseButton;
    private System.Windows.Forms.Label SourceFMDirectoryLabel;
    private System.Windows.Forms.TextBox OutputArchiveTextBox;
    private System.Windows.Forms.Label OutputArchiveLabel;
    private System.Windows.Forms.Button CreateSingleArchiveButton;
    private System.Windows.Forms.ComboBox CompressionLevelComboBox;
    private System.Windows.Forms.Label CompressionLevelLabel;
    private System.Windows.Forms.ComboBox CompressionMethodComboBox;
    private System.Windows.Forms.Label CompressionMethodLabel;
    private System.Windows.Forms.Button OutputArchiveBrowseButton;
    private System.Windows.Forms.Button Test1Button;
    private System.Windows.Forms.Label NumberOfCPUThreadsLabel;
    private System.Windows.Forms.ComboBox NumberOfCPUThreadsComboBox;
    private ProgressBarCustom MainProgressBar;
    private System.Windows.Forms.Label ProgressMessageLabel;
    private System.Windows.Forms.Button Cancel_Button;
    private System.Windows.Forms.Panel MainPanel;
    private System.Windows.Forms.GroupBox StatusGroupBox;
    private System.Windows.Forms.ListBox ArchivesToRepackListBox;
    private System.Windows.Forms.Label RepackOutputDirectoryLabel;
    private System.Windows.Forms.TextBox RepackOutputDirectoryTextBox;
    private System.Windows.Forms.Button RepackOutputDirectoryBrowseButton;
    private System.Windows.Forms.TabControl ModeTabControl;
    private System.Windows.Forms.TabPage CreateTabPage;
    private System.Windows.Forms.TabPage RepackTabPage;
    private System.Windows.Forms.Button RepackButton;
    private System.Windows.Forms.Button ClearSourceArchivesButton;
    private System.Windows.Forms.Button RemoveSourceArchiveButton;
    private System.Windows.Forms.Button AddSourceArchiveButton;
    private System.Windows.Forms.Label ProgressSubMessageLabel;
    private ProgressBarCustom SubProgressBar;
    private System.Windows.Forms.CheckBox IgnoreFMSelBakFilesCheckBox;
}
