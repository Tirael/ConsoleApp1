using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Regions;
using ReactiveUI;
using TestApp.Common.Interfaces;
using WpfApp1.Constants;
using WpfApp1.Enums;
using WpfApp1.Events;
using WpfApp1.Interfaces;
using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly IMusicInfoProvider _musicInfoProvider;
        private readonly IImageProvider _imageProvider;
        private readonly IAudioPlayer _audioPlayer;

        public MainWindowViewModel(IRegionManager regionManager, IMusicInfoProvider musicInfoProvider,
            IImageProvider imageProvider, IEventAggregator eventAggregator, IAudioPlayer audioPlayer)
        {
            _musicInfoProvider = musicInfoProvider;
            _imageProvider = imageProvider;
            _audioPlayer = audioPlayer;

            regionManager.RegisterViewWithRegion(UIConstants.SelectedAlbumContentRegionName, typeof(SelectedAlbumView));

            _searchResults = this
                .WhenAnyValue(x => x.SearchString)
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.TaskpoolScheduler)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .SelectMany(SearchAllAlbums)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(x => SelectedItem = x.FirstOrDefault())
                .ToProperty(this, x => x.SearchResults);

            _isAvailable = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(x => x != null)
                .ToProperty(this, x => x.IsAvailable);

            this.ObservableForProperty(x => x.SelectedItem)
                .Throttle(TimeSpan.FromMilliseconds(50), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .Subscribe(x => OnSelectedItemChanged(eventAggregator, x));

            this.ObservableForProperty(x => x._audioPlayer.CurrentState)
                .Throttle(TimeSpan.FromMilliseconds(1000), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .Subscribe(OnAudioPlayerCurrentStateChanged);

            eventAggregator
                .GetEvent<TrackSelectedEvent>()
                .Subscribe(audioPlayer.SetTrack, ThreadOption.BackgroundThread);

            InitializeCommands();
        }

        private void OnAudioPlayerCurrentStateChanged(AudioPlayerState audioPlayerState)
        {
            PlayCommandIsAvailable = audioPlayerState == AudioPlayerState.Stopped || audioPlayerState == AudioPlayerState.Paused;
            PauseCommandIsAvailable = audioPlayerState == AudioPlayerState.Playing;
            StopCommandIsAvailable = audioPlayerState == AudioPlayerState.Playing || audioPlayerState == AudioPlayerState.Paused;
        }

        private void InitializeCommands()
        {
            OpenProjectOnGitHub = ReactiveCommand.CreateFromTask(OpenProjectOnGitHubExecuteAsync);
            PlaySelectedTrack = ReactiveCommand.CreateFromTask(PlaySelectedTrackExecuteAsync);
            PauseSelectedTrack = ReactiveCommand.CreateFromTask(PauseSelectedTrackExecuteAsync);
            StopSelectedTrack = ReactiveCommand.CreateFromTask(StopSelectedTrackExecuteAsync);

            _openProjectOnGitHubIsRunning = OpenProjectOnGitHub
                .IsExecuting
                .ToProperty(this, nameof(OpenProjectOnGitHubIsRunning), scheduler: RxApp.MainThreadScheduler);
        }

        private void OnSelectedItemChanged(IEventAggregator eventAggregator, AlbumLookupEntryViewModel selectedItem)
        {
            WindowTitle = null == selectedItem ? "Artist not found" : $"{selectedItem.ArtistName}: {selectedItem.CollectionName}";

            if (null == selectedItem) _audioPlayer.SetTrack(null);

            eventAggregator
                .GetEvent<AlbumSelectedEvent>()
                .Publish(selectedItem);
        }

        #region Commands

        #region OpenProjectOnGitHub

        #region OpenProjectOnGitHubIsRunning

        private ObservableAsPropertyHelper<bool> _openProjectOnGitHubIsRunning;

        public bool OpenProjectOnGitHubIsRunning => _openProjectOnGitHubIsRunning.Value;

        #endregion

        public ReactiveCommand<Unit, Unit> OpenProjectOnGitHub { get; private set; }

        private Task OpenProjectOnGitHubExecuteAsync() => Task.Run(OpenProjectOnGitHubExecute);

        private void OpenProjectOnGitHubExecute()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Tirael/ConsoleApp1",
                UseShellExecute = true
            });
        }

        #endregion

        #region PlaySelectedTrack

        #region PlaySelectedTrackIsRunning

        private bool _playSelectedTrackIsRunning;

        public bool PlaySelectedTrackIsRunning
        {
            get => _playSelectedTrackIsRunning;
            set => this.RaiseAndSetIfChanged(ref _playSelectedTrackIsRunning, value);
        }

        #endregion

        public ReactiveCommand<Unit, Unit> PlaySelectedTrack { get; private set; }

        private Task PlaySelectedTrackExecuteAsync() => Task.Run(PlaySelectedTrackExecute);

        private void PlaySelectedTrackExecute()
        {
            _audioPlayer.Play();
        }

        #endregion

        #region PauseSelectedTrack

        #region PauseSelectedTrackIsRunning

        private bool _pauseSelectedTrackIsRunning;

        public bool PauseSelectedTrackIsRunning
        {
            get => _pauseSelectedTrackIsRunning;
            set => this.RaiseAndSetIfChanged(ref _pauseSelectedTrackIsRunning, value);
        }

        #endregion

        public ReactiveCommand<Unit, Unit> PauseSelectedTrack { get; private set; }

        private Task PauseSelectedTrackExecuteAsync() => Task.Run(PauseSelectedTrackExecute);

        private void PauseSelectedTrackExecute()
        {
            _audioPlayer.Pause();
        }
        
        #endregion

        #region StopSelectedTrack
        
        #region StopSelectedTrackIsRunning

        private bool _stopSelectedTrackIsRunning = true;

        public bool StopSelectedTrackIsRunning
        {
            get => _stopSelectedTrackIsRunning;
            set => this.RaiseAndSetIfChanged(ref _stopSelectedTrackIsRunning, value);
        }

        #endregion

        public ReactiveCommand<Unit, Unit> StopSelectedTrack { get; private set; }

        private Task StopSelectedTrackExecuteAsync() => Task.Run(StopSelectedTrackExecute);

        private void StopSelectedTrackExecute()
        {
            _audioPlayer.Stop();
        }

        #endregion

        #endregion

        #region Properties

        #region WindowTitle

        private string _windowTitle;

        public string WindowTitle
        {
            get => _windowTitle;
            private set => this.RaiseAndSetIfChanged(ref _windowTitle, value);
        }

        #endregion

        #region PlayCommandIsAvailable

        private bool _playCommandIsAvailable;

        public bool PlayCommandIsAvailable
        {
            get => _playCommandIsAvailable;
            private set => this.RaiseAndSetIfChanged(ref _playCommandIsAvailable, value);
        }

        #endregion

        #region PauseCommandIsAvailable

        private bool _pauseCommandIsAvailable;

        public bool PauseCommandIsAvailable
        {
            get => _pauseCommandIsAvailable;
            private set => this.RaiseAndSetIfChanged(ref _pauseCommandIsAvailable, value);
        }

        #endregion

        #region StopCommandIsAvailable

        private bool _stopCommandIsAvailable;

        public bool StopCommandIsAvailable
        {
            get => _stopCommandIsAvailable;
            private set => this.RaiseAndSetIfChanged(ref _stopCommandIsAvailable, value);
        }

        #endregion

        #region SearchString

        private string _searchString;

        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }

        #endregion

        #region SelectedItem

        private AlbumLookupEntryViewModel _selectedItem;

        public AlbumLookupEntryViewModel SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        #endregion

        #region SearchResults

        private readonly ObservableAsPropertyHelper<IEnumerable<AlbumLookupEntryViewModel>> _searchResults;
        public IEnumerable<AlbumLookupEntryViewModel> SearchResults => _searchResults.Value;

        #endregion

        #region IsAvailable

        private readonly ObservableAsPropertyHelper<bool> _isAvailable;
        public bool IsAvailable => _isAvailable.Value;

        #endregion

        #endregion

        private async Task<IEnumerable<AlbumLookupEntryViewModel>> SearchAllAlbums(string searchString,
            CancellationToken token)
        {
            var albums = await _musicInfoProvider.GetAllAlbums(searchString, token);

            return albums
                .OrderBy(x => x.ReleaseDate)
                .Select(x => new AlbumLookupEntryViewModel(x, _imageProvider));
        }
    }
}