using BepInEx.Configuration;
using System;
using System.IO;

namespace BepInEx.GUI.Config
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static class MainConfig
    {
        public const string FileName = "BepInEx.GUI.cfg";

        public static ConfigFile File { get; private set; }

        public const string EnableDeveloperToolsText = "Enable Developer Tools";
        public static ConfigEntry<bool> EnableDeveloperToolsConfig { get; private set; }

        /// <summary>
        /// This is done through LocalApplicationData because
        /// the BepInEx.GUI cfg file may be copied across different users 
        /// by r2modman profile sharing feature
        /// thus they may never end up seeing the one time only disclaimer
        /// </summary>
        public static bool ShowOneTimeOnlyDisclaimerConfig
        {
            get
            {
                try
                {
                    var localAppDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (string.IsNullOrWhiteSpace(localAppDataFolderPath))
                    {
                        return false;
                    }

                    var bepinexGuiAppDataFolderPath = Path.Combine(localAppDataFolderPath, "BepInEx.GUI");

                    var alreadyShownDisclaimer = Directory.Exists(bepinexGuiAppDataFolderPath);
                    if (alreadyShownDisclaimer)
                    {
                        return false;

                    }
                    else
                    {
                        Directory.CreateDirectory(bepinexGuiAppDataFolderPath);
                        return true;
                    }
                }
                catch (Exception)
                {
                }

                return false;
            }
        }

        public const string CloseWindowWhenGameLoadedConfigKey = "Close Window When Game Loaded";
        public const string CloseWindowWhenGameLoadedConfigDescription = "Close the graphic user interface window when the game is loaded";
        public static ConfigEntry<bool> CloseWindowWhenGameLoadedConfig { get; private set; }

        public const string CloseWindowWhenGameClosesConfigKey = "Close Window When Game Closes";
        public const string CloseWindowWhenGameClosesConfigDescription = "Close the graphic user interface window when the game closes";
        public static ConfigEntry<bool> CloseWindowWhenGameClosesConfig { get; private set; }

        public const string EnableBepInExGUIConfigKey = "Enable BepInEx GUI";
        public const string EnableBepInExGUIConfigDescription = "Enable the custom BepInEx GUI";
        public static ConfigEntry<bool> EnableBepInExGUIConfig { get; private set; }

        public static void Init(string configFilePath)
        {
            File = new ConfigFile(configFilePath, true);

            EnableDeveloperToolsConfig = File.Bind("Settings", EnableDeveloperToolsText, false, EnableDeveloperToolsText);

            CloseWindowWhenGameLoadedConfig = File.Bind("Settings", CloseWindowWhenGameLoadedConfigKey, false, CloseWindowWhenGameLoadedConfigDescription);

            CloseWindowWhenGameClosesConfig = File.Bind("Settings", CloseWindowWhenGameClosesConfigKey, true, CloseWindowWhenGameClosesConfigDescription);

            EnableBepInExGUIConfig = File.Bind("Settings", EnableBepInExGUIConfigKey, true, EnableBepInExGUIConfigDescription);
        }
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
