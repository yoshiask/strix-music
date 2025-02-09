﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Xml.Serialization;

namespace StrixMusic.Sdk.FileMetadata.Models.Playlist.Smil
{
    /// <summary>
    /// Holds all metadata and title of the playlist.
    /// </summary>
    public class Head
    {
        /// <inheritdoc cref="Meta"/>
        [XmlElement("meta")]
        public Meta[]? Meta { get; set; }

        /// <inheritdoc cref="Title"/>
        [XmlElement("title")]
        public string? Title { get; set; }
    }
}