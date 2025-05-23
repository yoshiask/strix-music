using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="Sdk.ViewModels.AlbumCollectionViewModel"/>s in the Media11 Shell.
    /// </summary>
    public partial class Media11AlbumCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11AlbumCollection"/> class.
        /// </summary>
        public Media11AlbumCollection()
        {
            DefaultStyleKey = typeof(Media11AlbumCollection);
            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(NavigateToAlbum);
        }

        /// <summary>
        /// A Command that requests a navigation to an album page.
        /// </summary>
        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        /// <summary>
        /// The album collection to display.
        /// </summary>
        public IAlbumCollectionViewModel? AlbumCollection
        {
            get => (IAlbumCollectionViewModel)GetValue(AlbumCollectionProperty);
            set => SetValue(AlbumCollectionProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="AlbumCollection"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumCollectionProperty =
            DependencyProperty.Register(nameof(AlbumCollection), typeof(IAlbumCollectionViewModel), typeof(Media11AlbumCollection), new PropertyMetadata(null, (d, e) => ((Media11AlbumCollection)d).OnAlbumCollectionChanged()));

        private void OnAlbumCollectionChanged()
        {
        }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel));
        }
    }
}
