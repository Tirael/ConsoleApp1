using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Serilog.Extensions.Logging;
using TestApp.Common.Interfaces;
using TestApp.Common.Providers;
using WpfApp1.Interfaces;
using WpfApp1.Providers;
using WpfApp1.ViewModels;
using WpfApp1.Views;

namespace WpfApp1.Extensions
{
    public static class ContainerRegistryEx
    {
        public static IContainerRegistry AddViewWithViewModels(this IContainerRegistry serviceCollection)
        {
            return serviceCollection
                .Register<MainWindowView, MainWindowView>()
                .Register<SelectedAlbumView, SelectedAlbumView>()
                .Register<MainWindowViewModel, MainWindowViewModel>()
                .Register<SelectedAlbumViewModel, SelectedAlbumViewModel>();
        }

        public static IContainerRegistry AddLoggers(this IContainerRegistry serviceCollection)
        {
            return serviceCollection
                .Register<ILogger<IMusicInfoProvider>, Logger<IMusicInfoProvider>>()
                .Register<ILogger<IImageProvider>, Logger<IImageProvider>>()
                .Register<ILogger<IAudioPlayer>, Logger<IAudioPlayer>>()
                .Register<ILoggerFactory, SerilogLoggerFactory>();
        }

        public static IContainerRegistry AddAudioPlayer(this IContainerRegistry serviceCollection)
        {
            return serviceCollection
                .Register<IAudioPlayer, NAudioPlayer>();
        }

        public static IContainerRegistry AddMusicInfoProvider(this IContainerRegistry serviceCollection)
        {
            return serviceCollection
                .RegisterSingleton<IMusicInfosCachingProvider, MusicInfosCachingProvider>()
                .RegisterSingleton<IMusicInfoProvider, AppleMusicInfoProvider>();
        }

        public static IContainerRegistry AddImageProvider(this IContainerRegistry serviceCollection)
        {
            return serviceCollection
                .RegisterSingleton<IImagesCachingProvider, ImagesCachingProvider>()
                .RegisterSingleton<IImageProvider, ImageProvider>();
        }
    }
}