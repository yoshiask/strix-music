﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Globalization;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Describes a generic user profile.
    /// </summary>
    public interface IUserProfileBase : IImageCollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// Identifier for the user
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Username shown to the user or others
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The user's full name
        /// </summary>
        string? FullName { get; }

        /// <summary>
        /// The user's email
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// The <see cref="DateTime"/> the user was born.
        /// </summary>
        /// <remarks>
        /// If missing data, replace the day, month and/or year with part of 1/1/1970.
        /// </remarks>
        DateTime? Birthdate { get; }

        /// <summary>
        /// The user's country or region of origin.
        /// </summary>
        CultureInfo Region { get; }

        /// <summary>
        /// If true, <see cref="ChangeDisplayNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeDisplayNameAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeBirthDateAsync(DateTime)"/> is supported.
        /// </summary>
        bool IsChangeBirthDateAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeFullNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeFullNameAsyncAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeRegionAsync(CultureInfo)"/> is supported.
        /// </summary>
        bool IsChangeRegionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeEmailAsync(string?)"/> is supported.
        /// </summary>
        bool IsChangeEmailAsyncAvailable { get; }

        /// <summary>
        /// Changes the <see cref="DisplayName"/> for this user.
        /// </summary>
        /// <param name="displayName">The new display name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDisplayNameAsync(string displayName);

        /// <summary>
        /// Changes the <see cref="Birthdate"/> for this user.
        /// </summary>
        /// <param name="birthdate">The new birthdate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeBirthDateAsync(DateTime birthdate);

        /// <summary>
        /// Changes the <see cref="FullName"/> for this user.
        /// </summary>
        /// <param name="fullname">The full name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeFullNameAsync(string fullname);

        /// <summary>
        /// Changes the <see cref="Region"/> for this user.
        /// </summary>
        /// <param name="region">The new region.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeRegionAsync(CultureInfo region);

        /// <summary>
        /// Changes the <see cref="Email"/> for this user.
        /// </summary>
        /// <param name="email">The new email.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeEmailAsync(string? email);

        /// <summary>
        /// Fires when the <see cref="DisplayName"/> has changed.
        /// </summary>
        event EventHandler<string>? DisplayNameChanged;

        /// <summary>
        /// Fires when the <see cref="Birthdate"/> has changed.
        /// </summary>
        event EventHandler<DateTime>? BirthDateChanged;

        /// <summary>
        /// Fires when the <see cref="FullName"/> has changed.
        /// </summary>
        event EventHandler<string>? FullNameChanged;

        /// <summary>
        /// Fires when the <see cref="Region"/> has changed.
        /// </summary>
        event EventHandler<CultureInfo>? RegionChanged;

        /// <summary>
        /// Fires when the <see cref="Email"/> has changed.
        /// </summary>
        event EventHandler<string?>? EmailChanged;
    }
}
