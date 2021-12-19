﻿using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreSearchHistory"/>
    /// </summary>
    public class RemoteCoreSearchHistory : RemoteCorePlayableCollectionGroupBase, ICoreSearchHistory
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreSearchHistory"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreSearchHistory(string sourceCoreInstanceId, string remotingId)
            : base(sourceCoreInstanceId, remotingId)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreSearchHistory"/>. Wraps around the given <paramref name="searchHistory"/> for remote interaction.
        /// </summary>
        /// <param name="searchHistory">The recently played collection to control remotely.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreSearchHistory(ICoreSearchHistory searchHistory, string remotingId)
            : base(searchHistory, remotingId)
        {
        }
    }
}
