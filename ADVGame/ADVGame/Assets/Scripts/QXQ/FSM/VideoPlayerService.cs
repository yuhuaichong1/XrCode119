using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using cfg;

namespace XrCode
{
    public class VideoPlayerService : IVideoPlayerService, IDisposable
    {
        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;
        private VideoPlayerStateMachine _stateMachine;
        private GameObject _videoPlayerObject;

        private string _currentVideoPath;
        private bool _shouldLoop;
        private float _normalSpeed = 1.0f;
        private Coroutine _seekingCoroutine;

        public event Action<float> OnProgressChanged;
        public event Action OnVideoCompleted;
        public event Action<string> OnErrorOccurred;
        public event Action<VideoPlayerState, VideoPlayerState> OnStateChanged;

        public VideoPlayerState CurrentState => _stateMachine.CurrentState;
        public bool IsPlaying => _stateMachine.CurrentState == VideoPlayerState.Playing;
        public float CurrentTime => _videoPlayer != null ? (float)_videoPlayer.time : 0;
        public float Duration => _videoPlayer != null ? (float)_videoPlayer.length : 0;
        public float Progress => Duration > 0 ? CurrentTime / Duration : 0;

        public VideoPlayerService()
        {
            Initialize();
        }

        public void Initialize()
        {
            _videoPlayerObject = new GameObject("VideoPlayer");
            UnityEngine.Object.DontDestroyOnLoad(_videoPlayerObject);
            _videoPlayer = _videoPlayerObject.AddComponent<VideoPlayer>();
            _audioSource = _videoPlayerObject.AddComponent<AudioSource>();

            SetupVideoPlayer();
            SetupStateMachine();
        }

        private void SetupVideoPlayer()
        {
            _videoPlayer.playOnAwake = false;
            _videoPlayer.waitForFirstFrame = true;
            _videoPlayer.skipOnDrop = true;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            _videoPlayer.controlledAudioTrackCount = 1;
            _videoPlayer.SetTargetAudioSource(0, _audioSource);
            _videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
            _videoPlayer.targetCamera = Camera.main;
            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.loopPointReached += OnVideoLoopPointReached;
            _videoPlayer.errorReceived += OnVideoError;
            _videoPlayer.seekCompleted += OnSeekCompleted;
        }

        private void SetupStateMachine()
        {
            _stateMachine = new VideoPlayerStateMachine();
            _stateMachine.OnStateChanged += OnStateMachineStateChanged;
        }

        private void OnStateMachineStateChanged(VideoPlayerState previous, VideoPlayerState current)
        {
            OnStateChanged?.Invoke(previous, current);
            Debug.Log($"VideoPlayer State Changed: {previous} -> {current}");
        }

        public void PlayVideo(string videoPath, bool loop = false)
        {
            if (!_stateMachine.CanLoad)
            {
                OnErrorOccurred?.Invoke($"Cannot load video in current state: {_stateMachine.CurrentState}");
                return;
            }

            if (string.IsNullOrEmpty(videoPath))
            {
                // 对于没有视频路径的配置，直接标记为完成
                if (_stateMachine.TryChangeState(VideoPlayerState.Completed))
                {
                    OnVideoCompleted?.Invoke();
                }
                return;
            }

            _currentVideoPath = videoPath;
            _shouldLoop = loop;

            if (!_stateMachine.TryChangeState(VideoPlayerState.Loading))
                return;

            _videoPlayer.Stop();
            VideoClip videoClip = ResourceMod.Instance.SyncLoad<VideoClip>(videoPath);
            if (videoClip == null)
            {
                _stateMachine.TryChangeState(VideoPlayerState.Error);
                OnErrorOccurred?.Invoke($"Video not found in Resources: {videoPath}");
                return;
            }

            _videoPlayer.source = VideoSource.VideoClip;
            _videoPlayer.clip = videoClip;
            _videoPlayer.isLooping = loop;

            // 准备视频
            if (_stateMachine.TryChangeState(VideoPlayerState.Preparing))
            {
                _videoPlayer.Prepare();
            }
        }

        public void Play()
        {
            if (!_stateMachine.CanPlay)
            {
                Debug.LogWarning($"Cannot play in current state: {_stateMachine.CurrentState}");
                return;
            }

            if (_stateMachine.TryChangeState(VideoPlayerState.Playing))
            {
                _videoPlayer.playbackSpeed = _normalSpeed;
                _videoPlayer.Play();
                Debug.Log($"Video playback started at time: {CurrentTime}");
            }
        }

