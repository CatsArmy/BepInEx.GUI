﻿using BepInEx.Logging;

namespace BepInEx.GUI.Loader;

internal static class Log
{
    internal static ManualLogSource _logSource { get; set; }

    internal static void Init() => _logSource = Logger.CreateLogSource("BepInEx.GUI.Loader");

    internal static void Debug(object data) => _logSource.LogDebug(data);
    internal static void Error(object data) => _logSource.LogError(data);
    internal static void Fatal(object data) => _logSource.LogFatal(data);
    internal static void Info(object data) => _logSource.LogInfo(data);
    internal static void Message(object data) => _logSource.LogMessage(data);
    internal static void Warning(object data) => _logSource.LogWarning(data);
}
