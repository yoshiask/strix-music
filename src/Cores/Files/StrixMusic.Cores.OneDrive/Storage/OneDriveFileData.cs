﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Graph;
using OwlCore.AbstractStorage;
using OwlCore.Validation.Mime;

namespace StrixMusic.Cores.OneDrive.Storage
{
    /// <summary>
    /// Wraps around an instance of  <see cref="DriveItem"/> to implement a file for AbstractStorage.
    /// </summary>
    public class OneDriveFileData : IFileData
    {
        private readonly GraphServiceClient _graphClient;
        private readonly DriveItem _driveItem;

        /// <summary>
        /// Creates a new instance of <see cref="OneDriveFolderData"/>.
        /// </summary>
        /// <param name="graphClient">The service that handles API requests to Microsoft Graph.</param>
        /// <param name="driveItem">An instance of <see cref="DriveItem"/> that represents the underlying OneDrive folder.</param>
        public OneDriveFileData(GraphServiceClient graphClient, DriveItem driveItem)
        {
            _graphClient = graphClient;
            _driveItem = driveItem;

            Properties = new OneDriveFileDataProperties(_driveItem);
        }

        /// <inheritdoc />
        public string? Id => _driveItem.Id;

        /// <inheritdoc />
        public string Path => _driveItem.AdditionalData["@microsoft.graph.downloadUrl"].ToString().Replace("ValueKind = String : ", string.Empty).Replace("\"", string.Empty);

        /// <inheritdoc />
        public string Name => _driveItem.Name;

        /// <inheritdoc />
        public string DisplayName => _driveItem.Name;

        /// <inheritdoc />
        public string FileExtension => MimeTypeMap.GetExtension(_driveItem.File.MimeType);

        /// <inheritdoc />
        public IFileDataProperties Properties { get; set; }

        /// <inheritdoc />
        public async Task<IFolderData> GetParentAsync()
        {
            var parent = await _graphClient.Drive.Items[_driveItem.ParentReference.DriveId].Request().GetAsync();

            return new OneDriveFolderData(_graphClient, parent);
        }

        /// <inheritdoc />
        public Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read)
        {
            return _graphClient.Drive.Items[_driveItem.Id].Content.Request().GetAsync();
        }

        /// <inheritdoc />
        public Task Delete()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<Stream> GetThumbnailAsync(ThumbnailMode thumbnailMode, uint requiredSize)
        {
            switch (thumbnailMode)
            {
                case ThumbnailMode.MusicView:

                    var thumbnails = _driveItem.Thumbnails;
                    if (thumbnails == null || thumbnails.Count == 0)
                        return Task.FromResult<Stream>(new MemoryStream());

                    return Task.FromResult(thumbnails[0].Source.Content);

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
