namespace Events
{
    public static class GameEvents
    {
        #region Player Events

        public static IntEventChannel OnPlayerDamaged { get; set; }
        public static IntEventChannel OnGoldChanged { get; set; }
        public static IntEventChannel OnExperienceGained { get; set; }
        public static IntEventChannel OnLevelChanged { get; set; }

        #endregion

        #region Audio Events

        public static StringEventChannel OnMusicRequested { get; set; }
        public static StringEventChannel OnSfxRequested { get; set; }

        #endregion

        #region Arena Events

        public static EnemyEventChannel OnEnemySpawned { get; set; }
        public static EnemyEventChannel OnEnemyDespawned { get; set; }
        public static ArenaStateEventChannel OnArenaStateChanged { get; set; }
        public static ArenaStateEventChannel OnArenaStateChangeRequested { get; set; }
        public static VoidEventChannel OnPauseRequested { get; set; }
        public static VoidEventChannel OnResumeRequested { get; set; }
        public static VoidEventChannel OnGameOverSequenceRequested { get; set; }
        public static VoidEventChannel OnGameWonSequenceRequested { get; set; }

        #endregion
        
        #region Game Events
        
        public static GameStateEventChannel OnGameStateChanged { get; set; }
        public static GameStateEventChannel OnGameStateChangeRequested { get; set; }
        
        #endregion
        
        #region Save Events
        
        public static VoidEventChannel OnPlayerSaveRequested { get; set; }
        public static VoidEventChannel OnPlayerSaveCompleted { get; set; }
        public static VoidEventChannel OnPlayerLoadRequested { get; set; }
        public static VoidEventChannel OnPlayerLoadCompleted { get; set; }
        
        public static VoidEventChannel OnSettingsSaveRequested { get; set; }
        public static VoidEventChannel OnSettingsSaveCompleted { get; set; }
        public static VoidEventChannel OnSettingsLoadRequested { get; set; }
        public static VoidEventChannel OnSettingsLoadCompleted { get; set; }
        
        #endregion
    }
}
