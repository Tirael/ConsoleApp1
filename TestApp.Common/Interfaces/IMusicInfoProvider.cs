using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Common.Models;

namespace TestApp.Common.Interfaces
{
    public interface IMusicInfoProvider
    {
        Task<IEnumerable<AlbumLookupEntry>> GetAllAlbums(string searchString, CancellationToken token);
        Task<IEnumerable<AlbumTracksSearchEntry>> GetAllMusicTracks(long collectionId, CancellationToken token);
    }
}