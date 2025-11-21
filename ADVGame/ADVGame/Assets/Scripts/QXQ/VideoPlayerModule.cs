using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XrCode;

public class VideoPlayerModule : BaseModule
{
    private IVideoPlayerService _videoPlayerService;
    private List<ConfStoryline> _videoList;
    private int _currentVideoIndex = 0;
    private ConfStoryline _currentVideoConfig;
    private Coroutine _progressUpdateCoroutine;
    private Dictionary<int, ConfStoryline> _videoConfigDict;

    #region 生命周期管理
    protected override void OnLoad()
    {
        base.OnLoad();
        InitializeVideoPlayer();
        SubscribeToEvents();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        Cleanup();
        UnsubscribeFromEvents();
    }
    #endregion

    #region 初始化与清理
    private void InitializeVideoPlayer()
    {
        _videoPlayerService = new VideoPlayerService();
        _videoPlayerService.OnVideoCompleted += OnVideoCompleted;
        _videoPlayerService.OnErrorOccurred += OnVideoError;
        _videoPlayerService.OnStateChanged += OnVideoStateChanged;

        LoadVideoConfigs();
    }

    private void LoadVideoConfigs()
    {
        _videoList = ConfigModule.Instance.Tables.TBStoryline.DataList;
        _videoConfigDict = new Dictionary<int, ConfStoryline>();

        foreach (var config in _videoList)
        {
            if (config.Sn >= 0)
            {
                _videoConfigDict[config.Sn] = config;
            }
        }

        Debug.Log($"Loaded {_videoList.Count} videos from config");
    }

    private void Cleanup()
    {
        StopProgressUpdate();
        ClearAllVideoInfo();

        if (_videoPlayerService != null)
        {
            _videoPlayerService.OnVideoCompleted -= OnVideoCompleted;
            _videoPlayerService.OnErrorOccurred -= OnVideoError;
            _videoPlayerService.OnStateChanged -= OnVideoStateChanged;
            _videoPlayerService.Dispose();
            _videoPlayerService = null;
        }
    }
    #endregion

    #region 事件订阅管理
    private void SubscribeToEvents()
    {
        VideoPlayerEvent.VideoPlay += OnVideoPlay;
        VideoPlayerEvent.SelectVideoPlay += OnSelectionMade;
        VideoPlayerEvent.VideoPlayIndex += PlayVideoByIndex;
        VideoPlayerEvent.VideoNext += NextVideo;
        VideoPlayerEvent.VideoPause += PauseVideo;
        VideoPlayerEvent.VideoFastForward += FastForwardVideo;
        VideoPlayerEvent.VideoRewind += RewindVideo;
        VideoPlayerEvent.VideoRestart += RestartCurrentVideo;
    }

    private void UnsubscribeFromEvents()
    {
        VideoPlayerEvent.VideoPlay -= OnVideoPlay;
        VideoPlayerEvent.SelectVideoPlay -= OnSelectionMade;
        VideoPlayerEvent.VideoPlayIndex -= PlayVideoByIndex;
        VideoPlayerEvent.VideoNext -= NextVideo;
        VideoPlayerEvent.VideoPause -= PauseVideo;
        VideoPlayerEvent.VideoFastForward -= FastForwardVideo;
        VideoPlayerEvent.VideoRewind -= RewindVideo;
        VideoPlayerEvent.VideoRestart -= RestartCurrentVideo;
    }
    #endregion

    #region 视频播放控制
    private void OnVideoPlay()
    {
        if (_videoPlayerService.CurrentState == VideoPlayerState.Paused)
        {
            _videoPlayerService.Play();
        }
        else
        {
            PlayVideoByIndex(_currentVideoIndex);
        }
    }

    private void PlayVideoByIndex(int index)
    {
        if (_videoList == null || index < 0 || index >= _videoList.Count) return;
        PlayVideoByConfig(_videoList[index]);
    }

    private void PlayVideoBySn(int sn)
    {
        if (_videoConfigDict.TryGetValue(sn, out ConfStoryline config))
        {
            PlayVideoByConfig(config);
        }
        else
        {
            Debug.LogError($"Video config not found for SN: {sn}");
        }
    }

    private void PlayVideoByConfig(ConfStoryline config)
    {
        _videoPlayerService.Stop();
        StopProgressUpdate();

        _currentVideoConfig = config;
        _currentVideoIndex = GetIndexBySn(config.Sn);

        Debug.Log($"Playing video: {config.Name} | Path: {config.VideoPath} | Loop: {config.IsLoop} | Type: {config.Type}");

        UpdateStaticVideoInfo();

        if (!string.IsNullOrEmpty(config.VideoPath))
        {
            _videoPlayerService.PlayVideo(config.VideoPath, config.IsLoop);
        }
        else
        {
            Debug.Log($"No video path for {config.Name}, handling next video directly");
            UpdateStatusText(VideoPlayerState.Completed);
        }
    }

    private void PauseVideo()
    {
        _videoPlayerService?.Pause();
    }

    private void FastForwardVideo()
    {
        _videoPlayerService?.FastForward(3.0f);
    }

    private void RewindVideo()
    {
        _videoPlayerService?.Rewind(-3.0f);
    }

    public void NextVideo()
    {
        HandleNextVideo();
    }

    public void RestartCurrentVideo()
    {
        _videoPlayerService?.Restart();
    }

    public void StopVideo()
    {
        _videoPlayerService?.Stop();
        ClearAllVideoInfo();
    }
    #endregion

    #region 视频播放逻辑处理
    private void OnVideoCompleted()
    {
        Debug.Log("Video completed");
        StopProgressUpdate();
        HandleNextVideo();
    }

