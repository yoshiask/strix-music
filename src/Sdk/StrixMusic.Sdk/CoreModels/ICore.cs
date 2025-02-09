﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;
using OwlCore.Provisos;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// An <see cref="ICore"/> is a common API surface that can be implemented to allow interfacing with any arbitrary music provider.
    /// </summary>
    /// <remarks> In a core's constructor, only do basic object initialization. For the rest, use <see cref="IAsyncInit.InitAsync"/>.
    /// 
    /// <para/> During <see cref="IAsyncInit.InitAsync"/>, if the core state is changed to <see cref="CoreState.NeedsConfiguration"/>,
    ///         the consuming application should display the current <see cref="AbstractConfigPanel"/> to the user for configuration and setup.
    ///         After the user has completed setup, execution of InitAsync should proceed as usual with the new data.
    /// </remarks>
    /// <seealso cref="IStrixDataRoot"/>
    public interface ICore : IAsyncInit, ICoreModel, IAsyncDisposable
    {
        /// <summary>
        /// The registered metadata for this core. Contains information to identify the core before creating an instance.
        /// </summary>
        public CoreMetadata Registration { get; }

        /// <summary>
        /// Identifies this instance of the core. This is given to each core via the constructor.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// A string of text to display to the user to help identify which core instance this is, such as a username or the path to a file location. Longer strings will be truncated as needed.
        /// </summary>
        public string InstanceDescriptor { get; }

        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        public AbstractUICollection AbstractConfigPanel { get; }

        /// <summary>
        /// The player type supported by this core.
        /// </summary>
        public MediaPlayerType PlaybackType { get; }

        /// <summary>
        /// The user that is authenticated with this core, if any. Only one user is supported per core.
        /// </summary>
        public ICoreUser? User { get; }

        /// <summary>
        /// The available devices. These should only be populated with remote devices, if supported by the core.
        /// Local playback is handled by the SDK by calling <see cref="GetMediaSourceAsync"/>.
        /// </summary>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core. This must never be null, but it may return 0 items if needed.
        /// </summary>
        public ICoreLibrary Library { get; }

        /// <summary>
        /// Pins act as "bookmarks" or "shortcuts", things that the user has chosen to "pin" somewhere for easy access.
        /// </summary>
        /// <remarks>
        /// If this is left null, this will be managed by the app. If not, the core will be entirely responsible for managing this.
        /// </remarks>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <summary>
        /// Contains various search-related data and activities.
        /// </summary>
        /// <remarks>
        /// If this is left null, this will be managed by the app. If not, the core will be entirely responsible for managing this.
        /// </remarks>
        public ICoreSearch? Search { get; }

        /// <summary>
        /// The items that the user has been recently played by the user. 
        /// </summary>
        /// <remarks>
        /// If this is left null, this will be managed by the app. If not, the core will be entirely responsible for managing this.
        /// </remarks>
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public ICoreDiscoverables? Discoverables { get; }

        /// <summary>
        /// The current state of the core.
        /// </summary>
        public CoreState CoreState { get; }

        /// <summary>
        /// Given the ID of an instance created by this core, return the fully constructed instance.
        /// </summary>
        /// <returns>The requested instance, cast down to <see cref="ICoreModel"/>.</returns>
        public Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Converts a <see cref="ICoreTrack"/> into a <see cref="IMediaSourceConfig"/> that can be used to play the track.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The value is an <see cref="IMediaSourceConfig"/> that can be used to play the track.</returns>
        public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when the <see cref="AppModels.CoreState"/> has changed.
        /// </summary>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <summary>
        /// Raised when the contents of <see cref="Devices"/> is changed.
        /// </summary>
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <summary>
        /// Raised when <see cref="AbstractUIElement"/> is changed.
        /// </summary>
        public event EventHandler? AbstractConfigPanelChanged;

        /// <summary>
        /// Raised when <see cref="InstanceDescriptor"/> is changed.
        /// </summary>
        public event EventHandler<string>? InstanceDescriptorChanged;
    }
}
