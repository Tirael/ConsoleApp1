using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using RestSharp;
using WpfApp1.Interfaces;

namespace WpfApp1.Providers
{
    public class ImageProvider : IImageProvider
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly ILogger<IImageProvider> _logger;
        private static readonly double ExpirationCacheInMinutes = TimeSpan.FromDays(1).TotalMinutes;

        public ImageProvider(IEasyCachingProvider cachingProvider, ILogger<IImageProvider> logger)
        {
            _cachingProvider = cachingProvider;
            _logger = logger;
        }

        public async Task<BitmapImage> GetImage(Uri uri, CancellationToken token)
        {
            _logger.LogInformation(uri.AbsoluteUri);

            string cacheKey = GetStringSha256Hash(uri.AbsoluteUri);
            
            var result = await _cachingProvider.GetAsync(cacheKey, () => GetImageInternal(uri, token),
                TimeSpan.FromMinutes(ExpirationCacheInMinutes));

            return result.HasValue && result.Value.Length > 0 ? await CreateImage(result.Value) : await CreateEmptyImage();
        }

        private async Task<byte[]> GetImageInternal(Uri uri, CancellationToken token)
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);

            IRestResponse response = await client.ExecuteAsync(request, token);

            return response.RawBytes;
        }

        private static string GetStringSha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using var sha = new System.Security.Cryptography.SHA256Managed();
            
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private static Task<BitmapImage> CreateImage(byte[] bytes)
        {
            return Task.Run(() =>
            {
                using var stream = new MemoryStream(bytes);

                stream.Seek(0, SeekOrigin.Begin);

                var image = new BitmapImage();

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();

                return image;
            });
        }
        private static Task<BitmapImage> CreateEmptyImage()
        {
            return Task.Run(() =>
            {
                var image = new BitmapImage();
                image.Freeze();

                return image;
            });
        }
    }
}