using LogLevel = BepInEx.Logging.LogLevel;

namespace BepInEx.GUI.Loader.Rust;

internal unsafe partial struct Utf8Str_cs
{
    public Utf8Str_cs(string str)
    {
        var utf8_str = Utility.UTF8NoBom.GetBytes(str);
        fixed (byte* p = utf8_str)
        {
            this.utf8_str = p;
            this.utf8_len = str.Length;
        }
    }
}

internal unsafe partial struct LogSourceRaw
{
    public LogSourceRaw(Utf8Str_cs source, Version version)
    {
        this.source = source;
        this.version = version;
    }
}

internal unsafe partial struct Version
{
    public Version(System.Version version) : this(version.Major, version.Minor, version.Build) { }

    public Version(int major, int minor, int patch)
    {
        this.major = major;
        this.minor = minor;
        this.patch = patch;
    }
}
internal unsafe partial struct LogEventRaw
{
    public LogEventRaw(string data, LogLevel level, string source, System.Version version)
        : this(data, level.Into(), source, new Version(version)) { }

    public LogEventRaw(string data, BepInExLogLevel level, string source, System.Version version)
        : this(data, level, source, new Version(version)) { }

    public LogEventRaw(string data, BepInExLogLevel level, string source, Version version)
        : this(data, level, new LogSourceRaw(new(source), version)) { }

    public LogEventRaw(string data, LogLevel level, string source, Version version)
        : this(data, level.Into(), new LogSourceRaw(new(source), version)) { }

    public LogEventRaw(string data, BepInExLogLevel level, LogSourceRaw source)
    {
        this.data = new Utf8Str_cs(data);
        this.level = level;
        this.source = source;
    }

}

internal static class LogLevelExtenstions
{
    public static BepInExLogLevel Into(this LogLevel level) => level switch
    {
        LogLevel.None => BepInExLogLevel.None,
        LogLevel.Fatal => BepInExLogLevel.Fatal,
        LogLevel.Error => BepInExLogLevel.Error,
        LogLevel.Warning => BepInExLogLevel.Warning,
        LogLevel.Message => BepInExLogLevel.Message,
        LogLevel.Info => BepInExLogLevel.Info,
        LogLevel.Debug => BepInExLogLevel.Debug,
        LogLevel.All => BepInExLogLevel.All,
        _ => BepInExLogLevel.None,
    };

}
