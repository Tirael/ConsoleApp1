using EasyCaching.SQLite;
using Microsoft.Extensions.Logging;
using TestApp.Common.Interfaces;

namespace WpfApp1.Providers
{
    public class MusicInfosCachingProvider : DefaultSQLiteCachingProvider, IMusicInfosCachingProvider
    {
        protected new const string ProviderName = "musicInfosCache";

        private static readonly SQLiteOptions Options = new SQLiteOptions
        {
            DBConfig = new SQLiteDBOptions
            {
                FileName = $"{ProviderName}.db"
            },
            EnableLogging = true
        };

        public MusicInfosCachingProvider(ILoggerFactory loggerFactory)
            : base(ProviderName, new[] {new SQLiteDatabaseProvider(ProviderName, Options)}, Options, loggerFactory)
        {
        }
    }
}