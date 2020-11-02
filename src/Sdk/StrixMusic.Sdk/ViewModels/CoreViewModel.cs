﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.Collections;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains information about a <see cref="ICore"/>
    /// </summary>
    public class CoreViewModel : ObservableObject, ICore
    {
        private readonly ICore _core;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreViewModel"/> class.
        /// </summary>
        /// <param name="core">The base <see cref="ICore"/></param>
        /// <remarks>
        /// Creating a new <see cref="CoreViewModel"/> will register itself into <see cref="MainViewModel.Cores"/>.
        /// </remarks>
        public CoreViewModel(ICore core)
        {
            _core = core;

            MainViewModel.Singleton?.Cores.Add(this);

            // TODO: Create merged items

            Devices = new SynchronizedObservableCollection<ICoreDevice>(_core.Devices.Select(x => new DeviceViewModel(x)));
            Library = new LibraryViewModel(_core.Library);
            CoreRecentlyPlayed = new RecentlyPlayedViewModel(_core.CoreRecentlyPlayed);
            Pins = new SynchronizedObservableCollection<IPlayable>(_core.Pins);
            CoreDiscoverables = new DiscoverablesViewModel(_core.CoreDiscoverables);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _core.CoreStateChanged += Core_CoreStateChanged;
        }

        private void DetachEvents()
        {
            _core.CoreStateChanged -= Core_CoreStateChanged;
        }

        /// <inheritdoc cref="ICore.CoreState" />
        private void Core_CoreStateChanged(object sender, CoreState e) => CoreState = e;

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc cref="ICore.Name" />
        public string Name => _core.Name;

        /// <inheritdoc cref="ICore.User" />
        public ICoreUser User => _core.User;

        /// <inheritdoc cref="ICore.CoreConfig" />
        public ICoreConfig CoreConfig => _core.CoreConfig;

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState
        {
            get => _core.CoreState;
            set => SetProperty(() => _core.CoreState, value);
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreDevice> Devices { get; }

        /// <inheritdoc cref="ICore.Library" />
        public ILibraryBase Library { get; }

        /// <inheritdoc cref="ICore.CoreRecentlyPlayed" />
        public ICoreRecentlyPlayed CoreRecentlyPlayed { get; }

        /// <inheritdoc cref="ICore.CoreDiscoverables" />
        public ICoreDiscoverables CoreDiscoverables { get; }

        /// <inheritdoc cref="ICore.Pins" />
        public SynchronizedObservableCollection<IPlayable> Pins { get; }

        /// <inheritdoc cref="ICore.GetSearchAutoCompleteAsync" />
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query) => _core.GetSearchAutoCompleteAsync(query);

        /// <inheritdoc cref="ICore.GetSearchResultsAsync" />
        public Task<ICoreSearchResults> GetSearchResultsAsync(string query) => _core.GetSearchResultsAsync(query);

        /// <inheritdoc cref="ICore.InitAsync" />
        public Task InitAsync(IServiceCollection services) => _core.InitAsync(services);

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync()
        {
            await _core.DisposeAsync().ConfigureAwait(false);
            DetachEvents();
        }

        /// <inheritdoc />
        public Task<bool> IsAddPinSupported(int index) => _core.IsAddPinSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePinSupported(int index) => _core.IsRemovePinSupported(index);

        /// <inheritdoc/>
        public IAsyncEnumerable<object?> GetContextById(string id) => _core.GetContextById(id);

        /// <inheritdoc />
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track) => _core.GetMediaSource(track);

        /// <inheritdoc cref="ICore.CoreStateChanged" />
        public event EventHandler<CoreState>? CoreStateChanged
        {
            add => _core.CoreStateChanged += value;

            remove => _core.CoreStateChanged -= value;
        }
    }
}
