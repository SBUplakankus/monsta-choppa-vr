using System;
using Constants;
using Data.Arena;
using Events;
using Systems.Arena;
using UnityEngine;

namespace Audio
{
    public class ArenaAudioManager : MonoBehaviour
    {
        #region Fields
        
        [Header("Arena Data")]
        [SerializeField] private ArenaData arenaData;
        
        [Header("Events")] 
        [SerializeField] private StringEventChannel onAmbienceRequested;
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onMusicFadeRequested;
        [SerializeField] private ArenaStateEventChannel onArenaStateChanged;
        
        #endregion
        
        #region Class Methods

        private void HandleGameStateChange(ArenaState newState)
        {
            var key = newState switch
            {
                ArenaState.WaveActive => arenaData.WaveMusicKey,
                ArenaState.WaveIntermission or ArenaState.BossIntermission => arenaData.IntermissionMusicKey,
                ArenaState.BossActive => arenaData.BossMusicKey,
                ArenaState.ArenaWon => AudioKeys.GameWonKey,
                ArenaState.ArenaOver => AudioKeys.GameOverKey,
                _ => null
            };
            
            if(key != null)
                onMusicFadeRequested.Raise(key);
        }
        
        #endregion
        
        #region Unity Methods
        
        private void Start()
        {
            onMusicRequested.Raise(AudioKeys.GameIntroKey);
            onAmbienceRequested.Raise(arenaData.Ambience);
        }

        private void OnEnable() => onArenaStateChanged.Subscribe(HandleGameStateChange);
        private void OnDisable() => onArenaStateChanged.Unsubscribe(HandleGameStateChange);

        #endregion
    }
}
