using System;
using System.Diagnostics;
using TestApp.Common.Models;

namespace WpfApp1.ViewModels
{
    [DebuggerDisplay("CollectionName:{CollectionName}, TrackId:{TrackId}, DiscNumber:{DiscNumber}, TrackNumber:{TrackNumber}, TrackName:{TrackName}")]
    public class AlbumTracksSearchEntryViewModel
    {
        private readonly AlbumTracksSearchEntry _model;
        
        public AlbumTracksSearchEntryViewModel(AlbumTracksSearchEntry model)
        {
            _model = model;
        }

        #region Properties
        
        public string CollectionName => _model.CollectionName;

        public long? CollectionId => _model.CollectionId;

        public DateTimeOffset? ReleaseDate => _model.ReleaseDate;

        public long? TrackCount => _model.TrackCount;

        public long? DiscNumber => _model.DiscNumber;

        public long? TrackId => _model.TrackId;

        public long? TrackNumber => _model.TrackNumber;

        public string TrackName => _model.TrackName;

        public Uri PreviewUrl => _model.PreviewUrl;

        #endregion
    }
}