using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Media11.Controls.Collections
{
    /// <summary>
    /// A single track with additional properties needed for using inside an items template.
    /// </summary>
    /// <remarks>
    /// This class was needed to access the context inside of a data template.
    /// <para/>
    /// The context is not available on the SDK view models yet because each track is treated as the same instance,
    /// and so it can't yet track the context of a single track.
    /// </remarks>
    public class Media11TrackViewModel : ObservableObject
    {
        /// <summary>
        /// Creates a new instance of <see cref="Media11TrackCollection"/>.
        /// </summary>
        public Media11TrackViewModel(ITrackCollectionViewModel context, ITrack track)
        {
            Context = context;
            Track = track;
        }

        /// <summary>
        /// The playback context of the <see cref="Track"/>.
        /// </summary>
        public ITrackCollectionViewModel Context { get; }

        /// <summary>
        /// The relevant track.
        /// </summary>
        public ITrack Track { get; }
    }
}
