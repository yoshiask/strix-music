using System.Collections.Generic;
using System.Threading;
using StrixMusic.Sdk.CoreModels;
using SubSonicMedia.Responses.Browsing;

namespace StrixMusic.Cores.OpenSubsonic;

public class OpenSubsonicLibrary : OpenSubsonicPlayableCollectionGroupBase, ICoreLibrary
{
    protected OpenSubsonicLibrary(OpenSubsonicCore sourceCore) : base(sourceCore)
    {
    }

    public override int TotalAlbumItemsCount { get; }

    public async override IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var albumList = await Client.Browsing.GetAlbumListAsync(
            type: AlbumListType.AlphabetalByName,
            size: limit, offset: offset,
            cancellationToken: cancellationToken);
        foreach (var album in albumList.AlbumList2.Album)
        {
            yield return album;
        }
    }
}
