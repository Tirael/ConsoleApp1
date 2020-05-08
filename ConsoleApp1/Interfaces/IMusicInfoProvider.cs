using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Models;

namespace ConsoleApp1.Interfaces
{
    public interface IMusicInfoProvider
    {
        Task<IEnumerable<AlbumLookupEntry>> GetAllAlbums(string searchString, CancellationToken cancellationToken);
    }
}