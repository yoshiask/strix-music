using System.Collections.ObjectModel;
using System.Threading.Tasks;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Media11.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="TrackCollectionViewModel"/>s in the Media11 Shell.
    /// </summary>
    public partial class Media11TrackCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media11TrackCollection"/> class.
        /// </summary>
        public Media11TrackCollection()
        {
            DefaultStyleKey = typeof(Media11TrackCollection);

            Tracks = new ObservableCollection<Media11TrackViewModel>();

            AttachEvents();
        }

        /// <summary>
        /// The backing dependency propery for <see cref="TrackCollection"/>.
        /// </summary>
        public static readonly DependencyProperty TrackCollectionProperty =
            DependencyProperty.Register(nameof(TrackCollection), typeof(ITrackCollectionViewModel), typeof(Media11TrackCollection), new PropertyMetadata(null, (d, e) => _ = ((Media11TrackCollection)d).OnTrackCollectionChangedAsync()));

        /// <summary>
        /// The track collection to display.
        /// </summary>
        public ITrackCollectionViewModel? TrackCollection
        {
            get { return (ITrackCollectionViewModel)GetValue(TrackCollectionProperty); }
            set { SetValue(TrackCollectionProperty, value); }
        }

        /// <summary>
        /// The tracks displayed in the collection, with additional properties.
        /// </summary>
        public ObservableCollection<Media11TrackViewModel> Tracks { get; set; }

        private void AttachEvents()
        {
            Unloaded += OnUnloaded;
        }

        private void DetachEvents()
        {
            if (TrackCollection != null)
                DetachEvents(TrackCollection.Tracks);

            Unloaded -= OnUnloaded;
        }

        private void AttachEvents(ObservableCollection<TrackViewModel> tracks)
        {
            tracks.CollectionChanged += Tracks_CollectionChanged;
        }

        private void DetachEvents(ObservableCollection<TrackViewModel> tracks)
        {
            tracks.CollectionChanged += Tracks_CollectionChanged;
        }

        private async Task OnTrackCollectionChangedAsync()
        {
            Tracks.Clear();

            if (TrackCollection is null)
                return;

            await TrackCollection.InitTrackCollectionAsync();

            foreach (var track in TrackCollection.Tracks)
            {
                Tracks.Add(new Media11TrackViewModel(TrackCollection, track));
            }

            AttachEvents(TrackCollection.Tracks);
        }

        private void Tracks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                    if (TrackCollection is null || e.NewItems is null)
                        return;

                    foreach (var track in e.NewItems)
                        Tracks.Add(new Media11TrackViewModel(TrackCollection, (TrackViewModel)track));

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is null)
                        return;

                    for (int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
                        Tracks.RemoveAt(i);

                    break;
                default:
                    break;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }
    }
}
