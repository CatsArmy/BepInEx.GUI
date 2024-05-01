using MonoMod.Utils;
using static Zx.Env;

namespace BepInEx.GUI.Loader
{
    internal static class Shell
    {
        public static async void RunShell(string Path, string Destination)
        {

            //if windows or powershell
            if (PlatformHelper.Is(Platform.Windows))
            {
                shell = "powershell.exe -NoProfile";
                const string Export_Icon =
                @"Add-Type -AssemblyName System.Drawing
                function Export-Icon {
                [CmdletBinding()]

                Param (
	            	# Path of the source file the icon is to be exported from.
	            	[Parameter(Mandatory=$true)]
                    [string]$Path,

	            	# Path to where the icon will be saved to. If this is blank or missing
	            	# the file will be saved in the current directory with the basename of
	            	# the $Path with the correct format extension.
	            	[string]$Destination,

	            	# Format to save the icon as. Defaults to a bitmap (BMP). 
	            	[System.Drawing.Imaging.ImageFormat]$Format = ""Png""
                )


                if (!$Destination) {
	            	$basename = (Get - Item $Path).BaseName
	            	$extension = $format.ToString().ToLower()
	            	$Destination = ""$basename.$extension""

                }

	            # Extract the icon from the source file.
	            $icon = [System.Drawing.Icon]::ExtractAssociatedIcon($Path)

	            # Save the icon in the specified format
	            $icon.ToBitmap().Save($Destination, [System.Drawing.Imaging.ImageFormat]::Png)

                echo ""Script is done""
                }";
                _ = await run($"{Export_Icon}{Environment.NewLine}Export-Icon {(
                    $@"-{nameof(Path)} ""{Path}""" +
                    $@"-{nameof(Destination)} ""{Destination}""")}");
                //_ = await run($"{Export_Icon}{Environment.NewLine}{"Export-Icon " +
                //    "-Path \"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Lethal Company\\Lethal Company.exe\" " +
                //    "-Destination \"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Lethal Company\\BepInEx.GUI.Icon\\icon.png\""}");

            }
        }
    }
}

