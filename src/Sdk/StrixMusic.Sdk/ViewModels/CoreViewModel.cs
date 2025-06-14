﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ICore"/>.
    /// </summary>
    public sealed partial class CoreViewModel : ObservableObject, ISdkViewModel, ICore, IDelegable<ICore>
    {
        private readonly ICore _core;
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreViewModel"/> class.
        /// </summary>
        /// <param name="core">The <see cref="ICore"/> to wrap around.</param>
        public CoreViewModel(ICore core)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

            _core = core;
            
            AttachEvents();
        }

        private void AttachEvents()
        {
            _core.InstanceDescriptorChanged += Core_InstanceDescriptorChanged;
            _core.LogoChanged += OnLogoChanged;
            _core.DisplayNameChanged += OnDisplayNameChanged;
        }

        private void DetachEvents()
        {
            _core.InstanceDescriptorChanged -= Core_InstanceDescriptorChanged;
            _core.LogoChanged -= OnLogoChanged;
            _core.DisplayNameChanged -= OnDisplayNameChanged;
        }

        private void OnDisplayNameChanged(object? sender, string e) => _syncContext.Post(_ =>
        {
            OnPropertyChanged(nameof(DisplayName));
        }, null);

        private void Core_InstanceDescriptorChanged(object? sender, string e) => _syncContext.Post(_ =>
        {
            OnPropertyChanged(nameof(InstanceDescriptor));
            InstanceDescriptorChanged?.Invoke(sender, e);
        }, null);

        private void OnLogoChanged(object? sender, ICoreImage? e) => _syncContext.Post(_ =>
        {
            OnPropertyChanged(nameof(Logo));
        }, null);

        /// <inheritdoc/>
        ICore IDelegable<ICore>.Inner => _core;

        /// <inheritdoc />
        public string Id => _core.Id;

        /// <inheritdoc />
        public ICoreImage? Logo => _core.Logo;

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc />
        public string InstanceDescriptor => _core.InstanceDescriptor;

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => _core.PlaybackType;

        /// <summary>
        /// The user-friendly name of the core.
        /// </summary>
        public string DisplayName => _core.DisplayName;

        /// <inheritdoc cref="ICore.User" />
        public ICoreUser? User => _core.User;

        /// <inheritdoc />
        public ICore SourceCore => _core.SourceCore;

        /// <inheritdoc />
        public event EventHandler<string>? DisplayNameChanged
        {
            add => _core.DisplayNameChanged += value;
            remove => _core.DisplayNameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ICoreImage?>? LogoChanged
        {
            add => _core.LogoChanged += value;
            remove => _core.LogoChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc />
        IReadOnlyList<ICoreDevice> ICore.Devices => _core.Devices;

        /// <inheritdoc cref="ICore.Library" />
        ICoreLibrary ICore.Library => _core.Library;

        /// <inheritdoc />
        ICoreSearch? ICore.Search => _core.Search;

        /// <inheritdoc />
        ICoreRecentlyPlayed? ICore.RecentlyPlayed => _core.RecentlyPlayed;

        /// <inheritdoc />
        ICoreDiscoverables? ICore.Discoverables => _core.Discoverables;

        /// <inheritdoc />
        ICorePlayableCollectionGroup? ICore.Pins => _core.Pins;

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents();
            await _core.DisposeAsync();
        }

        /// <inheritdoc/>
        public Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default) => _core.GetContextByIdAsync(id, cancellationToken);

        /// <inheritdoc />
        public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default) => _core.GetMediaSourceAsync(track, cancellationToken);

        /// <inheritdoc />
        [RelayCommand(IncludeCancelCommand = true)]
        public Task InitAsync(CancellationToken cancellationToken = default) => _core.InitAsync(cancellationToken);

        /// <inheritdoc />
        public bool IsInitialized => _core.IsInitialized;
    }
}
