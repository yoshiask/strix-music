﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Extensions
{
    public static partial class Cores
    {
        /// <summary>
        /// Checks a collection for support for adding an item at a specific index.
        /// </summary>
        /// <typeparam name="TCollection">The type of collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="index">The index to check.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public static Task<bool> IsAddAvailable<TCollection>(this TCollection source, int index)
            where TCollection : ICoreCollection
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return typeof(TCollection) switch
            {
                IPlayableCollectionGroupBase _ => ((ICorePlayableCollectionGroup)source).IsAddChildAvailableAsync(index),
                IAlbumCollectionBase _ => ((ICoreAlbumCollection)source).IsAddAlbumItemAvailableAsync(index),
                IArtistCollectionBase _ => ((ICoreArtistCollection)source).IsAddArtistItemAvailableAsync(index),
                IPlaylistCollectionBase _ => ((ICorePlaylistCollection)source).IsAddPlaylistItemAvailableAsync(index),
                ITrackCollectionBase _ => ((ICoreTrackCollection)source).IsAddTrackAvailableAsync(index),
                IImageCollectionBase _ => ((ICoreImageCollection)source).IsAddImageAvailableAsync(index),
                IGenreCollectionBase _ => ((ICoreGenreCollection)source).IsAddGenreAvailableAsync(index),
                IUrlCollectionBase _ => ((ICoreUrlCollection)source).IsAddUrlAvailableAsync(index),
                _ => throw new NotSupportedException("Collection type not handled"),
            };
        }
    }
}