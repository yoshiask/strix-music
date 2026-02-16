using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OpenSubsonic.Models;

/// <inheritdoc cref="ICoreImage" />
public sealed class OpenSubsonicCoreImage : OpenSubsonicCoreModelBase, ICoreImage
{
    private readonly string _id;
    
    /// <summary>
    /// Creates a new instance of <see cref="OpenSubsonicCoreImage"/>.
    /// </summary>
    /// <param name="sourceCore">The source core.</param>
    /// <param name="coverArtId">The ID or the covert art to wrap.</param>
    public OpenSubsonicCoreImage(ICore sourceCore, string coverArtId) : base(sourceCore)
    {
        _id = coverArtId;
    }

    /// <inheritdoc />
    public async Task<Stream> OpenStreamAsync()
    {
        var stream = await Client.Media.GetCoverArtAsync(_id);
        Guard.IsNotNull(stream);

        return stream;
    }

    /// <inheritdoc />
    public string? MimeType => null;

    /// <inheritdoc />
    public double? Height => null;

    /// <inheritdoc />
    public double? Width => null;
}
