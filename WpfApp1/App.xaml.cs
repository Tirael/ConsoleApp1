using System.Windows;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Serilog;
using WpfApp1.Extensions;
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
                .AddViewWithViewModels()
                .AddAudioPlayer()
                .AddLoggers()
                .AddMusicInfoProvider()
                .AddImageProvider()
                .RegisterSerilog();
        }
    }
}