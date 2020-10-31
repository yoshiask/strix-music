﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IPlayableCollectionGroupChildrenBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlayableCollectionGroupChildren : IPlayableCollectionGroupChildrenBase, ISdkMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="IPlayableCollectionGroupBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset);

        /// <summary>
        /// Adds a new child to the collection on the backend.
        /// </summary>
        /// <param name="child">The child to create.</param>
        /// <param name="index">the position to insert the child at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddChildAsync(IPlayableCollectionGroup child, int index);
    }
}