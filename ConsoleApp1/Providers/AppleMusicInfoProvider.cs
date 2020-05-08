using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Extensions;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Models;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace ConsoleApp1.Providers
{
    public sealed class AppleMusicInfoProvider : IMusicInfoProvider
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly ILogger<AppleMusicInfoProvider> _logger;
        private const int ExpirationCacheInMinutes = 1;

        public AppleMusicInfoProvider(IEasyCachingProvider cachingProvider, ILogger<AppleMusicInfoProvider> logger)
        {
            _cachingProvider = cachingProvider;
            _logger = logger;
        }

        #region Implementation of IMusicInfoProvider

        public async Task<IEnumerable<AlbumLookupEntry>> GetAllAlbums(string searchString,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{searchString}");

            if (string.IsNullOrWhiteSpace(searchString)) throw new ArgumentException(nameof(searchString));

            var result = await GetAllAlbumsFromApi(searchString, cancellationToken)
                .Otherwise(() => GetAllAlbumsFromCache(searchString));

            return result;
        }

        #endregion

        /// <summary>
        /// Счетчик запросов для имитации ошибки при выполнении запроса
        /// </summary>
        private static int _requestCounter = 0;

        private async Task<IEnumerable<AlbumLookupEntry>> GetAllAlbumsFromApi(string searchString,
            CancellationToken cancellationToken)
        {
            // При повторном выполнении запроса будет исключение
            ++_requestCounter;

            _logger.LogInformation($"GetAllAlbumsFromApi: {searchString}, _requestCounter: {_requestCounter}");

            var client =
                new RestClient($"https://itunes.apple.com/search?term={searchString}&media=music&entity=album");
            var request = new RestRequest(Method.GET);

            IRestResponse response = await AsyncHelper.Retry(() =>
                {
                    // Имитация ошибки при выполнении запроса
                    if (_requestCounter > 1)
                    {
                        _logger.LogError("Connection error");

                        throw new Exception("Connection error");
                    }

                    return client.ExecuteAsync(request, cancellationToken);
                }, 3,
                TimeSpan.FromMilliseconds(100), cancellationToken);

            ApiResponse<AlbumLookupEntry> searchResponse =
                JsonConvert.DeserializeObject<ApiResponse<AlbumLookupEntry>>(response.Content);

            await _cachingProvider.SetAsync(searchString, searchResponse.Results,
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return searchResponse.Results;
        }

        private async Task<IEnumerable<AlbumLookupEntry>> GetAllAlbumsFromCache(string searchString)
        {
            _logger.LogInformation($"GetAllAlbumsFromCache: {searchString}");
            
            var result = await _cachingProvider.GetAsync(searchString, () => GetEmptyAlbums(),
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return result.Value;
        }

        private async Task<IEnumerable<AlbumLookupEntry>> GetEmptyAlbums() => new List<AlbumLookupEntry>();
    }
}