using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;
using SubSonicMedia.Responses.Browsing;

namespace StrixMusic.Cores.OpenSubsonic.Models;

public sealed class OpenSubsonicCoreLibrary : OpenSubsonicCorePlayableCollectionGroupBase, ICoreLibrary,  IAsyncInit
{
    public OpenSubsonicCoreLibrary(OpenSubsonicCore sourceCore) : base(sourceCore)
    {
        Id = "library";
        Name = "Library";
        Description = null;
        
        AttachEvents();
    }
    
    /// <inheritdoc/>
    public async override Task InitAsync(CancellationToken cancellationToken = default)
    {
        var indexesResponse = await _client.Browsing.GetIndexesAsync(cancellationToken: cancellationToken);
        TotalArtistItemsCount = indexesResponse.Indexes.Index.Sum(i => i.Artist.Count);
        
        // Some servers (e.g. Navidrome) only send artists in the indexes response.
        // The search endpoint should be more reliable across servers.
        var songSearchResponse = await _client.Search.Search2Async("''",
            artistCount: 0, albumCount: 0,
            songCount: int.MaxValue,
            cancellationToken: cancellationToken);
        var searchResultSongs = songSearchResponse.SearchResult.Songs;
        TotalTrackCount = searchResultSongs.Count;

        HashSet<string> albumIds = [];
        foreach (var song in searchResultSongs)
            albumIds.Add(song.AlbumId);
        TotalAlbumItemsCount = albumIds.Count;
        
        var playlistResponse = await _client.Playlists.GetPlaylistsAsync(cancellationToken: cancellationToken);
        TotalPlaylistItemsCount = playlistResponse.Playlists.Playlist.Count;

        IsInitialized = true;
    }

    private void AttachEvents()
    {
        base.TracksCountChanged += Library_TracksCountChanged;
        base.ArtistItemsCountChanged += Library_ArtistItemsCountChanged;
        base.AlbumItemsCountChanged += Library_AlbumItemsCountChanged;
        base.PlaylistItemsCountChanged += Library_PlaylistItemsCountChanged;
        base.ImagesCountChanged += Library_ImagesCountChanged;
    }

    private void DetachEvents()
    {
        base.TracksCountChanged -= Library_TracksCountChanged;
        base.ArtistItemsCountChanged -= Library_ArtistItemsCountChanged;
        base.AlbumItemsCountChanged -= Library_AlbumItemsCountChanged;
        base.PlaylistItemsCountChanged -= Library_PlaylistItemsCountChanged;
        base.ImagesCountChanged -= Library_ImagesCountChanged;
    }

    private void Library_PlaylistItemsCountChanged(object sender, int e)
    {
        PlaylistItemsCountChanged?.Invoke(sender, e);
    }

    private void Library_AlbumItemsCountChanged(object sender, int e)
    {
        AlbumItemsCountChanged?.Invoke(sender, e);
    }

    private void Library_ArtistItemsCountChanged(object sender, int e)
    {
        ArtistItemsCountChanged?.Invoke(sender, e);
    }

    private void Library_ImagesCountChanged(object sender, int e)
    {
        ImagesCountChanged?.Invoke(this, e);
    }

    private void Library_TracksCountChanged(object sender, int e)
    {
        TracksCountChanged?.Invoke(sender, e);
    }

    /// <summary>
    /// Determines if collection base is initialized or not.
    /// </summary>
    public override bool IsInitialized { get; protected set; }

    /// <inheritdoc />
    public override string Id { get; protected set; } = "library";

    /// <inheritdoc />
    public override string Name { get; protected set; } = "Library";

    /// <inheritdoc />
    public override string? Description { get; protected set; } = null;

    /// <inheritdoc />?
    public override event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

    /// <inheritdoc />
    public override event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

    /// <inheritdoc />
    public override event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc />
    public override event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

    /// <inheritdoc />
    public override event EventHandler<int>? AlbumItemsCountChanged;

    /// <inheritdoc />
    public override event EventHandler<int>? ArtistItemsCountChanged;

    /// <inheritdoc />
    public override event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc />
    public override event EventHandler<int>? PlaylistItemsCountChanged;

    /// <inheritdoc />
    public override event EventHandler<int>? ChildrenCountChanged;

    /// <inheritdoc />
    public override event EventHandler<int>? ImagesCountChanged;
    
    /// <inheritdoc/>
    public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var playlistsResponse = await _client.Playlists.GetPlaylistsAsync(cancellationToken: cancellationToken);
        
        Guard.IsTrue(playlistsResponse.IsSuccess);
        
        foreach (var playlistSummary in playlistsResponse.Playlists.Playlist.Skip(offset).Take(limit))
        {
            var playlistResponse = await _client.Playlists.GetPlaylistAsync(playlistSummary.Id, cancellationToken);
            yield return new OpenSubsonicCorePlaylist(_sourceCore, playlistResponse.Playlist);
        }
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var albumList = await _client.Browsing.GetAlbumListAsync(
            type: AlbumListType.Random,
            size: limit, offset: offset,
            cancellationToken: cancellationToken);
        
        foreach (var album in albumList.AlbumList2.Album)
        {
            Guard.IsNotNull(album.Id);
            yield return new OpenSubsonicCoreAlbum(_sourceCore, album);
        }
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var indexesResponse = await _client.Browsing.GetIndexesAsync(cancellationToken: cancellationToken);

        foreach (var artist in indexesResponse.Indexes.Index.SelectMany(ix => ix.Artist))
        {
            Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));

            yield return new OpenSubsonicCoreArtist(SourceCore, artist);
        }
    }

    /// <inheritdoc/>
    public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var searchResponse = await _client.Search.Search2Async("''",
            artistCount: 0, albumCount: 0,
            songCount: limit, songOffset: offset,
            cancellationToken: cancellationToken);

        foreach (var song in searchResponse.SearchResult.Songs)
        {
            Guard.IsNotNullOrWhiteSpace(song.Id);
            yield return new OpenSubsonicCoreTrack(SourceCore, song);
        }
    }

    /// <inheritdoc/>
    public override IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerable.Empty<ICoreUrl>();
    }
}