        public void Pause()
        {
            if (!_stateMachine.CanPause)
            {
                Debug.LogWarning($"Cannot pause in current state: {_stateMachine.CurrentState}");
                return;
            }

            if (_stateMachine.TryChangeState(VideoPlayerState.Paused))
            {
                _videoPlayer.Pause();
                Debug.Log($"Video paused at time: {CurrentTime}");
            }
        }

        public void Stop()
        {
            if (!_stateMachine.CanStop)
            {
                Debug.LogWarning($"Cannot stop in current state: {_stateMachine.CurrentState}");
                return;
            }

            if (_stateMachine.TryChangeState(VideoPlayerState.Idle))
            {
                _videoPlayer.Stop();
                _currentVideoPath = null;
                Debug.Log("Video stopped and reset to idle state");
            }
        }

        public void FastForward(float seconds = 3.0f)
        {
            if (!_stateMachine.CanSeek) return;

            float targetTime = Mathf.Min(Duration, CurrentTime + Mathf.Abs(seconds));
            SeekInternal(targetTime);
        }

        public void Rewind(float seconds = -3.0f)
        {
            if (!_stateMachine.CanSeek) return;

            float targetTime = Mathf.Max(0, CurrentTime - Mathf.Abs(seconds));
            SeekInternal(targetTime);
        }

        private void SeekInternal(float targetTime)
        {
            if (_stateMachine.TryChangeState(VideoPlayerState.Seeking))
            {
                if (_seekingCoroutine != null)
                    CoroutineHelper.Instance.StopCoroutine(_seekingCoroutine);

                _seekingCoroutine = CoroutineHelper.Instance.StartCoroutine(PerformSeek(targetTime));
            }
        }

        private IEnumerator PerformSeek(float targetTime)
        {
            _videoPlayer.time = targetTime;

            // 等待跳转完成
            yield return new WaitForSeconds(0.1f);

            // 根据之前的播放状态决定跳转后的状态
            if (_videoPlayer.isPlaying)
            {
                _stateMachine.TryChangeState(VideoPlayerState.Playing);
            }
            else
            {
                _stateMachine.TryChangeState(VideoPlayerState.Paused);
            }

            _seekingCoroutine = null;
        }

        public void Seek(float time)
        {
            float clampedTime = Mathf.Clamp(time, 0, Duration);
            SeekInternal(clampedTime);
        }

        public void NextVideo()
        {
            Stop();
        }

        public void Restart()
        {
            if (_stateMachine.TryChangeState(VideoPlayerState.Seeking))
            {
                Seek(0);
                Play();
            }
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            // 视频准备完成，开始播放
            if (_stateMachine.TryChangeState(VideoPlayerState.Playing))
            {
                _videoPlayer.playbackSpeed = _normalSpeed;
                _videoPlayer.Play();
                Debug.Log($"Video prepared and started playing, length: {Duration}");
            }
        }

        private void OnSeekCompleted(VideoPlayer source)
        {
            // Unity的seekCompleted回调，这里处理跳转完成
            if (_stateMachine.CurrentState == VideoPlayerState.Seeking)
            {
                if (_videoPlayer.isPlaying)
                {
                    _stateMachine.TryChangeState(VideoPlayerState.Playing);
                }
                else
                {
                    _stateMachine.TryChangeState(VideoPlayerState.Paused);
                }
            }
        }

        private void OnVideoLoopPointReached(VideoPlayer source)
        {
            if (_shouldLoop)
            {
                Restart();
            }
            else
            {
                if (_stateMachine.TryChangeState(VideoPlayerState.Completed))
                {
                    OnVideoCompleted?.Invoke();
                }
            }
        }

        private void OnVideoError(VideoPlayer source, string message)
        {
            _stateMachine.TryChangeState(VideoPlayerState.Error);
            OnErrorOccurred?.Invoke($"Video Error: {message}");
        }

        public void Dispose()
        {
            if (_seekingCoroutine != null)
            {
                CoroutineHelper.Instance.StopCoroutine(_seekingCoroutine);
            }

            if (_videoPlayer != null)
            {
                _videoPlayer.Stop();
                _videoPlayer.prepareCompleted -= OnVideoPrepared;
                _videoPlayer.loopPointReached -= OnVideoLoopPointReached;
                _videoPlayer.errorReceived -= OnVideoError;
                _videoPlayer.seekCompleted -= OnSeekCompleted;
            }

            if (_stateMachine != null)
            {
                _stateMachine.OnStateChanged -= OnStateMachineStateChanged;
                _stateMachine.Reset();
            }

            if (_videoPlayerObject != null)
            {
                UnityEngine.Object.Destroy(_videoPlayerObject);
            }
        }
    }
}