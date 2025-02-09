﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreRecentlyPlayed"/>
    /// </summary>
    public sealed class RemoteCoreRecentlyPlayed : RemoteCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreRecentlyPlayed"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="id">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreRecentlyPlayed(string sourceCoreInstanceId, string id)
            : base(sourceCoreInstanceId, id)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreRecentlyPlayed"/>. Wraps around the given <paramref name="recentlyPlayed"/> for remote interaction.
        /// </summary>
        /// <param name="recentlyPlayed">The recently played collection to control remotely.</param>
        internal RemoteCoreRecentlyPlayed(ICoreRecentlyPlayed recentlyPlayed)
            : base(recentlyPlayed)
        {
        }
    }
}
