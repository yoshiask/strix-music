using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using SubSonicMedia.Interfaces;

namespace StrixMusic.Cores.OpenSubsonic;

public class OpenSubsonicPlayableCollectionGroupBase : ICorePlayableCollectionGroup
{
    protected OpenSubsonicPlayableCollectionGroupBase(OpenSubsonicCore sourceCore)
    {
        SourceCore = sourceCore;
        Client = sourceCore.Client;
    }
    
    protected ISubsonicClient Client { get; }

    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public int TotalImageCount { get; }
    public event EventHandler<int>? ImagesCountChanged;
    public int TotalUrlCount { get; }
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event EventHandler<int>? UrlsCountChanged;
    public string Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public DateTime? LastPlayed { get; }
    public PlaybackState PlaybackState { get; }
    public TimeSpan Duration { get; }
    public bool IsChangeNameAsyncAvailable { get; }
    public bool IsChangeDescriptionAsyncAvailable { get; }
    public bool IsChangeDurationAsyncAvailable { get; }
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event EventHandler<PlaybackState>? PlaybackStateChanged;
    public event EventHandler<string>? NameChanged;
    public event EventHandler<string?>? DescriptionChanged;
    public event EventHandler<TimeSpan>? DurationChanged;
    public event EventHandler<DateTime?>? LastPlayedChanged;
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;
    public DateTime? AddedAt { get; }
    public ICore SourceCore { get; }
    public int TotalPlaylistItemsCount { get; }
    public bool IsPlayPlaylistCollectionAsyncAvailable { get; }
    public bool IsPausePlaylistCollectionAsyncAvailable { get; }
    public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;
    public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;
    public event EventHandler<int>? PlaylistItemsCountChanged;
    public int TotalTrackCount { get; }
    public bool IsPlayTrackCollectionAsyncAvailable { get; }
    public bool IsPauseTrackCollectionAsyncAvailable { get; }
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;
    public event EventHandler<int>? TracksCountChanged;
    
    public virtual int TotalAlbumItemsCount { get; }
    public virtual bool IsPlayAlbumCollectionAsyncAvailable { get; }
    public virtual bool IsPauseAlbumCollectionAsyncAvailable { get; }
    public virtual Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public virtual Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public virtual Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public virtual Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public virtual Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;
    public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;
    public event EventHandler<int>? AlbumItemsCountChanged;
    
    public int TotalArtistItemsCount { get; }
    public bool IsPlayArtistCollectionAsyncAvailable { get; }
    public bool IsPauseArtistCollectionAsyncAvailable { get; }
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;
    public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;
    public event EventHandler<int>? ArtistItemsCountChanged;
    public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public int TotalChildrenCount { get; }
    public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event EventHandler<int>? ChildrenCountChanged;
    public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
    public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;
    public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;
    public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;
    public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public virtual IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;
    public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;
    public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task AddChildAsync(ICorePlayableCollectionGroup child, int index, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;
}
