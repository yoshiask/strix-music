﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Settings;
using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.SuperShell;
using StrixMusic.Shell.Default.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Fires when the Super buttons is clicked. Temporary until a proper trigger mechanism is found for touch devices.
        /// </summary>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Ioc.Default.GetService<ISuperShellService>().Show(Sdk.Services.SuperShell.SuperShellDisplayState.Settings);
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            await Initialize();
            AttachEvents();

            // Events must be attached before initializing if you want them to fire correctly.
            await Ioc.Default.GetService<IFileSystemService>().Init();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AttachEvents()
        {
            var settingsSvc = Ioc.Default.GetService<ISettingsService>();

            settingsSvc.SettingChanged += SettingsService_SettingChanged;
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetService<ISettingsService>().SettingChanged -= SettingsService_SettingChanged;
        }

        private async void SettingsService_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.Key == nameof(SettingsKeys.PreferredShell))
            {
                await SetupPreferredShell();
            }
        }

        private async Task Initialize()
        {
            // TODO: Remove or replace.
            await SetupPreferredShell();

            SuperShellDisplay.Content = new SuperShell();
        }

        private async Task SetupPreferredShell()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Removes the current shell.
                ShellDisplay.Content = null;

                // Removes old resource(s).
                foreach (var dict in Application.Current.Resources.MergedDictionaries)
                {
                    Match shellMatch = Regex.Match(dict.Source.AbsoluteUri, Constants.Shells.ShellResourceDictionaryRegex);
                    if (shellMatch.Success)
                    {
                        // Skips removing the default ResourceDictionary.
                        if (shellMatch.Groups[1].Value == Constants.Shells.DefaultShellAssemblyName)
                            continue;

                        Application.Current.Resources.MergedDictionaries.Remove(dict);
                        break;
                    }
                }

                // Gets the preferred shell from settings.
                string preferredShell = await Ioc.Default.GetService<ISettingsService>().GetValue<string>(nameof(SettingsKeys.PreferredShell));

                // Makes sure the saved shell is valid, falls back to Default.
                if (preferredShell == null || !Constants.Shells.LoadedShells.ContainsKey(preferredShell))
                {
                    preferredShell = Constants.Shells.DefaultShellAssemblyName;
                }

                if (preferredShell != Constants.Shells.DefaultShellAssemblyName)
                {
                    // Loads the preferred shell
                    var resourcePath = $"{Constants.ResourcesPrefix}{Constants.Shells.ShellNamespacePrefix}.{preferredShell}/{Constants.Shells.ShellResourcesSuffix}";
                    var resourceDictionary = new ResourceDictionary() { Source = new Uri(resourcePath) };
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }

                ShellDisplay.Content = CreateShellControl();
            });
        }

        private ShellContainer CreateShellControl()
        {
            return new ShellContainer
            {
                DataContext = MainViewModel.Singleton,
            };
        }
    }
}
