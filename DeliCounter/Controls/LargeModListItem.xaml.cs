﻿using DeliCounter.Backend;
using DeliCounter.Controls.Abstract;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DeliCounter.Controls
{
    /// <summary>
    ///     Interaction logic for ModListItem.xaml
    /// </summary>
    public partial class LargeModListItem : ModListItem
    {
        public LargeModListItem(Mod mod, bool showIcon) : base(mod)
        {
            InitializeComponent();

            // Get the version to display and edit the name and short description
            Mod.ModVersion version = mod.Latest;

            ModTitle.Text = version.Name;
            ModShortDescription.Text = version.ShortDescription;

            if (showIcon)
            {
                // Set the icon
                if (version.IconUrl is not null)
                {
                    try
                    {
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(version.IconUrl, UriKind.Absolute);
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.DecodePixelHeight = 64;
                        bi.DecodePixelWidth = 64;
                        bi.EndInit();
                        ModImage.Source = bi;
                    } catch (IOException)
                    {
                        // Ignored. Happens when it can't create a temporary file to download the image.
                    }
                }
            } else
            {
                ModImage.Visibility = Visibility.Collapsed;
            }
            

            // Update the local status icons (Up to date, update available)
            if (mod.IsInstalled)
            {
                LocalStatusIcon.Text = mod.UpToDate ? SegoeGlyphs.Checkmark : SegoeGlyphs.Repeat;
                LocalStatusIcon.Foreground = new SolidColorBrush(mod.UpToDate ? Colors.Green : Colors.DarkOrange);
            }
            else if (version.IncompatibleInstalledMods.Any())
            {
                LocalStatusIcon.Text = "";
            }
            else
            {
                LocalStatusIcon.Text = SegoeGlyphs.Download;
                LocalStatusIcon.Style = (Style)Application.Current.Resources["BaseTextBlockStyle"];
            }
        }
    }
}