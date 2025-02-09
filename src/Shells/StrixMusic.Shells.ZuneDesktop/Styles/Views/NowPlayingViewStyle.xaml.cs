﻿using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Styles.Views
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="Sdk.Uno.Controls.NowPlayingView"/> in the ZuneDesktop Shell.
    /// </summary>
    public sealed partial class NowPlayingViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingViewStyle"/> class.
        /// </summary>
        public NowPlayingViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Shell.Ioc.GetService<INavigationService<Control>>()!.GoBack();
        }
    }
}
