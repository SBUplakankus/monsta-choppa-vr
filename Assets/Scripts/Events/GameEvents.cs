namespace Events
{
    public static class GameEvents
    {
        #region Player Events
        
        public static IntEventChannel OnPlayerDamaged;
        public static IntEventChannel OnGoldChanged;
        public static IntEventChannel OnExperienceGained;
        public static IntEventChannel OnLevelChanged;
        
        #endregion
        
        #region Audio Events
        
        public static StringEventChannel OnMusicRequested;
        public static StringEventChannel OnSfxRequested;
        
        #endregion
        
        #region Game Events

        public static EnemyEventChannel OnEnemySpawned;
        public static EnemyEventChannel OnEnemyDespawned;
        public static GameStateEventChannel OnGameStateChanged;
        public static VoidEventChannel OnPauseRequested;
        public static VoidEventChannel OnGameOverSequenceRequested;
        public static VoidEventChannel OnGameWonSequenceRequested;

        #endregion
    }
}
