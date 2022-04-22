﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockArtist : MockPlayableCollectionGroup, IArtist
{
    private int _totalGenreCount;

    public int TotalGenreCount
    {
        get => _totalGenreCount;
        set
        {
            _totalGenreCount = value;
            GenresCountChanged?.Invoke(this, value);
        }
    }

    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default)
    {
        TotalGenreCount--;
        GenresChanged?.Invoke(this, new List<CollectionChangedItem<IGenre>>(), new List<CollectionChangedItem<IGenre>> { new(new MockGenre(), index) });

        return Task.CompletedTask;
    }

    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public event EventHandler<int>? GenresCountChanged;

    public bool Equals(ICoreGenreCollection? other) => false;

    IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources { get; } = new List<ICoreGenreCollection>();

    public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<IGenre>>(Enumerable.Range(0, limit).Select(_ => new MockGenre()).ToList());

    public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) 
    {
        TotalGenreCount++;
        GenresChanged?.Invoke(this, new List<CollectionChangedItem<IGenre>> { new(genre, index) }, new List<CollectionChangedItem<IGenre>>());

        return Task.CompletedTask;
    }

    public event CollectionChangedEventHandler<IGenre>? GenresChanged;
    public bool Equals(ICoreArtist? other) => false;

    public IReadOnlyList<ICoreArtist> Sources { get; } = new List<ICoreArtist>();
    public IPlayableCollectionGroup? RelatedItems { get; } = new MockPlayableCollectionGroup();
}
