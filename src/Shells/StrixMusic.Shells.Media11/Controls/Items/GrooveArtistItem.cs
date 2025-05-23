using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.Media11.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Media11.Controls.Items
{
    /// <inheritdoc/>
    public partial class Media11ArtistItem : ArtistItem
    {
        /// <inheritdoc/>
        public Media11ArtistItem()
        {
            this.DefaultStyleKey = typeof(Media11ArtistItem);

            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(new Action<ArtistViewModel?>(NavigateToArtist));
        }

        /// <summary>
        /// A command that triggers navigation to the provided artist.
        /// </summary>
        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }
    }
}
