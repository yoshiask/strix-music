﻿using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the main app.
    /// </summary>
    /// <remarks>
    /// This <see lang="class"/> is used with reflection to generate settings files.
    /// </remarks>
    public static class SettingsKeys
    {
        /// <summary>
        /// Gets the default value for <see cref="PreferredShell"/> in settings.
        /// </summary>
        public static readonly string PreferredShell = "Default";

        /// <summary>
        /// Stored information about the cores that have been configured.
        /// </summary>
        public static readonly Dictionary<string, Type> CoreRegistry = new Dictionary<string, Type>();

        /// <summary>
        /// Contains keyed information for the last selected Pivot item in various pivots throughout the app. Key is a unique ID for the pivot, value is the index of the selected pivot.
        /// </summary>
        public static readonly Dictionary<string, int> PivotSelectionMemo = new Dictionary<string, int>();
    }
}
