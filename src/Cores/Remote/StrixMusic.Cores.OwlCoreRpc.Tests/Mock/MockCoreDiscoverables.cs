﻿using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock
{
    public class MockCoreDiscoverables : MockCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        public MockCoreDiscoverables(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreDiscoverables), "Discoverables")
        {

        }
    }
}
