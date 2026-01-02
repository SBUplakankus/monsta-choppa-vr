using System;
using Attributes;
using Constants;
using Databases;
using Events;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        #region Fields

        [Header("Audio Sources")] 
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource uiSfxSource;

        [Header("Audio Mixer")] 
        [SerializeField] private AudioMixer audioMixer;
        
        [Header("Audio Settings")]
        [SerializeField] private FloatAttribute masterVolume;
        [SerializeField] private FloatAttribute musicVolume;
        [SerializeField] private FloatAttribute ambienceVolume;
        [SerializeField] private FloatAttribute sfxVolume;
        [SerializeField] private FloatAttribute uiVolume;
        
        [Header("Audio Database")]
        [SerializeField] private AudioClipDatabase audioDatabase;

        [Header("Audio Events")] 
        [SerializeField] private StringEventChannel onUISfxRequested;
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onAmbienceRequested;

        #endregion

        #region Methods
        
        private AudioClip GetClip(string key)
        {
            var entry = audioDatabase.Get(key);
            if (entry != null) return entry.Clip;
            Debug.LogWarning($"AudioController: Audio clip not found for key '{key}'");
            return null;

        }
        
        private void PlayMusic(string key)
        {
            if (audioDatabase == null || musicSource == null) return;

            var clip = GetClip(key);
            if (clip == null) return;

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void PlayAmbience(string key)
        {
            if (audioDatabase == null || ambienceSource == null) return;
            var clip = GetClip(key);
            
            if (clip == null) return;
            ambienceSource.clip = clip;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }

        private void PlaySfx(string key)
        {
            if (audioDatabase == null || uiSfxSource == null) return;

            var clip = GetClip(key);
            if (clip == null) return;

            uiSfxSource.PlayOneShot(clip);
        }
        
        private static float ToDecibels(float value) => Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        
        private void SetMasterVolume(float value) => audioMixer.SetFloat(AudioKeys.MixerMaster, ToDecibels(value));
        private void SetMusicVolume(float value) => audioMixer.SetFloat(AudioKeys.MixerMusic, ToDecibels(value));
        private void SetAmbienceVolume(float value) => audioMixer.SetFloat(AudioKeys.MixerAmbience, ToDecibels(value));
        private void SetSfxVolume(float value) => audioMixer.SetFloat(AudioKeys.MixerSfx, ToDecibels(value));
        private void SetUIVolume(float value) => audioMixer.SetFloat(AudioKeys.MixerUI, ToDecibels(value));

        private void SubscribeEvents()
        {
            onUISfxRequested.Subscribe(PlaySfx);
            onMusicRequested.Subscribe(PlayMusic);
            onAmbienceRequested.Subscribe(PlayAmbience);
            
            if(masterVolume != null) masterVolume.OnValueChanged += SetMasterVolume;
            if(musicVolume != null) musicVolume.OnValueChanged += SetMusicVolume;
            if(ambienceVolume != null) ambienceVolume.OnValueChanged += SetAmbienceVolume;
            if(sfxVolume != null) sfxVolume.OnValueChanged += SetSfxVolume;
            if(uiVolume != null) uiVolume.OnValueChanged += SetUIVolume;
        }

        private void UnsubscribeEvents()
        {
            onUISfxRequested.Unsubscribe(PlaySfx);
            onMusicRequested.Unsubscribe(PlayMusic);
            onAmbienceRequested.Unsubscribe(PlayAmbience);
            
            masterVolume.OnValueChanged -= SetMasterVolume;
            musicVolume.OnValueChanged -= SetMusicVolume;
            sfxVolume.OnValueChanged   -= SetSfxVolume;
            uiVolume.OnValueChanged -= SetUIVolume;
            ambienceVolume.OnValueChanged -= SetAmbienceVolume;
        }

        private void SetInitialValues()
        {
            if(masterVolume != null) SetMasterVolume(masterVolume.Value);
            if(musicVolume != null) SetMusicVolume(musicVolume.Value);
            if(ambienceVolume != null) SetAmbienceVolume(ambienceVolume.Value);
            if(sfxVolume != null) SetSfxVolume(sfxVolume.Value);
            if(uiVolume != null) SetUIVolume(uiVolume.Value);
        }
        
        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            if(musicSource != null) musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(AudioKeys.MixerMusic)[0];
            if(ambienceSource != null) ambienceSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(AudioKeys.MixerAmbience)[0];
            if(uiSfxSource != null) uiSfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(AudioKeys.MixerUI)[0];
        }
        
        private void OnEnable()
        {
            audioDatabase = GameDatabases.AudioClipDatabase;
            SubscribeEvents();
            SetInitialValues();
        }

        private void OnDisable() => UnsubscribeEvents();
        
        #endregion
    }
}
