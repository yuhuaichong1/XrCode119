
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
    private Dictionary<int, ConfStoryline> _videoConfigDict; // 通过sn快速查找配置

    protected override void OnLoad()
    {
        base.OnLoad();
        InitializeVideoPlayer();

        VideoPlayerEvent.VideoPlay += OnVideoPlay;
        VideoPlayerEvent.VideoPlayIndex += PlayVideoByIndex;
        VideoPlayerEvent.VideoNext += NextVideo;
        VideoPlayerEvent.VideoPause += PauseVideo;
        VideoPlayerEvent.VideoFastForward += FastForwardVideo;
        VideoPlayerEvent.VideoRewind += RewindVideo;
        VideoPlayerEvent.VideoRestart += RestartCurrentVideo;
    }

    private void InitializeVideoPlayer()
    {
        _videoPlayerService = new VideoPlayerService();
        _videoPlayerService.OnVideoCompleted += OnVideoCompleted;
        _videoPlayerService.OnErrorOccurred += OnVideoError;
        _videoPlayerService.OnStateChanged += OnVideoStateChanged;

        _videoList = ConfigModule.Instance.Tables.TBStoryline.DataList;
        _videoConfigDict = new Dictionary<int, ConfStoryline>();

        // 构建字典便于快速查找
        foreach (var config in _videoList)
        {
            if (config.Sn >= 0) // 只添加有效的配置
            {
                _videoConfigDict[config.Sn] = config;
            }
        }

        Debug.Log($"Loaded {_videoList.Count} videos from config");
    }

    private void OnVideoCompleted()
    {
        Debug.Log("Video completed");
        StopProgressUpdate();

        // 根据当前视频的nextTip决定下一个视频
        HandleNextVideo();
    }

    private void HandleNextVideo()
    {
        if (_currentVideoConfig == null) return;
        int nextIndex = _currentVideoIndex + 1;
        if (nextIndex >= 0 && nextIndex < _videoList.Count)
        {
            ConfStoryline nextVideoConfig = _videoList[nextIndex];
            PlayVideoByConfig(nextVideoConfig);
            if (nextVideoConfig.Type == 1)
            {
               //ShowSelectionPanel();
            }
        }
        else
        {
            Debug.Log("No next video available, reached the end");
        }
    }

    private void ShowSelectionPanel()
    {
        if (_currentVideoConfig == null || string.IsNullOrEmpty(_currentVideoConfig.NextTip))
            return;
        string[] nextTips = _currentVideoConfig.NextTip.Split(',');
        List<int> availableChoices = new List<int>();
        foreach (string tip in nextTips)
        {
            if (int.TryParse(tip.Trim(), out int nextSn))
            {
                availableChoices.Add(nextSn);
            }
        }
        if (availableChoices.Count > 0)
        {
            //UISelectionPanel.OnSelectionMade += OnSelectionMade;
            UIManager.Instance.OpenAsync<UISelectionPanel>(EUIType.EUIUISelectionPanel, null, availableChoices);
        }
    }

    private void OnSelectionMade(int selectedSn)
    {
        PlayVideoBySn(selectedSn);
    }

    private void PlayNextVideoByTip()
    {
        if (_currentVideoConfig == null || string.IsNullOrEmpty(_currentVideoConfig.NextTip))
        {
            return;
        }
        string[] nextTips = _currentVideoConfig.NextTip.Split(',');// 解析nextTip，取第一个作为默认下一个
        if (nextTips.Length > 0 && int.TryParse(nextTips[0].Trim(), out int nextSn))
        {
            PlayVideoBySn(nextSn);
        }
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
            //HandleNextVideo();
        }
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

    private void OnVideoError(string error)
    {
        Debug.LogError($"Video Player Error: {error}");
        StopProgressUpdate();
        VideoPlayerEvent.OnVideoStatusChanged?.Invoke("Error");
    }

    private void OnVideoStateChanged(VideoPlayerState previous, VideoPlayerState current)
    {
        VideoPlayerEvent.OnVideoStatusChanged?.Invoke(current.ToString());
        if (current == VideoPlayerState.Playing)
        {
            StartProgressUpdate();
        }
        else if (current == VideoPlayerState.Paused || current == VideoPlayerState.Completed || current == VideoPlayerState.Error)
        {
            StopProgressUpdate();
            UpdateProgressInfo();
        }
    }

    #region 进度更新控制
    private void StartProgressUpdate()
    {
        StopProgressUpdate();
        _progressUpdateCoroutine = Game.Instance.StartCoroutine(ProgressUpdateRoutine());
    }

    private void StopProgressUpdate()
    {
        if (_progressUpdateCoroutine != null)
        {
            Game.Instance.StopCoroutine(_progressUpdateCoroutine);
            _progressUpdateCoroutine = null;
        }
    }

    private IEnumerator ProgressUpdateRoutine()
    {
        while (_videoPlayerService != null && _videoPlayerService.IsPlaying)
        {
            UpdateProgressInfo();
            yield return new WaitForSeconds(0.1f); // 每0.1秒更新一次进度
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

    #region 视频控制方法
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

    private void UpdateStaticVideoInfo()
    {
        if (_currentVideoConfig == null) return;

        string videoName = _currentVideoConfig.Name;
        string duration = FormatTime(_videoPlayerService?.Duration ?? 0);
        bool isLooping = _currentVideoConfig.IsLoop;

        VideoPlayerEvent.OnVideoInfoUpdated?.Invoke(videoName, duration, isLooping);
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
        // 修改：不再自动播放下一个，而是根据nextTip逻辑
        HandleNextVideo();
    }

    public void RestartCurrentVideo()
    {
        _videoPlayerService?.Restart();
    }

    // 格式化时间方法
    private string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds <= 0) return "00:00";

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        if (timeSpan.Hours > 0)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
    #endregion

    protected override void OnDispose()
    {
        base.OnDispose();
        StopProgressUpdate();

        if (_videoPlayerService != null)
        {
            _videoPlayerService.OnVideoCompleted -= OnVideoCompleted;
            _videoPlayerService.OnErrorOccurred -= OnVideoError;
            _videoPlayerService.OnStateChanged -= OnVideoStateChanged;
            _videoPlayerService.Dispose();
        }

        VideoPlayerEvent.VideoPlay -= OnVideoPlay;
        VideoPlayerEvent.VideoPlayIndex -= PlayVideoByIndex;
        VideoPlayerEvent.VideoNext -= NextVideo;
        VideoPlayerEvent.VideoPause -= PauseVideo;
        VideoPlayerEvent.VideoFastForward -= FastForwardVideo;
        VideoPlayerEvent.VideoRewind -= RewindVideo;
        VideoPlayerEvent.VideoRestart -= RestartCurrentVideo;
    }
}