using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using SubSonicMedia;
using SubSonicMedia.Interfaces;
using SubSonicMedia.Models;

namespace StrixMusic.Cores.OpenSubsonic;

public class OpenSubsonicCore : ICore
{
    private readonly ISubsonicClient _client;

    public OpenSubsonicCore(string instanceId, string serverUrl, string username, string password)
        : this(instanceId, new(serverUrl, username, password))
    {
    }
        
    public OpenSubsonicCore(string instanceId, SubsonicConnectionInfo connectionInfo)
    {
        InstanceId = instanceId;
        Id = nameof(OpenSubsonicCore);
        SourceCore = this;
        
        DisplayName = "OpenSubsonic";
        InstanceDescriptor = "";
        Devices = new List<ICoreDevice>();
        
        _client = new SubsonicClient(connectionInfo);
        
        // Library = TODO
    }
    
    public ISubsonicClient Client => _client;
    
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
    
    public Task InitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    
    public ValueTask DisposeAsync()
    {
        return default;
    }
}
