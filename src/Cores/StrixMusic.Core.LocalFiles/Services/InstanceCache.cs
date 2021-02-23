﻿using System;
using OwlCore.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Services
{
    /// <summary>
    /// Contains caches for all instance created across the core.
    /// </summary>
    public static class InstanceCache
    {
        //public static InstanceCacheRepository<LocalFilesCoreAlbum> PlaylistCache = new InstanceCacheRepository<LocalFilesCorePlaylist>();

        /// <summary>
        /// A cache of all albums across all core instances.
        /// </summary>
        public static AlbumCacheRepo Albums { get; set; } = new AlbumCacheRepo();

        /// <summary>
        /// A cache of all artists across all core instances.
        /// </summary>
        public static ArtistCacheRepo Artists { get; set; } = new ArtistCacheRepo();

        /// <summary>
        /// A cache of all tracks across all core instances.
        /// </summary>
        public static TrackCacheRepo Tracks { get; set; } = new TrackCacheRepo();

        /// <summary>
        /// A cache of all images across all core instances.
        /// </summary>
        public static ImageCacheRepo Images { get; set; } = new ImageCacheRepo();
    }

    /// <summary>
    /// A cache of all albums across all core instances.
    /// </summary>
    public class AlbumCacheRepo : InstanceCacheRepository<LocalFilesCoreAlbum>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public LocalFilesCoreAlbum GetOrCreate(string id, ICore sourceCore, AlbumMetadata albumMetadata, int totalTracksCount, LocalFilesCoreImage? image)
        {
            return GetOrCreate(id, () => new LocalFilesCoreAlbum(sourceCore, albumMetadata, totalTracksCount, image));
        }
    }

    /// <summary>
    /// A cache of all artists across all core instances.
    /// </summary>
    public class ArtistCacheRepo : InstanceCacheRepository<LocalFilesCoreArtist>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public LocalFilesCoreArtist GetOrCreate(string id, ICore sourceCore, ArtistMetadata artistMetadata, int totalTracksCount, LocalFilesCoreImage? image = null)
        {
            return GetOrCreate(id, () => new LocalFilesCoreArtist(sourceCore, artistMetadata, totalTracksCount, image));
        }
    }

    /// <summary>
    /// A cache of all tracks across all core instances.
    /// </summary>
    public class TrackCacheRepo : InstanceCacheRepository<LocalFilesCoreTrack>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, Func{T})"/>
        public LocalFilesCoreTrack GetOrCreate(string id, ICore sourceCore, TrackMetadata trackMetadata)
        {
            return GetOrCreate(id, () => new LocalFilesCoreTrack(sourceCore, trackMetadata));
        }
    }

    /// <summary>
    /// A cache of all images across all core instances.
    /// </summary>
    public class ImageCacheRepo : InstanceCacheRepository<LocalFilesCoreImage>
    {
        /// <inheritdoc cref="IInstanceCacheRepository{T}.GetOrCreate(string, System.Func{T})"/>
        public LocalFilesCoreImage GetOrCreate(string id, ICore sourceCore, Uri uri, double? width = null, double? height = null)
        {
            return GetOrCreate(id, () => new LocalFilesCoreImage(sourceCore, uri, width, height));
        }
    }
}