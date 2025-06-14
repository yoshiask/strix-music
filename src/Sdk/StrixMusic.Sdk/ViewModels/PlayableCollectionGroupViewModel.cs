﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore;
using OwlCore.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, ISdkViewModel, IPlayableCollectionGroup, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel, IDelegable<IPlayableCollectionGroup>
    {
        private readonly IPlayableCollectionGroup _collectionGroup;

        private readonly SemaphoreSlim _populateTracksMutex = new(1, 1);
        private readonly SemaphoreSlim _populateAlbumsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateArtistsMutex = new(1, 1);
        private readonly SemaphoreSlim _populatePlaylistsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateChildrenMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionGroup"/> containing properties about this class.</param>
        public PlayableCollectionGroupViewModel(IPlayableCollectionGroup collectionGroup)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

            _collectionGroup = collectionGroup;
            
            PauseAlbumCollectionAsyncCommand = new AsyncRelayCommand(PauseAlbumCollectionAsync);
            PlayAlbumCollectionAsyncCommand = new AsyncRelayCommand(PlayAlbumCollectionAsync);
            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);
            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);
            PausePlaylistCollectionAsyncCommand = new AsyncRelayCommand(PausePlaylistCollectionAsync);
            PlayPlaylistCollectionAsyncCommand = new AsyncRelayCommand(PlayPlaylistCollectionAsync);
            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>((x, y) => _collectionGroup.PlayTrackCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<ITrack>(nameof(x)), y));
            PlayAlbumAsyncCommand = new AsyncRelayCommand<IAlbumCollectionItem>((x, y) => _collectionGroup.PlayAlbumCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IAlbumCollectionItem>(nameof(x)), y));
            PlayPlaylistAsyncCommand = new AsyncRelayCommand<IPlaylistCollectionItem>((x, y) => _collectionGroup.PlayPlaylistCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IPlaylistCollectionItem>(nameof(x)), y));
            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>((x, y) => _collectionGroup.PlayArtistCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IArtistCollectionItem>(nameof(x)), y));

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMorePlaylistsCommand = new AsyncRelayCommand<int>(PopulateMorePlaylistsAsync);
            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreChildrenCommand = new AsyncRelayCommand<int>(PopulateMoreChildrenAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);
            InitTrackCollectionAsyncCommand = new AsyncRelayCommand(InitTrackCollectionAsync);
            InitArtistCollectionAsyncCommand = new AsyncRelayCommand(InitArtistCollectionAsync);
            InitAlbumCollectionAsyncCommand = new AsyncRelayCommand(InitAlbumCollectionAsync);
            InitPlaylistCollectionAsyncCommand = new AsyncRelayCommand(InitPlaylistCollectionAsync);

            ChangeTrackCollectionSortingTypeCommand = new RelayCommand<TrackSortingType>(x => SortTrackCollection(x, CurrentTracksSortingDirection));
            ChangeTrackCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortTrackCollection(CurrentTracksSortingType, x));
            ChangeArtistCollectionSortingTypeCommand = new RelayCommand<ArtistSortingType>(x => SortArtistCollection(x, CurrentArtistSortingDirection));
            ChangeArtistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortArtistCollection(CurrentArtistSortingType, x));
            ChangeAlbumCollectionSortingTypeCommand = new RelayCommand<AlbumSortingType>(x => SortAlbumCollection(x, CurrentAlbumSortingDirection));
            ChangeAlbumCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortAlbumCollection(CurrentAlbumSortingType, x));
            ChangePlaylistCollectionSortingTypeCommand = new RelayCommand<PlaylistSortingType>(x => SortPlaylistCollection(x, CurrentPlaylistSortingDirection));
            ChangePlaylistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortPlaylistCollection(CurrentPlaylistSortingType, x));

            Albums = new ObservableCollection<IAlbumCollectionItem>();
            Artists = new ObservableCollection<IArtistCollectionItem>();
            Children = new ObservableCollection<PlayableCollectionGroupViewModel>();
            Playlists = new ObservableCollection<IPlaylistCollectionItem>();
            Tracks = new ObservableCollection<TrackViewModel>();
            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();

            UnsortedAlbums = new ObservableCollection<IAlbumCollectionItem>();
            UnsortedArtists = new ObservableCollection<IArtistCollectionItem>();
            UnsortedPlaylists = new ObservableCollection<IPlaylistCollectionItem>();
            UnsortedTracks = new ObservableCollection<TrackViewModel>();

            AttachPropertyEvents();
        }

        private void AttachPropertyEvents()
        {
            PlaybackStateChanged += CollectionGroupPlaybackStateChanged;
            DescriptionChanged += CollectionGroupDescriptionChanged;
            NameChanged += CollectionGroupNameChanged;
            LastPlayedChanged += CollectionGroupLastPlayedChanged;
            DownloadInfoChanged += OnDownloadInfoChanged;

            AlbumItemsCountChanged += CollectionGroupOnAlbumItemsCountChanged;
            TracksCountChanged += CollectionGroupOnTrackItemsCountChanged;
            ArtistItemsCountChanged += CollectionGroupOnArtistItemsCountChanged;
            PlaylistItemsCountChanged += CollectionGroupOnPlaylistItemsCountChanged;
            ChildrenCountChanged += CollectionGroupOnTotalChildrenCountChanged;
            ImagesCountChanged += PlayableCollectionGroupViewModel_ImagesCountChanged;
            UrlsCountChanged += PlayableCollectionGroupViewModel_UrlsCountChanged;

            IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsPlayPlaylistCollectionAsyncAvailableChanged += OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            IsPausePlaylistCollectionAsyncAvailableChanged += OnIsPausePlaylistCollectionAsyncAvailableChanged;
            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsChanged += PlayableCollectionGroupViewModel_AlbumItemsChanged;
            TracksChanged += PlayableCollectionGroupViewModel_TrackItemsChanged;
            ArtistItemsChanged += PlayableCollectionGroupViewModel_ArtistItemsChanged;
            PlaylistItemsChanged += PlayableCollectionGroupViewModel_PlaylistItemsChanged;
            ChildItemsChanged += PlayableCollectionGroupViewModel_ChildItemsChanged;
            ImagesChanged += PlayableCollectionGroupViewModel_ImagesChanged;
            UrlsChanged += PlayableCollectionGroupViewModel_UrlsChanged;
        }

        private void DetachPropertyEvents()
        {
            PlaybackStateChanged -= CollectionGroupPlaybackStateChanged;
            DescriptionChanged -= CollectionGroupDescriptionChanged;
            NameChanged -= CollectionGroupNameChanged;
            LastPlayedChanged -= CollectionGroupLastPlayedChanged;
            DownloadInfoChanged -= OnDownloadInfoChanged;

            AlbumItemsCountChanged -= CollectionGroupOnAlbumItemsCountChanged;
            TracksCountChanged -= CollectionGroupOnTrackItemsCountChanged;
            ArtistItemsCountChanged -= CollectionGroupOnArtistItemsCountChanged;
            PlaylistItemsCountChanged -= CollectionGroupOnPlaylistItemsCountChanged;
            ChildrenCountChanged -= CollectionGroupOnTotalChildrenCountChanged;
            ImagesCountChanged += PlayableCollectionGroupViewModel_ImagesCountChanged;

            IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsPlayPlaylistCollectionAsyncAvailableChanged -= OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            IsPausePlaylistCollectionAsyncAvailableChanged -= OnIsPausePlaylistCollectionAsyncAvailableChanged;
            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsChanged -= PlayableCollectionGroupViewModel_AlbumItemsChanged;
            TracksChanged -= PlayableCollectionGroupViewModel_TrackItemsChanged;
            ArtistItemsChanged -= PlayableCollectionGroupViewModel_ArtistItemsChanged;
            PlaylistItemsChanged -= PlayableCollectionGroupViewModel_PlaylistItemsChanged;
            ChildItemsChanged -= PlayableCollectionGroupViewModel_ChildItemsChanged;
            ImagesChanged -= PlayableCollectionGroupViewModel_ImagesChanged;
        }

        /// <inheritdoc/>
        IPlayableCollectionGroup IDelegable<IPlayableCollectionGroup>.Inner => _collectionGroup;

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => _collectionGroup.SourcesChanged += value;
            remove => _collectionGroup.SourcesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _collectionGroup.NameChanged += value;
            remove => _collectionGroup.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _collectionGroup.DescriptionChanged += value;
            remove => _collectionGroup.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collectionGroup.PlaybackStateChanged += value;
            remove => _collectionGroup.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => _collectionGroup.DownloadInfoChanged += value;
            remove => _collectionGroup.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collectionGroup.DurationChanged += value;
            remove => _collectionGroup.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _collectionGroup.LastPlayedChanged += value;
            remove => _collectionGroup.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _collectionGroup.IsChangeNameAsyncAvailableChanged += value;
            remove => _collectionGroup.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _collectionGroup.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _collectionGroup.IsChangeDurationAsyncAvailableChanged += value;
            remove => _collectionGroup.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged
        {
            add => _collectionGroup.TracksCountChanged += value;
            remove => _collectionGroup.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _collectionGroup.ArtistItemsCountChanged += value;
            remove => _collectionGroup.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _collectionGroup.AlbumItemsCountChanged += value;
            remove => _collectionGroup.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged
        {
            add => _collectionGroup.PlaylistItemsCountChanged += value;
            remove => _collectionGroup.PlaylistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _collectionGroup.ImagesCountChanged += value;
            remove => _collectionGroup.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _collectionGroup.UrlsCountChanged += value;
            remove => _collectionGroup.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _collectionGroup.ImagesChanged += value;
            remove => _collectionGroup.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => _collectionGroup.PlaylistItemsChanged += value;
            remove => _collectionGroup.PlaylistItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => _collectionGroup.TracksChanged += value;
            remove => _collectionGroup.TracksChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => _collectionGroup.AlbumItemsChanged += value;
            remove => _collectionGroup.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _collectionGroup.ArtistItemsChanged += value;
            remove => _collectionGroup.ArtistItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged
        {
            add => _collectionGroup.ChildItemsChanged += value;
            remove => _collectionGroup.ChildItemsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ChildrenCountChanged
        {
            add => _collectionGroup.ChildrenCountChanged += value;
            remove => _collectionGroup.ChildrenCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _collectionGroup.UrlsChanged += value;
            remove => _collectionGroup.UrlsChanged -= value;
        }

        private void CollectionGroupNameChanged(object? sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void CollectionGroupDescriptionChanged(object? sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void CollectionGroupPlaybackStateChanged(object? sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object? sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void CollectionGroupOnTotalChildrenCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalChildrenCount)), null);

        private void CollectionGroupOnPlaylistItemsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalPlaylistItemsCount)), null);

        private void CollectionGroupOnArtistItemsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalArtistItemsCount)), null);

        private void CollectionGroupOnTrackItemsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalTrackCount)), null);

        private void CollectionGroupOnAlbumItemsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalAlbumItemsCount)), null);

        private void PlayableCollectionGroupViewModel_ImagesCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void PlayableCollectionGroupViewModel_UrlsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void CollectionGroupLastPlayedChanged(object? sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseAlbumCollectionAsyncAvailable)), null);

        private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayAlbumCollectionAsyncAvailable)), null);

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)), null);

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)), null);

        private void OnIsPausePlaylistCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPausePlaylistCollectionAsyncAvailable)), null);

        private void OnIsPlayPlaylistCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayPlaylistCollectionAsyncAvailable)), null);

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)), null);

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)), null);

        private void PlayableCollectionGroupViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems) => _syncContext.Post(_ =>
        {
            if (CurrentAlbumSortingType == AlbumSortingType.Unsorted)
            {
                Albums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IAlbum album => new AlbumViewModel(album),
                    IAlbumCollection collection => new AlbumCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            }
            else
            {
                // Make sure both ordered and unordered album are updated. 
                UnsortedAlbums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IAlbum album => new AlbumViewModel(album),
                    IAlbumCollection collection => new AlbumCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });

                foreach (var item in UnsortedAlbums)
                {
                    if (!Albums.Contains(item))
                        Albums.Add(item);
                }

                foreach (var item in Albums.ToArray())
                {
                    if (!UnsortedAlbums.Contains(item))
                        Albums.Remove(item);
                }

                SortAlbumCollection(CurrentAlbumSortingType, CurrentAlbumSortingDirection);
            }
        }, null);

        private void PlayableCollectionGroupViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems) => _syncContext.Post(_ =>
        {
            Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
            {
                IArtist artist => new ArtistViewModel(artist),
                IArtistCollection collection => new ArtistCollectionViewModel(collection),
                _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
            });
        }, null);

        private void PlayableCollectionGroupViewModel_ChildItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems) => _syncContext.Post(_ =>
        {
            Children.ChangeCollection(addedItems, removedItems, item => new PlayableCollectionGroupViewModel(item.Data));
        }, null);

        private void PlayableCollectionGroupViewModel_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems) => _syncContext.Post(_ =>
        {
            if (CurrentPlaylistSortingType == PlaylistSortingType.Unsorted)
            {
                Playlists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IPlaylist playlist => new PlaylistViewModel(playlist),
                    IPlaylistCollection collection => new PlaylistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IPlaylistCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            }
            else
            {
                // Make sure both ordered and unordered playlists are updated. 
                UnsortedPlaylists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IPlaylist playlist => new PlaylistViewModel(playlist),
                    IPlaylistCollection collection => new PlaylistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IPlaylistCollection>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });

                foreach (var item in UnsortedPlaylists)
                {
                    if (!Playlists.Contains(item))
                        Playlists.Add(item);
                }

                foreach (var item in Playlists.ToArray())
                {
                    if (!UnsortedPlaylists.Contains(item))
                        Playlists.Remove(item);
                }

                SortPlaylistCollection(CurrentPlaylistSortingType, CurrentPlaylistSortingDirection);
            }
        }, null);

        private void PlayableCollectionGroupViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems) => _syncContext.Post(_ =>
        {
            if (this.CurrentTracksSortingType == TrackSortingType.Unsorted)
            {
                Tracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(x.Data));
            }
            else
            {
                // Make sure both ordered and unordered track are updated. 
                UnsortedTracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(x.Data));

                foreach (var item in UnsortedTracks)
                {
                    if (!Tracks.Contains(item))
                        Tracks.Add(item);
                }

                foreach (var item in Tracks.ToArray())
                {
                    if (!UnsortedTracks.Contains(item))
                        Tracks.Remove(item);
                }

                SortTrackCollection(CurrentTracksSortingType, CurrentTracksSortingDirection);
            }
        }, null);

        private void PlayableCollectionGroupViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _syncContext.Post(_ =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        }, null);

        private void PlayableCollectionGroupViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems) => _syncContext.Post(_ =>
        {
            Urls.ChangeCollection(addedItems, removedItems);
        }, null);

        /// <inheritdoc />
        public string Id => _collectionGroup.Id;

        /// <summary>
        /// The merged sources for this item.
        /// </summary>
        public IReadOnlyList<ICorePlayableCollectionGroup> Sources => _collectionGroup.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public TimeSpan Duration => _collectionGroup.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collectionGroup.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collectionGroup.AddedAt;

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        ///<inheritdoc />
        public ObservableCollection<IAlbumCollectionItem> UnsortedAlbums { get; }

        ///<inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> UnsortedPlaylists { get; }

        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroupBase"/> items in this collection.
        /// </summary>
        public ObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

        /// <inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; set; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public TrackSortingType CurrentTracksSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentTracksSortingDirection { get; private set; }

        ///<inheritdoc />
        public PlaylistSortingType CurrentPlaylistSortingType { get; private set; }

        ///<inheritdoc />
        public SortDirection CurrentPlaylistSortingDirection { get; private set; }

        /// <inheritdoc />
        public ArtistSortingType CurrentArtistSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentArtistSortingDirection { get; private set; }

        /// <inheritdoc />
        public AlbumSortingType CurrentAlbumSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentAlbumSortingDirection { get; private set; }

        /// <inheritdoc />
        public string Name => _collectionGroup.Name;

        /// <inheritdoc />
        public int TotalTrackCount => _collectionGroup.TotalTrackCount;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _collectionGroup.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collectionGroup.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalChildrenCount => _collectionGroup.TotalChildrenCount;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collectionGroup.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _collectionGroup.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _collectionGroup.TotalUrlCount;

        /// <inheritdoc />
        public string? Description => _collectionGroup.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collectionGroup.PlaybackState;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _collectionGroup.DownloadInfo;

        /// <inheritdoc />
        public bool IsPlayPlaylistCollectionAsyncAvailable => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPausePlaylistCollectionAsyncAvailable => _collectionGroup.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _collectionGroup.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _collectionGroup.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => _collectionGroup.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => _collectionGroup.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => _collectionGroup.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => _collectionGroup.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _collectionGroup.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collectionGroup.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collectionGroup.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddAlbumItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddChildAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddPlaylistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemoveAlbumItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemoveChildAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemovePlaylistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemoveTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _collectionGroup.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => ChangeNameInternalAsync(name, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _collectionGroup.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _collectionGroup.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddAlbumItemAsync(album, index, cancellationToken);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddArtistItemAsync(artistItem, index, cancellationToken);

        /// <inheritdoc />
        public Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddChildAsync(child, index, cancellationToken);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddPlaylistItemAsync(playlistItem, index, cancellationToken);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddTrackAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemoveAlbumItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemoveArtistItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemoveChildAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemovePlaylistItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemoveTrackAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _collectionGroup.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetChildrenAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetPlaylistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetTracksAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetAlbumItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetArtistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _collectionGroup.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collectionGroup.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populatePlaylistsMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populatePlaylistsMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetPlaylistItemsAsync(limit, Playlists.Count, cancellationToken))
                    {
                        switch (item)
                        {
                            case IPlaylist playlist:
                                var pvm = new PlaylistViewModel(playlist);
                                Playlists.Add(pvm);
                                UnsortedPlaylists.Add(pvm);
                                break;
                            case IPlaylistCollection collection:
                                var pcvm = new PlaylistCollectionViewModel(collection);
                                Playlists.Add(pcvm);
                                UnsortedPlaylists.Add(pcvm);
                                break;
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateTracksMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateTracksMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetTracksAsync(limit, Tracks.Count, cancellationToken))
                    {
                        var tvm = new TrackViewModel(item);
                        Tracks.Add(tvm);
                        UnsortedTracks.Add(tvm);
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateAlbumsMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateAlbumsMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetAlbumItemsAsync(limit, Albums.Count, cancellationToken))
                    {
                        switch (item)
                        {
                            case IAlbum album:
                                var avm = new AlbumViewModel(album);
                                Albums.Add(avm);
                                UnsortedAlbums.Add(avm);
                                break;
                            case IAlbumCollection collection:
                                var acvm = new AlbumCollectionViewModel(collection);
                                Albums.Add(acvm);
                                UnsortedAlbums.Add(acvm);
                                break;
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateArtistsMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateArtistsMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetArtistItemsAsync(limit, Artists.Count, cancellationToken))
                    {
                        switch (item)
                        {
                            case IArtist artist:
                                var avm = new ArtistViewModel(artist);
                                Artists.Add(avm);
                                UnsortedArtists.Add(avm);
                                break;
                            case IArtistCollection collection:
                                var acvm = new ArtistCollectionViewModel(collection);
                                Artists.Add(acvm);
                                UnsortedArtists.Add(acvm);
                                break;
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreChildrenAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateChildrenMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateChildrenMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetChildrenAsync(limit, Albums.Count, cancellationToken))
                    {
                        Children.Add(new PlayableCollectionGroupViewModel(item));
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateImagesMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateImagesMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetImagesAsync(limit, Images.Count, cancellationToken))
                    {
                        Images.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateUrlsMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateUrlsMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _collectionGroup.GetUrlsAsync(limit, Urls.Count, cancellationToken))
                    {
                        Urls.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => _collectionGroup.PlayAlbumCollectionAsync(albumItem, cancellationToken);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PlayAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PauseAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _collectionGroup.PlayArtistCollectionAsync(artistItem, cancellationToken);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default) => _collectionGroup.PlayPlaylistCollectionAsync(collectionGroup, cancellationToken);

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => _collectionGroup.PlayPlayableCollectionGroupAsync(cancellationToken);

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => _collectionGroup.PausePlayableCollectionGroupAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => _collectionGroup.PlayPlaylistCollectionAsync(playlistItem, cancellationToken);

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PlayPlaylistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PausePlaylistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _collectionGroup.PlayTrackCollectionAsync(track, cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _collectionGroup.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public void SortAlbumCollection(AlbumSortingType albumSorting, SortDirection sortDirection)
        {
            CurrentAlbumSortingType = albumSorting;
            CurrentAlbumSortingDirection = sortDirection;

            CollectionSorting.SortAlbums(Albums, albumSorting, sortDirection, UnsortedAlbums);
        }

        ///<inheritdoc />
        public void SortArtistCollection(ArtistSortingType artistSorting, SortDirection sortDirection)
        {
            CurrentArtistSortingType = artistSorting;
            CurrentArtistSortingDirection = sortDirection;

            CollectionSorting.SortArtists(Artists, artistSorting, sortDirection, UnsortedArtists);
        }

        ///<inheritdoc />
        public void SortPlaylistCollection(PlaylistSortingType playlistSorting, SortDirection sortDirection)
        {
            CurrentPlaylistSortingType = playlistSorting;
            CurrentPlaylistSortingDirection = sortDirection;

            CollectionSorting.SortPlaylists(Playlists, playlistSorting, sortDirection, UnsortedPlaylists);
        }

        ///<inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        /// <inheritdoc />
        public Task InitAlbumCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.AlbumCollectionAsync(this, cancellationToken);

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollectionAsync(this, cancellationToken);

        /// <inheritdoc />
        public Task InitArtistCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ArtistCollectionAsync(this, cancellationToken);

        /// <inheritdoc />
        public Task InitTrackCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.TrackCollectionAsync(this, cancellationToken);

        /// <inheritdoc />
        public Task InitPlaylistCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.PlaylistCollectionAsync(this, cancellationToken);

        /// <summary>
        /// Command to change the name. It triggers <see cref="ChangeNameAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string> ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Command to change the description. It triggers <see cref="ChangeDescriptionAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Command to change the duration. It triggers <see cref="ChangeDurationAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IAlbumCollectionItem> PlayAlbumAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IPlaylistCollectionItem> PlayPlaylistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PausePlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<AlbumSortingType> ChangeAlbumCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeAlbumCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<ArtistSortingType> ChangeArtistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeArtistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<PlaylistSortingType> ChangePlaylistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangePlaylistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroupChildren? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroup? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection? other) => _collectionGroup.Equals(other!);

        /// <inheritdoc />
        public virtual Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(cancellationToken), InitPlaylistCollectionAsync(cancellationToken), InitTrackCollectionAsync(cancellationToken), InitAlbumCollectionAsync(cancellationToken), InitArtistCollectionAsync(cancellationToken));
        }

        private Task ChangeNameInternalAsync(string? name, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(name, nameof(name));
            return _collectionGroup.ChangeNameAsync(name, cancellationToken);
        }

        /// <inheritdoc />
        public bool IsInitialized { get; protected set; }
    }
}
