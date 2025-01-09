using System;

namespace SevenZipSpeedOptimizer;

internal static class WinVersion
{
    internal static readonly bool Is7OrAbove = WinVersionIs7OrAbove();
    internal static readonly bool Is11OrAbove = WinVersionIs11OrAbove();

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

    private static bool WinVersionIs11OrAbove()
    {
        try
        {
            OperatingSystem osVersion = Environment.OSVersion;
            return osVersion.Platform == PlatformID.Win32NT &&
                   osVersion.Version >= new Version(10, 0, 22000);
        }
        catch
        {
            return false;
        }
    }
}
