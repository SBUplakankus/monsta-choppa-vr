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
        [SerializeField] private StringEventChannel _onAmbienceRequested;
        [SerializeField] private StringEventChannel _onMusicRequested;
        [SerializeField] private StringEventChannel _onMusicFadeRequested;
        [SerializeField] private ArenaStateEventChannel _onArenaStateChanged;
        
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
                _onMusicFadeRequested.Raise(key);
        }
        
        #endregion
        
        #region Unity Methods
        private void Start()
        {
            _onMusicRequested.Raise(AudioKeys.GameIntroKey);
            _onAmbienceRequested.Raise(arenaData.Ambience);
        }

        private void OnEnable() => _onArenaStateChanged.Subscribe(HandleGameStateChange);
        private void OnDisable() => _onArenaStateChanged.Unsubscribe(HandleGameStateChange);

        #endregion
    }
}
