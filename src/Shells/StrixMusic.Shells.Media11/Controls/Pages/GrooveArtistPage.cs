using StrixMusic.Sdk.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display an <see cref="ArtistViewModel"/>.
    /// </summary>
    public partial class Media11ArtistPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11ArtistPage"/> class.
        /// </summary>
        public Media11ArtistPage()
        {
            DefaultStyleKey = typeof(Media11ArtistPage);
        }

        /// <summary>
        /// Backing property for <see cref="BackgroundColor"/>.
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color?), typeof(Media11AlbumPage), new PropertyMetadata(null, null));

        /// <summary>
        /// Gets or sets the color of the background for the <see cref="Controls.Pages.Media11AlbumPage"/>.
        /// </summary>
        public Color? BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        /// <summary>
        /// The artist to display in this control.
        /// </summary>
        public ArtistViewModel? Artist
        {
            get { return (ArtistViewModel?)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        /// <summary>
        /// Backing dependency property for <see cref="Artist"/>.
        /// </summary>
        public static readonly DependencyProperty ArtistProperty = 
            DependencyProperty.Register(nameof(Artist), typeof(ArtistViewModel), typeof(Media11ArtistPage), new PropertyMetadata(null, (d, e) => ((Media11ArtistPage)d).OnArtistChanged()));

        private void OnArtistChanged()
        {
        }
    }
}
