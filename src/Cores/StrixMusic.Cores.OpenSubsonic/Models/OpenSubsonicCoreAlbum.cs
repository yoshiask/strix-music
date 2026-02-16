using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using SubSonicMedia.Responses.Browsing.Models;
using SubSonicMedia.Responses.Search.Models;

namespace StrixMusic.Cores.OpenSubsonic.Models;

public class OpenSubsonicCoreAlbum : OpenSubsonicCoreModelBase, ICoreAlbum
{
    private readonly string? _artistId;
    private readonly string? _genreName;
    private readonly string? _coverArt;
    
    public OpenSubsonicCoreAlbum(ICore sourceCore, Album album) : base(sourceCore)
    {
        Guard.IsNotNull(album.Id);
        Id = album.Id;
        
        Name = album.Name ?? string.Empty;
        DatePublished = album.Created is not null ? new DateTime(album.Created.Value) : null;
        Duration = TimeSpan.FromSeconds(album.Duration);
        TotalImageCount = album.CoverArt is null ? 0 : 1;
        TotalArtistItemsCount = album.ArtistId is null ? 0 : 1;
        TotalTrackCount = album.SongCount;
        TotalGenreCount = album.Genre is null ? 0 : 1;

        _artistId = album.ArtistId;
        _genreName = album.Genre;
        _coverArt = album.CoverArt;
    }
    
    public OpenSubsonicCoreAlbum(ICore sourceCore, AlbumSummary albumSummary) : base(sourceCore)
    {
        Guard.IsNotNull(albumSummary.Id);
        Id = albumSummary.Id;
        
        Name = albumSummary.Name ?? string.Empty;
        DatePublished = albumSummary.Created is not null
            ? new DateTime(albumSummary.Created.Value) : null;
        Duration = TimeSpan.FromSeconds(albumSummary.Duration);
        TotalImageCount = albumSummary.CoverArt is null ? 0 : 1;
        TotalArtistItemsCount = albumSummary.ArtistId is null ? 0 : 1;
        TotalTrackCount = albumSummary.SongCount;
        TotalGenreCount = albumSummary.Genre is null ? 0 : 1;

        _artistId = albumSummary.ArtistId;
        _genreName = albumSummary.Genre;
        _coverArt = albumSummary.CoverArt;
    }

    /// <inheritdoc/>
    public event EventHandler<PlaybackState>? PlaybackStateChanged;

    /// <inheritdoc/>
    public event EventHandler<string>? NameChanged;

    /// <inheritdoc/>
    public event EventHandler<string?>? DescriptionChanged;

    /// <inheritdoc/>
    public event EventHandler<DateTime?>? DatePublishedChanged;

    /// <inheritdoc/>
    public event EventHandler<TimeSpan>? DurationChanged;

    /// <inheritdoc />
    public event EventHandler<DateTime?>? LastPlayedChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<int>? ArtistItemsCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? ImagesCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? UrlsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? GenresCountChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public DateTime? DatePublished { get; }

    /// <inheritdoc/>
    public string? Description => null;

    /// <inheritdoc/>
    public PlaybackState PlaybackState { get; }

    /// <inheritdoc/>
    public TimeSpan Duration { get; }

    /// <inheritdoc />
    public DateTime? LastPlayed { get; }

    /// <inheritdoc />
    public DateTime? AddedAt { get; }

    /// <inheritdoc />
    public int TotalImageCount { get; }

    /// <inheritdoc />
    public int TotalArtistItemsCount { get; }

    /// <inheritdoc/>
    public int TotalTrackCount { get; }

    /// <inheritdoc/>
    public int TotalGenreCount { get; }

    /// <inheritdoc/>
    public int TotalUrlCount => 0;

    /// <inheritdoc/>
    public ICorePlayableCollectionGroup? RelatedItems { get; }

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDatePublishedAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => false;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;

    /// <inheritdoc/>
    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc/>
    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddUrlAsync(ICoreUrl image, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task AddGenreAsync(ICoreGenre genre, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(Client);
        
        var albumResponse = await Client.Browsing.GetAlbumAsync(Id, cancellationToken);

        foreach (var song in albumResponse.Album.Song)
        {
            Guard.IsNotNullOrWhiteSpace(song.Id);
            yield return new OpenSubsonicCoreTrack(SourceCore, song);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // TODO: OS supports multiple artists
        
        if (_artistId is null)
            yield break;
        
        var artistResponse = await Client.Browsing.GetArtistAsync(_artistId, cancellationToken);
        var artist = artistResponse.Artist;
        
        yield return new OpenSubsonicCoreArtist(SourceCore, artist);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_coverArt is null || limit < 1 || offset > 0)
            yield break;
        
        await Task.CompletedTask;

        yield return new OpenSubsonicCoreImage(SourceCore, _coverArt);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_genreName is null || limit < 1 || offset > 0)
            yield break;

        yield return new OpenSubsonicCoreGenre(SourceCore, _genreName);

        await Task.CompletedTask;
    }
}
