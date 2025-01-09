using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SevenZipSpeedOptimizer.WinFormsNative;

namespace SevenZipSpeedOptimizer;

internal static class FormsUtils
{
    internal static int GetFlowLayoutPanelControlsWidthAll(FlowLayoutPanel flp)
    {
        int ret = 0;
        for (int i = 0; i < flp.Controls.Count; i++)
        {
            Control c = flp.Controls[i];
            ret += c.Margin.Horizontal + c.Width;
        }
        ret += flp.Padding.Horizontal;

        return ret;
    }

    internal static void SetMessageBoxIcon(PictureBox pictureBox, MessageBoxIcon icon)
    {
        var sii = new Native.SHSTOCKICONINFO();
        try
        {
            Native.SHSTOCKICONID sysIcon = icon switch
            {
                MessageBoxIcon.Error or
                    MessageBoxIcon.Hand or
                    MessageBoxIcon.Stop
                    => Native.SHSTOCKICONID.SIID_ERROR,
                MessageBoxIcon.Question
                    => Native.SHSTOCKICONID.SIID_HELP,
                MessageBoxIcon.Exclamation or
                    MessageBoxIcon.Warning
                    => Native.SHSTOCKICONID.SIID_WARNING,
                MessageBoxIcon.Asterisk or
                    MessageBoxIcon.Information
                    => Native.SHSTOCKICONID.SIID_INFO,
                _
                    => throw new ArgumentOutOfRangeException(),
            };

            sii.cbSize = (uint)Marshal.SizeOf(typeof(Native.SHSTOCKICONINFO));

            int result = Native.SHGetStockIconInfo(sysIcon, Native.SHGSI_ICON, ref sii);
            Marshal.ThrowExceptionForHR(result, new IntPtr(-1));

            pictureBox.Image = Icon.FromHandle(sii.hIcon).ToBitmap();
        }
        catch
        {
            // "Wrong style" image (different style from the MessageBox one) but better than nothing if the
            // above fails
            pictureBox.Image = icon switch
            {
                MessageBoxIcon.Error or
                    MessageBoxIcon.Hand or
                    MessageBoxIcon.Stop
                    => SystemIcons.Error.ToBitmap(),
                MessageBoxIcon.Question
                    => SystemIcons.Question.ToBitmap(),
                MessageBoxIcon.Exclamation or
                    MessageBoxIcon.Warning
                    => SystemIcons.Warning.ToBitmap(),
                MessageBoxIcon.Asterisk or
                    MessageBoxIcon.Information
                    => SystemIcons.Information.ToBitmap(),
                _
                    => null,
            };
        }
        finally
        {
            Native.DestroyIcon(sii.hIcon);
        }
    }

    internal static void AddUniqueItems(this ListBox listBox, IEnumerable<string> items)
    {
        try
        {
            listBox.BeginUpdate();

            Utils.HashSetPathI hash = listBox.ItemsAsStrings().ToHashSetPathI();

            foreach (string dir in items)
            {
                if (!hash.Contains(dir)) listBox.Items.Add(dir);
            }
        }
        finally
        {
            listBox.EndUpdate();
        }
    }

    internal static void RemoveAndSelectNearest(this ListBox listBox)
    {
        if (listBox.SelectedIndex == -1) return;

        int oldSelectedIndex = listBox.SelectedIndex;

        listBox.Items.RemoveAt(listBox.SelectedIndex);

        if (oldSelectedIndex < listBox.Items.Count && listBox.Items.Count > 1)
        {
            listBox.SelectedIndex = oldSelectedIndex;
        }
        else if (listBox.Items.Count > 1)
        {
            listBox.SelectedIndex = oldSelectedIndex - 1;
        }
        else if (listBox.Items.Count == 1)
        {
            listBox.SelectedIndex = 0;
        }
    }

    public static string[] ItemsAsStrings(this ListBox listBox)
    {
        string[] ret = new string[listBox.Items.Count];
        for (int i = 0; i < listBox.Items.Count; i++)
        {
            ret[i] = listBox.Items[i].ToStringOrEmpty();
        }
        return ret;
    }

    public static string[] SelectedItemsAsStrings(this ListBox listBox)
    {
        string[] ret = new string[listBox.SelectedItems.Count];
        for (int i = 0; i < listBox.SelectedItems.Count; i++)
        {
            ret[i] = listBox.SelectedItems[i].ToStringOrEmpty();
        }
        return ret;
    }

    internal static bool SelectedIndexIsInRange(this ComboBox comboBox) =>
        comboBox.SelectedIndex > -1 && comboBox.SelectedIndex < comboBox.Items.Count;

    internal static bool IndexIsInRange(this ComboBox comboBox, int index) =>
        index > -1 && index < comboBox.Items.Count;

    #region Centering

    internal static void CenterH(this Control control, Control parent, bool clientSize = false)
    {
        int pWidth = clientSize ? parent.ClientSize.Width : parent.Width;
        control.Location = control.Location with { X = (pWidth / 2) - (control.Width / 2) };
    }

    #endregion
}
