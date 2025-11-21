using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XrCode;

public class TimePoint : MonoBehaviour
{
    public int PrecedingTP;//上一时间点Id
    public int TPId;//当前时间点Id
    public List<int> nextTPs = new List<int>();//后继时间点Id
    public TPStatus TPStatus;//当前时间点状态
    [Space]
    public RectTransform Entrance;//线图入口
    public RectTransform Export;//线图出口
    public Button TPBtn;//点击按钮
    public GameObject Bg_InLine;//进线背景
    public GameObject PlayingBg;//进行中背景

    void Awake()
    {
        if (TPBtn != null)
            TPBtn.onClick.AddListener(OnTPBtnClick);
    }

    protected virtual void UnlockTP()
    {
        if(Bg_InLine != null)
            Bg_InLine.gameObject.SetActive(true);
        if(TPBtn != null)
            TPBtn.gameObject.SetActive(true);
    }

    protected virtual void LockTP()
    {
        if (Bg_InLine != null)
            Bg_InLine.gameObject.SetActive(false);
        if (TPBtn != null)
            TPBtn.gameObject.SetActive(false);
    }

    protected virtual void SetPlaying(bool b)
    {
        if(PlayingBg != null)
            PlayingBg.gameObject.SetActive(b);
        if(TPBtn!= null)
            TPBtn.gameObject.SetActive(!b);
    }

    public void OnTPBtnClick()
    {
        D.Log("播放：" + TPId);

        UIManager.Instance.CloseUI(EUIType.EUITimeLine);
        UIManager.Instance.OpenAsync<UIGameTest>(EUIType.EUIUIGameTest, (BaseUI) => 
        {
            VideoPlayerEvent.SelectVideoPlay(TPId);
        });
    }

    void OnDestroy()
    {
        if(TPBtn != null)
            TPBtn.onClick.RemoveAllListeners();
    }
}
