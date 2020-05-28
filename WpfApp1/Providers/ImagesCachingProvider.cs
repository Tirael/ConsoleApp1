using EasyCaching.Disk;
using Microsoft.Extensions.Logging;
using TestApp.Common.Interfaces;

namespace WpfApp1.Providers
{
    public class ImagesCachingProvider : DefaultDiskCachingProvider, IImagesCachingProvider
    {
        protected new const string ProviderName = "albumCoversCache";

        private static readonly DiskOptions Options = new DiskOptions
        {
            DBConfig = new DiskDbOptions
            {
                BasePath = ProviderName
            },
            EnableLogging = true
        };

        public ImagesCachingProvider(ILoggerFactory loggerFactory)
            : base(ProviderName, Options, loggerFactory)
        {
        }
    }
}