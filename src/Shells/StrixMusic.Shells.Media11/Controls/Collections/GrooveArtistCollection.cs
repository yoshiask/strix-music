using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="ArtistCollectionViewModel"/>s in the Media11 Shell.
    /// </summary>
    public partial class Media11ArtistCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11TrackCollection"/> class.
        /// </summary>
        public Media11ArtistCollection()
        {
            DefaultStyleKey = typeof(Media11ArtistCollection);
            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(NavigateToArtist);
        }

        /// <summary>
        /// The backing dependency property for <see cref="ArtistCollection"/>.F
        /// </summary>
        public static readonly DependencyProperty ArtistCollectionProperty =
            DependencyProperty.Register(nameof(ArtistCollection), typeof(IArtistCollectionViewModel), typeof(Media11ArtistCollection), new PropertyMetadata(null, (d, e) => ((Media11ArtistCollection)d).OnArtistCollectionChanged()));

        /// <summary>
        /// The artist collection to display.
        /// </summary>
        public IArtistCollectionViewModel ArtistCollection
        {
            get { return (IArtistCollectionViewModel)GetValue(ArtistCollectionProperty); }
            set { SetValue(ArtistCollectionProperty, value); }
        }

        /// <summary>
        /// A Command that requests a navigation to an artist page.
        /// </summary>
        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }

        private void OnArtistCollectionChanged()
        {
        }
    }
}
