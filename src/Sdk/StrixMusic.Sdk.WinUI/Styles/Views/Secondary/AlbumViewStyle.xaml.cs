﻿using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Sdk.WinUI.Controls.Views.Secondary;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Styles.Views.Secondary
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="AlbumView"/>.
    /// </summary>
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
            if ((sender as Control)?.DataContext is ArtistViewModel viewModel)
            {
                var navigationService = Ioc.Default.GetRequiredService<INavigationService<Control>>();

                navigationService.NavigateTo(typeof(ArtistView), false, viewModel);
            }
        }
    }
}
