﻿using DeliCounter.Properties;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DeliCounter.Backend.ModOperation
{
    public class UninstallModOperation : ModOperation
    {
        public UninstallModOperation(Mod mod) : base(mod, mod.InstalledVersion)
        {
        }

        public override async Task Run()
        {
            await base.Run();

            var cached = Mod.Cached;

            await Task.Run(() =>
            {
                var gameLocation = App.Current.Settings.GameLocationOrError;
                foreach (var item in cached.Files)
                {
                    var path = Path.Combine(gameLocation, item);
                    if (string.IsNullOrWhiteSpace(gameLocation) || string.IsNullOrWhiteSpace(item) ||
                        !path.Contains(gameLocation)) return;

                    if (File.Exists(path)) File.Delete(path);
                    else if (Directory.Exists(path)) Directory.Delete(path, true);
                }

                DeleteEmptyDirectories(gameLocation);
            });
            Mod.InstalledVersion = null;
            Mod.Cached = null;

            // If this mod version doesn't have a download URL also remove it from the database
            var ver = Mod.Versions[VersionNumber];
            if (ver.DownloadUrl is null)
                Mod.Versions.Remove(VersionNumber);

            // If the mod no longer has any versions left, just yeet it.
            if (Mod.Versions.Count == 0)
                ModRepository.Instance.Mods.Remove(Mod.Guid);

            Completed = true;
        }

        private static void DeleteEmptyDirectories(string startLocation)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(startLocation))
                {
                    DeleteEmptyDirectories(directory);
                    if (Directory.GetFiles(directory).Length == 0 &&
                        Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                    }
                }
            } catch (DirectoryNotFoundException)
            {
                // Empty catch. I have NO idea how it's even possible to get into this scenario but SOMEONE managed to so.
            }
        }
    }
}