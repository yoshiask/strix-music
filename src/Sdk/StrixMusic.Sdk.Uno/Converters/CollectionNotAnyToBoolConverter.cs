﻿using System.Collections;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// A simple converter that converts a given <see cref="ICollection"/> to an inversed bool based on the presence of any items in the <see cref="ICollection"/>.
    /// </summary>
    public sealed class CollectionNotAnyToBoolConverter
    {
        /// <summary>
        /// Converts an <see cref="ICollection"/> an <see cref="Visibility"/> based on the presence of any items in the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="value">The <see cref="ICollection"/>.</param>
        /// <returns>A <see cref="Visibility"/>.</returns>
        public static bool Convert(object value)
        {
            return !CollectionAnyToBoolConverter.Convert(value);
        }
    }
}
