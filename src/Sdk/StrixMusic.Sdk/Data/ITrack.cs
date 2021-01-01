﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="ITrackBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface ITrack : ITrackBase, IArtistCollection, IGenreCollection, ISdkMember<ICoreTrack>
    {
        /// <summary>
        /// An <see cref="IAlbum"/> object that this track belongs to.
        /// </summary>
        IAlbum? Album { get; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        ILyrics? Lyrics { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// Changes the <see cref="Lyrics"/> for this track.
        /// </summary>
        /// <param name="lyrics">The new lyrics data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeLyricsAsync(ILyrics? lyrics);

        /// <summary>
        /// Fires when the <see cref="Album"/> metadata changes.
        /// </summary>
        event EventHandler<IAlbum?>? AlbumChanged;

        /// <summary>
        /// Fires when the <see cref="Lyrics"/> metadata changes.
        /// </summary>
        event EventHandler<ILyrics?>? LyricsChanged;

        /// <summary>
        /// Changes the album for this track.
        /// </summary>
        /// <param name="album">The new album.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeAlbumAsync(IAlbum? album);
    }
}