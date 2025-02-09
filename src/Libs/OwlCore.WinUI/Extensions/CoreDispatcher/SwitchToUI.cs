﻿using System;
using System.Runtime.CompilerServices;
using Windows.UI.Core;

namespace OwlCore.WinUI.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="CoreDispatcher"/>
    /// </summary>
    public static class CoreDispatcherExtensions
    {
        /// <summary>
        /// Continues the current async method on the UI thread.
        /// </summary>
        /// <param name="dispatcher">The CoreDispatcher to use for dispatching.</param>
        /// <returns></returns>
        public static SwitchToUIAwaitable SwitchToUI(this CoreDispatcher dispatcher)
        {
            return new SwitchToUIAwaitable(dispatcher);
        }

        /// <summary>
        /// An extension to switch to the UI thread.
        /// </summary>
        public readonly struct SwitchToUIAwaitable : INotifyCompletion
        {
            private readonly CoreDispatcher _dispatcher;

            /// <summary>
            /// Creates a new instance of <see cref="SwitchToUIAwaitable"/>.
            /// </summary>
            /// <param name="dispatcher"></param>
            public SwitchToUIAwaitable(CoreDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            /// <summary>
            /// Gets the current awaiter.
            /// </summary>
            public SwitchToUIAwaitable GetAwaiter()
            {
                return this;
            }

            /// <summary>
            /// Gets the result of the task.
            /// </summary>
            public void GetResult()
            {
            } 

            /// <summary>
            /// Gets the current completion state.
            /// </summary>
            public bool IsCompleted => _dispatcher.HasThreadAccess;

            /// <summary>
            /// Fires when the task is completed.
            /// </summary>
            /// <param name="continuation">The action to perform when continuing.</param>
            public void OnCompleted(Action continuation)
            {
                _ = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => continuation());
            }
        }
    }
}