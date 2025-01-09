using System;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SevenZipSpeedOptimizer;

[PublicAPI]
public sealed class DarkErrorDialog : TaskDialogCustom
{
    public DarkErrorDialog(
        string message,
        string? title = null,
        MessageBoxIcon icon = MessageBoxIcon.Error) :
        base(
            message: message,
            title: title ?? "Error",
            icon: icon,
            yesText: "View log",
            noText: "OK",
            defaultButton: MBoxButton.Yes)
    {
        AcceptButton = NoButton;
        YesButton.DialogResult = DialogResult.None;
        NoButton.DialogResult = DialogResult.OK;

        YesButton.Click += YesButton_Click;
    }

    private void YesButton_Click(object sender, EventArgs e) => Core.OpenLogFile();

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        YesButton.Focus();
    }
}
