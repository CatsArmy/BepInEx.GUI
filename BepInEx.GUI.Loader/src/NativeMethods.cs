// <auto-generated>
// This code is generated by csbindgen.
// DON'T CHANGE THIS DIRECTLY.
// </auto-generated>
#pragma warning disable CS8500
#pragma warning disable CS8981
using System;
using System.Runtime.InteropServices;


namespace BepInEx.GUI.Loader.Rust
{
    internal static unsafe partial class NativeMethods
    {
        const string __DllName = "bepinex_gui";



        [DllImport(__DllName, EntryPoint = "send", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void send(LogEventRaw log);


    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe partial struct Utf8Str_cs
    {
        public byte* utf8_str;
        public int utf8_len;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe partial struct LogEventRaw
    {
        public Utf8Str_cs data;
        public BepInExLogLevel level;
        public LogSourceRaw source;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe partial struct LogSourceRaw
    {
        public Utf8Str_cs source;
        public Version version;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe partial struct Version
    {
        public int major;
        public int minor;
        public int patch;
    }


    internal enum BepInExLogLevel : byte
    {
        None = 0,
        Fatal = 1,
        Error = 2,
        Warning = 4,
        Message = 8,
        Info = 16,
        Debug = 32,
        All = 64,
    }


}