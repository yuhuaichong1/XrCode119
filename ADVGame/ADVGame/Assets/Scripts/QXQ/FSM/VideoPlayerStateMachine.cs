using System;
using System.Collections.Generic;
using UnityEngine;

namespace XrCode
{
    public enum VideoPlayerState
    {
        Idle,           // 空闲
        Loading,        // 加载中
        Preparing,      // 准备中
        Playing,        // 播放中
        Paused,         // 暂停
        Seeking,        // 跳转中（快进/快退）
        Completed,      // 播放完成
        Error           // 错误状态
    }

    public class VideoPlayerStateMachine
    {
        private VideoPlayerState _currentState;
        public VideoPlayerState CurrentState => _currentState;

        // 状态转换规则
        private readonly Dictionary<VideoPlayerState, HashSet<VideoPlayerState>> _allowedTransitions = new Dictionary<VideoPlayerState, HashSet<VideoPlayerState>>
        {
            // 从空闲状态可以转到：加载、错误
            { VideoPlayerState.Idle, new HashSet<VideoPlayerState>
                { VideoPlayerState.Loading, VideoPlayerState.Error } },
            
            // 从加载状态可以转到：准备、错误、空闲
            { VideoPlayerState.Loading, new HashSet<VideoPlayerState>
                { VideoPlayerState.Preparing, VideoPlayerState.Error, VideoPlayerState.Idle } },
            
            // 从准备状态可以转到：播放、错误、空闲
            { VideoPlayerState.Preparing, new HashSet<VideoPlayerState>
                { VideoPlayerState.Playing, VideoPlayerState.Error, VideoPlayerState.Idle } },
            
            // 从播放状态可以转到：暂停、跳转、完成、错误、空闲
            { VideoPlayerState.Playing, new HashSet<VideoPlayerState>
                { VideoPlayerState.Paused, VideoPlayerState.Seeking, VideoPlayerState.Completed, VideoPlayerState.Error, VideoPlayerState.Idle } },
            
            // 从暂停状态可以转到：播放、跳转、错误、空闲
            { VideoPlayerState.Paused, new HashSet<VideoPlayerState>
                { VideoPlayerState.Playing, VideoPlayerState.Seeking, VideoPlayerState.Error, VideoPlayerState.Idle } },
            
            // 从跳转状态可以转到：播放、暂停、错误、空闲
            { VideoPlayerState.Seeking, new HashSet<VideoPlayerState>
                { VideoPlayerState.Playing, VideoPlayerState.Paused, VideoPlayerState.Error, VideoPlayerState.Idle } },
            
            // 从完成状态可以转到：播放、空闲、错误
            { VideoPlayerState.Completed, new HashSet<VideoPlayerState>
                { VideoPlayerState.Playing, VideoPlayerState.Idle, VideoPlayerState.Error } },
            
            // 从错误状态可以转到：空闲
            { VideoPlayerState.Error, new HashSet<VideoPlayerState>
                { VideoPlayerState.Idle } }
        };

        public event Action<VideoPlayerState, VideoPlayerState> OnStateChanged;

        public bool TryChangeState(VideoPlayerState newState)
        {
            if (_currentState == newState)
                return true;

            if (!CanTransitionTo(newState))
            {
                Debug.LogWarning($"Invalid state transition: {_currentState} -> {newState}");
                return false;
            }

            VideoPlayerState previousState = _currentState;
            _currentState = newState;

            Debug.Log($"VideoPlayer State: {previousState} -> {newState}");
            OnStateChanged?.Invoke(previousState, newState);
            return true;
        }

        public bool CanTransitionTo(VideoPlayerState newState)
        {
            return _allowedTransitions.ContainsKey(_currentState) &&
                   _allowedTransitions[_currentState].Contains(newState);
        }

        // 便捷属性
        public bool CanPlay => _currentState == VideoPlayerState.Idle ||
                              _currentState == VideoPlayerState.Paused ||
                              _currentState == VideoPlayerState.Completed ||
                              _currentState == VideoPlayerState.Preparing;

        public bool CanPause => _currentState == VideoPlayerState.Playing ||
                               _currentState == VideoPlayerState.Seeking;

        public bool CanSeek => _currentState == VideoPlayerState.Playing ||
                              _currentState == VideoPlayerState.Paused;

        public bool CanStop => _currentState != VideoPlayerState.Idle &&
                              _currentState != VideoPlayerState.Error;

        public bool CanLoad => _currentState == VideoPlayerState.Idle ||
                              _currentState == VideoPlayerState.Completed ||
                              _currentState == VideoPlayerState.Error;

        public void Reset()
        {
            _currentState = VideoPlayerState.Idle;
        }
    }
}