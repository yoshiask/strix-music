﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Uwp.UI.Animations;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Uno.Controls.Collections;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// The collection to perform zune specific behaviors.
    /// </summary>
    public class ZuneAlbumCollection : AlbumCollection
    {
        /// <summary>
        /// Flag to determine if albums are already loaded.
        /// </summary>
        public bool AlbumsLoaded { get; private set; }

        /// <summary>
        /// The AlbumCollection GridView control.
        /// </summary>
        public GridView? PART_Selector { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ZuneAlbumCollection"/>.
        /// </summary>
        public ZuneAlbumCollection()
        {
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Selector = GetTemplateChild(nameof(PART_Selector)) as GridView;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            Collection.Albums.CollectionChanged += Albums_CollectionChanged;

            PART_Selector.Loaded += PART_Selector_Loaded;
            Unloaded += ZuneAlbumCollection_Unloaded;
        }

        /// <summary>
        /// Detaches events from the already loaded <see href="ZuneAlbumItem"/>
        /// </summary>
        public void DetachItemEvents()
        {
            Collection.Albums.CollectionChanged -= Albums_CollectionChanged;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            int itemIndex = 0;
            foreach (var item in Collection.Albums)
            {
                // NOTE: ContainerFromItem doesn't work in uno.
                var gridViewItem = (GridViewItem)PART_Selector.ContainerFromIndex(itemIndex);

                if (gridViewItem == null)
                    break;

                var uiElement = gridViewItem.ContentTemplateRoot;

                if (uiElement is ZuneAlbumItem zuneAlbumItem)
                {
                    zuneAlbumItem.AlbumPlaybackTriggered -= ZuneAlbumItem_AlbumPlaybackTriggered;
                }
            }
        }

        private async void Albums_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                // Intentional delay, this gives enough time for the UI to update. There is no event GridView that tells when its Data is updated.
                await Task.Delay(1000);
                Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

                foreach (var item in e.NewItems)
                {
                    Guard.IsNotNull(item, nameof(item));

                    var index = Collection.Albums.IndexOf((IAlbumCollectionItem)item);
                    var gridViewItem = (GridViewItem)PART_Selector.ContainerFromIndex(index);
                    var uiElement = gridViewItem.ContentTemplateRoot;

                    if (uiElement is ZuneAlbumItem zuneAlbumItem)
                    {
                        zuneAlbumItem.AlbumPlaybackTriggered += ZuneAlbumItem_AlbumPlaybackTriggered;
                    }
                }
            }
        }

        private void ZuneAlbumCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            PART_Selector.Loaded -= PART_Selector_Loaded;
            Unloaded -= ZuneAlbumCollection_Unloaded;
        }

        private void PART_Selector_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateCollection();
        }

        /// <summary>
        /// Gets the list of the <see cref="UIElement"/> and animates it.
        /// </summary>
        public void AnimateCollection()
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            var uiElments = new List<UIElement>();

            int itemIndex = 0;
            foreach (var item in Collection.Albums)
            {
                // NOTE: ContainerFromItem doesn't work in uno.
                var gridViewItem = (GridViewItem)PART_Selector.ContainerFromIndex(itemIndex);

                if (gridViewItem == null)
                    break;

                var uiElement = gridViewItem.ContentTemplateRoot;

                if (uiElement is ZuneAlbumItem zuneAlbumItem)
                {
                    zuneAlbumItem.AlbumPlaybackTriggered += ZuneAlbumItem_AlbumPlaybackTriggered;
                }

                // This needs to be explicitly casted to UIElement to avoid a compiler error specific to android in uno.
                uiElments.Add((UIElement)uiElement);
                itemIndex++;
            }

            FadeInAlbumCollectionItems(uiElments);
        }

        /// <summary>
        /// The method handles album update whenever an artist is selected.
        /// </summary>
        /// <param name="selectedItem"></param>
        public void ArtistSelected(ArtistViewModel selectedItem)
        {
            Collection.Albums.CollectionChanged += Albums_CollectionChanged;
        }

        private async void ZuneAlbumItem_AlbumPlaybackTriggered(object sender, Sdk.ViewModels.AlbumViewModel e)
        {
            await Collection.PlayAlbumCollectionAsync(e);
        }

        private void FadeInAlbumCollectionItems(List<UIElement> uiElements)
        {
            double delay = 0;

            foreach (var item in uiElements)
            {
                var animationSet = new AnimationSet();
                var duration = 250;

                animationSet.Add(new OpacityAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(duration),
                    Delay = TimeSpan.FromMilliseconds(delay),
                    EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut,
                    EasingType = EasingType.Linear
                });
                delay += 75;

                animationSet.Start(item);
            }

            AlbumsLoaded = true;
        }
    }
}
