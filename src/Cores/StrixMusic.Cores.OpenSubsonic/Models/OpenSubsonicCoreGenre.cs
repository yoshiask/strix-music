using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OpenSubsonic.Models;

/// <inheritdoc cref="ICoreGenre" />
public sealed class OpenSubsonicCoreGenre : OpenSubsonicCoreModelBase, ICoreGenre
{
    /// <summary>
    /// Creates a new instance of <see cref="OpenSubsonicCoreGenre"/>.
    /// </summary>
    /// <param name="sourceCore">The source core that this instance belongs to.</param>
    /// <param name="genre">The name of the genre.</param>
    public OpenSubsonicCoreGenre(ICore sourceCore, string genre) : base(sourceCore)
    {
        Name = genre;
    }

    /// <inheritdoc/>
    public string Name { get; }
}
