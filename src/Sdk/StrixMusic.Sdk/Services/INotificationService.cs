﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// A service that facilitates raising <see cref="Notification"/>s.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// The maximum number of notification that will be raised at once. Dismiss a notification to show remaining notifications in the queue.
        /// </summary>
        public int MaxActiveNotifications { get; }

        /// <summary>
        /// Enqueue a notification for the Shell to display.
        /// </summary>
        /// <param name="title">The title for the notification.</param>
        /// <param name="message">The body content for the notification.</param>
        /// <returns>The created notification.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Method raises event")]
        Notification RaiseNotification(string title, string message = "");

        /// <summary>
        /// Enqueue a notification for the Shell to display.
        /// </summary>
        /// <returns>The created notification.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Method raises event")]
        Notification RaiseNotification(AbstractUICollection elementGroup);

        /// <summary>
        /// Raised when a new notification needs to be displayed.
        /// </summary>
        public event EventHandler<Notification>? NotificationRaised;

        /// <summary>
        /// Raised when the user dismisses a notification.
        /// </summary>
        public event EventHandler<Notification>? NotificationDismissed;
    }
}
