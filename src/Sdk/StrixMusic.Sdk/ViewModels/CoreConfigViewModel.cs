﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// ViewModel for an <see cref="ICoreConfig"/>
    /// </summary>
    public class CoreConfigViewModel : ObservableObject, ICoreConfig
    {
        private readonly ICoreConfig _coreConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreConfigViewModel"/> class.
        /// </summary>
        /// <param name="coreConfig">The instance of <see cref="ICoreConfig"/> to wrap around for this view model.</param>
        public CoreConfigViewModel(ICoreConfig coreConfig)
        {
            _coreConfig = coreConfig;

            AbstractUIElements = new ObservableCollection<AbstractUICollectionViewModel>();
            AbstractUIElements.Clear();

            foreach (var abstractUIElement in _coreConfig.AbstractUIElements)
            {
                AbstractUIElements.Add(new AbstractUICollectionViewModel(abstractUIElement));
            }

            AttachEvents();
        }

        private void AttachEvents()
        {
            _coreConfig.AbstractUIElementsChanged += CoreConfig_AbstractUIElementsChanged;
        }

        private void DetachEvents()
        {
            _coreConfig.AbstractUIElementsChanged -= CoreConfig_AbstractUIElementsChanged;
        }

        private async void CoreConfig_AbstractUIElementsChanged(object sender, EventArgs e)
        {
            await Threading.OnPrimaryThread(() =>
            {
                AbstractUIElements.Clear();

                foreach (var abstractUIElement in _coreConfig.AbstractUIElements)
                {
                    AbstractUIElements.Add(new AbstractUICollectionViewModel(abstractUIElement));
                }
            });
        }

        /// <inheritdoc/>
        public IServiceProvider? Services => _coreConfig.Services;

        /// <inheritdoc/>
        IReadOnlyList<AbstractUICollection> ICoreConfigBase.AbstractUIElements => _coreConfig.AbstractUIElements;

        /// <inheritdoc cref="ICoreConfigBase.AbstractUIElements" />
        public ObservableCollection<AbstractUICollectionViewModel> AbstractUIElements { get; }

        /// <inheritdoc/>
        public MediaPlayerType PlaybackType => _coreConfig.PlaybackType;

        /// <inheritdoc />
        public event EventHandler? AbstractUIElementsChanged;

        /// <inheritdoc/>
        public ICore SourceCore => _coreConfig.SourceCore;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _coreConfig.DisposeAsync();
        }
    }
}
