using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Media11.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for an <see cref="Controls.Pages.Media11PlaylistsPage"/>.
    /// </summary>
    public class Media11PlaylistsPageViewModel : ObservableObject
    {
        private IPlaylistCollectionViewModel? _playlistCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Media11PlaylistPageViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="PlaylistViewModel"/> inside this ViewModel on display.</param>
        public Media11PlaylistsPageViewModel()
        {
        }

        /// <summary>
        /// The <see cref="PlaylistViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IPlaylistCollectionViewModel? PlaylistCollection
        {
            get => _playlistCollectionViewModel;
            set => SetProperty(ref _playlistCollectionViewModel, value);
        }
    }
}
