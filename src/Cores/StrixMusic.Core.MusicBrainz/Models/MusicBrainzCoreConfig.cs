﻿using System;
using System.Collections.Generic;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Utils;
using StrixMusic.Sdk.AbstractUI;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// MockCore config
    /// </summary>
    public class MusicBrainzCoreConfig : ICoreConfig
    {
        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUIElementGroup> CoreDataUIElements => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreConfig"/> class.
        /// </summary>
        public MusicBrainzCoreConfig(ICore sourceCore)
        {
            Services = ConfigureServices(sourceCore.InstanceId);
        }

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        private IServiceProvider ConfigureServices(string instanceId)
        {
            IServiceCollection services = new ServiceCollection();

            var cacheService = new MusicBrainzCacheService();

            var musicBrainzClient = new MusicBrainzClient
            {
                Cache = new FileRequestCache(cacheService.RootFolder.Path),
            };

            services.Add(new ServiceDescriptor(typeof(MusicBrainzClient), musicBrainzClient));

            return services.BuildServiceProvider();
        }
    }
}
