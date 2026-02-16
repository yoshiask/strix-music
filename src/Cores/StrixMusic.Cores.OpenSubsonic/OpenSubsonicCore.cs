using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Cores.OpenSubsonic.Models;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using SubSonicMedia.Interfaces;

namespace StrixMusic.Cores.OpenSubsonic;

public class OpenSubsonicCore : ICore
{

    public OpenSubsonicCore(string instanceId, ISubsonicClient client)
    {
        InstanceId = instanceId;
        Id = nameof(OpenSubsonicCore);
        SourceCore = this;
        
        DisplayName = "OpenSubsonic";
        InstanceDescriptor = "";
        Devices = new List<ICoreDevice>();
        
        Client = client;

        Library = new OpenSubsonicCoreLibrary(this);
    }
    
    internal ISubsonicClient Client { get; }

    public bool IsInitialized { get; private set; }
    public ICore SourceCore { get; private set; }
    public string Id { get; private set; }
    public string InstanceId { get; private set; }
    public string InstanceDescriptor { get; private set; }
    public string DisplayName { get; private set; }
    public ICoreImage? Logo { get; private set; }
    public MediaPlayerType PlaybackType { get; private set; }
    public ICoreUser? User { get; private set; }
    public IReadOnlyList<ICoreDevice> Devices { get; private set; }
    public ICoreLibrary Library { get; private set; }
    public ICorePlayableCollectionGroup? Pins { get; private set; }
    public ICoreSearch? Search { get; private set; }
    public ICoreRecentlyPlayed? RecentlyPlayed { get; private set; }
    public ICoreDiscoverables? Discoverables { get; private set; }
    public Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult<ICoreModel?>(null);

    public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default) => Task.FromResult<IMediaSourceConfig?>(null);

    public event EventHandler<ICoreImage?>? LogoChanged;
    public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;
    public event EventHandler<string>? DisplayNameChanged;
    public event EventHandler<string>? InstanceDescriptorChanged;

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (Library is IAsyncInit asyncLibrary)
            await asyncLibrary.InitAsync(cancellationToken);
        
        IsInitialized = true;
    }
    
    public ValueTask DisposeAsync()
    {
        return default;
    }
}
