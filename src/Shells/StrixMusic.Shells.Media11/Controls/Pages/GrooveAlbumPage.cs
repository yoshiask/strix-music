using System.Linq;
using System.Threading.Tasks;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.Helper;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="AlbumViewModel"/> on a page.
    /// </summary>
    public partial class Media11AlbumPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11AlbumPage"/> class.
        /// </summary>
        public Media11AlbumPage()
        {
            DefaultStyleKey = typeof(Media11AlbumPage);
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
        /// Backing property for <see cref="Album"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel), typeof(Media11AlbumPage), new PropertyMetadata(null, (d, e) => ((Media11AlbumPage)d).OnAlbumChanged()));

        /// <summary>
        /// The album being displayed.
        /// </summary>
        public AlbumViewModel? Album
        {
            get => (AlbumViewModel)GetValue(AlbumProperty);
            set => SetValue(AlbumProperty, value);
        }

        private void OnAlbumChanged()
        {
            if (!(Album is null))
                _ = ProcessAlbumArtColorAsync(Album);
        }

        private async Task ProcessAlbumArtColorAsync(AlbumViewModel album)
        {
            // Load images if there aren't images loaded.
            await album.InitImageCollectionAsync();

            if (album.Images.Count > 0)
            {
                var images = await album.GetImagesAsync(1, 0).ToListAsync();
                if (images.Count < 1)
                    return;

                using var stream = await images[0].OpenStreamAsync();

                BackgroundColor = await Task.Run(() => DynamicColorHelper.GetImageAccentColorAsync(stream));
            }
        }
    }
}
