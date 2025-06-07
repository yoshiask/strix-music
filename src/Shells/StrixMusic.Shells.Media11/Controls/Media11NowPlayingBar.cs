using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Shells.Media11.Helper;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls
{
    /// <summary>
    /// A <see cref="Control"/> to display the now playing bar.
    /// </summary>
    public sealed partial class Media11NowPlayingBar : NowPlayingBar
    {
        /// <summary>
        /// Backing dependency property for <see cref="BackgroundColor"/>.
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color?), typeof(Media11NowPlayingBar), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="Media11NowPlayingBar"/> class.
        /// </summary>
        public Media11NowPlayingBar()
        {
            DefaultStyleKey = typeof(Media11NowPlayingBar);
        }

        /// <summary>
        /// Gets or sets the color of the <see cref="Controls.Media11NowPlayingBar"/> background.
        /// </summary>
        public Color? BackgroundColor
        {
            get => (Color?)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        /// <inheritdoc/>
        private void AttachEvents_ActiveDevice(IDevice device)
        {
            device.NowPlayingChanged += ActiveDevice_NowPlayingChanged;
        }

        /// <inheritdoc/>
        private void DetachEvents_ActiveDevice(IDevice device)
        {
            device.NowPlayingChanged -= ActiveDevice_NowPlayingChanged;
        }

        private async void ActiveDevice_NowPlayingChanged(object? sender, PlaybackItem e)
        {
            return;

            // Load images if there aren't images loaded.
            // Uncommenting this will cause NowPlaying album art to break randomly while skipping tracks.
            // Maybe just ask the api for the first image directly, glhf.
            Guard.IsNotNull(e.Track, nameof(e.Track));

            if (e.Track.TotalImageCount != 0)
            {
                // If there are images, grab the color from the first image.
                var images = await e.Track.GetImagesAsync(1, 0).ToListAsync();

                if (images.Count == 0)
                    return;

                using var stream = await images[0].OpenStreamAsync();

                BackgroundColor = await Task.Run(() => DynamicColorHelper.GetImageAccentColorAsync(stream));
            }
        }

        /// <inheritdoc/>
        protected override void OnActiveDeviceChanged(DeviceViewModel? oldValue, DeviceViewModel? newValue)
        {
            if (!(oldValue is null))
                DetachEvents_ActiveDevice(oldValue);

            if (!(newValue is null))
                AttachEvents_ActiveDevice(newValue);
        }
    }
}
