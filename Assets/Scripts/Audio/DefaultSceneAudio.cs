using System;
using Data.Core;
using Databases;
using Events;
using Events.Registries;
using UnityEngine;

namespace Audio
{
    public class DefaultSceneAudio : MonoBehaviour
    {
        [Header("Audio IDs")] 
        [SerializeField] private AudioClipData musicKey;
        [SerializeField] private AudioClipData ambienceKey;

        private void Start()
        {
            AudioEvents.MusicFadeRequested.Raise(musicKey.ID);
            AudioEvents.AmbienceRequested.Raise(ambienceKey.ID);
        }
    }
}
