using System;
using Data.Core;
using Databases;
using Events;
using UnityEngine;

namespace Audio
{
    public class DefaultSceneAudio : MonoBehaviour
    {
        [SerializeField] private StringEventChannel _onMusicRequested;
        [SerializeField] private StringEventChannel _onAmbienceRequested;

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
