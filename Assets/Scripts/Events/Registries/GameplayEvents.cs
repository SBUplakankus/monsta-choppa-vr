using Characters.Enemies;
using Systems.Arena;
using Systems.Core;

namespace Events.Registries
{
    public static class GameplayEvents
    {
        #region Events

        // Arena
        public static readonly EventChannel<int> PlayerDamaged = new();
        public static readonly EventChannel<int> PlayerHealed = new();
        public static readonly EventChannel<EnemyController> EnemySpawned  = new();
        public static readonly EventChannel<EnemyController> EnemyDespawned = new();
        public static readonly EventChannel ArenaDefeatSequenceRequested = new();
        public static readonly EventChannel ArenaVictorySequenceRequested = new();
        
        // Stats
        public static readonly EventChannel<int> GoldChanged = new();
        public static readonly EventChannel<int> ExperienceChanged = new();
        public static readonly EventChannel<int> LevelChanged = new();
        
        // State
        public static readonly EventChannel GamePaused = new();
        public static readonly EventChannel GameResumed = new();
        public static readonly EventChannel<GameState> GameStateChanged = new();
        public static readonly EventChannel<GameState> GameStateChangeRequested = new();
        public static readonly EventChannel<ArenaState> ArenaStateChanged = new();
        public static readonly EventChannel<ArenaState> ArenaStateChangeRequested = new();

        #endregion

        #region Methods

        public static void Clear()
        {
            PlayerDamaged.Clear();
            PlayerHealed.Clear();
            EnemySpawned.Clear();
            EnemyDespawned.Clear();
            ArenaDefeatSequenceRequested.Clear();
            ArenaVictorySequenceRequested.Clear();
            
            GoldChanged.Clear();
            ExperienceChanged.Clear();
            LevelChanged.Clear();
            
            GamePaused.Clear();
            GameResumed.Clear();
            GameStateChanged.Clear();
            GameStateChangeRequested.Clear();
            ArenaStateChanged.Clear();
            ArenaStateChangeRequested.Clear();
        }

        #endregion
    }
}