using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{
    public partial class UIGameTest : BaseUI
    {	protected Text mName;	protected Text mStatusText;	protected Text mCurrenText;	protected Text mLoopstatus;	protected Button mStart;	protected Button mPause;	protected Button mBack;	protected Button mFastForward;	protected Button mNext;	protected Button mRestart;	protected Button mClose;	protected Button mSetting;
        protected override void LoadPanel()
        {
            base.LoadPanel();
            		mName = mTransform.Find("Plane/name").GetComponent<Text>();		mStatusText = mTransform.Find("Plane/statusText").GetComponent<Text>();		mCurrenText = mTransform.Find("Plane/currenText").GetComponent<Text>();		mLoopstatus = mTransform.Find("Plane/loopstatus").GetComponent<Text>();		mStart = mTransform.Find("Plane/root/start").GetComponent<Button>();		mPause = mTransform.Find("Plane/root/pause").GetComponent<Button>();		mBack = mTransform.Find("Plane/root/back").GetComponent<Button>();		mFastForward = mTransform.Find("Plane/root/fastForward").GetComponent<Button>();		mNext = mTransform.Find("Plane/root/next").GetComponent<Button>();		mRestart = mTransform.Find("Plane/root/restart").GetComponent<Button>();		mClose = mTransform.Find("Plane/Close").GetComponent<Button>();		mSetting = mTransform.Find("Plane/root/setting").GetComponent<Button>();
        }
    
        protected override void BindButtonEvent() 
        {
            		mStart.onClick.AddListener( OnStartClickHandle);		mPause.onClick.AddListener( OnPauseClickHandle);		mBack.onClick.AddListener( OnBackClickHandle);		mFastForward.onClick.AddListener( OnFastForwardClickHandle);		mNext.onClick.AddListener( OnNextClickHandle);		mRestart.onClick.AddListener( OnRestartClickHandle);		mClose.onClick.AddListener( OnCloseClickHandle);		mSetting.onClick.AddListener( OnSettingClickHandle);
        }

     

        protected override void UnBindButtonEvent() 
        {
            		mStart.onClick.RemoveAllListeners();		mPause.onClick.RemoveAllListeners();		mBack.onClick.RemoveAllListeners();		mFastForward.onClick.RemoveAllListeners();		mNext.onClick.RemoveAllListeners();		mRestart.onClick.RemoveAllListeners();		mClose.onClick.RemoveAllListeners();		mSetting.onClick.RemoveAllListeners();
        }
    
    }
}