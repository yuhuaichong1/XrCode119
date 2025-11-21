
using System;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{
    public partial class UIGameTest : BaseUI
    {
        protected override void OnAwake() { }

        protected override void OnEnable()
        {
      
            VideoPlayerEvent.OnVideoInfoUpdated += OnVideoInfoUpdated;
            VideoPlayerEvent.OnVideoStatusChanged += OnVideoStatusChanged;
            VideoPlayerEvent.OnVideoProgressUpdated += OnVideoProgressUpdated;
        }


        private void OnVideoInfoUpdated(string videoName, string duration, bool isLooping)
        {
            if (mName != null)
                mName.text = $"视频: {videoName}";

            if (mLoopstatus != null)
                mLoopstatus.text = $"循环: {(isLooping ? "是" : "否")}";
        }
        private void OnVideoStatusChanged(string status)
        {
            if (mStatusText != null) mStatusText.text = $"状态: {status}";
        }

    
        private void OnVideoProgressUpdated(string currentTime, string duration, float progress)
        {
            if (mCurrenText != null) mCurrenText.text = $"进度: {currentTime} / {duration} ({progress:F1}%)";
        }

        private void OnRestartClickHandle()
        {
            VideoPlayerEvent.VideoRestart();
        }

        private void OnNextClickHandle()
        {
            VideoPlayerEvent.VideoNext();
        }

        private void OnBackClickHandle()
        {
            VideoPlayerEvent.VideoRewind();
        }

        private void OnFastForwardClickHandle()
        {
            VideoPlayerEvent.VideoFastForward();
        }

        private void OnPauseClickHandle()
        {
            VideoPlayerEvent.VideoPause();
        }

        private void OnStartClickHandle()
        {
            VideoPlayerEvent.VideoPlay();
        }
    
        private void OnSettingClickHandle()
        {
            UIManager.Instance.OpenSync<UISetting>(EUIType.EUISetting);
        }

        private void OnCloseClickHandle()
        {
            UIManager.Instance.CloseUI(EUIType.EUIUIGameTest);
        }
        protected  void OnDestroy()
        {
       
            VideoPlayerEvent.OnVideoInfoUpdated -= OnVideoInfoUpdated;
            VideoPlayerEvent.OnVideoStatusChanged -= OnVideoStatusChanged;
            VideoPlayerEvent.OnVideoProgressUpdated -= OnVideoProgressUpdated;
        }
    }
}