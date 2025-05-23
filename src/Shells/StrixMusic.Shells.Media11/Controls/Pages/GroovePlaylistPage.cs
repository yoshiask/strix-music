using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="Sdk.ViewModels.PlaylistViewModel"/> on a page.
    /// </summary>
    public partial class Media11PlaylistPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11PlaylistPage"/> class.
        /// </summary>
        public Media11PlaylistPage()
        {
            DefaultStyleKey = typeof(Media11PlaylistPage);
        }

        /// <summary>
        /// The backing depenency property for <see cref="Playlist"/>.s
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(PlaylistViewModel), typeof(Media11PlaylistPage), new PropertyMetadata(null, (d, e) => ((Media11PlaylistPage)d).OnPlaylistChanged()));

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public PlaylistViewModel Playlist
        {
            get { return (PlaylistViewModel)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        private void OnPlaylistChanged()
        {
        }
    }
}
