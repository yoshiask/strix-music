using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Media11.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.Media11PlaylistCollection"/>.
    /// </summary>
    public class Media11PlaylistCollectionViewModel : ObservableObject
    {
        private IPlaylistCollectionViewModel? _playlistCollectionViewModel;
        private PlaylistViewModel? _selectedPlaylist;

        /// <summary>
        /// Initializes a new instance of the <see cref="Media11ArtistCollectionViewModel"/> class.
        /// </summary>
        public Media11PlaylistCollectionViewModel()
        {
            NavigateToPlaylistCommand = new RelayCommand<PlaylistViewModel>(NavigateToPlaylist);
        }

        /// <summary>
        /// The <see cref="IPlaylistCollectionViewModel"/> to display.
        /// </summary>
        public IPlaylistCollectionViewModel? PlaylistCollection
        {
            get => _playlistCollectionViewModel;
            set => SetProperty(ref _playlistCollectionViewModel, value, nameof(PlaylistCollection));
        }

        /// <summary>
        /// Gets or sets the selected playlist
        /// </summary>
        public PlaylistViewModel SelectedPlaylist
        {
            get => _selectedPlaylist!;
            set => SetProperty(ref _selectedPlaylist, value);
        }

        /// <summary>
        /// A Command that requests a navigation to a playlist page.
        /// </summary>
        public RelayCommand<PlaylistViewModel> NavigateToPlaylistCommand { get; private set; }

        private void NavigateToPlaylist(PlaylistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new PlaylistViewNavigationRequestMessage(viewModel));
        }
    }
}
