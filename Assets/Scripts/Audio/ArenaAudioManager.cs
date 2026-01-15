using System;
using Constants;
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
        [SerializeField] private GameStateEventChannel onGameStateChanged;
        
        #endregion
        
        #region Class Methods

        private void HandleGameStateChange(GameState newState)
        {
            var key = newState switch
            {
                GameState.WaveActive => arenaData.WaveMusicKey,
                GameState.WaveIntermission or GameState.BossIntermission => arenaData.IntermissionMusicKey,
                GameState.BossActive => arenaData.BossMusicKey,
                GameState.GameWon => AudioKeys.GameWonKey,
                GameState.GameOver => AudioKeys.GameOverKey,
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

        private void OnEnable() => onGameStateChanged.Subscribe(HandleGameStateChange);
        private void OnDisable() => onGameStateChanged.Unsubscribe(HandleGameStateChange);

        #endregion
    }
}
