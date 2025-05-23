using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Media11.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.Media11TrackCollectionViewModel"/>.
    /// </summary>
    public class Media11TrackCollectionViewModel : ObservableObject
    {
        private ITrackCollectionViewModel? _trackCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Media11TrackCollectionViewModel"/> class.
        /// </summary>
        public Media11TrackCollectionViewModel()
        {
        }

        /// <summary>
        /// The <see cref="ITrackCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public ITrackCollectionViewModel? TrackCollection
        {
            get => _trackCollectionViewModel;
            set => SetProperty(ref _trackCollectionViewModel, value);
        }
    }
}
