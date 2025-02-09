﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Search
{
    public class MockCoreSearch : ICoreSearch
    {
        public MockCoreSearch(ICore sourceCore)
        {
            SourceCore = sourceCore;
            SearchHistory = new MockCoreSearchHistory(sourceCore);
        }

        public ICoreSearchHistory? SearchHistory { get; set; }

        public ICore SourceCore { get; set; }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerable<ICoreSearchQuery> GetRecentSearchQueries(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query, CancellationToken cancellationToken = default)
        {
            return AsyncEnumerable.Empty<string>();
        }

        public Task<ICoreSearchResults> GetSearchResultsAsync(string query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ICoreSearchResults>(new MockCoreSearchResults(SourceCore, query));
        }
    }
}
