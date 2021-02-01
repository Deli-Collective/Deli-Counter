﻿using System;
using System.Collections.Generic;
using System.Linq;
using Deli_Counter.Properties;
using Deli_Counter.XAML;
using Deli_Counter.XAML.Pages;
using ModernWpf;
using ModernWpf.Controls;

namespace Slicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow? Instance;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            // Set the page to the home page when we start
            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(_pages["home"]);

            if (Settings.Default.DarkMode)
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            else
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
        }

        /// <summary>
        ///     This is a dictionary of the pages accessible from the navigation menu
        /// </summary>
        private readonly Dictionary<string, object> _pages = new()
        {
            ["home"] = new HomePage(),
            ["installed"] = null,
            ["settings"] = new SettingsPage(),
            ["mods"] = null,
            ["not_found"] = null
        };
        
        /// <summary>
        ///     Called when the user clicks (invokes) an item in the nav menu
        /// </summary>
        private void NavView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // Get the tag that was clicked and navigate the content frame to the correct screen
            var tag = args.IsSettingsInvoked ? "settings" : args.InvokedItemContainer.Tag.ToString();
            ContentFrame.Navigate(_pages[tag ?? "not_found"]);
        }
    }
}