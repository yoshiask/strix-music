﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using OwlCore.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StrixMusic.Sdk.FileMetadata.Models;
using ImageMetadata = StrixMusic.Sdk.FileMetadata.Models.ImageMetadata;

namespace StrixMusic.Sdk.FileMetadata.Scanners
{
    public partial class AudioMetadataScanner
    {
        private static readonly IReadOnlyList<int> _standardImageSizes = new int[] { 64, 128, 256, 512 };
        private static readonly IReadOnlyList<int> _highPerfImageSizes = new int[] { 64, 128 };
        private static readonly IReadOnlyList<int> _ultraHighPerfImageSizes = new int[] { 64 };

        private readonly ConcurrentDictionary<string, Task<IEnumerable<ImageMetadata>>> _ongoingImageProcessingTasks;
        private readonly SemaphoreSlim _ongoingImageProcessingTasksSemaphore;
        private readonly SemaphoreSlim _ongoingImageProcessingSemaphore;

        private static async Task<string> GenerateImageIdAsync(Stream imageStream)
        {
            // Create a unique ID for the image by
            // collecting every 64th byte in the data
            // and calculating a rough hash for it.
            // Each resized version will use this as the name
            // with its size appended.
            var hashData = new byte[imageStream.Length / 64];

            var imageBytes = await imageStream.ToBytesAsync();

            for (var i = 0; i < hashData.Length; i++)
            {
                hashData[i] = imageBytes[i * 64];
            }

            return hashData.HashMD5Fast();
        }

        private async Task ProcessImagesAsync(IFileData fileData, Models.FileMetadata fileMetadata, IEnumerable<Stream> imageStreams)
        {
            Guard.IsNotNull(ImageOutputFolder, nameof(ImageOutputFolder));

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            Logger.LogInformation($"Started {nameof(ProcessImagesAsync)} for {nameof(IFileData)} at {fileData.Path}");

            var results = new List<ImageMetadata>();

            await imageStreams.InParallel(async image =>
            {
                // Image IDs are unique to the content of the image.
                var imageId = await GenerateImageIdAsync(image);

                // Check if the data has already been emitted.
                await _batchLock.WaitAsync();

                var foundData = false;
                foreach (var metadata in _allFileMetadata)
                {
                    if (metadata.ImageMetadata is null)
                        continue;

                    foreach (var imageMetadata in metadata.ImageMetadata)
                    {
                        if (imageMetadata.Id == imageId)
                        {
                            Logger.LogInformation($"{nameof(ProcessImagesAsync)}: Found existing {nameof(ImageMetadata)} (id {imageId}) for file at {fileData.Path}");
                            results.Add(imageMetadata);
                            foundData = true;
                        }
                    }
                }

                _batchLock.Release();

                if (foundData)
                {
                    image.Dispose();
                    return;
                }

                // Check if this image is still processing
                await _ongoingImageProcessingTasksSemaphore.WaitAsync();

                if (_ongoingImageProcessingTasks.TryGetValue(imageId, out var ongoingTask))
                {
                    Logger.LogInformation($"{nameof(ProcessImagesAsync)}: Found / waiting for existing {nameof(ongoingTask)} (id {imageId}) for file at {fileData.Path}");

                    _ongoingImageProcessingTasksSemaphore.Release();
                    var relevantImages = await ongoingTask;
                    results.AddRange(relevantImages);
                    image.Dispose();
                    return;
                }

                // Start processing and add to ongoing tasks.
                var processImageTask = ProcessImageAsync(image);

                if (!_ongoingImageProcessingTasks.TryAdd(imageId, processImageTask))
                    ThrowHelper.ThrowInvalidOperationException($"Unable to add new task to {nameof(_ongoingImageProcessingTasks)}");

                _ongoingImageProcessingTasksSemaphore.Release();

                Logger.LogInformation($"{nameof(ProcessImagesAsync)}: New task (id {imageId}) for file at {fileData.Path}");
                var imageScanResult = await processImageTask;
                image.Dispose();

                await _ongoingImageProcessingTasksSemaphore.WaitAsync();

                if (!_ongoingImageProcessingTasks.TryRemove(imageId, out _))
                    ThrowHelper.ThrowInvalidOperationException($"Unable to remove finished task from {nameof(_ongoingImageProcessingTasks)}");

                results.AddRange(imageScanResult);

                _ongoingImageProcessingTasksSemaphore.Release();

                Logger.LogInformation($"{nameof(ProcessImagesAsync)}: Completed image processing task (id {imageId}) for file at {fileData.Path}");
            });

            await _batchLock.WaitAsync();

            fileMetadata.ImageMetadata = results;

            Guard.IsNotNull(fileMetadata.AlbumMetadata, nameof(fileMetadata.AlbumMetadata));
            Guard.IsNotNull(fileMetadata.TrackMetadata, nameof(fileMetadata.TrackMetadata));

            // Must create a new instance to make an update.
            // FileCoreAlbum holds a reference to any emitted data, meaning it sees any changes to IDs before the data is in the repo.
            var updatedAlbumMetadata = new AlbumMetadata
            {
                Id = fileMetadata.AlbumMetadata.Id,
                Title = fileMetadata.AlbumMetadata.Title,
                Description = fileMetadata.AlbumMetadata.Description,
                TrackIds = fileMetadata.AlbumMetadata.TrackIds,
                ArtistIds = fileMetadata.AlbumMetadata.ArtistIds,
                Duration = fileMetadata.AlbumMetadata.Duration,
                DatePublished = fileMetadata.AlbumMetadata.DatePublished,
                Genres = fileMetadata.AlbumMetadata.Genres,
                ImageIds = new HashSet<string>(results.Select(x => x.Id).PruneNull()),
            };

            var updateTrackMetadata = new TrackMetadata
            {
                Id = fileMetadata.TrackMetadata.Id,
                Title = fileMetadata.TrackMetadata.Title,
                Description = fileMetadata.TrackMetadata.Description,
                AlbumId = fileMetadata.TrackMetadata.AlbumId,
                ArtistIds = fileMetadata.TrackMetadata.ArtistIds,
                Duration = fileMetadata.TrackMetadata.Duration,
                TrackNumber = fileMetadata.TrackMetadata.TrackNumber,
                DiscNumber = fileMetadata.TrackMetadata.DiscNumber,
                Genres = fileMetadata.TrackMetadata.Genres,
                ImageIds = new HashSet<string>(results.Select(x => x.Id).PruneNull()),
                Language = fileMetadata.TrackMetadata.Language,
                Lyrics = fileMetadata.TrackMetadata.Lyrics,
                Url = fileMetadata.TrackMetadata.Url,
                Year = fileMetadata.TrackMetadata.Year
            };

            fileMetadata.AlbumMetadata = updatedAlbumMetadata;
            fileMetadata.TrackMetadata = updateTrackMetadata;

            AssignMissingRequiredData(fileData, fileMetadata);
            LinkMetadataIdsForFile(fileMetadata);

            // Re-emit as an update if not already queued.
            _allFileMetadata.Add(fileMetadata);
            _batchMetadataToEmit.Add(fileMetadata);

            _batchLock.Release();

            _ = HandleChangedAsync();
        }

