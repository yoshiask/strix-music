﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

/// <summary>
/// Wraps an instance of <see cref="IPlayableCollectionGroup"/> with the provided plugins.
/// </summary>
public class MockPlayableCollectionGroup : IPlayableCollectionGroup
{
    private int _totalUrlCount;
    private int _totalImageCount;
    private bool _isChangeNameAsyncAvailable;
    private bool _isChangeDescriptionAsyncAvailable;
    private bool _isChangeDurationAsyncAvailable;
    private string? _description;
    private string _name = string.Empty;
    private TimeSpan _duration;
    private int _totalPlaylistItemsCount;
    private bool _isPausePlaylistCollectionAsyncAvailable;
    private bool _isPlayPlaylistCollectionAsyncAvailable;
    private PlaybackState _playbackState;
    private int _totalTrackCount;
    private readonly string _id = Guid.NewGuid().ToString();
    private bool _isPlayTrackCollectionAsyncAvailable;
    private bool _isPauseTrackCollectionAsyncAvailable;
    private int _totalAlbumItemsCount;
    private bool _isPlayAlbumCollectionAsyncAvailable;
    private bool _isPauseAlbumCollectionAsyncAvailable;
    private int _totalArtistItemsCount;
    private bool _isPlayArtistCollectionAsyncAvailable;
    private bool _isPauseArtistCollectionAsyncAvailable;
    private int _totalChildrenCount;
    private DownloadInfo _downloadInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockPlayableCollectionGroup"/> class.
    /// </summary>
    internal MockPlayableCollectionGroup()
    {
    }

    /// <inheritdoc/>
    public event EventHandler<int>? ImagesCountChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? UrlsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<PlaybackState>? PlaybackStateChanged;

    /// <inheritdoc/>
    public event EventHandler<string>? NameChanged;

    /// <inheritdoc/>
    public event EventHandler<string?>? DescriptionChanged;

    /// <inheritdoc/>
    public event EventHandler<TimeSpan>? DurationChanged;

    /// <inheritdoc/>
    public event EventHandler<DateTime?>? LastPlayedChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? PlaylistItemsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? AlbumItemsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? ArtistItemsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? ChildrenCountChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IImage>? ImagesChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event EventHandler<DownloadInfo>? DownloadInfoChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<ITrack>? TracksChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged;

