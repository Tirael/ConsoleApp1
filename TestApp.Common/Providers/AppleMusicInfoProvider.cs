using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using TestApp.Common.Extensions;
using TestApp.Common.Interfaces;
using TestApp.Common.Models;

namespace TestApp.Common.Providers
{
    public sealed class AppleMusicInfoProvider : IMusicInfoProvider
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly ILogger<IMusicInfoProvider> _logger;
        private static readonly double ExpirationCacheInMinutes = TimeSpan.FromDays(1).TotalMinutes;

        public AppleMusicInfoProvider(IEasyCachingProvider cachingProvider, ILogger<IMusicInfoProvider> logger)
        {
            _cachingProvider = cachingProvider;
            _logger = logger;
        }

        #region Implementation of IMusicInfoProvider

        public async Task<IEnumerable<AlbumLookupEntry>> GetAllAlbums(string searchString, CancellationToken token)
        {
            _logger.LogInformation(searchString);

            if (string.IsNullOrWhiteSpace(searchString)) throw new ArgumentException(nameof(searchString));

            var result = await _cachingProvider.GetAsync(searchString,
                async () => await GetAllAlbumsInternal(searchString, token),
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return result.HasValue ? result.Value : Enumerable.Empty<AlbumLookupEntry>();
        }

        public async Task<IEnumerable<AlbumTracksSearchEntry>> GetAllMusicTracks(long collectionId, CancellationToken token)
        {
            var searchString = collectionId.ToString();

            _logger.LogInformation(searchString);

            if (string.IsNullOrWhiteSpace(searchString)) throw new ArgumentException(nameof(searchString));

            var result = await _cachingProvider.GetAsync(searchString,
                async () => await GetAllMusicTracksInternal(searchString, token),
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return result.HasValue ? result.Value : Enumerable.Empty<AlbumTracksSearchEntry>();
        }

        #endregion

        private async Task<IEnumerable<AlbumLookupEntry>> GetAllAlbumsInternal(string searchString,
            CancellationToken token)
        {
            _logger.LogInformation(searchString);

            var client =
                new RestClient($"https://itunes.apple.com/search?term={searchString}&media=music&entity=album");
            var request = new RestRequest(Method.GET);

            IRestResponse response = await AsyncEx.Retry(() => client.ExecuteAsync(request, token), 3,
                TimeSpan.FromMilliseconds(100), token);

            var searchResponse =
                JsonConvert.DeserializeObject<ApiResponse<AlbumLookupEntry>>(response.Content);

            await _cachingProvider.SetAsync(searchString, searchResponse.Results,
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return searchResponse.Results.Where(x => x.CollectionId != null);
        }

        private async Task<IEnumerable<AlbumTracksSearchEntry>> GetAllMusicTracksInternal(string searchString,
            CancellationToken token)
        {
            _logger.LogInformation(searchString);

            var client =
                new RestClient($"https://itunes.apple.com/lookup?id={searchString}&entity=song");
            var request = new RestRequest(Method.GET);

            IRestResponse response = await AsyncEx.Retry(() => client.ExecuteAsync(request, token), 3,
                TimeSpan.FromMilliseconds(100), token);

            var searchResponse =
                JsonConvert.DeserializeObject<ApiResponse<AlbumTracksSearchEntry>>(response.Content);

            await _cachingProvider.SetAsync(searchString, searchResponse.Results,
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return searchResponse.Results.Where(x => x.TrackId != null);
        }
    }
}