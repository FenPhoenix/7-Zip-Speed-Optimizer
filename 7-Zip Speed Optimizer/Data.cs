using System;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace SevenZipSpeedOptimizer;

public sealed class ConfigData
{
    public Global.Mode Mode = Global.Mode.Create;

    private const int DefaultCompressionLevel = 9;

    private int _compressionLevel = DefaultCompressionLevel;
    public int CompressionLevel
    {
        get => _compressionLevel;
        set => _compressionLevel = value.Clamp(0, 9);
    }

    public Global.CompressionMethod CompressionMethod = Global.CompressionMethod.LZMA2;

    private int _threads;
    public int Threads
    {
        get => _threads;
        set
        {
            if (value == -1)
            {
                _threads = -1;
            }
            else
            {
                int max = CompressionMethod == Global.CompressionMethod.LZMA2 ? CPUThreads * 2 : Math.Min(CPUThreads, 2);
                _threads = value.Clamp(0, max);
            }
        }
    }
}

public sealed class FriendlyStringAndBackingValue<TBacking>
{
    public readonly string FriendlyString;
    public readonly TBacking BackingValue;

    public FriendlyStringAndBackingValue(string friendlyString, TBacking backingValue)
    {
        FriendlyString = friendlyString;
        BackingValue = backingValue;
    }
}

public static class Global
{
    public const int MAX_PATH = 260;

    public enum MBoxButton
    {
        Yes,
        No,
        Cancel,
    }

    public enum Mode
    {
        Create,
        Repack,
    }

    /// <summary>
    /// Shorthand for <see cref="Environment.NewLine"/>
    /// </summary>
    public static readonly string NL = Environment.NewLine;

    public static readonly int CPUThreads = Environment.ProcessorCount;

    public static object[] ToFriendlyStrings<T>(this FriendlyStringAndBackingValue<T>[] items)
    {
        object[] ret = new object[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            ret[i] = items[i].FriendlyString;
        }
        return ret;
    }

    public static class ByteSize
    {
        public const long KB = 1024;
        public const long MB = KB * 1024;
        public const long GB = MB * 1024;
    }

    [PublicAPI]
    public enum CompressionMethod
    {
        LZMA2,
        LZMA,
    }

    public static readonly UTF8Encoding UTF8NoBOM = new(false, true);

    private static readonly string[] _compressionMethodArgStrings =
    {
        "LZMA2",
        "LZMA",
    };

    public static string GetCompressionMethodString(CompressionMethod method) =>
        _compressionMethodArgStrings[(int)method];

    public static readonly ConfigData Config = new();

    private static FriendlyStringAndBackingValue<int>[] FillLzma2ThreadItems()
    {
        if (CPUThreads < 1)
        {
            return Array.Empty<FriendlyStringAndBackingValue<int>>();
        }

        int count = (CPUThreads * 2).Clamp(0, 256) + 1;

        FriendlyStringAndBackingValue<int>[] ret = new FriendlyStringAndBackingValue<int>[count];
        ret[0] = new FriendlyStringAndBackingValue<int>("Auto", -1);
        for (int i = 1; i < count; i++)
        {
            ret[i] = new FriendlyStringAndBackingValue<int>(i.ToStrInv(), i);
        }

        return ret;
    }

    private static FriendlyStringAndBackingValue<int>[] FillLzmaThreadItems()
    {
        if (CPUThreads < 1)
        {
            return Array.Empty<FriendlyStringAndBackingValue<int>>();
        }

        int threads = Math.Min(CPUThreads, 2);

        int count = threads + 1;

        FriendlyStringAndBackingValue<int>[] ret = new FriendlyStringAndBackingValue<int>[count];
        ret[0] = new FriendlyStringAndBackingValue<int>("Auto", -1);
        for (int i = 1; i < count; i++)
        {
            ret[i] = new FriendlyStringAndBackingValue<int>(i.ToStrInv(), i);
        }

        return ret;
    }

    public static readonly FriendlyStringAndBackingValue<int>[] Lzma2ThreadItems = FillLzma2ThreadItems();
    public static readonly FriendlyStringAndBackingValue<int>[] LzmaThreadItems = FillLzmaThreadItems();

    public static readonly FriendlyStringAndBackingValue<CompressionMethod>[] CompressionMethodItems =
    {
        new("* LZMA2", CompressionMethod.LZMA2),
        new("LZMA", CompressionMethod.LZMA),
    };

    /// <summary>
    /// Stores a filename/index pair for quick lookups into a zip file.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct NameAndIndex
    {
        public readonly string Name;
        public readonly int Index;

        public NameAndIndex(string name, int index)
        {
            Name = name;
            Index = index;
        }

        public NameAndIndex(string name)
        {
            Name = name;
            Index = -1;
        }
    }
}
