﻿using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// Contains information that describes an artist, scanned from one or more files.
    /// </summary>
    public class ArtistMetadata : IFileMetadata
    {
        /// <summary>
        /// The unique identifier for this artist.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The unique identifier(s) for <see cref="TrackMetadata"/>.
        /// </summary>
        public List<string>? TrackIds { get; set; }

        /// <summary>
        /// Holds unique identifier(s) for the <see cref="AlbumMetadata"/>.
        /// </summary>
        public List<string>? AlbumIds { get; set; }

        /// <summary>
        /// Holds the name of the artist.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Probably bio of the artist.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The external link associated with the artist.
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// The unique identifier(s) for this artist's image(s).
        /// </summary>
        public List<string>? ImageIds { get; set; }

        /// <summary>
        /// The genres of this track.
        /// </summary>
        public List<string>? Genres { get; set; }
    }
}
