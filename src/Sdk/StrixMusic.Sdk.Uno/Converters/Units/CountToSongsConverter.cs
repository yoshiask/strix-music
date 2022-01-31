﻿using System;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Helpers;
using StrixMusic.Sdk.Services.Localization;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Sdk.Uno.Converters.Units
{
    /// <summary>
    /// A converter that adds a "Songs" suffix to a unit.
    /// </summary>
    public sealed class CountToSongsConverter : IValueConverter
    {
        /// <summary>
        /// Adds a "Songs" suffix to a unit.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert</param>
        /// <returns>The converted value.</returns>
        public static string Convert(int value)
        {
            var localizationService = Ioc.Default.GetRequiredService<ILocalizationService>();
            return string.Format(localizationService[Constants.Localization.MusicResource, "SongsCount"], value);
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((int)value);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
