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
                FieldInfo? field = typeof(Global.Mode).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.Mode = (Global.Mode)field.GetValue(null);
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
                FieldInfo? field = typeof(Global.CompressionMethod).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.CompressionMethod = (Global.CompressionMethod)field.GetValue(null);
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

        return;

        static bool? GetNullableBoolValue(string value)
        {
            if (value == bool.TrueString)
            {
                return true;
            }
            else if (value == bool.FalseString)
            {
                return false;
            }
            else
            {
                return null;
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
        return;

        static string GetNullableBoolValue(bool? value) => value switch
        {
            true => bool.TrueString,
            false => bool.FalseString,
            _ => "",
        };
    }
}
