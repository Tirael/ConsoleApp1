using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TestApp.Common.Models;
using WpfApp1.Interfaces;

namespace WpfApp1.ViewModels
{
    [DebuggerDisplay("ArtistName:{ArtistName}, CollectionName:{CollectionName}, CollectionId:{CollectionId}, TrackCount:{TrackCount}")]
    public class AlbumLookupEntryViewModel
    {
        private readonly AlbumLookupEntry _model;

        public static async Task<AlbumLookupEntryViewModel> CreateAsync(AlbumLookupEntry model, IImageProvider imageProvider)
        {
            return new AlbumLookupEntryViewModel(model, imageProvider);
        }

        private AlbumLookupEntryViewModel(AlbumLookupEntry model, IImageProvider imageProvider)
        {
            _model = model;
            
            Uri uri =  model.ArtworkUrl100 ?? model.ArtworkUrl60 ?? model.ArtworkUrl30;
            
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Artwork = new LazyProperty<BitmapImage>(
                    async cancelToken => await imageProvider
                        .GetImage(uri, cancelToken)
                        .ConfigureAwait(true),
                    new BitmapImage());
            });
        }

        #region Properties

        public string ArtistName => _model.ArtistName;

        public string CollectionName => _model.CollectionName;

        public long? CollectionId => _model.CollectionId;

        public DateTimeOffset? ReleaseDate => _model.ReleaseDate;

        public long? TrackCount => _model.TrackCount;

        public Uri ArtworkUrl => _model.ArtworkUrl100;

        public LazyProperty<BitmapImage> Artwork { get; private set; }

        #endregion
    }
}