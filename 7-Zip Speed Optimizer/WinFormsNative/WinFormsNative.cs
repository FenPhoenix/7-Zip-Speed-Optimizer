using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace SevenZipSpeedOptimizer.WinFormsNative;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal static class Native
{
    #region MessageBox/TaskDialog

    internal enum SHSTOCKICONID : uint
    {
        SIID_HELP = 23,
        SIID_WARNING = 78,
        SIID_INFO = 79,
        SIID_ERROR = 80,
    }

    internal const uint SHGSI_ICON = 0x000000100;

    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct SHSTOCKICONINFO
    {
        internal uint cbSize;
        internal IntPtr hIcon;
        internal int iSysIconIndex;
        internal int iIcon;
        internal fixed char szPath[MAX_PATH];
    }

    [DllImport("Shell32.dll", ExactSpelling = true, SetLastError = false)]
    internal static extern int SHGetStockIconInfo(SHSTOCKICONID siid, uint uFlags, ref SHSTOCKICONINFO psii);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyIcon(IntPtr hIcon);

    #endregion
}
