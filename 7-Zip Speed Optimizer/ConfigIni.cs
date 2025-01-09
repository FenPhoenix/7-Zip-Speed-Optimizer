using System.IO;
using System.Reflection;

namespace SevenZipSpeedOptimizer;

internal static class ConfigIni
{
    internal static void ReadConfigData()
    {
        if (!File.Exists(Paths.ConfigFile))
        {
            return;
        }

        const BindingFlags _bFlagsEnum =
        BindingFlags.Instance |
        BindingFlags.Static |
        BindingFlags.Public |
        BindingFlags.NonPublic;

        string[] lines = File.ReadAllLines(Paths.ConfigFile);
        for (int i = 0; i < lines.Length; i++)
        {
            string lineT = lines[i].Trim();
            if (lineT.TryGetValueO("Mode=", out string value))
            {
                FieldInfo? field = typeof(Mode).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.Mode = (Mode)field.GetValue(null);
                }
            }
            if (lineT.TryGetValueO("CompressionLevel=", out value))
            {
                if (int.TryParse(value, out int result))
                {
                    Config.CompressionLevel = result;
                }
            }
            else if (lineT.TryGetValueO("CompressionMethod=", out value))
            {
                FieldInfo? field = typeof(CompressionMethod).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.CompressionMethod = (CompressionMethod)field.GetValue(null);
                }
            }
            else if (lineT.TryGetValueO("Threads=", out value))
            {
                if (int.TryParse(value, out int result))
                {
                    Config.Threads = result;
                }
            }
        }
    }

    internal static void WriteConfigData()
    {
        using var sw = new StreamWriter(Paths.ConfigFile);
        sw.WriteLine("Mode=" + Config.Mode);
        sw.WriteLine("CompressionLevel=" + Config.CompressionLevel.ToStrInv());
        sw.WriteLine("CompressionMethod=" + Config.CompressionMethod);
        sw.WriteLine("Threads=" + Config.Threads.ToStrInv());
    }
}
