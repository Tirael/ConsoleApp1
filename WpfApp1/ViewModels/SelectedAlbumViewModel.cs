using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using ReactiveUI;
using TestApp.Common.Interfaces;
using WpfApp1.Events;

namespace WpfApp1.ViewModels
{
    public class SelectedAlbumViewModel : ReactiveObject
    {
        private readonly IMusicInfoProvider _musicInfoProvider;

        public SelectedAlbumViewModel(IEventAggregator eventAggregator, IMusicInfoProvider musicInfoProvider)
        {
            _musicInfoProvider = musicInfoProvider;

            _allTracks = this
                .WhenAnyValue(x => x.SelectedItem)
                .Throttle(TimeSpan.FromMilliseconds(50), RxApp.TaskpoolScheduler)
                .Select(x => x?.CollectionId ?? 0)
                .SelectMany(GetAllMusicTracks)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.AllTracks);

            _isAvailable = this
                .WhenAnyValue(x => x.AllTracks)
                .Select(x => x != null && x.Any())
                .ToProperty(this, x => x.IsAvailable);

            eventAggregator
                .GetEvent<AlbumSelectedEvent>()
                .Subscribe(album => SelectedItem = album, ThreadOption.UIThread);

            this.ObservableForProperty(x => x.SelectedTrack)
                .Throttle(TimeSpan.FromMilliseconds(50), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .Where(x => null != x)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x.PreviewUrl)
                .Subscribe(x => eventAggregator
                    .GetEvent<TrackSelectedEvent>()
                    .Publish(x));
        }

        #region Properties

        #region SelectedItem

        private AlbumLookupEntryViewModel _selectedItem;

        public AlbumLookupEntryViewModel SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        #endregion

        #region SelectedTrack

        private AlbumTracksSearchEntryViewModel _selectedTrack;

        public AlbumTracksSearchEntryViewModel SelectedTrack
        {
            get => _selectedTrack;
            set => this.RaiseAndSetIfChanged(ref _selectedTrack, value);
        }

        #endregion

        #region AllTracks

        private readonly ObservableAsPropertyHelper<IEnumerable<AlbumTracksSearchEntryViewModel>> _allTracks;
        public IEnumerable<AlbumTracksSearchEntryViewModel> AllTracks => _allTracks.Value;

        #endregion

        #region IsAvailable

        private readonly ObservableAsPropertyHelper<bool> _isAvailable;
        public bool IsAvailable => _isAvailable.Value;

        #endregion

        #endregion

        private async Task<IEnumerable<AlbumTracksSearchEntryViewModel>> GetAllMusicTracks(long collectionId,
            CancellationToken token)
        {
            if (0 == collectionId)
                return Enumerable.Empty<AlbumTracksSearchEntryViewModel>();

            var albums = await _musicInfoProvider.GetAllMusicTracks(collectionId, token);

            return albums
                .OrderBy(x => x.DiscNumber)
                .ThenBy(x => x.TrackNumber)
                .Select(x => new AlbumTracksSearchEntryViewModel(x));
        }
    }
}