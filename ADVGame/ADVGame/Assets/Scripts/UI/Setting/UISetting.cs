using System;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{

    public partial class UISetting : BaseUI
    {
        protected override void OnAwake()
        {

        }

        protected override void OnEnable()
        {
            // 获取当前设置
            float masterVolume = FacadeAudio.GetMasterVolume();
            float musicVolume = FacadeAudio.GetMusicVolume();
            float effectsVolume = FacadeAudio.GetEffectsVolume();
            bool isFullscreen = FacadeAudio.GetDisplayMode();

            // 设置开关状态（根据音量是否大于0）
            mMasterToggle.isOn = masterVolume > 0;
            mMusicToggle.isOn = musicVolume > 0;
            mEffectsToggle.isOn = effectsVolume > 0;
            mDisplayToggle.isOn = isFullscreen;

            // 设置滑动条值
            mMasterSlider.value = masterVolume;
            mMusicSlider.value = musicVolume;
            mEffectsSlider.value = effectsVolume;

            mUserNameText.text = $"{FacadePlayer.GetPlayerName()}";
            mUserIDText.text = $"{FacadeLanguage.GetText("10009")}:{FacadePlayer.GetPlayerID().Substring(0, 13)}...";

            ShowAnim(mPlane);
        }

        private void OnExitBtnClickHandle()
        {
            HideAnim(mPlane, () => {
                UIManager.Instance.CloseUI(EUIType.EUISetting);
            });
        }

        private void OnUserLevelBtnClickHandle()
        {
            HideAnim(mPlane, () => {
                UIManager.Instance.CloseUI(EUIType.EUISetting);
            });
        }

        private void OnWRBtnClickHandle()
        {
            HideAnim(mPlane, () => {
                UIManager.Instance.CloseUI(EUIType.EUISetting);
            });
        }

        // 主音乐开关值改变
        private void OnMasterToggleValueChange(bool b)
        {
            float volume = b ? (mMasterSlider.value > 0 ? mMasterSlider.value : 0.5f) : 0f;
            FacadeAudio.SetMasterVolume(volume);
            mMasterSlider.value = volume;
        }

        // 背景音乐开关值改变
        private void OnMusicToggleValueChange(bool b)
        {
            float volume = b ? (mMusicSlider.value > 0 ? mMusicSlider.value : 0.5f) : 0f;
            FacadeAudio.SetMusicVolume(volume);
            mMusicSlider.value = volume;
        }

        // 音效开关值改变
        private void OnEffectsToggleValueChange(bool b)
        {
            float volume = b ? (mEffectsSlider.value > 0 ? mEffectsSlider.value : 0.5f) : 0f;
            FacadeAudio.SetEffectsVolume(volume);

  
            mEffectsSlider.value = volume;
        }


        private void OnDisplayToggleValueChange(bool b)
        {
            FacadeAudio.SetDisplayMode(b);
        }

        // 主音量滑动条值改变
        private void OnMasterSliderValueChange(float value)
        {
            FacadeAudio.SetMasterVolume(value);

          
            if (value > 0 && !mMasterToggle.isOn)
            {
                mMasterToggle.isOn = true;
            }

            else if (value == 0 && mMasterToggle.isOn)
            {
                mMasterToggle.isOn = false;
            }
        }

        // 背景音乐滑动条值改变
        private void OnMusicSliderValueChange(float value)
        {
            FacadeAudio.SetMusicVolume(value);

      
            if (value > 0 && !mMusicToggle.isOn)
            {
                mMusicToggle.isOn = true;
            }
   
            else if (value == 0 && mMusicToggle.isOn)
            {
                mMusicToggle.isOn = false;
            }
        }

    
        private void OnEffectsSliderValueChange(float value)
        {
            FacadeAudio.SetEffectsVolume(value);

        
            if (value > 0 && !mEffectsToggle.isOn)
            {
                mEffectsToggle.isOn = true;
            }
            
            else if (value == 0 && mEffectsToggle.isOn)
            {
                mEffectsToggle.isOn = false;
            }
        }

        protected override void OnDisable()
        {

        }

        protected override void OnDispose()
        {

        }
    }
}