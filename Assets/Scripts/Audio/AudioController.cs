using System;
using Constants;
using Databases.Base;
using Events.Base;
using UnityEngine;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        [Header("Audio Sources")] 
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource masterSource;
        
        [Header("Audio Database")]
        [SerializeField] private AudioClipDatabase audioDatabase;

        private void OnEnable()
        {
            PlayMusic();
        }

        private void PlayMusic()
        {
            musicSource.clip = audioDatabase.TryGet(GameConstants.MainMusicKey).clip;
            musicSource.Play();
        }
    }
}
