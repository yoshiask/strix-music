﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Extensions;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;
using StrixMusic.ViewModels.Bindables;

namespace StrixMusic.ViewModels
{
    /// <summary>
    /// The MainViewModel used throughout the app
    /// </summary>
    public class MainViewModel : ObservableRecipient
    {
        private readonly IReadOnlyList<ICore> _cores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public MainViewModel()
        {
            LoadLibraryCommand = new AsyncRelayCommand(LoadLibraryAsync);
            LoadDevicesCommand = new AsyncRelayCommand(LoadDevicesAsync);
            LoadRecentlyPlayedCommand = new AsyncRelayCommand(LoadRecentlyPlayedAsync);
            LoadDiscoverablesCommand = new AsyncRelayCommand(LoadDiscoverablesAsync);

            Devices = new ObservableCollection<IDevice>();

            _cores = Ioc.Default.GetServices<ICore>().ToArray();

            _ = InitializeCores(_cores);
        }

        private async Task InitializeCores(IEnumerable<ICore> coresToLoad)
        {
            await coresToLoad.InParallel(core => Task.Run(core.InitAsync));

            foreach (var core in coresToLoad)
            {
                Users.Add(core.User);
                AttachEvents(core);
            }
        }

        private void AttachEvents(ICore core)
        {
            core.DevicesChanged += Core_DevicesChanged;
            core.CoreStateChanged += Core_CoreStateChanged;
            core.SearchResultsChanged += Core_SearchResultsChanged;
        }

        private void DetachEvents(ICore core)
        {
            core.DevicesChanged -= Core_DevicesChanged;
            core.CoreStateChanged -= Core_CoreStateChanged;
            core.SearchResultsChanged -= Core_SearchResultsChanged;
        }

        private void Core_SearchResultsChanged(object sender, ISearchResults e)
        {
            if (sender is ICore core)
            {
                // todo: rethink merging search results / rethink storing search results per core.
            }
        }

        private void Core_CoreStateChanged(object sender, CoreState e)
        {

            // TODO - create a "bindable core" object with basic properties about the core (to be used in the UI), and save them in a list.
        }

        private void Core_DevicesChanged(object sender, CoreInterfaces.CollectionChangedEventArgs<IDevice> e)
        {
            foreach (var device in e.AddedItems)
            {
                Devices.Add(device);
            }

            foreach (var device in e.RemovedItems)
            {
                Devices.Remove(device);
            }
        }

        /// <summary>
        /// Contains data about the cores that are loaded.
        /// </summary>
        public ObservableCollection<BindableCoreData> BindableCores { get; } = new ObservableCollection<BindableCoreData>();

        /// <summary>
        /// A consolidated list of all users in the app.
        /// </summary>
        public ObservableCollection<IUser> Users { get; } = new ObservableCollection<IUser>();

        /// <summary>
        /// All available devices.
        /// </summary>
        public ObservableCollection<IDevice> Devices { get; }

        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public BindableLibrary? Library { get; set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public BindableRecentlyPlayed RecentlyPlayed { get; set; } = new BindableRecentlyPlayed();

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public ObservableCollection<BindableCollectionGroup>? Discoverables { get; } = new ObservableCollection<BindableCollectionGroup>();

        /// <summary>
        /// Search results.
        /// </summary>
      //  public BindableSearchResults SearchResults { get; } = new BindableSearchResults(); //How to initialize it? comment:Amaid

        /// <summary>
        /// Current search query.
        /// </summary>
        public string SearchQuery { get; set; } = string.Empty;

        /// <summary>
        /// Autocomplete for the current search query.
        /// </summary>
        public ObservableCollection<string>? SearchSuggestions { get; set; }

        /// <summary>
        /// Loads the <see cref="RecentlyPlayed"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadRecentlyPlayedCommand { get; }

        private async Task<BindableRecentlyPlayed> LoadRecentlyPlayedAsync()
        {
            var recents = await _cores.InParallel(core => Task.Run(core.GetRecentlyPlayedAsync)).ConfigureAwait(false);

            // TODO: Re-evaluate. We might not need to merge them like this, just replace the existing items per core.
            var mergedRecents = Mergers.MergeRecentlyPlayed(recents);

            RecentlyPlayed = mergedRecents;

            return mergedRecents;
        }

        /// <summary>
        /// Loads the <see cref="RecentlyPlayed"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadDiscoverablesCommand { get; }

        private async Task<IAsyncEnumerable<IPlayableCollectionGroup>?> LoadDiscoverablesAsync()
        {
            var discoverables = await _cores.InParallel(core => Task.Run(core.GetDiscoverablesAsync)).ConfigureAwait(false);
            var mergedDiscoverables = Mergers.MergePlayableCollectionGroups(discoverables);

            return mergedDiscoverables;
        }

        /// <summary>
        /// Loads the <see cref="Library"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadLibraryCommand { get; }

        private async Task<ILibrary> LoadLibraryAsync()
        {
            var libs = await _cores.InParallel(core => Task.Run(core.GetLibraryAsync)).ConfigureAwait(false);
            var mergedLibrary = Mergers.MergeLibrary(libs);

            return mergedLibrary;
        }

        /// <summary>
        /// Loads the <see cref="Devices"/> into the view model.
        /// </summary>
        public IAsyncRelayCommand LoadDevicesCommand { get; }

        private async Task<IAsyncEnumerable<IDevice>> LoadDevicesAsync()
        {
            var devices = await _cores.InParallel(core => Task.Run(core.GetDevicesAsync));
            var mergedDevices = Mergers.MergeDevices(devices);

            return mergedDevices;
        }
    }
}
