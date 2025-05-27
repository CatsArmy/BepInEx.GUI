global using System;
global using System.Collections.Generic;
global using System.Text;

using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using Mono.Cecil;

namespace BepInEx.GUI.Loader;

internal static class EntryPoint
{
    public static IEnumerable<string> TargetDLLs { get; } = Array.Empty<string>();

    public static void Patch(AssemblyDefinition _) { }

    public static void Initialize()
    {
        Log.Init();

        try
        {
            InitializeInternal();
        } catch (Exception e)
        {
            Log.Error($"Failed to initialize : ({e.GetType()}) {e.Message}{Environment.NewLine}{e}");
        }
    }

    private static void InitializeInternal()
    {
        Config.Init(Paths.ConfigPath);

        var consoleConfig = (ConfigEntry<bool>)typeof(BepInPlugin).Assembly.
            GetType("BepInEx.ConsoleManager", true).
            GetField("ConfigConsoleEnabled",
            BindingFlags.Static | BindingFlags.Public).GetValue(null);

        if (consoleConfig.Value)
        {
            Log.Info("BepInEx regular console is enabled, aborting launch.");
        }
        else if (Config.EnableBepInExGUIConfig.Value)
        {
            FindAndLaunchGUI();
        }
        else
        {
            Log.Info("Custom BepInEx.GUI is disabled in the config, aborting launch.");
        }
    }

    private static string FindGUIExecutable()
    {
        const string GuiFileName = "bepinex_gui";

        var path = Directory.GetParent(typeof(EntryPoint).Assembly.Location);
        var gui = $"{path}/{GuiFileName}.exe";
        if (File.Exists(gui))
        {
            return gui;
        }

        foreach (var filePath in Directory.GetFiles(Paths.PatcherPluginPath, "*", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(filePath);


            // No platform check because proton is used for RoR2 and it handles it perfectly anyway:
            // It makes the Process.Start still goes through proton and makes the bep gui
            // that was compiled for Windows works fine even in linux operating systems.

            if (fileName == $"{GuiFileName}.exe")
            {
                var versInfo = FileVersionInfo.GetVersionInfo(filePath);
                if (versInfo.FileMajorPart == 3)
                {
                    Log.Info($"Found bepinex_gui v3 executable in {filePath}");
                    return filePath;
                }
            }
        }

        return null;
    }

    private static void FindAndLaunchGUI()
    {
        Log.Info("Finding and launching GUI");

        var executablePath = FindGUIExecutable();
        if (executablePath != null)
        {
            var freePort = FindFreePort();
            var process = LaunchGUI(executablePath, freePort);
            if (process != null)
            {
                Logger.Listeners.Add(new SendLogToClientSocket(freePort));
                Logger.Listeners.Add(new CloseProcessOnChainloaderDone(process));
            }
            else
            {
                Log.Info("LaunchGUI failed");
            }
        }
        else
        {
            Log.Info("bepinex_gui executable not found.");
        }
    }

    private static int FindFreePort()
    {
        int port = 0;
        Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            IPEndPoint localEP = new(IPAddress.Any, 0);
            socket.Bind(localEP);
            localEP = (IPEndPoint)socket.LocalEndPoint;
            port = localEP.Port;
        }
        finally
        {
            socket.Close();
        }

        return port;
    }

    private static Process LaunchGUI(string executablePath, int socketPort)
    {
        var processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = executablePath;
        processStartInfo.WorkingDirectory = Path.GetDirectoryName(executablePath);

        processStartInfo.Arguments =
            $"\"{typeof(Paths).Assembly.GetName().Version}\" " +
            $"\"{Paths.ProcessName}\" " +
            $"\"{Paths.GameRootPath}\" " +
            $"\"{EntryPoint.GetLogOutputFilePath()}\" " +
            $"\"{Config.ConfigFilePath}\" " +
            $"\"{Process.GetCurrentProcess().Id}\" " +
            $"\"{socketPort}\"";

        return Process.Start(processStartInfo);
    }

    private static string GetLogOutputFilePath()
    {
        var parent = Directory.GetParent(Paths.PluginPath);
        string path = $"{parent}/LogOutput.log";
        if (!File.Exists(path))
        {
            foreach (var file in parent.EnumerateFiles())
            {
                if (file.Extension.ToLower() == ".log")
                {
                    path = file.FullName;
                    break;
                }
            }
        }

        return path;
    }
}
