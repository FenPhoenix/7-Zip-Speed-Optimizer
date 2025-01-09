using System.Runtime.InteropServices;

namespace SevenZipSpeedOptimizer;

public interface IEventDisabler
{
    /// <summary>
    /// True if greater than 0.
    /// </summary>
    int EventsDisabled { get; set; }
}

[StructLayout(LayoutKind.Auto)]
internal readonly ref struct DisableEvents
{
    /*
    For some reason VS 17.6+ no longer allows "using ref struct" statements in async methods, even if there is
    no actual async call IN the using block. So we can use these in a manual try-finally statement ourselves to
    still avoid the allocation of a class-based version. Yeesh...
    */
    public static void Open(IEventDisabler obj, bool active = true)
    {
        if (active) obj.EventsDisabled++;
    }

    public static void Close(IEventDisabler obj, bool active = true)
    {
        if (active) obj.EventsDisabled = (obj.EventsDisabled - 1).ClampToZero();
    }

    private readonly bool _active;
    private readonly IEventDisabler _obj;
    public DisableEvents(IEventDisabler obj, bool active = true)
    {
        _active = active;
        _obj = obj;

        if (_active) _obj.EventsDisabled++;
    }

    public void Dispose()
    {
        if (_active) _obj.EventsDisabled = (_obj.EventsDisabled - 1).ClampToZero();
    }
}

public interface IUpdateRegion
{
    void BeginUpdate();
    void EndUpdate();
}

[StructLayout(LayoutKind.Auto)]
internal readonly ref struct UpdateRegion
{
    private readonly IUpdateRegion _obj;

    public UpdateRegion(IUpdateRegion obj)
    {
        _obj = obj;
        _obj.BeginUpdate();
    }

    public void Dispose()
    {
        _obj.EndUpdate();
    }
}
