using System;

public static class VideoPlayerEvent
{
    public static Action VideoPlay;
    public static Action<int> VideoPlayIndex;
    public static Action VideoPause;
    public static Action VideoFastForward;
    public static Action VideoRewind;
    public static Action VideoNext;
    public static Action VideoRestart;


    public static Action<string, string, bool> OnVideoInfoUpdated; // 视频名称，总时长，循环状态（不需要实时更新）
    public static Action<string> OnVideoStatusChanged; // 播放状态变化
    public static Action<string, string, float> OnVideoProgressUpdated; // 当前时间，总时长，进度百分比（需要实时更新）
}