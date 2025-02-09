﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalModelPluginConnector
{
    [TestClass]
    public class GenreCollectionTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [TestMethod]
        public void AccessedThroughAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<GenreCollectionPluginBaseTests.FullyCustom>, GenreCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));
            plugins.Album.Add(x => new AlbumPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            // Ensure an Album plugin can still be accessed through GenreCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumPluginBaseTests.FullyCustom>, GenreCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<GenreCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<AlbumPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughArtist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Artist.Execute(new ArtistPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<GenreCollectionPluginBaseTests.FullyCustom>, GenreCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingArtist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));
            plugins.Artist.Add(x => new ArtistPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Artist.Execute(new ArtistPluginBaseTests.Unimplemented());

            // Ensure an Artist plugin can still be accessed through GenreCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistPluginBaseTests.FullyCustom>, GenreCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingArtist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Artist.Execute(new ArtistPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<GenreCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<ArtistPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<GenreCollectionPluginBaseTests.FullyCustom>, GenreCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));
            plugins.Track.Add(x => new TrackPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            // Ensure an Track plugin can still be accessed through GenreCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackPluginBaseTests.FullyCustom>, GenreCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.GenreCollection.Add(x => new GenreCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<GenreCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<TrackPluginBaseTests.Unimplemented>),
            });
        }
    }
}
