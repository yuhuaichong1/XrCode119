using System;
using UnityEngine;

namespace XrCode
{
    public interface IVideoPlayerService
    {
        /// <summary>
        /// 播放进度发生变化时触发
        /// </summary>
        event Action<float> OnProgressChanged;

        /// <summary>
        /// 完成时触发
        /// </summary>
        event Action OnVideoCompleted;

        /// <summary>
        /// 视频播放过程中发生错误时触发
        /// </summary>
        event Action<string> OnErrorOccurred;

        /// <summary>
        /// 播放状态发生变化时触发
        /// </summary>
        event Action<VideoPlayerState, VideoPlayerState> OnStateChanged;

        VideoPlayerState CurrentState { get; }
        bool IsPlaying { get; }
        float CurrentTime { get; }
        float Duration { get; }
        float Progress { get; }

        void Initialize();
        void PlayVideo(string videoPath, bool loop = false);
        void Play();
        void Pause();
        void Stop();
        void FastForward(float seconds = 3.0f);
        void Rewind(float seconds = -3.0f);
        void Seek(float time);
        void NextVideo();
        void Restart();
        void Dispose();
    }
}