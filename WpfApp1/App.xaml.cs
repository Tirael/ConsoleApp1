using System.Windows;
using EasyCaching.Core;
using EasyCaching.Disk;
using EasyCaching.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Serilog;
using Serilog.Extensions.Logging;
using TestApp.Common.Interfaces;
using TestApp.Common.Providers;
using Unity;
using Unity.Injection;
using WpfApp1.Interfaces;
using WpfApp1.Providers;
using WpfApp1.ViewModels;
using WpfApp1.Views;

namespace WpfApp1
{
    public partial class App
    {
        private IConfigurationRoot _configuration;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();

            base.OnExit(e);
        }

        protected override Window CreateShell() => Container.Resolve<MainWindowView>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry
                .Register<MainWindowView, MainWindowView>()
                .Register<SelectedAlbumView, SelectedAlbumView>()
                .Register<MainWindowViewModel, MainWindowViewModel>()
                .Register<SelectedAlbumViewModel, SelectedAlbumViewModel>()
                .Register<ILogger<IMusicInfoProvider>, Logger<IMusicInfoProvider>>()
                .Register<ILogger<IImageProvider>, Logger<IImageProvider>>()
                .Register<ILogger<IAudioPlayer>, Logger<IAudioPlayer>>()
                .Register<ILoggerFactory, SerilogLoggerFactory>()
                .Register<IAudioPlayer, NAudioPlayer>()
                .RegisterSerilog();

            var loggerFactory = Container.Resolve<ILoggerFactory>();
            
            IEasyCachingProvider cachingProvider = CreateMusicInfosCachingProvider(loggerFactory);
            IEasyCachingProvider cachingImagesProvider = CreateAlbumCoversCachingProvider(loggerFactory);

            Container
                .Resolve<IUnityContainer>()
                .RegisterType<IMusicInfoProvider, AppleMusicInfoProvider>(
                    new InjectionConstructor(cachingProvider, typeof(ILogger<IMusicInfoProvider>)))
                .RegisterType<IImageProvider, ImageProvider>(new InjectionConstructor(cachingImagesProvider, typeof(ILogger<IImageProvider>)));
        }

        private static IEasyCachingProvider CreateMusicInfosCachingProvider(ILoggerFactory loggerFactory)
        {
            const string providerName = "musicInfosCache";

            var options = new SQLiteOptions
            {
                DBConfig = new SQLiteDBOptions
                {
                    FileName = $"{providerName}.db"
                },
                EnableLogging = true
            };

            var dbProvider = new SQLiteDatabaseProvider(providerName, options);

            return new DefaultSQLiteCachingProvider(providerName, new[] { dbProvider }, options, loggerFactory);
        }

        private static DefaultDiskCachingProvider CreateAlbumCoversCachingProvider(ILoggerFactory loggerFactory)
        {
            const string providerName = "albumCoversCache";

            var imagesOptions = new DiskOptions
            {
                DBConfig = new DiskDbOptions
                {
                    BasePath = providerName
                },
                EnableLogging = true
            };

            return new DefaultDiskCachingProvider(providerName, imagesOptions, loggerFactory);
        }
    }
}