using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Media11.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="PlaylistCollectionViewModel"/>s in the Media11 Shell.
    /// </summary>
    public partial class Media11PlaylistCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11PlaylistCollection"/> class.
        /// </summary>
        public Media11PlaylistCollection()
        {
            DataContext = this;
            this.DefaultStyleKey = typeof(Media11PlaylistCollection);
        }

        /// <summary>
        /// The backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IPlaylistCollection), typeof(Media11PlaylistCollection), new PropertyMetadata(null, (d, e) => ((Media11PlaylistCollection)d).OnPlaylistCollectionChanged()));

        /// <summary>
        /// A view model for this control.
        /// </summary>
        public Media11PlaylistCollectionViewModel ViewModel
        {
            get => (Media11PlaylistCollectionViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// The backing Dependency Property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(Media11PlaylistCollectionViewModel), typeof(Media11PlaylistCollection), new PropertyMetadata(new Media11PlaylistCollectionViewModel()));

        /// <summary>
        /// The playlist collection to display.
        /// </summary>
        public IPlaylistCollection? Collection
        {
            get => (IPlaylistCollection)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        private void OnPlaylistCollectionChanged()
        {
            if (Collection is null)
                return;

            if (Collection is not IPlaylistCollectionViewModel pvm)
                pvm = new PlaylistCollectionViewModel(Collection);

            _ = pvm.InitPlaylistCollectionAsync();
            ViewModel.PlaylistCollection = pvm;
        }

        /// <summary>
        /// Clears the selected item in the <see cref="Media11PlaylistCollection"/>.
        /// </summary>
        public void ClearSelected()
        {
            ViewModel.SelectedPlaylist = null!;
        }
    }
}
