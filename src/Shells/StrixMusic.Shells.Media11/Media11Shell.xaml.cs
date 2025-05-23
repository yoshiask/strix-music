using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Controls;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Sdk.WinUI.Globalization;
using StrixMusic.Shells.Media11.Helper;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages.Abstract;
using StrixMusic.Shells.Media11.ViewModels.Collections;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Shells.Media11
{
    public sealed partial class Media11Shell : Shell
    {
        private readonly NavigationTracker _navigationTracker = new();

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Media11Shell), new PropertyMetadata(string.Empty));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="ShowLargeHeader"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLargeHeaderProperty =
            DependencyProperty.Register(nameof(ShowLargeHeader), typeof(bool), typeof(Media11Shell), new PropertyMetadata(true));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="PlaylistCollectionViewModel"/> property.
        /// </summary>
        public static readonly DependencyProperty PlaylistCollectionViewModelProperty =
            DependencyProperty.Register(nameof(PlaylistCollectionViewModel), typeof(Media11PlaylistCollectionViewModel), typeof(Media11Shell), new PropertyMetadata(null));
        
        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty HamburgerPressedCommandProperty =
            DependencyProperty.Register(nameof(HamburgerPressedCommand), typeof(RelayCommand), typeof(Media11Shell), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="Media11Music"/> class.
        /// </summary>
        public Media11Shell()
        {
            this.InitializeComponent();

            WindowHostOptions.IsSystemBackButtonVisible = true;
            WindowHostOptions.BackgroundColor = Colors.Black;
            WindowHostOptions.ForegroundColor = Colors.White;
            
            WindowHostOptions.IsSystemBackButtonVisible = true;
            WindowHostOptions.ExtendViewIntoTitleBar = true;
            WindowHostOptions.CustomTitleBar = CustomTitleBarBorder;

            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested += CurrentView_BackRequested;

            // Register home page navigation
            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register album, artist, and playlist page navigation
            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<ArtistViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<PlaylistViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register playlists page navigation
            WeakReferenceMessenger.Default.Register<PlaylistsViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            RegisterPropertyChangedCallback(RootProperty, (x, _) => ((Media11Shell)x).OnRootChanged());

            Unloaded += Media11Shell_Unloaded;
            Loaded += Media11Shell_Loaded;
        }

        private void Media11Shell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Media11Shell_Loaded;

            _navigationTracker.Initialize();
            OnRootChanged();
        }

        private void Media11Shell_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= Media11Shell_Unloaded;

            WeakReferenceMessenger.Default.Reset();
        }

        private void OnRootChanged()
        {
            if (Root is null)
                return;

            var libVm = Root.Library as LibraryViewModel ?? new LibraryViewModel(Root.Library);

            PlaylistCollectionViewModel = new Media11PlaylistCollectionViewModel
            {
                PlaylistCollection = libVm,
            };

            if (Root?.Library != null)
            {
                _ = WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequestMessage(libVm));
            }
        }

        /// <summary>
        /// Gets or sets the Title text for the Media11 Shell.
        /// </summary>
        public bool ShowLargeHeader
        {
            get { return (bool)GetValue(ShowLargeHeaderProperty); }
            set { SetValue(ShowLargeHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title text for the Media11 Shell.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The <see cref="Media11PlaylistCollectionViewModel"/> for the <see cref="Controls.Collections.Media11PlaylistCollection"/> on display in the pane.
        /// </summary>
        public Media11PlaylistCollectionViewModel? PlaylistCollectionViewModel
        {
            get => (Media11PlaylistCollectionViewModel?)GetValue(PlaylistCollectionViewModelProperty);
            set => SetValue(PlaylistCollectionViewModelProperty, value);
        }

        /// <summary>
        /// Gets a Command that handles a Hamburger button press.
        /// </summary>
        public RelayCommand HamburgerPressedCommand
        {
            get => (RelayCommand)GetValue(HamburgerPressedCommandProperty);
            set => SetValue(HamburgerPressedCommandProperty, value);
        }

        private void CurrentView_BackRequested(object? sender, BackRequestedEventArgs e)
        {
            _navigationTracker.NavigateBackwards();
        }

        private void NavigatePage<T>(PageNavigationRequestMessage<T> viewModel)
        {
            MainContent.Content = viewModel.PageData;

            if (viewModel is PlaylistsViewNavigationRequestMessage playlistsNavReq)
            {
                if (Resources.TryGetValue("Media11PlaylistsPageDataTemplate", out var dataTemplate))
                    MainContent.ContentTemplate = (DataTemplate)dataTemplate;
            }
            
            Title = LocalizationResources.Music?.GetString(viewModel.PageTitleResource) ?? viewModel.PageTitleResource;
            ShowLargeHeader = viewModel.ShowLargeHeader;
        }

        private void Segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not Segmented navigationBar || Root is null)
                return;

            switch (navigationBar.SelectedIndex)
            {
                case 2:
                    WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequestMessage((LibraryViewModel)Root.Library));
                    break;

                case 3:
                    WeakReferenceMessenger.Default.Send(new PlaylistsViewNavigationRequestMessage((LibraryViewModel)Root.Library));
                    break;
            }
        }
    }
}
