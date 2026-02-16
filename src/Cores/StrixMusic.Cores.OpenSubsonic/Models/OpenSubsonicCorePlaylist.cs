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
using SubSonicMedia.Responses.Playlists.Models;

namespace StrixMusic.Cores.OpenSubsonic.Models;

/// <summary>
/// Wraps around <see cref="PlaylistMetadata"/> to provide playlist information extracted from a file to the Strix SDK.
/// </summary>
public sealed class OpenSubsonicCorePlaylist : OpenSubsonicCoreModelBase, ICorePlaylist
{
    private readonly Playlist _playlist;

    /// <summary>
    /// Creates a new instance of <see cref="OpenSubsonicCorePlaylist"/>
    /// </summary>
    public OpenSubsonicCorePlaylist(ICore sourceCore, Playlist playlist) : base(sourceCore)
    {
        SourceCore = sourceCore;
        _playlist = playlist;

        Guard.IsNotNull(playlist);
        Guard.IsNotNullOrWhiteSpace(playlist.Id, nameof(playlist.Id));
    }

    /// <inheritdoc />
    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? ImagesCountChanged;

    /// <inheritdoc />
    public event EventHandler<int>? UrlsCountChanged;

    /// <inheritdoc />
    public event EventHandler<PlaybackState>? PlaybackStateChanged;

    /// <inheritdoc />
    public event EventHandler<string>? NameChanged;

    /// <inheritdoc />
    public event EventHandler<string?>? DescriptionChanged;

    /// <inheritdoc />
    public event EventHandler<TimeSpan>? DurationChanged;

    /// <inheritdoc />
    public event EventHandler<DateTime?>? LastPlayedChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

    /// <inheritdoc />
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

    /// <inheritdoc />
    public ICore SourceCore { get; }

    /// <inheritdoc />
    public string Id => _playlist.Id;

    /// <inheritdoc />
    public string Name => _playlist.Name;

    /// <inheritdoc />
    public string? Description => _playlist.Comment;

    /// <inheritdoc />
    public DateTime? LastPlayed { get; }

    /// <inheritdoc />
    public PlaybackState PlaybackState { get; }

    /// <inheritdoc />
    public TimeSpan Duration => TimeSpan.FromSeconds(_playlist.Duration);

    /// <inheritdoc />
    public bool IsChangeNameAsyncAvailable => false;

    /// <inheritdoc />
    public bool IsChangeDescriptionAsyncAvailable => false;

    /// <inheritdoc />
    public bool IsChangeDurationAsyncAvailable => false;

    /// <inheritdoc />
    public ICoreUserProfile? Owner { get; }

    /// <inheritdoc />
    public ICorePlayableCollectionGroup? RelatedItems { get; }

    /// <inheritdoc />
    public DateTime? AddedAt { get; }

    /// <inheritdoc />
    public int TotalTrackCount => _playlist.Entry.Count;

    /// <inheritdoc />
    public int TotalImageCount => _playlist.CoverArt is null ? 0 : 1;

    /// <inheritdoc />
    public int TotalUrlCount { get; }

    /// <inheritdoc />
    public bool IsPlayTrackCollectionAsyncAvailable => false;

    /// <inheritdoc />
    public bool IsPauseTrackCollectionAsyncAvailable => false;

    /// <inheritdoc />
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
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
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
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
    public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
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

    /// <inheritdoc />
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc />
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
    /// <inheritdoc />
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var song in _playlist.Entry.Skip(offset).Take(limit))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            yield return new OpenSubsonicCoreTrack(SourceCore, song);
        }
    }

    /// <inheritdoc />
    public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<ICoreImage>();

    /// <inheritdoc/>
    public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<ICoreUrl>();
}