    private void HandleNextVideo()
    {
        if (_currentVideoConfig == null) return;
        int nextIndex = _currentVideoIndex + 1;
        if (nextIndex < 0 || nextIndex >= _videoList.Count)
        {
            Debug.Log("No next video available, reached the end");
            ClearAllVideoInfo();
            return;
        }
        ConfStoryline nextVideoConfig = _videoList[nextIndex];
        switch (nextVideoConfig.Type)
        {
            case 1:
                PlayVideoByConfig(nextVideoConfig);
                ShowSelectionPanel();
                break;
            default:
                PlayVideoByConfig(nextVideoConfig);
                break;
        }
    }

    private void PlayNextSequentialVideo()
    {
        int nextIndex = _currentVideoIndex + 1;
        if (nextIndex >= 0 && nextIndex < _videoList.Count)
        {
            PlayVideoByConfig(_videoList[nextIndex]);
        }
        else
        {
            Debug.Log("No next video available, reached the end");
            ClearAllVideoInfo();
        }
    }

    private void OnSelectionMade(int selectedSn)
    {
        PlayVideoBySn(selectedSn);
    }

    private void ShowSelectionPanel()
    {
        if (_currentVideoConfig == null || string.IsNullOrEmpty(_currentVideoConfig.NextTip))
            return;

        var availableChoices = ParseNextTips(_currentVideoConfig.NextTip);
        if (availableChoices.Count > 0)
        {
            UIManager.Instance.OpenAsync<UISelectionPanel>(EUIType.EUIUISelectionPanel, null, availableChoices);
        }
    }

    private List<int> ParseNextTips(string nextTip)
    {
        var choices = new List<int>();
        string[] nextTips = nextTip.Split(',');

        foreach (string tip in nextTips)
        {
            if (int.TryParse(tip.Trim(), out int nextSn))
            {
                choices.Add(nextSn);
            }
        }

        return choices;
    }
    #endregion

    #region 进度更新管理
    private void OnVideoStateChanged(VideoPlayerState previous, VideoPlayerState current)
    {
        UpdateStatusText(current);

        switch (current)
        {
            case VideoPlayerState.Playing:
                StartProgressUpdate();
                break;
            case VideoPlayerState.Paused:
            case VideoPlayerState.Completed:
            case VideoPlayerState.Error:
                StopProgressUpdate();
                UpdateProgressInfo();
                break;
            case VideoPlayerState.Idle:
                StopProgressUpdate();
                // Idle 状态不调用 UpdateProgressInfo()，直接清空所有信息
                ClearAllVideoInfo();
                break;
            case VideoPlayerState.Loading:
            case VideoPlayerState.Preparing:
            case VideoPlayerState.Seeking:
                // 这些状态只需要更新状态文本
                break;
        }
    }

    private void UpdateStatusText(VideoPlayerState state)
    {
        string statusText = GetStatusText(state);
        VideoPlayerEvent.OnVideoStatusChanged?.Invoke(statusText);
    }

    private string GetStatusText(VideoPlayerState state)
    {
        switch (state)
        {
            case VideoPlayerState.Idle:
                return "准备就绪";
            case VideoPlayerState.Loading:
                return "加载中...";
            case VideoPlayerState.Preparing:
                return "准备播放...";
            case VideoPlayerState.Playing:
                return "播放中";
            case VideoPlayerState.Paused:
                return "已暂停";
            case VideoPlayerState.Seeking:
                return "跳转中...";
            case VideoPlayerState.Completed:
                return "播放完成";
            case VideoPlayerState.Error:
                return "播放错误";
            default:
                return state.ToString();
        }
    }

    private void StartProgressUpdate()
    {
        StopProgressUpdate();
        _progressUpdateCoroutine = CoroutineHelper.Instance.StartCoroutine(ProgressUpdateRoutine());
    }

    private void StopProgressUpdate()
    {
        if (_progressUpdateCoroutine != null)
        {
            CoroutineHelper.Instance.StopCoroutine(_progressUpdateCoroutine);
            _progressUpdateCoroutine = null;
        }
    }

    private IEnumerator ProgressUpdateRoutine()
    {
        while (_videoPlayerService != null && _videoPlayerService.IsPlaying)
        {
            UpdateProgressInfo();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateProgressInfo()
    {
        if (_videoPlayerService == null) return;

        string currentTime = FormatTime(_videoPlayerService.CurrentTime);
        string duration = FormatTime(_videoPlayerService.Duration);
        float progress = _videoPlayerService.Progress * 100f;

        VideoPlayerEvent.OnVideoProgressUpdated?.Invoke(currentTime, duration, progress);
    }
    #endregion

    #region 工具方法
    private void UpdateStaticVideoInfo()
    {
        if (_currentVideoConfig == null) return;

        string videoName = _currentVideoConfig.Name;
        string duration = FormatTime(_videoPlayerService?.Duration ?? 0);
        bool isLooping = _currentVideoConfig.IsLoop;

        VideoPlayerEvent.OnVideoInfoUpdated?.Invoke(videoName, duration, isLooping);
    }

    private void ClearAllVideoInfo()
    {
        VideoPlayerEvent.OnVideoInfoUpdated?.Invoke("", "00:00", false);
        VideoPlayerEvent.OnVideoProgressUpdated?.Invoke("00:00", "00:00", 0f);
    }

    private int GetIndexBySn(int sn)
    {
        for (int i = 0; i < _videoList.Count; i++)
        {
            if (_videoList[i].Sn == sn)
                return i;
        }
        return 0;
    }

    private string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds <= 0) return "00:00";

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return timeSpan.Hours > 0
            ? string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds)
            : string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void OnVideoError(string error)
    {
        Debug.LogError($"Video Player Error: {error}");
        StopProgressUpdate();
        UpdateStatusText(VideoPlayerState.Error);
    }
    #endregion
}