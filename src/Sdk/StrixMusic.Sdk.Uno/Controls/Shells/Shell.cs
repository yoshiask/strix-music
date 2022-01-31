﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Shells
{
    /// <summary>
    /// A base class for the root control that all shells implement.
    /// </summary>
    public abstract partial class Shell : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        protected Shell()
        {
            Loaded += ShellControl_Loaded;

            // Creating a new instance here so the old Ioc is wiped even if they don't call base.InitServices();
            Ioc = new Ioc();
        }

        /// <summary>
        /// A unique Ioc container for shell-specific services. Wiped and recreated when the user switches shells.
        /// </summary>
        public static Ioc Ioc { get; private set; } = new Ioc();

        /// <summary>
        /// Initializes the services for this shell's IoC.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task InitServices(IServiceCollection serviceCollection)
        {
            Ioc.ConfigureServices(serviceCollection.BuildServiceProvider());

            return Task.CompletedTask;
        }

        /// <summary>
        /// The root <see cref="MainViewModel" /> used by the shell.
        /// </summary>
        public MainViewModel? DataRoot
        {
            get { return (MainViewModel)GetValue(DataRootProperty); }
            set { SetValue(DataRootProperty, value); }
        }

        /// <summary>
        /// The backing dependency property for <see cref="DataRoot"/>.
        /// </summary>
        public static readonly DependencyProperty DataRootProperty =
            DependencyProperty.Register(nameof(DataRoot), typeof(MainViewModel), typeof(Shell), new PropertyMetadata(null));

        private void ShellControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= ShellControl_Loaded;
            SetupTitleBar();
        }

        /// <summary>
        /// Sets properties of the title bar for this shell.
        /// </summary>
        protected virtual void SetupTitleBar()
        {
#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Resources["SystemAltHighColor"] as Color?;
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
#endif
        }
    }
}
