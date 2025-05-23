using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Media11.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.Media11ArtistCollection"/>.
    /// </summary>
    public class Media11ArtistCollectionViewModel : ObservableObject
    {
        private IArtistCollectionViewModel? _artistCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Media11ArtistCollectionViewModel"/> class.
        /// </summary>
        public Media11ArtistCollectionViewModel()
        {
        }

        /// <summary>
        /// The <see cref="IArtistCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IArtistCollectionViewModel? ArtistCollection
        {
            get => _artistCollectionViewModel;
            set => SetProperty(ref _artistCollectionViewModel, value);
        }
    }
}
