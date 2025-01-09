using System;

namespace SevenZipSpeedOptimizer;

internal static class WinVersion
{
    internal static readonly bool Is7OrAbove = WinVersionIs7OrAbove();

    private static bool WinVersionIs7OrAbove()
    {
        try
        {
            OperatingSystem osVersion = Environment.OSVersion;
            return osVersion.Platform == PlatformID.Win32NT &&
                   osVersion.Version >= new Version(6, 1);

            // Windows 8 is 6, 2
        }
        catch
        {
            return false;
        }
    }
}
