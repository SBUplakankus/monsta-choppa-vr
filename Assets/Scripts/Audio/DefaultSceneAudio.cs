using System;
using Data.Core;
using Databases;
using Events;
using UnityEngine;

namespace Audio
{
    public class DefaultSceneAudio : MonoBehaviour
    {
        [Header("Events")] 
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onAmbienceRequested;

        [Header("Audio IDs")] 
        [SerializeField] private AudioClipData musicKey;
        [SerializeField] private AudioClipData ambienceKey;

        private void Start()
        {
            onMusicRequested.Raise(musicKey.ID);
            onAmbienceRequested.Raise(ambienceKey.ID);
        }
    }
}
