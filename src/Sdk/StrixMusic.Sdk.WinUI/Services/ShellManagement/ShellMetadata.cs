﻿using Windows.Foundation;

namespace StrixMusic.Sdk.WinUI.Services.ShellManagement
{
    /// <summary>
    /// Holds metadata for a shell registered with the Strix SDK.
    /// </summary>
    public sealed class ShellMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellMetadata"/> class.
        /// </summary>
        /// <param name="id"> A unique identifier for this shell.</param>
        /// <param name="displayName">The display name for the shell.</param>
        /// <param name="description"> A brief summary of the shell that will be displayed to the user.</param>
        /// <param name="inputMethods">The supported input methods.</param>
        /// <param name="maxWindowSize">The maximum size of the window for the shell.</param>
        /// <param name="minWindowSize">The minimum size of the window for the shell.</param>
        public ShellMetadata(
            string id,
            string displayName,
            string description,
            InputMethods inputMethods = InputMethods.Mouse | InputMethods.Touch | InputMethods.Controller,
            Size? maxWindowSize = null,
            Size? minWindowSize = null)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            InputMethods = inputMethods;
            MaxWindowSize = maxWindowSize ?? new Size(double.PositiveInfinity, double.PositiveInfinity);
            MinWindowSize = minWindowSize ?? new Size(0, 0);
        }

        /// <summary>
        /// A unique identifier for this shell.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The displayed name for this shell.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// A brief summary of the shell that will be displayed to the user.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The input methods that this shell supports.
        /// </summary>
        public InputMethods InputMethods { get; set; }

        /// <summary>
        /// The maximum window size for the shell
        /// </summary>
        public Size MaxWindowSize { get; set; }

        /// <summary>
        /// The minimum window size for the shell
        /// </summary>
        public Size MinWindowSize { get; set; }
    }
}
