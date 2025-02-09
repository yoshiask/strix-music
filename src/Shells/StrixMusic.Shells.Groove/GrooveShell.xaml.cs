﻿using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Sdk.WinUI.Services.Localization;
using StrixMusic.Sdk.WinUI.Services.ShellManagement;
using StrixMusic.Shells.Groove.Helper;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Shells.Groove
{
    public sealed partial class GrooveShell : Shell
    {
        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(GrooveShell), new PropertyMetadata(null));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="ShowLargeHeader"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLargeHeaderProperty =
            DependencyProperty.Register(nameof(ShowLargeHeader), typeof(bool), typeof(GrooveShell), new PropertyMetadata(true));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="PlaylistCollectionViewModel"/> property.
        /// </summary>
        public static readonly DependencyProperty PlaylistCollectionViewModelProperty =
            DependencyProperty.Register(nameof(PlaylistCollectionViewModel), typeof(GroovePlaylistCollectionViewModel), typeof(GrooveShell), new PropertyMetadata(null));

        private LocalizationResourceLoader _localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveShell"/> class.
        /// </summary>
        public GrooveShell(StrixDataRootViewModel dataRootViewModel)
            : base(dataRootViewModel)
        {
            this.InitializeComponent();
            
            // Register home page navigation
            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register album, artist, and playlist page navigation
            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<ArtistViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<PlaylistViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register playlists page navigation
            WeakReferenceMessenger.Default.Register<PlaylistsViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            _localizationService = Ioc.Default.GetRequiredService<LocalizationResourceLoader>();

            HamburgerPressedCommand = new RelayCommand(HamburgerToggled);

            RegisterPropertyChangedCallback(DataRootProperty, new DependencyPropertyChangedCallback((x, y) => x.Cast<GrooveShell>().OnDataRootChanged()));

            Unloaded += GrooveShell_Unloaded;
            Loaded += GrooveShell_Loaded;
        }

        /// <summary>
        /// Metadata used to identify this shell before instantiation.
        /// </summary>
        public static ShellMetadata Metadata { get; }
            = new ShellMetadata(id: "GrooveMusic.10.21061.10121.0",
                displayName: "Groove Music",
                description: "A faithful recreation of the Groove Music app from Windows 10");

        private void GrooveShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= GrooveShell_Loaded;

            Guard.IsNotNull(DataRoot, nameof(DataRoot));

            Notifications.IsHandled = true;
            NavigationTracker.Instance.Initialize();
            OnDataRootChanged();
        }

        private void GrooveShell_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= GrooveShell_Unloaded;

            WeakReferenceMessenger.Default.Reset();
        }

        private void OnDataRootChanged()
        {
            if (DataRoot is null)
                return;

            PlaylistCollectionViewModel = new GroovePlaylistCollectionViewModel
            {
                PlaylistCollection = (LibraryViewModel)DataRoot.Library
            };

            if (DataRoot?.Library != null)
            {
                Bindings.Update();
                _ = WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequestMessage((LibraryViewModel)DataRoot.Library));
            }
        }

        /// <summary>
        /// Gets or sets the Title text for the Groove Shell.
        /// </summary>
        public bool ShowLargeHeader
        {
            get { return (bool)GetValue(ShowLargeHeaderProperty); }
            set { SetValue(ShowLargeHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title text for the Groove Shell.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The <see cref="GroovePlaylistCollectionViewModel"/> for the <see cref="Controls.Collections.GroovePlaylistCollection"/> on display in the pane.
        /// </summary>
        public GroovePlaylistCollectionViewModel? PlaylistCollectionViewModel
        {
            get => (GroovePlaylistCollectionViewModel?)GetValue(PlaylistCollectionViewModelProperty);
            set => SetValue(PlaylistCollectionViewModelProperty, value);
        }

        /// <summary>
        /// Gets a Command that handles a Hamburger button press.
        /// </summary>
        public RelayCommand HamburgerPressedCommand { get; }

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            base.SetupTitleBar();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
#endif

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentView_BackRequested;
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            NavigationTracker.Instance.NavigateBackwards();
        }

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                switch (button.Tag as string)
                {
                    case "MyMusic":
                        Guard.IsNotNull(DataRoot?.Library, nameof(DataRoot.Library));
                        WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequestMessage((LibraryViewModel)DataRoot.Library));
                        break;
                    case "Playlists":
                        Guard.IsNotNull(DataRoot?.Library, nameof(DataRoot.Library));
                        WeakReferenceMessenger.Default.Send(new PlaylistsViewNavigationRequestMessage((LibraryViewModel)DataRoot.Library));
                        break;
                }
            }
        }

        private void HamburgerToggled()
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void OnPaneStateChanged(SplitView sender, object e) => UpdatePaneState();

        private void UpdatePaneState()
        {
            if (MainSplitView.IsPaneOpen)
                VisualStateManager.GoToState(this, "Full", true);
            else
                VisualStateManager.GoToState(this, "Compact", true);
        }

        private void NavigatePage<T>(PageNavigationRequestMessage<T> viewModel)
        {
            MainContent.Content = viewModel.PageData;

            if (viewModel is PlaylistsViewNavigationRequestMessage playlistsNavReq)
            {
                if (Resources.TryGetValue("GroovePlaylistsPageDataTemplate", out var dataTemplate))
                    MainContent.ContentTemplate = (DataTemplate)dataTemplate;
                else
                    Ioc.Default.GetRequiredService<INotificationService>().RaiseNotification("Error", "Unable to show page.");
            }

            Guard.IsNotNull(_localizationService, nameof(_localizationService));
            Title = _localizationService.Music?.GetString(viewModel.PageTitleResource) ?? viewModel.PageTitleResource;
            ShowLargeHeader = viewModel.ShowLargeHeader;

            UpdateSelectedNavigationButton(viewModel);
        }

        private void UpdateSelectedNavigationButton<T>(PageNavigationRequestMessage<T> viewModel)
        {
            ToggleButton? button = viewModel switch
            {
                HomeViewNavigationRequestMessage _ => MyMusicButton,
                PlaylistViewNavigationRequestMessage _ => PlaylistsButton,
                _ => null,
            };

            // Reset all buttons, but not the PlaylistList
            MyMusicButton.IsChecked = false;
            RecentButton.IsChecked = false;
            NowPlayingButton.IsChecked = false;
            PlaylistsButton.IsChecked = false;

            if (button != null)
            {
                PlaylistList.ClearSelected();

                // Set the active navigation button
                button.IsChecked = true;
            }
        }
    }
}
