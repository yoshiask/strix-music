﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs;
using Ipfs.Http;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Storage;
using OwlCore.Storage.System.IO;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.AppModels;

/// <summary>
/// Enables managed access to IPFS.
/// </summary>
public partial class IpfsAccess : ObservableObject, IAsyncInit
{
    private readonly SemaphoreSlim _initMutex = new(1, 1);

    private IpfsSettings _settings;
    private bool _isInitialized;
    private bool _isRunningBootstrapNode;
    private string _initStatus = "IPFS is not loaded";
    private KuboBootstrapper? _kuboBootstrapper;
    private AppReleaseContentLoader? _appReleaseContentLoader;
    private PeerRoom? _everyone;
    private IpfsClient? _client;
    private Ipfs.Peer? _thisPeer;

    /// <summary>
    /// Creates a new instance of <see cref="IpfsAccess"/>.
    /// </summary>
    /// <param name="settings"></param>
    public IpfsAccess(IpfsSettings settings)
    {
        _settings = settings;
        _settings.PropertyChanged += SettingsOnPropertyChanged;
    }

    /// <summary>
    /// A container for all settings related to Ipfs.
    /// </summary>
    public IpfsSettings Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    /// <summary>
    /// Gets a boolean that indicates if IPFS can be accessed.
    /// </summary>
    public bool IsInitialized
    {
        get => _isInitialized;
        set => SetProperty(ref _isInitialized, value);
    }

    /// <summary>
    /// Gets a boolean that indicates if a node was downloaded and bootstrapped by the application.
    /// </summary>
    public bool IsRunningBootstrapNode
    {
        get => _isRunningBootstrapNode;
        set => SetProperty(ref _isRunningBootstrapNode, value);
    }

    /// <summary>
    /// Detailed status info for <see cref="InitAsync"/>. 
    /// </summary>
    public string InitStatus
    {
        get => _initStatus;
        set
        {
            SetProperty(ref _initStatus, value);

            if (!string.IsNullOrWhiteSpace(value))
                OwlCore.Diagnostics.Logger.LogInformation(value);
        }
    }

    /// <summary>
    /// The bootstrapper that was used to start an embedded node.
    /// </summary>
    public AppReleaseContentLoader? AppReleaseContentLoader
    {
        get => _appReleaseContentLoader;
        set => SetProperty(ref _appReleaseContentLoader, value);
    }

    /// <summary>
    /// The bootstrapper that was used to start an embedded node.
    /// </summary>
    public KuboBootstrapper? KuboBootstrapper
    {
        get => _kuboBootstrapper;
        set => SetProperty(ref _kuboBootstrapper, value);
    }

    /// <summary>
    /// A room where all users of the application are present.
    /// </summary>
    public PeerRoom? Everyone
    {
        get => _everyone;
        set => SetProperty(ref _everyone, value);
    }

    /// <summary>
    /// The configured IPFS client, if available.
    /// </summary>
    public IpfsClient? Client
    {
        get => _client;
        set => SetProperty(ref _client, value);
    }

    /// <summary>
    /// The peer information for this user, if the daemon is running and accessible.
    /// </summary>
    public Ipfs.Peer? ThisPeer
    {
        get => _thisPeer;
        set => SetProperty(ref _thisPeer, value);
    }

