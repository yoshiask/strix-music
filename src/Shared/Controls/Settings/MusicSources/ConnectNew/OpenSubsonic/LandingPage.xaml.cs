using System;
using System.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StrixMusic.Cores.OpenSubsonic.Models;
using StrixMusic.Cores.OpenSubsonic.Models.Rest;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.OpenSubsonic;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class LandingPage : Page
{
    [ObservableProperty] private ConnectNewMusicSourceNavigationParams? _param;
    [ObservableProperty] private OpenSubsonicCoreSettings? _settings = null;
    private TimeSpan _serverMaxResponseTime = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Creates a new instance of <see cref="LandingPage"/>.
    /// </summary>
    public LandingPage()
    {
        this.InitializeComponent();
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true, AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
    private async Task TryContinueAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        Guard.IsNotNull(Param?.AppRoot?.MusicSourcesSettings);
        Guard.IsNotNull(Settings);

        // Create new token we can cancel on a timer
        using var cancellationTokenSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationTokenSrc.CancelAfter(_serverMaxResponseTime);

        cancellationToken = cancellationTokenSrc.Token;
        
        // Trim extraneous whitespace characters
        char[] extraWhitespaceChars = ['\t', '\r', '\n'];
        Settings.ServerUrl = Settings.ServerUrl.Trim();
        Settings.Username = Settings.Username.TrimEnd(extraWhitespaceChars);
        Settings.Password = Settings.Password.TrimEnd(extraWhitespaceChars);

        try
        {
            var messageHandler = Param?.HttpMessageHandler;
            using HttpClient httpClient = messageHandler is null
                ? new()
                : new(messageHandler);

            var pingReply = await httpClient.GetFromJsonAsync<OpenSubsonicRestPingReply>(
                System.IO.Path.Join(Settings.ServerUrl, $"/rest/ping.view?v=1.16.1&u={Settings.Username}&p={Settings.Password}&c=Strix%20Music&f=json"),
                cancellationToken: cancellationToken);

            var error = pingReply!.SubsonicResponse.Error;
            if (error is not null)
                throw new Exception(error.Message ?? $"Subsonic error {error.Code}");

            if (!pingReply.SubsonicResponse.Status?.Equals("ok", StringComparison.InvariantCultureIgnoreCase) ?? false)
                throw new Exception("Unknown failure from Subsonic server");
            
            CompleteCoreSetup();
        }
        catch (OperationCanceledException)
        {
            throw new InvalidOperationException("Request timed out. Make sure the address is valid");
        }
        catch (System.Text.Json.JsonException)
        {
            throw new Exception("Invalid response from server. Make sure you entered the address of an OpenSubsonic-compatible instance.");
        }
    }

    private void CompleteCoreSetup()
    {
        Guard.IsNotNull(Param?.AppRoot?.MusicSourcesSettings?.ConfiguredOpenSubsonicCores);
        Guard.IsNotNull(Settings);
        Param.AppRoot.MusicSourcesSettings.ConfiguredOpenSubsonicCores.Add(Settings);
        Param.SetupCompleteTaskCompletionSource.SetResult(null);
    }

    [RelayCommand]
    private void CancelCoreSetup()
    {
        Guard.IsNotNull(Param);
        Param.SetupCompleteTaskCompletionSource.SetCanceled();
    }

    /// <inheritdoc />
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        var param = (ConnectNewMusicSourceNavigationParams)e.Parameter;
        Guard.IsNotNull(param.SelectedSourceToConnect);

        // Save in a field to access from another method
        Param = param;

        // The instance ID here is a temporary placeholder.
        // We need to log in and get a folder ID before we can create the final instance ID.
        Settings = (OpenSubsonicCoreSettings)await Param.SelectedSourceToConnect.DefaultSettingsFactory(string.Empty);
        Settings.InstanceId = Settings.Folder.Id;
    }

    private bool IsFormValid(string serverUrl, string username, string password) =>
        Uri.IsWellFormedUriString(serverUrl.Trim(), UriKind.Absolute)  && !string.IsNullOrWhiteSpace(username);
}
