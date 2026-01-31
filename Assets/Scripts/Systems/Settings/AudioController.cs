using System.Collections;
using Constants;
using Data.Settings;
using Databases;
using Events;
using Events.Registries;
using UnityEngine;
using UnityEngine.Audio;

namespace Systems.Settings
{
    public class AudioController : MonoBehaviour
    {
        #region Fields

        [Header("Audio Sources")] 
        [SerializeField] private AudioSource musicASource;
        [SerializeField] private AudioSource musicBSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource uiSfxSource;

        [Header("Audio Mixer")] 
        [SerializeField] private AudioMixer audioMixer;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioSettingsConfig audioSettings;
        
        [Header("Audio Database")]
        [SerializeField] private AudioClipDatabase audioDatabase;

        private const float MusicFadeDuration = 2.5f;
        private Coroutine _musicFadeRoutine;

        #endregion

        #region Routines

        private IEnumerator CrossFadeMusic(
            AudioSource fromSource,
            AudioSource toSource,
            string fromMixer,
            string toMixer,
            AudioClip newClip,
            float duration)
        {
            if (newClip == null) yield break;

            toSource.clip = newClip;
            toSource.loop = true;
            toSource.Play();

            var time = 0f;

            SetMixerVolume(toMixer, 0f);
            SetMixerVolume(fromMixer, 1f);

            while (time < duration)
            {
                time += Time.deltaTime;
                var t = time / duration;

                SetMixerVolume(fromMixer, 1f - t);
                SetMixerVolume(toMixer, t);

                yield return null;
            }

            SetMixerVolume(fromMixer, 0f);
            SetMixerVolume(toMixer, 1f);

            fromSource.Stop();
        }

        #endregion
        
        #region Methods
        
        private void SetMixerVolume(string mixerKey, float normalized)
        {
            audioMixer.SetFloat(mixerKey, ToDecibels(normalized));
        }
        
        private AudioClip GetClip(string key)
        {
            var entry = audioDatabase.Get(key);
            if (entry != null) return entry.Clip;
            Debug.LogWarning($"AudioController: Audio clip not found for key '{key}'");
            return null;

        }
        
        private void PlayMusic(string key)
        {
            var clip = GetClip(key);
            if (clip == null) return;

            musicASource.clip = clip;
            musicASource.loop = true;
            musicASource.Play();

            SetMixerVolume(AudioKeys.MixerMusicA, 1f);
            SetMixerVolume(AudioKeys.MixerMusicB, 0f);
        }

        private void PlayBlendedMusic(string key)
        {
            if (audioDatabase == null || musicASource == null || musicBSource == null)
                return;

            var clip = GetClip(key);
            if (clip == null) return;

            if (_musicFadeRoutine != null)
                StopCoroutine(_musicFadeRoutine);

            var aIsPlaying = musicASource.isPlaying;

            _musicFadeRoutine = StartCoroutine(
                aIsPlaying
                    ? CrossFadeMusic(
                        musicASource,
                        musicBSource,
                        AudioKeys.MixerMusicA,
                        AudioKeys.MixerMusicB,
                        clip,
                        MusicFadeDuration)
                    : CrossFadeMusic(
                        musicBSource,
                        musicASource,
                        AudioKeys.MixerMusicB,
                        AudioKeys.MixerMusicA,
                        clip,
                        MusicFadeDuration)
            );
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

        private void PlayUISfx(string key)
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
            AudioEvents.AmbienceRequested.Subscribe(PlayAmbience);
            AudioEvents.UISfxRequested.Subscribe(PlayUISfx);
            AudioEvents.MusicRequested.Subscribe(PlayMusic);
            AudioEvents.MusicFadeRequested.Subscribe(PlayBlendedMusic);
            
            audioSettings.MasterVolume.OnValueChanged += SetMasterVolume;
            audioSettings.MusicVolume.OnValueChanged += SetMusicVolume;
            audioSettings.AmbienceVolume.OnValueChanged += SetAmbienceVolume;
            audioSettings.SfxVolume.OnValueChanged += SetSfxVolume;
            audioSettings.UIVolume.OnValueChanged += SetUIVolume;
        }

        private void UnsubscribeEvents()
        {
            AudioEvents.AmbienceRequested.Unsubscribe(PlayAmbience);
            AudioEvents.UISfxRequested.Unsubscribe(PlayUISfx);
            AudioEvents.MusicRequested.Unsubscribe(PlayMusic);
            AudioEvents.MusicFadeRequested.Unsubscribe(PlayBlendedMusic);
            
            audioSettings.MasterVolume.OnValueChanged -= SetMasterVolume;
            audioSettings.MusicVolume.OnValueChanged -= SetMusicVolume;
            audioSettings.AmbienceVolume.OnValueChanged -= SetAmbienceVolume;
            audioSettings.SfxVolume.OnValueChanged   -= SetSfxVolume;
            audioSettings.UIVolume.OnValueChanged -= SetUIVolume;
        }

        private void SetInitialValues()
        {
            SetMasterVolume(audioSettings.MasterVolume.Value);
            SetMusicVolume(audioSettings.MusicVolume.Value);
            SetAmbienceVolume(audioSettings.AmbienceVolume.Value);
            SetSfxVolume(audioSettings.SfxVolume.Value);
            SetUIVolume(audioSettings.UIVolume.Value);
        }
        
        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            SubscribeEvents();
            SetInitialValues();
        }
        
        private void Awake() => SetInitialValues();
        private void Start() => SetInitialValues();  
        private void OnDisable() => UnsubscribeEvents();

        #endregion
    }
}
