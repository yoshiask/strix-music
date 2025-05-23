using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the <see cref="Media11Music"/> based on the ViewModel.
    /// </summary>
    public class MainContentTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="Media11AlbumPageViewModel"/>.
        /// </summary>
        public DataTemplate? AlbumPageTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="Media11ArtistPageViewModel"/>.
        /// </summary>
        public DataTemplate? ArtistPageTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="Media11HomePageViewModel"/>.
        /// </summary>
        public DataTemplate? HomePageTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="Media11PlaylistPageViewModel"/>.
        /// </summary>
        public DataTemplate? PlaylistPageTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> to use if the ViewModel is a <see cref="Media11PlaylistsPageViewModel"/>.
        /// </summary>
        public DataTemplate? PlaylistsPageTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case AlbumViewModel _:
                    Guard.IsNotNull(AlbumPageTemplate, nameof(AlbumPageTemplate));
                    return AlbumPageTemplate;
                case ArtistViewModel _:
                    Guard.IsNotNull(ArtistPageTemplate, nameof(ArtistPageTemplate));
                    return ArtistPageTemplate;
                case LibraryViewModel _:
                    Guard.IsNotNull(HomePageTemplate, nameof(HomePageTemplate));
                    return HomePageTemplate;
                case PlaylistViewModel _:
                    Guard.IsNotNull(PlaylistPageTemplate, nameof(PlaylistPageTemplate));
                    return PlaylistPageTemplate;
                case IPlaylistCollectionViewModel _:
                    Guard.IsNotNull(PlaylistsPageTemplate, nameof(PlaylistsPageTemplate));
                    return PlaylistsPageTemplate;
                default:
                    return base.SelectTemplateCore(item);
            }
        }
    }
}
