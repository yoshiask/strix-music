﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace StrixMusic.Sdk.FileMetadata.Models
{
    /// <summary>
    /// Contains information that describes track, scanned from a single file.
    /// </summary>
    public sealed class TrackMetadata : IFileMetadata
    {
        /// <summary>
        /// The unique identifier for this track.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The location of the file.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// The unique identifier for this track's album.
        /// </summary>
        public string? AlbumId { get; set; }

        /// <summary>
        /// The unique identifier(s) for this track's artist(s).
        /// </summary>
        public HashSet<string>? ArtistIds { get; set; }

        /// <summary>
        /// The title of this track.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The duration of this track.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// The track number of this track in the album.
        /// </summary>
        public uint? TrackNumber { get; set; }

        /// <summary>
        /// The disk this track is present on.
        /// </summary>
        public uint? DiscNumber { get; set; }

        /// <summary>
        /// The language of this track.
        /// </summary>
        public CultureInfo? Language { get; set; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        public Lyrics? Lyrics { get; set; }

        /// <summary>
        /// The unique identifier(s) for this track's image(s).
        /// </summary>
        public HashSet<string>? ImageIds { get; set; }

        /// <summary>
        /// The description of this track.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The genres of this track.
        /// </summary>
        public HashSet<string>? Genres { get; set; }

        /// <summary>
        /// The year this track was released.
        /// </summary>
        public uint? Year { get; set; }
    }
}
