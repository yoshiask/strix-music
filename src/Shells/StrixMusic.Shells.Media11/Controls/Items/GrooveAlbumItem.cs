using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Media11.Controls.Items
{
    /// <inheritdoc/>
    public partial class Media11AlbumItem : AlbumItem
    {
        /// <inheritdoc/>
        public Media11AlbumItem()
        {
            this.DefaultStyleKey = typeof(Media11AlbumItem);

            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(new Action<AlbumViewModel?>(NavigateToAlbum));
        }
        
        /// <summary>
        /// A command that triggers navigation to the provided album.
        /// </summary>
        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel));
        }
    }
}
