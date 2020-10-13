﻿using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Observables;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Styles
{
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
            if ((sender as Control)?.DataContext is ObservableAlbum viewModel)
            {
                if (viewModel.Artist is ObservableArtist observableArtist)
                {
                    INavigationService<Control> navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
                    navigationService.NavigateTo(typeof(ArtistView), false, observableArtist);
                }
            }
        }
    }
}