        private async Task<IEnumerable<ImageMetadata>> ProcessImageAsync(Stream imageStream)
        {
            await _ongoingImageProcessingSemaphore.WaitAsync();

            Guard.IsNotNull(ImageOutputFolder, nameof(ImageOutputFolder));

            // We store images for a file in the following structure:
            // CacheFolder/Images/{image ID}-{size}.png
            // image ID is calculated based on content. Using a shared folder means no duplicate images.
            var fileImagesFolder = await ImageOutputFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            Image? img = null;

            try
            {
                imageStream.Position = 0;
                img = await Image.LoadAsync(imageStream);
            }
            catch (UnknownImageFormatException)
            {
                _ongoingImageProcessingSemaphore.Release();
                return Enumerable.Empty<ImageMetadata>();
            }

            using var image = img;

            var imageId = await GenerateImageIdAsync(imageStream);
            imageStream.Position = 0;

            // We don't want to scale the image up (only scale down if necessary), so we have to determine which of the
            // standard image sizes we have to resize to using the size of the original image.

            // Use the maximum of the width and height for the ceiling to handle cases where the image isn't a 1:1 aspect ratio.
            var ceiling = Math.Max(image.Width, image.Height);

            var imageSizes = _standardImageSizes;

            // Loop through the image sizes (in ascending order)
            // and determine the maximum size that the original image is larger than.
            // We'll scale it down to that size and all the sizes smaller than it.
            var useOriginal = false;
            var ceilingSizeIndex = -1;
            for (var i = 0; i < imageSizes.Count; i++)
            {
                if (ceiling > imageSizes[i])
                {
                    ceilingSizeIndex = i;
                }
                else
                {
                    if (ceiling == imageSizes[i])
                    {
                        // If the size of the image is equal to one of the standard image sizes,
                        // we can skip the expensive resize operation and just copy the original image.
                        useOriginal = true;
                    }

                    break;
                }
            }

            // If the ceiling size index is -1, the image is smaller than all of the standard image sizes.
            // In this case, we'll just use the original image size and copy the image into the cache folder
            // rather than resizing it.
            // We can also use this same logic when useOriginal is true.
            if (ceilingSizeIndex == -1 || useOriginal)
            {
                var fullId = $"{imageId}-{ceiling}";

                var imageFile = await fileImagesFolder.CreateFileAsync($"{fullId}.png", CreationCollisionOption.ReplaceExisting);
                using var stream = await imageFile.GetStreamAsync(FileAccessMode.ReadWrite);

                await image.SaveAsPngAsync(stream);
                await stream.FlushAsync();

                _ongoingImageProcessingSemaphore.Release();
                return new ImageMetadata
                {
                    Id = fullId,
                    Uri = new Uri(imageFile.Path),
                    Width = image.Width,
                    Height = image.Height
                }.IntoList();
            }

            var results = new List<ImageMetadata>();

            for (var i = ceilingSizeIndex; i >= 0; i--)
            {
                var resizedSize = imageSizes[i];
                var resizedWidth = image.Width > image.Height ? 0 : resizedSize;
                var resizedHeight = image.Height > image.Width ? 0 : resizedSize;

                var fullId = $"{imageId}-{resizedSize}";
                var imageFile = await fileImagesFolder.CreateFileAsync($"{fullId}.png", CreationCollisionOption.ReplaceExisting);
                var stream = await imageFile.GetStreamAsync(FileAccessMode.ReadWrite);

                image.Mutate(x => x.Resize(resizedWidth, resizedHeight));

                await image.SaveAsPngAsync(stream);

                await stream.FlushAsync();
                stream.Dispose();

                results.Add(new ImageMetadata
                {
                    Id = fullId,
                    Uri = new Uri(imageFile.Path),
                    Width = image.Width,
                    Height = image.Height
                });
            }

            _ongoingImageProcessingSemaphore.Release();

            return results;
        }
    }
}
