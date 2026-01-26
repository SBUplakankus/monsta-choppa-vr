using System;
using Constants;
using Data.Arena;
using Events;
using Events.Registries;
using Systems.Arena;
using UnityEngine;

namespace Audio
{
    public class ArenaAudioManager : MonoBehaviour
    {
        #region Fields
        
        [Header("Arena Data")]
        [SerializeField] private ArenaData arenaData;
        
        #endregion
        
        #region Class Methods

        private void HandleGameStateChange(ArenaState newState)
        {
            var key = newState switch
            {
                ArenaState.WaveActive => arenaData.WaveMusicKey,
                ArenaState.WaveIntermission or ArenaState.BossIntermission => arenaData.IntermissionMusicKey,
                ArenaState.BossActive => arenaData.BossMusicKey,
                ArenaState.ArenaVictory => AudioKeys.GameWonKey,
                ArenaState.ArenaDefeat => AudioKeys.GameOverKey,
                _ => null
            };
            
            if(key != null)
                AudioEvents.MusicFadeRequested.Raise(key);
        }
        
        #endregion
        
        #region Unity Methods
        private void Start()
        {
            AudioEvents.MusicRequested.Raise(AudioKeys.GameIntroKey);
            AudioEvents.AmbienceRequested.Raise(arenaData.Ambience);
        }

        private void OnEnable() => GameplayEvents.ArenaStateChanged.Subscribe(HandleGameStateChange);
        private void OnDisable() => GameplayEvents.ArenaStateChanged.Unsubscribe(HandleGameStateChange);

        #endregion
    }
}
