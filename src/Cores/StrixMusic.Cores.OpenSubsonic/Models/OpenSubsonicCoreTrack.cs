using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using SubSonicMedia.Responses.Search.Models;

namespace StrixMusic.Cores.OpenSubsonic.Models;

/// <summary>
/// Wraps around <see cref="Song"/> to provide track information from an OpenSubsonic server to the Strix SDK.
/// </summary>
public sealed class OpenSubsonicCoreTrack : OpenSubsonicCoreModelBase, ICoreTrack
{
    private readonly Song _song;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSubsonicCoreTrack"/> class.
    /// </summary>
    /// <param name="sourceCore">The source core.</param>
    /// <param name="song">The song to wrap around</param>
    public OpenSubsonicCoreTrack(ICore sourceCore, Song song) : base(sourceCore)
    {
        _song = song;
    }

    /// <inheritdoc/>
    public event EventHandler<ICoreAlbum?>? AlbumChanged;

    /// <inheritdoc/>
    public event EventHandler<int?>? TrackNumberChanged;

    /// <inheritdoc/>
    public event EventHandler<CultureInfo?>? LanguageChanged;

    /// <inheritdoc/>
    public event EventHandler<ICoreLyrics?>? LyricsChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsExplicitChanged;

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
    public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<int>? ArtistItemsCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? GenresCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? ImagesCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? UrlsCountChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

    /// <inheritdoc/>
    public string Id => _song.Id;

    /// <inheritdoc/>
    public TrackType Type => TrackType.Song;

    /// <inheritdoc />
    public int TotalArtistItemsCount => 1;  // TODO: OpenSubsonicMedia seems to be missing OS's artists field

    /// <inheritdoc />
    public int TotalImageCount => _song.CoverArt is null ? 0 : 1;

    /// <inheritdoc/>
    public int TotalGenreCount => _song.Genre is null ? 0 : 2; // TODO: OSM seems to be missing OS's genres field

    /// <inheritdoc />
    public int TotalUrlCount => 0;

    /// <inheritdoc/>
    public ICoreAlbum? Album { get; init; }

    /// <inheritdoc/>
    /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
    public int? TrackNumber => _song.Track;

    /// <inheritdoc />
    public int? DiscNumber => _song.DiscNumber;

    /// <inheritdoc/>
    public CultureInfo? Language { get; }

    /// <inheritdoc/>
    public ICoreLyrics? Lyrics => null;

    /// <inheritdoc/>
    public bool IsExplicit => false;

    /// <inheritdoc/>
    public string Name => _song.Title;

    /// <inheritdoc/>
    public string? Description => null;

    /// <inheritdoc/>
    public PlaybackState PlaybackState { get; }

    /// <inheritdoc/>
    public TimeSpan Duration => TimeSpan.FromSeconds(_song.Duration);

    /// <inheritdoc />
    public DateTime? LastPlayed { get; }

    /// <inheritdoc />
    public DateTime? AddedAt { get; }

    /// <inheritdoc/>
    public ICorePlayableCollectionGroup? RelatedItems => null;

    /// <inheritdoc/>
    public bool IsChangeAlbumAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeTrackNumberAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeLanguageAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeLyricsAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeIsExplicitAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => false;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => false;

    /// <inheritdoc/>
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task ChangeAlbumAsync(ICoreAlbum? albums, CancellationToken cancellationToken = default)
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
    public Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeLyricsAsync(ICoreLyrics? lyrics, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task AddGenreAsync(ICoreGenre genre, int index, CancellationToken cancellationToken = default)
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
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // TODO: OS supports multiple artists
        var artist = await Client.Browsing.GetArtistAsync(_song.ArtistId, cancellationToken);
        
        var response = await Client.Browsing.GetArtistInfoAsync(_song.ArtistId, cancellationToken: cancellationToken);
        var artistInfo = response.ArtistInfo;
        if (artistInfo is not null)
        {
            
        }

        OpenSubsonicCoreArtist coreArtist = new(SourceCore, artist.Artist);

        yield return coreArtist;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_song.CoverArt is null || limit < 1 || offset > 0)
            yield break;

        yield return new OpenSubsonicCoreImage(SourceCore, _song.CoverArt);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // TODO: OS supports multiple genres
        
        if (_song.Genre is null || limit < 1 || offset > 0)
            yield break;

        await Task.CompletedTask;
        
        yield return new OpenSubsonicCoreGenre(SourceCore, _song.Genre);
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerable.Empty<ICoreUrl>();
    }
}
