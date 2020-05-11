using System;
using WpfApp1.Enums;

namespace WpfApp1.Interfaces
{
    public interface IAudioPlayer
    {
        AudioPlayerState CurrentState { get; }
        void SetTrack(Uri uri);
        void Play();
        void Pause();
        void Stop();
    }
}