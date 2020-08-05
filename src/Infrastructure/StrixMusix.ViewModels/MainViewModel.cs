﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.Services.Settings;
using StrixMusic.ViewModels.Bindables;

namespace StrixMusix.ViewModels
{
    /// <summary>
    /// The MainViewModel used throughout the app
    /// </summary>
    public class MainViewModel : ObservableRecipient
    {
        private BindableLibrary? _pageContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public MainViewModel(ISettingsService settings)
        {
            IEnumerable<ICore> loadedCores = Ioc.Default.GetServices<ICore>();
            InitializeCores(loadedCores);
        }

        private async void InitializeCores(IEnumerable<ICore> loadedCores)
        {
            foreach (ICore core in loadedCores)
            {
                Library = await core.GetLibrary();
            }

            PageContent = new BindableLibrary(Library!);
        }

        /// <summary>
        /// The DataContext to be bound to the Shell's ContentPresenter.
        /// </summary>
        public BindableLibrary? PageContent
        {
            get => _pageContent;
            set => SetProperty(ref _pageContent, value);
        }

        /// <summary>
        /// A consolidated list of all users in the app
        /// </summary>
        public ObservableCollection<IUser>? Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<IDevice>? Devices { get; }

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public IPlayableCollectionGroup? Library { get; set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public IPlayableCollectionGroup? RecentlyPlayed { get; set; }

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public ObservableCollection<IPlayableCollectionGroup>? Discoverables { get; }

        /// <summary>
        /// Search results.
        /// </summary>
        public ISearchResults? SearchResults { get; }

        /// <summary>
        /// Current search query.
        /// </summary>
        public string SearchQuery { get; set; } = string.Empty;

        /// <summary>
        /// Autocomplete for the current search query.
        /// </summary>
        public IEnumerable<string>? SearchSuggestions { get; set; }
    }
}
