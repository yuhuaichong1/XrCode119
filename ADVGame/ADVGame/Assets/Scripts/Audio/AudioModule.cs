using cfg;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XrCode
{
    public class AudioModule : BaseModule
    {
        private AudioSource musicSource;
        private List<AudioSource> effectSources;
        private Dictionary<int, AudioClip> clipMap;
        private bool isPlayBgming;
        private GameObject gameObject;

        // 音量设置
        private float masterVolume = 1.0f;
        private float musicVolume = 0.45f;
        private float effectsVolume = 0.6f;
        private float mCoolDownDur = 0.05f;
        private bool enableBtn = true;
        private bool isFullscreen = true;

        protected override void OnLoad()
        {
            FacadeAudio.PlayBgm += PlayBgm;
            FacadeAudio.StopBgm += StopBgm;
            FacadeAudio.PlayEffect += PlayEffect;
            FacadeAudio.SetMusicVolume += SetMusicVolume;
            FacadeAudio.SetEffectsVolume += SetEffectsVolume;
            FacadeAudio.GetMusicVolume += GetMusicVolume;
            FacadeAudio.GetEffectsVolume += GetEffectsVolume;
            FacadeAudio.GetEATypeByString += GetEATypeByString;

            // 新增的事件
            FacadeAudio.SetMasterVolume += SetMasterVolume;
            FacadeAudio.GetMasterVolume += GetMasterVolume;
            FacadeAudio.SetDisplayMode += SetDisplayMode;
            FacadeAudio.GetDisplayMode += GetDisplayMode;

            gameObject = new GameObject("AudioModule");
            GameObject.DontDestroyOnLoad(gameObject);
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            effectSources = new List<AudioSource>();
            clipMap = new Dictionary<int, AudioClip>();
            RedirectButton();

            LoadData();

            PlayBgm();
        }

        private void LoadData()
        {
            // 加载音量设置
            masterVolume = SPlayerPrefs.GetFloat("MasterVolume", 1.0f);
            musicVolume = SPlayerPrefs.GetFloat("MusicVolume", 0.45f);
            effectsVolume = SPlayerPrefs.GetFloat("EffectsVolume", 0.6f);
            isFullscreen = SPlayerPrefs.GetBool("DisplayMode", true);

            // 应用显示模式
            ApplyDisplayMode();

            // 更新所有音量
            UpdateAllVolumes();
        }

        // 统一重定向按钮事件
        private void RedirectButton()
        {
            typeof(ExecuteEvents).GetField("s_PointerClickHandler", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, new ExecuteEvents.EventFunction<IPointerClickHandler>(OnPointerClick));
            typeof(ExecuteEvents).GetField("s_PointerDownHandler", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, new ExecuteEvents.EventFunction<IPointerDownHandler>(OnPointerDown));
            typeof(ExecuteEvents).GetField("s_PointerUpHandler", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, new ExecuteEvents.EventFunction<IPointerUpHandler>(OnPointerUp));
        }

        void OnPointerClick(IPointerClickHandler handler, BaseEventData eventData)
        {
            PointerEventData pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            if (pointerEventData != null)
            {
                if (!enableBtn) return;

                if (eventData.selectedObject == null)
                {
                    PlayButtonSound();
                }

                handler.OnPointerClick(pointerEventData);
                enableBtn = false;
                TimerManager.Instance.CreateTimer(mCoolDownDur, () => { enableBtn = true; }, 0);
            }
        }

        public void OnPointerDown(IPointerDownHandler handler, BaseEventData eventData)
        {
            PointerEventData pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            if (pointerEventData != null)
            {
                if (pointerEventData.pointerPressRaycast.gameObject != null)
                {
                    GameObject obj = pointerEventData.pointerPressRaycast.gameObject;
                    if (obj.transform.tag != "NoUpDownAnim" && obj.transform.GetComponent<Button>() != null)
                    {
                        obj.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
                    }
                }
                handler.OnPointerDown(pointerEventData);
            }
        }

        public void OnPointerUp(IPointerUpHandler handler, BaseEventData eventData)
        {
            PointerEventData pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            if (pointerEventData != null)
            {
                if (eventData.selectedObject != null)
                {
                    GameObject obj = eventData.selectedObject;
                    if (obj.transform.tag != "NoUpDownAnim" && obj.transform.GetComponent<Button>() != null)
                    {
                        obj.transform.DOScale(new Vector3(1, 1, 1), 0.2f);
                    }
                }
                handler.OnPointerUp(pointerEventData);
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            FacadeAudio.PlayBgm -= PlayBgm;
            FacadeAudio.StopBgm -= StopBgm;
            FacadeAudio.PlayEffect -= PlayEffect;
            FacadeAudio.SetMusicVolume -= SetMusicVolume;
            FacadeAudio.SetEffectsVolume -= SetEffectsVolume;
            FacadeAudio.GetMusicVolume -= GetMusicVolume;
            FacadeAudio.GetEffectsVolume -= GetEffectsVolume;
            FacadeAudio.GetEATypeByString -= GetEATypeByString;

            // 移除新增的事件
            FacadeAudio.SetMasterVolume -= SetMasterVolume;
            FacadeAudio.GetMasterVolume -= GetMasterVolume;
            FacadeAudio.SetDisplayMode -= SetDisplayMode;
            FacadeAudio.GetDisplayMode -= GetDisplayMode;

            gameObject = null;
            musicSource = null;

            StopBgm();

            if (effectSources != null)
            {
                for (int i = 0; i < effectSources.Count; i++)
                {
                    effectSources[i].clip = null;
                }
                effectSources.Clear();
            }

            clipMap.Clear();
            clipMap = null;
        }

        // 获取音频片段
        private AudioClip GetAudioClip(EAudioType aType)
        {
            int audioId = (int)aType;
            if (!clipMap.TryGetValue(audioId, out AudioClip clip))
            {
                ConfAudio conf = ConfigModule.Instance.Tables.TBAudio.Get(audioId);
                if (conf == null) return null;
                clip = ResourceMod.Instance.SyncLoad<AudioClip>(conf.AudioPath);
                clipMap.Add(audioId, clip);
            }
            return clip;
        }

        private void PlayButtonSound()
        {
            D.Log("统一播放按钮音效");
            AudioClip clip = GetAudioClip(EAudioType.EButton);
            if (clip == null) return;
            AudioSource source = GetAvailableAudioSource();
            source.clip = clip;
            source.volume = effectsVolume * masterVolume;
            source.Play();
        }

        // 播放背景音乐
        private void PlayBgm()
        {
            if (isPlayBgming) return;
            isPlayBgming = true;
            AudioClip bgm = GetAudioClip(EAudioType.EBgm);
            if (bgm == null) return;

            musicSource.clip = bgm;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        // 停止背景音乐
        private void StopBgm()
        {
            if (!isPlayBgming) return;
            isPlayBgming = false;
            if (musicSource != null)
                musicSource.Stop();
        }

        // 播放音效
        private void PlayEffect(EAudioType aType)
        {
            AudioClip clip = GetAudioClip(aType);
            if (clip == null) return;
            AudioSource source = GetAvailableAudioSource();
            source.clip = clip;
            source.volume = effectsVolume * masterVolume;
            source.Play();
        }

        // 获取可用的音频源
        private AudioSource GetAvailableAudioSource()
        {
            AudioSource source = effectSources.Find(s => !s.isPlaying);
            if (source == null)
            {
                source = gameObject.AddComponent<AudioSource>();
                effectSources.Add(source);
            }
            return source;
        }

        // 设置主音量
        private void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();

            SPlayerPrefs.SetFloat("MasterVolume", masterVolume);
            SPlayerPrefs.Save();
        }

        // 设置背景音乐音量
        private void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();

            SPlayerPrefs.SetFloat("MusicVolume", musicVolume);
            SPlayerPrefs.Save();
        }

        // 设置音效音量
        private void SetEffectsVolume(float volume)
        {
            effectsVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();

            SPlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
            SPlayerPrefs.Save();
        }

        // 更新所有音量
        private void UpdateAllVolumes()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;

            foreach (var source in effectSources)
            {
                source.volume = effectsVolume * masterVolume;
            }
        }

        // 设置显示模式
        private void SetDisplayMode(bool fullscreen)
        {
            isFullscreen = fullscreen;
            ApplyDisplayMode();

            SPlayerPrefs.SetBool("DisplayMode", isFullscreen);
            SPlayerPrefs.Save();
        }

        // 应用显示模式
        private void ApplyDisplayMode()
        {
            Screen.fullScreen = isFullscreen;
            if (!isFullscreen)
            {
                Screen.SetResolution(1280, 720, false);
            }
        }

        private float GetMusicVolume()
        {
            return musicVolume;
        }

        private float GetEffectsVolume()
        {
            return effectsVolume;
        }

        // 获取主音量
        private float GetMasterVolume()
        {
            return masterVolume;
        }

        // 获取显示模式
        private bool GetDisplayMode()
        {
            return isFullscreen;
        }

        private EAudioType GetEATypeByString(string str)
        {
            switch (str)
            {
                case "merge_1":
                    return EAudioType.EMerge_1;
                case "merge_2":
                    return EAudioType.EMerge_2;
                case "merge_3":
                    return EAudioType.EMerge_3;
                case "merge_4":
                    return EAudioType.EMerge_4;
                default:
                    return EAudioType.EMerge_1;
            }
        }
    }
}