using System;
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
        [SerializeField] private string musicKey;
        [SerializeField] private string ambienceKey;

        private void Start()
        {
            onMusicRequested.Raise(musicKey);
            onAmbienceRequested.Raise(ambienceKey);
        }
    }
}
