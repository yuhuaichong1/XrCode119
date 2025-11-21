using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{
    public partial class UISetting : BaseUI
    {
        protected RectTransform mPlane;
        protected Button mExitBtn;
        protected Image mPlayerIcon;
        protected Text mUserNameText;
        protected Text mUserIDText;
        protected Text mUserLv;
        protected Button mUserLevelBtn;

        // 主音乐控件
        protected Toggle mMasterToggle;
        protected Slider mMasterSlider;
        protected RectTransform mMasterIcon;

        // 背景音乐控件
        protected Toggle mMusicToggle;
        protected Slider mMusicSlider;
        protected RectTransform mMusicIcon;

        // 音效控件
        protected Toggle mEffectsToggle;
        protected Slider mEffectsSlider;
        protected RectTransform mEffectsIcon;

        // 显示模式控件
        protected Toggle mDisplayToggle;

        protected Button mWRBtn;

        protected override void LoadPanel()
        {
            base.LoadPanel();

            mPlane = mTransform.Find("Plane").GetComponent<RectTransform>();
            mExitBtn = mTransform.Find("Plane/ExitBtn").GetComponent<Button>();
            mPlayerIcon = mTransform.Find("Plane/Bg2/PlayerIcon").GetComponent<Image>();
            mUserNameText = mTransform.Find("Plane/Bg2/UserNameText").GetComponent<Text>();
            mUserIDText = mTransform.Find("Plane/Bg2/UserIDText").GetComponent<Text>();
            mUserLv = mTransform.Find("Plane/Bg2/UserLv").GetComponent<Text>();
            mUserLevelBtn = mTransform.Find("Plane/Bg2/UserLevelBtn").GetComponent<Button>();

            // 主音乐控件
            mMasterToggle = mTransform.Find("Plane/MasterVolume/MasterToggle").GetComponent<Toggle>();
            mMasterSlider = mTransform.Find("Plane/MasterVolume/MasterSlider").GetComponent<Slider>();
            mMasterIcon = mTransform.Find("Plane/MasterVolume/MasterToggle/Background/MasterIcon").GetComponent<RectTransform>();

            // 背景音乐控件
            mMusicToggle = mTransform.Find("Plane/MusicVolume/MusicToggle").GetComponent<Toggle>();
            mMusicSlider = mTransform.Find("Plane/MusicVolume/MusicSlider").GetComponent<Slider>();
            mMusicIcon = mTransform.Find("Plane/MusicVolume/MusicToggle/Background/MusicIcon").GetComponent<RectTransform>();

            // 音效控件
            mEffectsToggle = mTransform.Find("Plane/EffectsVolume/EffectsToggle").GetComponent<Toggle>();
            mEffectsSlider = mTransform.Find("Plane/EffectsVolume/EffectsSlider").GetComponent<Slider>();
            mEffectsIcon = mTransform.Find("Plane/EffectsVolume/EffectsToggle/Background/EffectsIcon").GetComponent<RectTransform>();

            // 显示模式控件
            mDisplayToggle = mTransform.Find("Plane/DisplayMode/DisplayToggle").GetComponent<Toggle>();
       
        }

        protected override void BindButtonEvent()
        {
            mExitBtn.onClick.AddListener(OnExitBtnClickHandle);
            mUserLevelBtn.onClick.AddListener(OnUserLevelBtnClickHandle);
    

            // 绑定开关事件
            mMasterToggle.onValueChanged.AddListener(OnMasterToggleValueChange);
            mMusicToggle.onValueChanged.AddListener(OnMusicToggleValueChange);
            mEffectsToggle.onValueChanged.AddListener(OnEffectsToggleValueChange);
            mDisplayToggle.onValueChanged.AddListener(OnDisplayToggleValueChange);

            // 绑定滑动条事件
            mMasterSlider.onValueChanged.AddListener(OnMasterSliderValueChange);
            mMusicSlider.onValueChanged.AddListener(OnMusicSliderValueChange);
            mEffectsSlider.onValueChanged.AddListener(OnEffectsSliderValueChange);
        }

        protected override void UnBindButtonEvent()
        {
            mExitBtn.onClick.RemoveAllListeners();
            mUserLevelBtn.onClick.RemoveAllListeners();
   

            // 移除开关事件
            mMasterToggle.onValueChanged.RemoveAllListeners();
            mMusicToggle.onValueChanged.RemoveAllListeners();
            mEffectsToggle.onValueChanged.RemoveAllListeners();
            mDisplayToggle.onValueChanged.RemoveAllListeners();

            // 移除滑动条事件
            mMasterSlider.onValueChanged.RemoveAllListeners();
            mMusicSlider.onValueChanged.RemoveAllListeners();
            mEffectsSlider.onValueChanged.RemoveAllListeners();
        }
    }
}