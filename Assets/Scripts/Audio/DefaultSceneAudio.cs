using System;
using Data.Core;
using Databases;
using Events;
using UnityEngine;

namespace Audio
{
    public class DefaultSceneAudio : MonoBehaviour
    {
        private readonly StringEventChannel _onMusicRequested = GameEvents.OnMusicRequested;
        private readonly StringEventChannel _onAmbienceRequested = GameEvents.OnAmbienceRequested;

        [Header("Audio IDs")] 
        [SerializeField] private AudioClipData musicKey;
        [SerializeField] private AudioClipData ambienceKey;

        private void Start()
        {
            _onMusicRequested.Raise(musicKey.ID);
            _onAmbienceRequested.Raise(ambienceKey.ID);
        }
    }
}
