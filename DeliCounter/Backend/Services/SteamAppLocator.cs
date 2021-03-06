﻿using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeliCounter.Backend
{
    public sealed class SteamAppLocator
    {
        /// <summary>
        ///     Steam application id
        /// </summary>
        public int AppId { get; }

        /// <summary>
        ///     Steam application folder name
        /// </summary>
        public string AppFolderName { get; }

        /// <summary>
        ///     Application executable name
        /// </summary>
        public string AppExecutableName { get; }

        /// <summary>
        ///     Returns the path to the H3 game directory (or null if not found)
        /// </summary>
        public string AppLocation { get; private set; }

        /// <summary>
        ///     Returns the path to the game executable (or null if not found)
        /// </summary>
        public string ExecutablePath => Path.Combine(AppLocation, AppExecutableName);

        public bool IsRunning
        {
            get
            {
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(AppExecutableName));
                return processes.Length > 0;
            }
        }

        public SteamAppLocator(int appId, string folderName, string executableName)
        {
            AppId = appId;
            AppFolderName = folderName;
            AppExecutableName = executableName;
            Locate();
        }

        /// <summary>
        ///     Attempts to locate the game
        /// </summary>
        public void Locate()
        {
            // Get the main steam installation location via registry.
            var steamDir = (
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", "") ??
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", ""))
                as string;

            // If we can't find it, return. This should really only happen if Steam isn't installed.
            if (string.IsNullOrEmpty(steamDir))
            {
                AppLocation = null;
                return;
            }


            // Check main steamapps library folder for h3 manifest.
            var result = "";
            if (File.Exists(Path.Combine(steamDir, @$"steamapps\appmanifest_{AppId}.acf")))
            {
                result = Path.Combine(steamDir, $@"steamapps\common\{AppFolderName}\");
            }
            else
            {
                // We didn't find it, look at other library folders by lazily parsing libraryfolders.
                var libraryFolders = Path.Combine(steamDir, @"steamapps\libraryfolders.vdf");
                foreach (Match match in Regex.Matches(File.ReadAllText(libraryFolders), @"^\s+\""path\""\s+\""(.+)\""$", RegexOptions.Multiline))
                {
                    var folder = match.Groups[1].Value;
                    if (!File.Exists(Path.Combine(folder, $"steamapps/appmanifest_{AppId}.acf"))) continue;
                    result = Path.Combine(folder, $"steamapps/common/{AppFolderName}");
                }
            }

            AppLocation = string.IsNullOrEmpty(result) ? null : Path.GetFullPath(result);
        }
    }
}