﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IArtistCollectionBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreArtistCollection : IArtistCollectionBase, ICoreArtistCollectionItem, ICoreMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="IArtist"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new artist to the collection on the backend.
        /// </summary>
        /// <param name="artist">The artist to create.</param>
        /// <param name="index">the position to insert the artist at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index);
    }
}
