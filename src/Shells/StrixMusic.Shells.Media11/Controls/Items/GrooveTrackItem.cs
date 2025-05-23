using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Media11.Controls.Items
{
    /// <inheritdoc/>
    public partial class Media11TrackItem : TrackItem
    {
        /// <inheritdoc/>
        public Media11TrackItem()
        {
            this.DefaultStyleKey = typeof(Media11TrackItem);

            NavigateToAlbumCommand = new RelayCommand<TrackViewModel>(new Action<TrackViewModel?>(NavigateToAlbum));
        }
        
        /// <summary>
        /// A command that triggers navigation to the provided track's album.
        /// </summary>
        public RelayCommand<TrackViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(TrackViewModel? viewModel)
        {
            if (viewModel != null && viewModel.Album != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel.Album));
        }
    }
}
