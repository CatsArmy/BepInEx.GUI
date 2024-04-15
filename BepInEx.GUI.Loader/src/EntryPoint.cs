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
    #region Preloader class structure
    public static IEnumerable<string> TargetDLLs { get; } = [];

    public static void Patch(AssemblyDefinition _) { }

    public static void Initialize()
    {
        Log.Init();
        try
        {
            InitializeInternal();
        }
        catch (Exception e)
        {
            Log.Error($"Failed to initialize : ({e.GetType()}) {e.Message}{Environment.NewLine}{e}");
        }
    }
    #endregion

    public static int Port { get; private set; } = 0;


    private static void InitializeInternal()
    {
        Config.Init(Paths.ConfigPath);

        var BepInExConsoleManager = typeof(BepInPlugin).Assembly.GetType("BepInEx.ConsoleManager", true);
        ConfigEntry<bool> consoleConfig =
                    (ConfigEntry<bool>)BepInExConsoleManager.GetField("ConfigConsoleEnabled",
                    BindingFlags.Static | BindingFlags.Public).GetValue(null);

        if (!Config.EnableBepInExGUIConfig.Value)
        {
            Log.Info("Custom BepInEx.GUI is disabled in the config, aborting launch.");
            return;
        }

        if (consoleConfig.Value)
        {
            consoleConfig.Value = false;
            Log.Warning("Disabled old console restart game for changes to take effect");
            return;
        }

        FindAndLaunchGUI();
    }

    #region Find Executable Path
    private static string FindGUIExecutable()
    {
        const string GuiFileName = "bepinex_gui";
        Assembly Assembly = typeof(EntryPoint);
        string ParentLocation = Directory.GetParent(Assembly.Location);
        if (ParentLocation.EndsWith("patchers") && ParentLocation[ParentLocation.Length - 1] != '\\')
        {
            ParentLocation = "\\";
        }

        string PatchersDir = $"{Directory.GetParent(ParentLocation)}\\plugins";
        string PatcherLocation = $"{ParentLocation}{GuiFileName}.exe";
        if (File.Exists(PatcherLocation))
        {
            return PatcherLocation;
        }

        string PluginsDir = $"{Directory.GetParent(ParentLocation)}\\plugins";
        string PluginsLocation = $"{PatchersDir}{GuiFileName}.exe";
        if (File.Exists(PluginsLocation))
        {
            return PluginsLocation;
        }
        //If quick find did not find the executable
        //then perform a manual search over all files in BepInEx/plugins and/or BepInEx/patcher

        //Search patchers Dir
        string executable = null;
        executable = SearchGUIExecutable(PatchersDir);
        if (executable != null)
        {
            return executable;
        }

        //Search plugins Dir
        executable = SearchGUIExecutable(PluginsDir);

        if (executable != null)
        {
            return executable;
        }

        return executable;
    }

    private static string SearchGUIExecutable(string ParentDir)
    {
        string[] Directorys = Directory.GetDirectories(ParentDir);
        foreach (string Directory in Directorys)
        {
            string[] Files = Directory.GetFiles(ParentDir);
            string[] Folders = Directory.Split('\\');
            string Folder = Folders[Folders.Length - 1];
            if (!Folder.EndsWith('\\'))
            {
                Folder += "\\";
            }
            foreach (string file in Files)
            {
                string FileName = file.Split(Folder);
                if (FileName == $"{GuiFileName}.exe")
                {
                    return file;
                }
            }
        }
        return null;
    }
    #endregion
    private static int FindFreePort()
    {
        Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            IPEndPoint localEP = new(IPAddress.Any, Port);
            socket.Bind(localEP);
            localEP = (IPEndPoint)socket.LocalEndPoint;
            Port = localEP.Port;
        }
        finally
        {
            socket.Close();
        }

        return Port;
    }

    private static void FindAndLaunchGUI()
    {
        Log.Info("Finding and launching GUI");

        string exePath = FindGUIExecutable();
        if (exePath == null)
        {
            Log.Info("bepinex_gui executable not found.");
        }

        Port = FindFreePort();

        Process process = LaunchGUI(exePath, Port);
        if (process == null)
        {
            Log.Info("LaunchGUI failed");
        }

        Logger.Listeners.Add(new SendLogToClientSocket(Port));
        Logger.Listeners.Add(new CloseProcessOnChainloaderDone(process));
    }

    private const readonly string[] args =
        [
            $"\"{typeof(Paths).Assembly.GetName().Version}\"", //arg[1] Version
            $"\"{Paths.ProcessName}\"",                        //arg[2] Target name
            $"\"{Paths.GameRootPath}\"",                       //arg[3] Game folder -P -F
            $"\"{Paths.BepInExRootPath}\\LogOutput.log\"",     //arg[4] BepInEx output -P -F
            $"\"{Config.ConfigFilePath}\"",                    //arg[5] ConfigPath
            $"\"{Process.GetCurrentProcess().Id}\"",           //arg[6] Process Id
            $"\"{socketPort}\"",                               //arg[7] socket port reciver
        ];

    private static Process LaunchGUI(string exePath, int socketPort)
    {
        if (exePath == null)
        {
            return null;
        }


        ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName: exePath, FormatArguments(args))
        {
            WorkingDirectory = Path.GetDirectoryName(exePath)
        };
        return Process.Start(processStartInfo);
    }

    private static string FormatArguments(this string[] kwargs)
    {
        string args = string.Empty;
        foreach (string arg in kwargs)
        {
            args += arg;
        }
        return args;
    }
}
