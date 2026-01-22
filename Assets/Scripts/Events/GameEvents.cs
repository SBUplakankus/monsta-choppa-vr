namespace Events
{
    public static class GameEvents
    {
        private static bool IsInstalled { get; set; }

        internal static void MarkInstalled()
        {
            if (IsInstalled)
                return;

            IsInstalled = true;
        }
        
        
        #region Player Events

        public static IntEventChannel OnPlayerDamaged { get; internal set; }
        public static IntEventChannel OnGoldIncreased { get; internal set; }
        public static IntEventChannel OnExperienceIncreased { get; internal set; }
        public static IntEventChannel OnLevelIncreased { get; internal set; }

        #endregion

        #region Audio Events

        public static StringEventChannel OnMusicRequested { get; internal set; }
        public static StringEventChannel OnMusicFadeRequested { get; internal set; }
        public static StringEventChannel OnAmbienceRequested { get; internal set; }
        public static StringEventChannel OnSfxRequested { get; internal set; }
        public static StringEventChannel OnUISfxRequested { get; internal set; }

        #endregion

        #region Arena Events

        public static EnemyEventChannel OnEnemySpawned { get; internal set; }
        public static EnemyEventChannel OnEnemyDespawned { get; internal set; }
        public static ArenaStateEventChannel OnArenaStateChanged { get; internal set; }
        public static ArenaStateEventChannel OnArenaStateChangeRequested { get; internal set; }
        public static VoidEventChannel OnPauseRequested { get; internal set; }
        public static VoidEventChannel OnResumeRequested { get; internal set; }
        public static VoidEventChannel OnGameOverSequenceRequested { get; internal set; }
        public static VoidEventChannel OnGameWonSequenceRequested { get; internal set; }

        #endregion
        
        #region Game Events
        
        public static GameStateEventChannel OnGameStateChanged { get; internal set; }
        public static GameStateEventChannel OnGameStateChangeRequested { get; internal set; }
        public static LocaleEventChannel OnLocaleChangeRequested { get; internal set; }
        
        #endregion
        
        #region Save Events
        
        public static VoidEventChannel OnPlayerSaveRequested { get; internal set; }
        public static VoidEventChannel OnPlayerSaveCompleted { get; internal set; }
        public static VoidEventChannel OnPlayerLoadRequested { get; internal set; }
        public static VoidEventChannel OnPlayerLoadCompleted { get; internal set; }
        
        public static VoidEventChannel OnSettingsSaveRequested { get; internal set; }
        public static VoidEventChannel OnSettingsSaveCompleted { get; internal set; }
        public static VoidEventChannel OnSettingsLoadRequested { get; internal set; }
        public static VoidEventChannel OnSettingsLoadCompleted { get; internal set; }
        
        #endregion
        
        /// <summary>
        /// Clear all event channel references.
        /// Call this when transitioning between scenes or shutting down the game.
        /// </summary>
        public static void Clear()
        {
            // Player Events
            OnPlayerDamaged = null;
            OnGoldIncreased = null;
            OnExperienceIncreased = null;
            OnLevelIncreased = null;
            
            // Audio Events
            OnMusicRequested = null;
            OnMusicFadeRequested = null;
            OnAmbienceRequested = null;
            OnSfxRequested = null;
            OnUISfxRequested = null;
            
            // Arena Events
            OnEnemySpawned = null;
            OnEnemyDespawned = null;
            OnArenaStateChanged = null;
            OnArenaStateChangeRequested = null;
            OnPauseRequested = null;
            OnResumeRequested = null;
            OnGameOverSequenceRequested = null;
            OnGameWonSequenceRequested = null;
            
            // Game Events
            OnGameStateChanged = null;
            OnGameStateChangeRequested = null;
            OnLocaleChangeRequested = null;
            
            // Save Events
            OnPlayerSaveRequested = null;
            OnPlayerSaveCompleted = null;
            OnPlayerLoadRequested = null;
            OnPlayerLoadCompleted = null;
            
            OnSettingsSaveRequested = null;
            OnSettingsSaveCompleted = null;
            OnSettingsLoadRequested = null;
            OnSettingsLoadCompleted = null;
        }
    }
}
