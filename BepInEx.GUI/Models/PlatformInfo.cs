﻿using MonoMod.Utils;

namespace BepInEx.GUI.Models
{
    public class PlatformInfo
    {
        public Platform Current { get; }

        public bool IsWindows { get; }
        public bool IsLinux { get; }
        public bool IsMacOs { get; }
        
        public PlatformInfo(string[] args)
        {
            const Platform windowsX64Platform = Platform.Windows | Platform.Bits64;
            const Platform linuxX64Platform = Platform.Linux | Platform.Bits64;
            const Platform macOsX64Platform = Platform.MacOS | Platform.Bits64;

            int.TryParse(args[0], out var current);
            Current = (Platform)current;

            IsWindows = (Current & windowsX64Platform) == Current;
            IsLinux = (Current & linuxX64Platform) == Current;
            IsMacOs = (Current & macOsX64Platform) == Current;
        }
    }
}
