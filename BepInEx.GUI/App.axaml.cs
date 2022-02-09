using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BepInEx.GUI.Config;
using BepInEx.GUI.Models;
using BepInEx.GUI.ViewModels;
using BepInEx.GUI.Views;
using System;
using WebSocketSharp;

namespace BepInEx.GUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Startup += (sender, eventArgs) =>
                {
                    AppDomain.CurrentDomain.UnhandledException += ShowUnhandledException;

                    var args = DefaultArgsIfNoneProvided(eventArgs.Args);

                    var platformInfo = new PlatformInfo(args);
                    var pathsInfo = new PathsInfo(args);
                    var targetInfo = new TargetInfo(args);

                    var webSocket = new WebSocket("ws://localhost:5892/Log");
                    webSocket.Connect();

                    MainConfig.Init(pathsInfo.ConfigFilePath);

                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = new MainWindowViewModel(webSocket, pathsInfo, platformInfo, targetInfo),
                    };

                    // Needed or the target closes
                    desktop.MainWindow.Closing += (sender, e) =>
                    {
                        webSocket.Close();
                    };
                };       
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ShowUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Debug.Message(ex.ToString());
        }

        private static string[] DefaultArgsIfNoneProvided(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[]
                {
                    (0x25 | 0x2).ToString(), // win64
                    "Unknown Version",
                    "Unknown Target",

                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                };
            }

            return args;
        }
    }
}
