﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// A collection of <see cref="IAlbumCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IAlbumCollectionBase : IPlayableCollectionBase, IAlbumCollectionItemBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of available Albums.
        /// </summary>
        int TotalAlbumItemsCount { get; }

        /// <summary>
        /// If true, <see cref="PlayAlbumCollectionAsync(CancellationToken)"/> can be used.
        /// </summary>
        bool IsPlayAlbumCollectionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PauseAlbumCollectionAsync(CancellationToken)"/> can be used.
        /// </summary>
        bool IsPauseAlbumCollectionAsyncAvailable { get; }

        /// <summary>
        /// Attempts to play the album collection, or resumes playback if already playing.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to pause the album collection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the album from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the album to remove.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IAlbumCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IAlbumCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IAlbumCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IAlbumCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when <see cref="IsPlayAlbumCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsPauseAlbumCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <summary>
        /// Fires when the merged <see cref="TotalAlbumItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? AlbumItemsCountChanged;
    }
}
