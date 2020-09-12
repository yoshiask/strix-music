﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class ObservableTrack : ObservableMergeableObject<ITrack>
    {
        private readonly ITrack _track;

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ITrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        public ObservableTrack(ITrack track)
        {
            _track = track;

            if (_track.Album != null)
                Album = new ObservableAlbum(_track.Album);

            if (_track.RelatedItems != null)
                RelatedItems = new ObservableCollectionGroup(_track.RelatedItems);

            Genres = new ObservableCollection<string>(_track.Genres);
            Artists = new ObservableCollection<ObservableArtist>(_track.Artists.Select(x => new ObservableArtist(x)));
            Images = new ObservableCollection<IImage>(_track.Images);
            SourceCore = new ObservableCore(_track.SourceCore);

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeImagesAsyncCommand = new AsyncRelayCommand<IReadOnlyList<IImage>>(ChangeImagesAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _track.AlbumChanged += Track_AlbumChanged;
            _track.ArtistsChanged += Track_ArtistsChanged;
            _track.DescriptionChanged += Track_DescriptionChanged;
            _track.GenresChanged += Track_GenresChanged;
            _track.IsExplicitChanged += Track_IsExplicitChanged;
            _track.LanguageChanged += Track_LanguageChanged;
            _track.LyricsChanged += Track_LyricsChanged;
            _track.NameChanged += Track_NameChanged;
            _track.PlaybackStateChanged += Track_PlaybackStateChanged;
            _track.PlayCountChanged += Track_PlayCountChanged;
            _track.TrackNumberChanged += Track_TrackNumberChanged;
            _track.UrlChanged += Track_UrlChanged;
            _track.ImagesChanged += Track_ImagesChanged;
        }

        private void DetachEvents()
        {
            _track.AlbumChanged -= Track_AlbumChanged;
            _track.ArtistsChanged -= Track_ArtistsChanged;
            _track.DescriptionChanged -= Track_DescriptionChanged;
            _track.GenresChanged -= Track_GenresChanged;
            _track.IsExplicitChanged -= Track_IsExplicitChanged;
            _track.LanguageChanged -= Track_LanguageChanged;
            _track.LyricsChanged -= Track_LyricsChanged;
            _track.NameChanged -= Track_NameChanged;
            _track.PlaybackStateChanged -= Track_PlaybackStateChanged;
            _track.PlayCountChanged -= Track_PlayCountChanged;
            _track.TrackNumberChanged -= Track_TrackNumberChanged;
            _track.UrlChanged -= Track_UrlChanged;
            _track.ImagesChanged -= Track_ImagesChanged;
        }

        private void Track_ImagesChanged(object sender, CollectionChangedEventArgs<IImage> e)
        {
            foreach (var item in e.AddedItems)
            {
                Images.Insert(item.Index, item.Data);
            }

            foreach (var item in e.RemovedItems)
            {
                Images.RemoveAt(item.Index);
            }
        }

        private void Track_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Track_TrackNumberChanged(object sender, int? e)
        {
            TrackNumber = e;
        }

        private void Track_PlayCountChanged(object sender, int? e)
        {
            PlayCount = e;
        }

        private void Track_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Track_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Track_LyricsChanged(object sender, ILyrics? e)
        {
            Lyrics = e;
        }

        private void Track_LanguageChanged(object sender, CultureInfo? e)
        {
            Language = e;
        }

        private void Track_IsExplicitChanged(object sender, bool e)
        {
            IsExplicit = e;
        }

        private void Track_GenresChanged(object sender, CollectionChangedEventArgs<string> e)
        {
            foreach (var item in e.AddedItems)
            {
                Genres.Insert(item.Index, item.Data);
            }

            foreach (var item in e.RemovedItems)
            {
                Genres.RemoveAt(item.Index);
            }
        }

        private void Track_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void Track_ArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Artists.Insert(item.Index, new ObservableArtist(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Artists.RemoveAt(item.Index);
            }
        }

        private void Track_AlbumChanged(object sender, IAlbum? e)
        {
            if (e != null)
                Album = new ObservableAlbum(e);
            else
                Album = null;
        }

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url
        {
            get => _track.Url;
            set => SetProperty(() => _track.Url, value);
        }

        /// <inheritdoc cref="ITrack.Type"/>
        public TrackType Type => _track.Type;

        /// <inheritdoc cref="IArtistCollection.Artists"/>
        public ObservableCollection<ObservableArtist> Artists { get; }

        private ObservableAlbum? _album;

        /// <inheritdoc cref="ITrack.Album"/>
        public ObservableAlbum? Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }

        /// <inheritdoc cref="ITrack.Genres"/>
        public ObservableCollection<string> Genres { get; }

        /// <inheritdoc cref="ITrack.TrackNumber"/>
        public int? TrackNumber
        {
            get => _track.TrackNumber;
            set => SetProperty(() => _track.TrackNumber, value);
        }

        /// <inheritdoc cref="ITrack.PlayCount"/>
        public int? PlayCount
        {
            get => _track.PlayCount;
            set => SetProperty(() => _track.PlayCount, value);
        }

        /// <inheritdoc cref="ITrack.Language"/>
        public CultureInfo? Language
        {
            get => _track.Language;
            set => SetProperty(() => _track.Language, value);
        }

        /// <inheritdoc cref="ITrack.Lyrics"/>
        public ILyrics? Lyrics
        {
            get => _track.Lyrics;
            set => SetProperty(() => _track.Lyrics, value);
        }

        /// <inheritdoc cref="ITrack.IsExplicit"/>
        public bool IsExplicit
        {
            get => _track.IsExplicit;
            set => SetProperty(() => _track.IsExplicit, value);
        }

        /// <inheritdoc cref="IPlayable.Duration"/>
        public TimeSpan Duration => _track.Duration;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public ObservableCore SourceCore { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _track.Id;

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name
        {
            get => _track.Name;
            set => SetProperty(() => _track.Name, value);
        }

        /// <inheritdoc cref="IPlayable.Images"/>
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description
        {
            get => _track.Description;
            set => SetProperty(() => _track.Description, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _track.PlaybackState;
            set => SetProperty(() => _track.PlaybackState, value);
        }

        /// <inheritdoc cref="IPlayable.IsPlayAsyncSupported"/>
        public bool IsPlayAsyncSupported
        {
            get => _track.IsPlayAsyncSupported;
            set => SetProperty(() => _track.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsPauseAsyncSupported"/>
        public bool IsPauseAsyncSupported
        {
            get => _track.IsPauseAsyncSupported;
            set => SetProperty(() => _track.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeNameAsyncSupported"/>
        public bool IsChangeNameAsyncSupported
        {
            get => _track.IsChangeNameAsyncSupported;
            set => SetProperty(() => _track.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeImagesAsyncSupported"/>
        public bool IsChangeImagesAsyncSupported
        {
            get => _track.IsChangeImagesAsyncSupported;
            set => SetProperty(() => _track.IsChangeImagesAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeDescriptionAsyncSupported"/>
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _track.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _track.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeDurationAsyncSupported"/>
        public bool IsChangeDurationAsyncSupported
        {
            get => _track.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _track.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.RelatedItems"/>
        public ObservableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc cref="ITrack.IsChangeArtistsAsyncSupported"/>
        public bool IsChangeArtistsAsyncSupported
        {
            get => _track.IsChangeArtistsAsyncSupported;
            set => SetProperty(() => _track.IsChangeArtistsAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.IsChangeAlbumAsyncSupported"/>
        public bool IsChangeAlbumAsyncSupported
        {
            get => _track.IsChangeAlbumAsyncSupported;
            set => SetProperty(() => _track.IsChangeAlbumAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.IsChangeGenresAsyncSupported"/>
        public bool IsChangeGenresAsyncSupported
        {
            get => _track.IsChangeGenresAsyncSupported;
            set => SetProperty(() => _track.IsChangeGenresAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.IsChangeTrackNumberAsyncSupported"/>
        public bool IsChangeTrackNumberAsyncSupported
        {
            get => _track.IsChangeTrackNumberAsyncSupported;
            set => SetProperty(() => _track.IsChangeTrackNumberAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.IsChangeLanguageAsyncSupported"/>
        public bool IsChangeLanguageAsyncSupported
        {
            get => _track.IsChangeLanguageAsyncSupported;
            set => SetProperty(() => _track.IsChangeLanguageAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.IsChangeLyricsAsyncSupported"/>
        public bool IsChangeLyricsAsyncSupported
        {
            get => _track.IsChangeLyricsAsyncSupported;
            set => SetProperty(() => _track.IsChangeLyricsAsyncSupported, value);
        }

        /// <inheritdoc cref="ITrack.IsChangeIsExplicitAsyncSupported"/>
        public bool IsChangeIsExplicitAsyncSupported
        {
            get => _track.IsChangeIsExplicitAsyncSupported;
            set => SetProperty(() => _track.IsChangeIsExplicitAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _track.PlaybackStateChanged += value;

            remove => _track.PlaybackStateChanged -= value;
        }

        /// <inheritdoc cref="IArtistCollection.ArtistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged
        {
            add => _track.ArtistsChanged += value;

            remove => _track.ArtistsChanged -= value;
        }

        /// <inheritdoc cref="ITrack.GenresChanged"/>
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged
        {
            add => _track.GenresChanged += value;

            remove => _track.GenresChanged -= value;
        }

        /// <inheritdoc cref="ITrack.AlbumChanged"/>
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add => _track.AlbumChanged += value;

            remove => _track.AlbumChanged -= value;
        }

        /// <inheritdoc cref="ITrack.TrackNumberChanged"/>
        public event EventHandler<int?> TrackNumberChanged
        {
            add => _track.TrackNumberChanged += value;

            remove => _track.TrackNumberChanged -= value;
        }

        /// <inheritdoc cref="ITrack.PlayCountChanged"/>
        public event EventHandler<int?> PlayCountChanged
        {
            add => _track.PlayCountChanged += value;

            remove => _track.PlayCountChanged -= value;
        }

        /// <inheritdoc cref="ITrack.LanguageChanged"/>
        public event EventHandler<CultureInfo?> LanguageChanged
        {
            add => _track.LanguageChanged += value;

            remove => _track.LanguageChanged -= value;
        }

        /// <inheritdoc cref="ITrack.LyricsChanged"/>
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add => _track.LyricsChanged += value;

            remove => _track.LyricsChanged -= value;
        }

        /// <inheritdoc cref="ITrack.IsExplicitChanged"/>
        public event EventHandler<bool> IsExplicitChanged
        {
            add => _track.IsExplicitChanged += value;

            remove => _track.IsExplicitChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.NameChanged"/>
        public event EventHandler<string> NameChanged
        {
            add => _track.NameChanged += value;

            remove => _track.NameChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.DescriptionChanged"/>
        public event EventHandler<string?> DescriptionChanged
        {
            add => _track.DescriptionChanged += value;

            remove => _track.DescriptionChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.UrlChanged"/>
        public event EventHandler<Uri?> UrlChanged
        {
            add => _track.UrlChanged += value;

            remove => _track.UrlChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged
        {
            add => _track.ImagesChanged += value;

            remove => _track.ImagesChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.DurationChanged"/>
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _track.DurationChanged += value;

            remove => _track.DurationChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        public Task PauseAsync()
        {
            return _track.PauseAsync();
        }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync()
        {
            return _track.PlayAsync();
        }

        /// <inheritdoc cref="IPlayable.ChangeNameAsync"/>
        public Task ChangeNameAsync(string name) => _track.ChangeNameAsync(name);

        /// <inheritdoc cref="IPlayable.ChangeImagesAsync"/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images) => _track.ChangeImagesAsync(images);

        /// <inheritdoc cref="IPlayable.ChangeDescriptionAsync"/>
        public Task ChangeDescriptionAsync(string? description) => _track.ChangeDescriptionAsync(description);

        /// <inheritdoc cref="IPlayable.ChangeDurationAsync"/>
        public Task ChangeDurationAsync(TimeSpan duration) => _track.ChangeDurationAsync(duration);

        /// <summary>
        /// Attempts to play the track.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the track, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the images for the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeImagesAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }
    }
}
