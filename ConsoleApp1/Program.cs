using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Interfaces;
using ConsoleApp1.Models;
using ConsoleApp1.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ConsoleApp1
{
    /// <summary>
    /// Нужно написать консольное приложение поиска музыкальных альбомов исполнителя.
    /// 
    /// При вводе пользователем названия группы, программа должна запрашивать сервер в поисках списка её альбомов.
    /// 
    /// При отсутствии соединения с сервером, список (если он был загружен ранее) должен подгружаться из локального кэша.
    /// 
    /// Допускается использование любого сервера с любым API (рекомендуется сервис iTunes как не требующий авторизации).
    /// 
    /// Для хранения кэша допускается использование любого носителя (файл, любая база данных).
    /// 
    /// Допускается использование любых сторонних библиотек.
    /// 
    /// </summary>
    public class Program
    {
        private static readonly ServiceProvider ServiceProvider;
        
        static Program()
        {
            var services = new ServiceCollection();
            
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ConfigureServices(configuration, services);

            ServiceProvider = services.BuildServiceProvider();
        }
        
        private static void ConfigureServices(IConfigurationRoot configuration, ServiceCollection services)
        {
            var appConfig = configuration.GetSection("Properties").Get<Properties>();

            services.AddEasyCaching(options =>
            {
                if (appConfig.CacheToDb)
                {
                    options.UseSQLite(configuration);
                }
                else
                {
                    options.UseDisk(configuration);
                }
            });

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

            services.AddScoped<IMusicInfoProvider, AppleMusicInfoProvider>();
        }
        
        /// <summary>
        /// Только первый запрос попадет в кэш, следующие запросы к удаленному api не будут производиться из-за имитации отсутствия соединения.
        /// Для имитации сбоя связи необходимо повторить запрос.
        /// Если запрос будет совпадать с первым, то он будет вычитан из кэша.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var logger = ServiceProvider.GetService<ILogger<Program>>();
            
            var musicInfo = ServiceProvider.GetService<IMusicInfoProvider>();

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                logger.LogInformation("Cancel event triggered");
                cts.Cancel();
                eventArgs.Cancel = true;
            };

            while (true)
            {
                Console.WriteLine("Enter artist name, empty string to exit:");

                string searchString = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(searchString))
                    break;

                IEnumerable<AlbumLookupEntry> albums = await musicInfo.GetAllAlbums(searchString, cts.Token);

                foreach (string collectionName in albums.Select(x => x.CollectionName))
                {
                    Console.WriteLine(collectionName);
                }

                Console.WriteLine();
            }

            logger.LogInformation("Finished...");

            cts.Dispose();
        }
    }
}
