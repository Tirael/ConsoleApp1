using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Stateless;
using WpfApp1.Enums;
using WpfApp1.Interfaces;

namespace WpfApp1
{
    public class NAudioPlayer : IAudioPlayer, INotifyPropertyChanged
    {
        private readonly ILogger<IAudioPlayer> _logger;
        private readonly StateMachine<AudioPlayerState, AudioPlayerTrigger> _stateMachine;

        private Uri _uriToPlay;
        private WaveOutEvent _outputAudioDevice;
        private FadeInOutSampleProvider _fadeInOutSampleProvider;
        private const int FadeInOutInMs = 2000;

        public NAudioPlayer(ILogger<IAudioPlayer> logger)
        {
            _logger = logger;

            _stateMachine =
                new StateMachine<AudioPlayerState, AudioPlayerTrigger>(AudioPlayerState.Resetting, FiringMode.Queued);

            _stateMachine.Configure(AudioPlayerState.Resetting)
                .OnEntryAsync(ResetPlayer)
                .Permit(AudioPlayerTrigger.Stop, AudioPlayerState.Stopped);

            _stateMachine.Configure(AudioPlayerState.Playing)
                .OnEntryAsync(StartPlayer)
                .Permit(AudioPlayerTrigger.Pause, AudioPlayerState.Paused)
                .Permit(AudioPlayerTrigger.Stop, AudioPlayerState.Stopped)
                .Permit(AudioPlayerTrigger.Reset, AudioPlayerState.Resetting);

            _stateMachine.Configure(AudioPlayerState.Paused)
                .OnEntryAsync(PausePlayer)
                .Permit(AudioPlayerTrigger.Play, AudioPlayerState.Playing)
                .Permit(AudioPlayerTrigger.Stop, AudioPlayerState.Stopped)
                .Permit(AudioPlayerTrigger.Reset, AudioPlayerState.Resetting);

            _stateMachine.Configure(AudioPlayerState.Stopped)
                .OnEntryAsync(StopPlayer)
                .PermitIf(AudioPlayerTrigger.Play, AudioPlayerState.Playing, () => CanPlayTrack(_uriToPlay))
                .Permit(AudioPlayerTrigger.Reset, AudioPlayerState.Resetting);

            _stateMachine.OnTransitioned(_ => OnPropertyChanged(nameof(CurrentState)));

            // Check graph on http://www.webgraphviz.com
            // string graph = UmlDotGraph.Format(_stateMachine.GetInfo());
        }

        private static bool CanPlayTrack(Uri uriToPlay) => null != uriToPlay && !string.IsNullOrWhiteSpace(uriToPlay.AbsoluteUri);

        private async Task StartPlayer()
        {
            if (_outputAudioDevice?.PlaybackState is PlaybackState.Paused)
            {
                _fadeInOutSampleProvider.BeginFadeIn(FadeInOutInMs);
                
                _outputAudioDevice.Play();

                return;
            }

            await using var mf = new MediaFoundationReader(_uriToPlay.AbsoluteUri);

            _fadeInOutSampleProvider = new FadeInOutSampleProvider(mf.ToSampleProvider(), true);
            _fadeInOutSampleProvider.BeginFadeIn(FadeInOutInMs);

            _logger.LogInformation($"file to play: {_uriToPlay.AbsoluteUri}, total time: {mf.TotalTime}");

            _outputAudioDevice = new WaveOutEvent();
            _outputAudioDevice.PlaybackStopped += OutputAudioDeviceOnPlaybackStopped;
            _outputAudioDevice.Init(_fadeInOutSampleProvider);
            _outputAudioDevice.Play();
        }

        private async Task PausePlayer()
        {
            if (!(_outputAudioDevice?.PlaybackState is PlaybackState.Playing))
                return;

            _fadeInOutSampleProvider.BeginFadeOut(FadeInOutInMs);

            await Task.Delay(FadeInOutInMs);

            _outputAudioDevice.Pause();
        }

        private async Task StopPlayer()
        {
            if (null != _fadeInOutSampleProvider)
            {
                _fadeInOutSampleProvider.BeginFadeOut(FadeInOutInMs);
                await Task.Delay(FadeInOutInMs);
            }

            DestroyAudioDevice();
        }

        private void DestroyAudioDevice()
        {
            if (null != _outputAudioDevice)
                _outputAudioDevice.PlaybackStopped -= OutputAudioDeviceOnPlaybackStopped;

            _outputAudioDevice?.Stop();
            _outputAudioDevice?.Dispose();

            _outputAudioDevice = null;
        }

        private async Task ResetPlayer() => await StopPlayer();

        private async void OutputAudioDeviceOnPlaybackStopped(object sender, StoppedEventArgs e) =>
            await _stateMachine.FireAsync(AudioPlayerTrigger.Stop);

        public async void SetTrack(Uri uri)
        {
            _uriToPlay = uri;

            if (null == uri)
            {
                await _stateMachine.FireAsync(AudioPlayerTrigger.Reset);
                return;
            }

            if (_stateMachine.CanFire(AudioPlayerTrigger.Stop))
                await _stateMachine.FireAsync(AudioPlayerTrigger.Stop);
        }

        #region Implementation of IAudioPlayer

        public AudioPlayerState CurrentState => _stateMachine.State;

        public async void Play()
        {
            if (_stateMachine.CanFire(AudioPlayerTrigger.Play))
                await _stateMachine.FireAsync(AudioPlayerTrigger.Play);
        }

        public async void Pause()
        {
            if (_stateMachine.CanFire(AudioPlayerTrigger.Pause))
                await _stateMachine.FireAsync(AudioPlayerTrigger.Pause);
        }

        public async void Stop()
        {
            if (_stateMachine.CanFire(AudioPlayerTrigger.Stop))
                await _stateMachine.FireAsync(AudioPlayerTrigger.Stop);
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}