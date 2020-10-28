﻿using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Uno.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Sdk.Uno.Controls
{
    [TemplatePart(Name = nameof(PART_Selector), Type = typeof(Selector))]
    public abstract partial class CollectionControl<TData, TItem> : Control
        where TData : class
        where TItem : ItemControl
    {
        protected Selector? PART_Selector;
        protected ScrollViewer? PART_Scroller;

        /// <summary>
        /// Fired when the
        /// </summary>
        public event EventHandler<Events.SelectionChangedEventArgs<TData>>? SelectionChanged;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += CollectionControl_Loaded;
        }

        /// <summary>
        /// Perform incremental loading
        /// </summary>
        protected abstract Task LoadMore();

        private void AttachHandlers()
        {
            Unloaded += CollectionControl_Unloaded;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(PART_Selector, nameof(PART_Scroller));
            PART_Selector!.SelectionChanged += SelectedItemChanged;
            PART_Scroller!.ViewChanged += CollectionControl_ViewChanged;
        }

        private void DetachHandlers()
        {
            Loaded -= CollectionControl_Loaded;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(PART_Selector, nameof(PART_Scroller));
            PART_Selector!.SelectionChanged -= SelectedItemChanged;
            PART_Scroller!.ViewChanged -= CollectionControl_ViewChanged;
        }

        private void CollectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Find and set Selector
            PART_Selector = VisualTreeHelpers.GetDataTemplateChild<Selector>(this, nameof(PART_Selector));
            PART_Scroller = VisualTreeHelpers.GetDataTemplateChild<ScrollViewer>(this);

            AttachHandlers();

            CheckScrollPosition();
        }

        private void CollectionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void CollectionControl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            CheckScrollPosition();
        }

        private void CheckScrollPosition()
        {
            double fromBottom = PART_Scroller!.ScrollableHeight - PART_Scroller!.VerticalOffset;

            // If approaching the bottom of the list
            if (fromBottom < 100)
            {
                _ = LoadMore();
            }
        }

        private void SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            e.AddedItems.ForEach(x => GetItemFromData(x).Selected = true);
            e.RemovedItems.ForEach(x => GetItemFromData(x).Selected = false);

            /// Get selected item
            // Invoke event
            Events.SelectionChangedEventArgs<TData> selectionChangedEventArgs = new Events.SelectionChangedEventArgs<TData>((PART_Selector!.SelectedItem as TData)!);
            SelectionChanged?.Invoke(this, selectionChangedEventArgs);
        }

        private TItem GetItemFromData(object data)
        {
            return VisualTreeHelpers.FindVisualChildren<TItem>(PART_Selector!.ContainerFromItem(data)).FirstOrDefault()!;
        }

    }
}
