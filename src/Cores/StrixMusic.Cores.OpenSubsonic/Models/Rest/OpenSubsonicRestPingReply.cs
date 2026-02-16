using System.Text.Json.Serialization;

// Used for de/serialization
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace StrixMusic.Cores.OpenSubsonic.Models.Rest;

public class OpenSubsonicRestPingReply
{
    [JsonPropertyName("subsonic-response")]
    public required OpenSubsonicRestSubsonicResponse SubsonicResponse { get; set; }
}

public class OpenSubsonicRestSubsonicResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("serverVersion")]
    public string? ServerVersion { get; set; }
    
    [JsonPropertyName("openSubsonic")]
    public bool? IsOpenSubsonic { get; set; }
    
    [JsonPropertyName("error")]
    public OpenSubsonicRestError? Error { get; set; }
}

public class OpenSubsonicRestError
{
    [JsonPropertyName("code")]
    public required int Code { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("helpUrl")]
    public string? HelpUrl { get; set; }
}