    /// <inheritdoc/>
    public int TotalImageCount
    {
        get => _totalImageCount;
        set
        {
            _totalImageCount = value;
            ImagesCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalImageCount--;
        ImagesChanged?.Invoke(this, new List<CollectionChangedItem<IImage>>(), new List<CollectionChangedItem<IImage>> { new(new MockImage(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public int TotalUrlCount
    {
        get => _totalUrlCount;
        set
        {
            _totalUrlCount = value;
            UrlsCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalUrlCount--;
        UrlsChanged?.Invoke(this, new List<CollectionChangedItem<IUrl>>(), new List<CollectionChangedItem<IUrl>> { new(new MockUrl(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public string Id => _id;

    /// <inheritdoc/>
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            NameChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            DescriptionChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public DateTime? LastPlayed => DateTime.UnixEpoch;

    /// <inheritdoc/>
    public PlaybackState PlaybackState
    {
        get => _playbackState;
        set
        {
            _playbackState = value;
            PlaybackStateChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            DurationChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable
    {
        get => _isChangeNameAsyncAvailable;
        set
        {
            _isChangeNameAsyncAvailable = value;
            IsChangeNameAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable
    {
        get => _isChangeDescriptionAsyncAvailable;
        set
        {
            _isChangeDescriptionAsyncAvailable = value;
            IsChangeDescriptionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable
    {
        get => _isChangeDurationAsyncAvailable;
        set
        {
            _isChangeDurationAsyncAvailable = value;
            IsChangeDurationAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
    {
        Name = name;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
    {
        Description = description;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        Duration = duration;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public DateTime? AddedAt => null;

    /// <inheritdoc/>
    public int TotalPlaylistItemsCount
    {
        get => _totalPlaylistItemsCount;
        set
        {
            _totalPlaylistItemsCount = value;
            PlaylistItemsCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPlayPlaylistCollectionAsyncAvailable
    {
        get => _isPlayPlaylistCollectionAsyncAvailable;
        set
        {
            _isPlayPlaylistCollectionAsyncAvailable = value;
            IsPlayPlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPausePlaylistCollectionAsyncAvailable
    {
        get => _isPausePlaylistCollectionAsyncAvailable;
        set
        {
            _isPausePlaylistCollectionAsyncAvailable = value;
            IsPausePlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Paused;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalPlaylistItemsCount--;
        PlaylistItemsChanged?.Invoke(this, new List<CollectionChangedItem<IPlaylistCollectionItem>>(), new List<CollectionChangedItem<IPlaylistCollectionItem>> { new(new MockPlaylist(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public int TotalTrackCount
    {
        get => _totalTrackCount;
        set
        {
            _totalTrackCount = value;
            TracksCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable
    {
        get => _isPlayTrackCollectionAsyncAvailable;
        set
        {
            _isPlayTrackCollectionAsyncAvailable = value;
            IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable
    {
        get => _isPauseTrackCollectionAsyncAvailable;
        set
        {
            _isPauseTrackCollectionAsyncAvailable = value;
            IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Paused;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalTrackCount--;
        TracksChanged?.Invoke(this, new List<CollectionChangedItem<ITrack>>(), new List<CollectionChangedItem<ITrack>> { new(new MockTrack(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public int TotalAlbumItemsCount
    {
        get => _totalAlbumItemsCount;
        set
        {
            _totalAlbumItemsCount = value;
            AlbumItemsCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPlayAlbumCollectionAsyncAvailable
    {
        get => _isPlayAlbumCollectionAsyncAvailable;
        set
        {
            _isPlayAlbumCollectionAsyncAvailable = value;
            IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPauseAlbumCollectionAsyncAvailable
    {
        get => _isPauseAlbumCollectionAsyncAvailable;
        set
        {
            _isPauseAlbumCollectionAsyncAvailable = value;
            IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Paused;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalAlbumItemsCount--;
        AlbumItemsChanged?.Invoke(this, new List<CollectionChangedItem<IAlbumCollectionItem>>(), new List<CollectionChangedItem<IAlbumCollectionItem>> { new(new MockAlbum(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public int TotalArtistItemsCount
    {
        get => _totalArtistItemsCount;
        set
        {
            _totalArtistItemsCount = value;
            ArtistItemsCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable
    {
        get => _isPlayArtistCollectionAsyncAvailable;
        set
        {
            _isPlayArtistCollectionAsyncAvailable = value;
            IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable
    {
        get => _isPauseArtistCollectionAsyncAvailable;
        set
        {
            _isPauseArtistCollectionAsyncAvailable = value;
            IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Paused;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalArtistItemsCount--;
        ArtistItemsChanged?.Invoke(this, new List<CollectionChangedItem<IArtistCollectionItem>>(), new List<CollectionChangedItem<IArtistCollectionItem>> { new(new MockAlbum(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Paused;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public int TotalChildrenCount
    {
        get => _totalChildrenCount;
        set
        {
            _totalChildrenCount = value;
            ChildrenCountChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalChildrenCount--;
        ChildItemsChanged?.Invoke(this, new List<CollectionChangedItem<IPlayableCollectionGroup>>(), new List<CollectionChangedItem<IPlayableCollectionGroup>> { new(new MockPlayableCollectionGroup(), index) });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection? other) => false;

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources { get; } = new List<ICoreImageCollection>();

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources { get; } = new List<ICoreUrlCollection>();

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources { get; } = new List<ICorePlaylistCollectionItem>();

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources { get; } = new List<ICorePlaylistCollection>();

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources { get; } = new List<ICoreTrackCollection>();

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources { get; } = new List<ICoreAlbumCollectionItem>();

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources { get; } = new List<ICoreAlbumCollection>();

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources { get; } = new List<ICoreArtistCollectionItem>();

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources { get; } = new List<ICoreArtistCollection>();

    /// <inheritdoc/>
    IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources { get; } = new List<ICorePlayableCollectionGroupChildren>();

    /// <inheritdoc/>
    IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources { get; } = new List<ICorePlayableCollectionGroup>();

    /// <inheritdoc cref="IMerged{T}.SourceCores"/>
    public IReadOnlyList<ICore> SourceCores { get; } = new List<ICore>();

    /// <inheritdoc/>
    public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IImage>>(Enumerable.Range(0, limit).Select(_ => new MockImage()).ToList());

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default)
    {
        TotalImageCount++;
        ImagesChanged?.Invoke(this, new List<CollectionChangedItem<IImage>> { new(image, index) }, new List<CollectionChangedItem<IImage>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection? other) => false;

    /// <inheritdoc/>
    public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IUrl>>(Enumerable.Range(0, limit).Select(_ => new MockUrl()).ToList());

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default)
    {
        TotalUrlCount++;
        UrlsChanged?.Invoke(this, new List<CollectionChangedItem<IUrl>> { new(url, index) }, new List<CollectionChangedItem<IUrl>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo
    {
        get => _downloadInfo;
        set
        {
            _downloadInfo = value;
            DownloadInfoChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default)
    {
        DownloadInfo = new DownloadInfo(DownloadState.Downloading);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollectionItem? other) => false;

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollection? other) => false;

    /// <inheritdoc/>
    public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IPlaylistCollectionItem>>(Enumerable.Range(0, limit).Select(_ => new MockPlaylist()).ToList());

    /// <inheritdoc/>
    public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default)
    {
        TotalPlaylistItemsCount++;
        PlaylistItemsChanged?.Invoke(this, new List<CollectionChangedItem<IPlaylistCollectionItem>> { new(playlistItem, index) }, new List<CollectionChangedItem<IPlaylistCollectionItem>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICoreTrackCollection? other) => false;

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ITrack>>(Enumerable.Range(0, limit).Select(_ => new MockTrack()).ToList());

    /// <inheritdoc/>
    public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default)
    {
        TotalTrackCount++;
        TracksChanged?.Invoke(this, new List<CollectionChangedItem<ITrack>> { new(track, index) }, new List<CollectionChangedItem<ITrack>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollectionItem? other) => false;

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollection? other) => false;

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IAlbumCollectionItem>>(Enumerable.Range(0, limit).Select(_ => new MockAlbum()).ToList());

    /// <inheritdoc/>
    public Task AddAlbumItemAsync(IAlbumCollectionItem albumItem, int index, CancellationToken cancellationToken = default)
    {
        TotalAlbumItemsCount++;
        AlbumItemsChanged?.Invoke(this, new List<CollectionChangedItem<IAlbumCollectionItem>> { new(albumItem, index) }, new List<CollectionChangedItem<IAlbumCollectionItem>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollectionItem? other) => false;

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollection? other) => false;

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IArtistCollectionItem>>(Enumerable.Range(0, limit).Select(_ => new MockArtist()).ToList());

    /// <inheritdoc/>
    public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default)
    {
        TotalArtistItemsCount++;
        ArtistItemsChanged?.Invoke(this, new List<CollectionChangedItem<IArtistCollectionItem>> { new(artistItem, index) }, new List<CollectionChangedItem<IArtistCollectionItem>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICorePlayableCollectionGroupChildren? other) => false;

    /// <inheritdoc/>
    public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default)
    {
        PlaybackState = PlaybackState.Playing;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IPlayableCollectionGroup>>(Enumerable.Range(0, limit).Select(_ => new MockPlayableCollectionGroup()).ToList());

    /// <inheritdoc/>
    public Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default)
    {
        TotalChildrenCount++;
        ChildItemsChanged?.Invoke(this, new List<CollectionChangedItem<IPlayableCollectionGroup>> { new(child, index) }, new List<CollectionChangedItem<IPlayableCollectionGroup>>());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool Equals(ICorePlayableCollectionGroup? other) => false;

    /// <inheritdoc/>
    public ValueTask DisposeAsync() => default;
}
