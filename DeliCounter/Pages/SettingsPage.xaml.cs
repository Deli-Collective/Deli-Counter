﻿using DeliCounter.Backend;
using DeliCounter.Controls;
using DeliCounter.Controls.Abstract;
using DeliCounter.Properties;
using ModernWpf;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace DeliCounter.Pages
{
    /// <summary>
    ///     Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        private readonly Settings _settings;

        public SettingsPage()
        {
            InitializeComponent();
            _settings = App.Current.Settings;
            DataContext = _settings;

            ComboBoxModListItem.ItemsSource = Enum.GetValues(typeof(ModListItemType));
            ComboBoxModListItem.SelectedIndex = _settings.ListItemType;
        }

        private void AutoDetectGameLocation_OnChecked(object sender, RoutedEventArgs e)
        {
            _settings.GameLocation = App.Current.SteamAppLocator.AppLocation;

            // If it wasn't set revert since it can't be found automatically
            if (string.IsNullOrEmpty(_settings.GameLocation))
            {
                _settings.AutoDetectGameLocation = false;
                var dialogue = new AlertDialogue("Couldn't detect game folder",
                    "Hey! It seems we couldn't auto-detect your game folder. Please set it manually!");
                App.Current.QueueDialog(dialogue);
            }
        }

        private void DarkMode_OnChecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme =
                _settings.EnableDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        private void GenerateDiagnosticFile(object sender, RoutedEventArgs e)
        {
            App.Current.DiagnosticInfoCollector.CollectAll();
            App.Current.QueueDialog(new AlertDialogue("Complete", "The diagnostic file has been saved to the desktop!"));
        }

        private void ShowModBetas_OnChecked(object sender, RoutedEventArgs e)
        {
            ModRepository.Instance.Refresh();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _settings.ListItemType = ComboBoxModListItem.SelectedIndex;
        }

        private void CleanInstallFolder(object sender, RoutedEventArgs e)
        {
            string loc = App.Current.Settings.GameLocation;
            if (!string.IsNullOrWhiteSpace(loc) && Directory.Exists(loc))
            {
                bool success = App.Current.DiagnosticInfoCollector.CleanInstallFolder(loc);
                if (success)
                    App.Current.QueueDialog(new AlertDialogue("Done", "Your install folder has been cleaned."));
                else
                    App.Current.QueueDialog(new AlertDialogue("Whoops", "Looks like I wasn't able to delete some files, please make sure you have the game closed before running this. You can try again in the settings page."));
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}