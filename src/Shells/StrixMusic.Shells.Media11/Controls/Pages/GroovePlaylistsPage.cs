using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display an <see cref="IPlaylistCollectionViewModel"/>.
    /// </summary>
    public partial class Media11PlaylistsPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11PlaylistPage"/> class.
        /// </summary>
        public Media11PlaylistsPage()
        {
            DefaultStyleKey = typeof(Media11PlaylistsPage);
            DataContext = new Media11PlaylistsPageViewModel();
        }

        /// <summary>
        /// The <see cref="Media11PlaylistsPageViewModel"/> for the <see cref="Media11PlaylistsPage"/> template.
        /// </summary>
        public Media11PlaylistsPageViewModel ViewModel => (Media11PlaylistsPageViewModel)DataContext;

        /// <summary>
        /// The playlist collection being displayed.
        /// </summary>
        public IPlaylistCollectionViewModel? PlaylistCollection
        {
            get { return (IPlaylistCollectionViewModel)GetValue(PlaylistCollectionProperty); }
            set { SetValue(PlaylistCollectionProperty, value); }
        }

        /// <summary>
        /// Backing dependency property for <see cref="PlaylistCollection"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistCollectionProperty =
            DependencyProperty.Register(nameof(PlaylistCollection), typeof(IPlaylistCollectionViewModel), typeof(Media11PlaylistsPage), new PropertyMetadata(null, (d,e) => ((Media11PlaylistsPage)d).OnPlaylistCollectionChanged()));

        private void OnPlaylistCollectionChanged()
        {
            ViewModel.PlaylistCollection = PlaylistCollection;
        }
    }
}