    private async void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_settings.Enabled))
        {
            if (IsInitialized && !_settings.Enabled)
                await StopCommand.ExecuteAsync(null);
        }
    }

    private void OnInitStatusChanged(string? value)
    {
        if (value is not null)
            OwlCore.Diagnostics.Logger.LogInformation(value);
    }

    /// <summary>
    /// The message handler to use when accessing IPFS over any HTTP API.
    /// </summary>
    public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();

    /// <summary>
    /// Performs a garbage collection sweep on the IPFS repo.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the ongoing task.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    public async Task ExecuteGarbageCollectionAsync(CancellationToken cancellationToken)
    {
        Guard.IsNotNull(Client);
        var originalStatus = InitStatus;

        InitStatus = "Performing garbage collection...";
        await Client.BlockRepository.RemoveGarbageAsync(cancellationToken);

        InitStatus = originalStatus;
    }

    /// <summary>
    /// Stops any running embedded IPFS nodes, removes the current peer, and disables access to IPFS.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Everyone?.Dispose();
        Everyone = null;

        if (KuboBootstrapper is not null)
        {
            KuboBootstrapper.Stop();
            KuboBootstrapper.Dispose();
        }

        Client = null;
        ThisPeer = null;

        IsInitialized = false;

        if (IsRunningBootstrapNode)
        {
            InitStatus = "The embedded IPFS node has been stopped.";
            IsRunningBootstrapNode = false;
        }
        else
        {
            InitStatus = "Access to the local node has been disabled.";
        }
    }

    /// <inheritdoc />
    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        await Settings.LoadCommand.ExecuteAsync(null);

        if (!Settings.Enabled || IsInitialized)
            return;

        using (await _initMutex.DisposableWaitAsync(cancellationToken))
        {
            if (IsInitialized)
                return;

            Client ??= await ScanLocalhostForRunningKuboCompliantRpcApi(cancellationToken);

#if __WASM__
            InitStatus = "Local Kubo node not found. IPFS cannot be initialized.";
            return;
#endif

            await BootstrapAndStartKuboAsync(cancellationToken);

            IsInitialized = true;
            InitStatus = "Kubo is running and ready to use";
        }

        // At this point, IPFS is set up and ready to use.
        // Additional features require async to setup, which we don't wait for.
        _ = PostSetupAsync();
    }

    private async Task PostSetupAsync()
    {
        Guard.IsNotNull(Client);
        Guard.IsNotNull(ThisPeer);
        Guard.IsNotNullOrWhiteSpace(Settings.ReleaseIpns);

        Everyone = new PeerRoom(ThisPeer, Client.PubSub, $"{Settings.ReleaseIpns}/app");

        Settings.ReleaseIpnsResolved = await ResolveReleaseIpnsAsync(default);

        if (!string.IsNullOrWhiteSpace(Settings.ReleaseIpnsResolved))
        {
            InitializeAppReleaseContentLoader();
        }
    }

    private void InitializeAppReleaseContentLoader()
    {
        Guard.IsNotNull(Client);
        AppReleaseContentLoader = new AppReleaseContentLoader(Settings, Client, new IpnsFolder(Settings.ReleaseIpns, Client));

        Logger.LogInformation($"Starting {nameof(AppReleaseContentLoader)} using {Settings.ReleaseIpnsResolved}");
        _ = AppReleaseContentLoader.InitCommand.ExecuteAsync(CancellationToken.None).ContinueWith(x =>
        {
            if (x.IsFaulted)
            {
                Logger.LogError($"Couldn't initialize release content loader", x.Exception?.GetBaseException());
            }
        }, CancellationToken.None);
    }

    private async Task<string> ResolveReleaseIpnsAsync(CancellationToken cancellationToken)
    {
        Guard.IsNotNull(Client);

        try
        {
            Logger.LogInformation($"Resolving release content from {Settings.ReleaseIpns}");
            return await Client.ResolveAsync(Settings.ReleaseIpns, recursive: true, cancellationToken);
        }
        catch (Exception ex)
        {
            // ignored
            Logger.LogError($"{Settings.ReleaseIpns} failed to resolve. Unable to retrieve latest content from publisher.", ex);
            return string.Empty;
        }
    }

    private async Task<IpfsClient?> BootstrapAndStartKuboAsync(CancellationToken cancellationToken)
    {
        var kuboBin = await GetOrDownloadKuboBinaryAsync(cancellationToken);
        await ShowFirewallWarningIfNeeded(cancellationToken);

        KuboBootstrapper = await BootstrapKuboAsync(kuboBin, cancellationToken);

        await KuboBootstrapper.StartAsync(cancellationToken);
        IsRunningBootstrapNode = true;

        InitStatus = "Kubo started, getting peer information";
        Client = new IpfsClient($"{KuboBootstrapper.ApiUri}");
        ThisPeer = await Client.IdAsync(cancel: cancellationToken);

        return Client;
    }

    private async Task ShowFirewallWarningIfNeeded(CancellationToken cancellationToken)
    {
        // Starting a Kubo binary for the first time will prompt Windows show a firewall exception dialog to the user.
        // Warn the user of this and guide them through it.
        if (!Settings.FirewallWarningDisplayed)
        {
            InitStatus = "Showing Kubo firewall warning";
            await ShowFirewallWarningAsync(cancellationToken);
        }
    }

    private async Task<KuboBootstrapper> BootstrapKuboAsync(IChildFile kuboBin, CancellationToken cancellationToken)
    {
        InitStatus = "Bootstrapping Kubo";
        Guard.IsNotNull(kuboBin);

        var kuboBinParentFolder = (SystemFolder?)await kuboBin.GetParentAsync(cancellationToken);
        Guard.IsNotNull(kuboBinParentFolder);

        var kuboBinParentFolderParent = (SystemFolder?)await kuboBinParentFolder.GetParentAsync(cancellationToken);
        Guard.IsNotNull(kuboBinParentFolderParent);

        var repoFolder = (SystemFolder)await kuboBinParentFolder.CreateFolderAsync(".ipfs", overwrite: false, cancellationToken);

        var bootstrapper = new KuboBootstrapper(repoFolder.Path, ctx => Task.FromResult<IFile>(kuboBin))
        {
            BinaryWorkingFolder = kuboBinParentFolderParent,
            ApiUri = new Uri($"http://127.0.0.1:{Settings.NodeApiPort}"),
            GatewayUri = new Uri($"http://127.0.0.1:{Settings.NodeGatewayPort}"),
            RoutingMode = (OwlCore.Kubo.DhtRoutingMode)Settings.BootstrapNodeDhtRouting,
            StartupProfiles =
            {
                Settings.BootstrapNodeEnableLocalDiscovery ? "local-discovery" : "server",
            }
        };

        return bootstrapper;
    }

    private async Task<IChildFile> GetOrDownloadKuboBinaryAsync(CancellationToken cancellationToken)
    {
        InitStatus = "Getting Kubo binary";

        var kuboBin = await GetDownloadedKuboBinaryAsync(cancellationToken);
        if (kuboBin is null)
        {
            InitStatus = "Downloading and extracting Kubo";
            var downloadedBinary = await KuboDownloader.GetBinaryVersionAsync(new Version(0, 19, 0), cancellationToken);

            kuboBin = await StoreDownloadedKuboBinaryAsync(downloadedBinary, cancellationToken);
        }

        return kuboBin;
    }

    private async Task ShowFirewallWarningAsync(CancellationToken cancellationToken)
    {
        Settings.FirewallWarningDisplayed = true;

        await new ContentDialog
        {
            Title = "Add IPFS to Windows Firewall",
            Content = new StackPanel
            {
                Spacing = 20,
                Children =
                {
                    new StackPanel
                    {
                        Spacing = 7,
                        Children =
                        {
                            new TextBlock { Text = "To enable IPFS functionality, we'll need to bootstrap Kubo.", TextWrapping = TextWrapping.WrapWholeWords },
                            new TextBlock { Text = "Windows may prompt you to add Kubo as a firewall exception.", TextWrapping = TextWrapping.WrapWholeWords },
                        }
                    },
                    new StackPanel
                    {
                        Spacing = 7,
                        Children =
                        {
                            new TextBlock { Text = "For the best experience it's recommended to allow Kubo on both public and private networks.", TextWrapping = TextWrapping.WrapWholeWords },
                            new TextBlock { Text = "This will not effect your existing communication preferences.", TextWrapping = TextWrapping.WrapWholeWords },
                        }
                    },
                },
            },
            CloseButtonText = "Continue"
        }.ShowAsync(ShowType.Interrupt);

        await Settings.SaveAsync(cancellationToken);
    }

    /// <summary>
    /// Scans localhost for a running Kubo RPC API.
    /// </summary>
    public async Task<IpfsClient?> ScanLocalhostForRunningKuboCompliantRpcApi(CancellationToken cancellationToken)
    {
        InitStatus = "Checking if Kubo is already running";
        cancellationToken.ThrowIfCancellationRequested();

        var innerCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var mutex = new SemaphoreSlim(20, 20);
        var checkedPorts = new HashSet<int>();

        // Scan default embedded node value
        await ScanPortAsync(Settings.NodeApiPort);

        // Scan default Kubo value
        await ScanPortAsync(5001);

        async Task ScanPortAsync(int port)
        {
            if (ThisPeer is not null || Client is not null || innerCancellationTokenSource.IsCancellationRequested)
                return;

            using (await mutex.DisposableWaitAsync(cancellationToken: innerCancellationTokenSource.Token))
            {
                if (ThisPeer is not null || Client is not null || innerCancellationTokenSource.IsCancellationRequested)
                    return;

                // Never scan the same port twice.
                if (!checkedPorts.Add(port))
                    return;

                var url = $"http://127.0.0.1:{port}";
                Logger.LogInformation($"Scanning {url} for a Kubo Compliant RPC API");

                var client = new IpfsClient(url);

                try
                {
                    ThisPeer = await client.IdAsync(cancel: cancellationToken);
                    Client = client;

                    innerCancellationTokenSource.Cancel();
                }
                catch
                {
                    // ignored
                }
            }
        }

        return Client;
    }

    /// <summary>
    /// Stores the provided Kubo <paramref name="binaryFile"/> in a place that can be quickly retrieved later.
    /// </summary>
    /// <param name="binaryFile">The Kubo binary to store.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    public async Task<IChildFile> StoreDownloadedKuboBinaryAsync(IFile binaryFile, CancellationToken cancellationToken)
    {
        var appDataFolder = new SystemFolder(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        var binariesFolder = (SystemFolder)await appDataFolder.CreateFolderAsync(nameof(KuboBootstrapper), overwrite: false, cancellationToken);

        var copy = await binariesFolder.CreateCopyOfAsync(binaryFile, overwrite: true, cancellationToken);
        Settings.DownloadKuboBinaryFileId = copy.Id;

        return copy;
    }

    /// <summary>
    /// Retrieves the latest kubo binary from the place where <see cref="StoreDownloadedKuboBinaryAsync"/> has stored it.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Task that represents the asynchronous operation. The value is the downloaded file, if found.</returns>
    public async Task<IChildFile?> GetDownloadedKuboBinaryAsync(CancellationToken cancellationToken)
    {
        var appDataFolder = new SystemFolder(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        var binariesFolder = (SystemFolder)await appDataFolder.CreateFolderAsync(nameof(KuboBootstrapper), overwrite: false, cancellationToken);

        if (!string.IsNullOrWhiteSpace(Settings.DownloadKuboBinaryFileId))
        {
            var item = await binariesFolder.GetItemAsync(Settings.DownloadKuboBinaryFileId, cancellationToken);
            return item as IChildFile;
        }

        return null;
    }
}
