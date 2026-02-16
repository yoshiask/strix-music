using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using SubSonicMedia.Responses.Browsing.Models;
using Artist = SubSonicMedia.Responses.Browsing.Artist;

namespace StrixMusic.Cores.OpenSubsonic.Models;

/// <summary>
/// Wraps around <see cref="SubSonicMedia.Responses.Browsing.Artist"/> to provide artist information extracted from a file to the Strix SDK.
/// </summary>
public sealed class OpenSubsonicCoreArtist : OpenSubsonicCoreModelBase, ICoreArtist
{
    private readonly string? _artistImage;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSubsonicCoreArtist"/> class.
    /// </summary>
    /// <param name="sourceCore">The source core.</param>
    /// <param name="artist">The artist metadata to wrap around.</param>
    public OpenSubsonicCoreArtist(ICore sourceCore, Artist artist) : base(sourceCore)
    {
        _artistImage = artist.CoverArt;
        
        Id = artist.Id ?? throw new ArgumentException();
        Name = artist.Name ?? string.Empty;
        TotalAlbumItemsCount = artist.AlbumCount;
    }

    public OpenSubsonicCoreArtist(ICore sourceCode, ArtistWithAlbums artistWithAlbums) : base(sourceCode)
    {
        _artistImage = artistWithAlbums.CoverArt;
        
        Id = artistWithAlbums.Id ?? throw new ArgumentException();
        Name = artistWithAlbums.Name ?? string.Empty;
        TotalAlbumItemsCount = artistWithAlbums.AlbumCount;
        Duration = TimeSpan.FromSeconds(artistWithAlbums.Album.Sum(a => a.Duration));
    }
    
    /// <inheritdoc/>
    public event EventHandler<PlaybackState>? PlaybackStateChanged;

    /// <inheritdoc/>
    public event EventHandler<string>? NameChanged;

    /// <inheritdoc/>
    public event EventHandler<string?>? DescriptionChanged;

    /// <inheritdoc/>
    public event EventHandler<TimeSpan>? DurationChanged;

    /// <inheritdoc />
    public event EventHandler<DateTime?>? LastPlayedChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<int>? ImagesCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? AlbumItemsCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

    /// <inheritdoc />
    public event EventHandler<int>? GenresCountChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

    /// <inheritdoc />
    public event EventHandler<int>? UrlsCountChanged;

    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public int TotalTrackCount => 0;

    /// <inheritdoc/>
    public int TotalAlbumItemsCount { get; }

    /// <inheritdoc />
    public int TotalGenreCount => 0;

    /// <inheritdoc />
    public int TotalImageCount => _artistImage is null ? 0 : 1;

    /// <inheritdoc />
    public int TotalUrlCount => 0;

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Description => string.Empty;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => PlaybackState.None;

    /// <inheritdoc/>
    public TimeSpan Duration { get; }

    /// <inheritdoc />
    public DateTime? LastPlayed => null;

    /// <inheritdoc />
    public DateTime? AddedAt => null;

    /// <inheritdoc/>
    public ICorePlayableCollectionGroup? RelatedItems => null;

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPlayAlbumCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPauseAlbumCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => false;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
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
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
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

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc/>
    public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddGenreAsync(ICoreGenre genre, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddUrlAsync(ICoreUrl image, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var artistResponse = await Client.Browsing.GetArtistAsync(Id, cancellationToken);

        foreach (var album in artistResponse.Artist.Album)
        {
            Guard.IsNotNull(album.Id);
            
            yield return new OpenSubsonicCoreAlbum(SourceCore, album);
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var artistResponse = await Client.Browsing.GetArtistAsync(Id, cancellationToken);

        foreach (var album in artistResponse.Artist.Album)
        {
            Guard.IsNotNull(album.Id);
            
            var albumResponse = await Client.Browsing.GetAlbumAsync(album.Id, cancellationToken);
            
            if (!albumResponse.IsSuccess)
                continue;
            
            foreach (var song in albumResponse.Album.Song)
                yield return new OpenSubsonicCoreTrack(SourceCore, song);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_artistImage is null || limit < 1 || offset > 0)
            yield break;

        await Task.CompletedTask;
        
        yield return new OpenSubsonicCoreImage(SourceCore, _artistImage);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}
