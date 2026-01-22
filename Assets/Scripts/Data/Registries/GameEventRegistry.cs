using Events;
using UnityEngine;

namespace Data.Registries
{
    [CreateAssetMenu(fileName = "GameEventRegistry", menuName = "Scriptable Objects/Registries/Game Events")]
    public class GameEventRegistry : ScriptableObject
    {
        [Header("Player Events")]
        public IntEventChannel onPlayerDamaged;
        public IntEventChannel onGoldIncreased;
        public IntEventChannel onExperienceIncreased;
        public IntEventChannel onLevelIncreased;

        [Header("Audio Events")]
        public StringEventChannel onMusicRequested;
        public StringEventChannel onMusicFadeRequested;
        public StringEventChannel onAmbienceRequested;
        public StringEventChannel onSfxRequested;
        public StringEventChannel onUISfxRequested;

        [Header("Arena Events")]
        public EnemyEventChannel onEnemySpawned;
        public EnemyEventChannel onEnemyDespawned;
        public ArenaStateEventChannel onArenaStateChanged;
        public ArenaStateEventChannel onArenaStateChangeRequested;
        public VoidEventChannel onPauseRequested;
        public VoidEventChannel onResumeRequested;
        public VoidEventChannel onGameOverSequenceRequested;
        public VoidEventChannel onGameWonSequenceRequested;

        [Header("Game Events")]
        public GameStateEventChannel onGameStateChanged;
        public GameStateEventChannel onGameStateChangeRequested;
        public LocaleEventChannel onLocaleChangeRequested;

        [Header("Save Events")]
        public VoidEventChannel onPlayerSaveRequested;
        public VoidEventChannel onPlayerSaveCompleted;
        public VoidEventChannel onPlayerLoadRequested;
        public VoidEventChannel onPlayerLoadCompleted;
        public VoidEventChannel onSettingsSaveRequested;
        public VoidEventChannel onSettingsSaveCompleted;
        public VoidEventChannel onSettingsLoadRequested;
        public VoidEventChannel onSettingsLoadCompleted;

        #region Installation

        public void Validate()
        {
            Debug.Assert(onPlayerDamaged, nameof(onPlayerDamaged));
            Debug.Assert(onGoldIncreased, nameof(onGoldIncreased));
            Debug.Assert(onExperienceIncreased, nameof(onExperienceIncreased));
            Debug.Assert(onLevelIncreased, nameof(onLevelIncreased));

            Debug.Assert(onMusicRequested, nameof(onMusicRequested));
            Debug.Assert(onMusicFadeRequested, nameof(onMusicFadeRequested));
            Debug.Assert(onAmbienceRequested, nameof(onAmbienceRequested));
            Debug.Assert(onSfxRequested, nameof(onSfxRequested));
            Debug.Assert(onUISfxRequested, nameof(onUISfxRequested));

            Debug.Assert(onEnemySpawned, nameof(onEnemySpawned));
            Debug.Assert(onEnemyDespawned, nameof(onEnemyDespawned));
            Debug.Assert(onArenaStateChanged, nameof(onArenaStateChanged));
            Debug.Assert(onArenaStateChangeRequested, nameof(onArenaStateChangeRequested));
            Debug.Assert(onPauseRequested, nameof(onPauseRequested));
            Debug.Assert(onResumeRequested, nameof(onResumeRequested));
            Debug.Assert(onGameOverSequenceRequested, nameof(onGameOverSequenceRequested));
            Debug.Assert(onGameWonSequenceRequested, nameof(onGameWonSequenceRequested));

            Debug.Assert(onGameStateChanged, nameof(onGameStateChanged));
            Debug.Assert(onGameStateChangeRequested, nameof(onGameStateChangeRequested));
            Debug.Assert(onLocaleChangeRequested, nameof(onLocaleChangeRequested));

            Debug.Assert(onPlayerSaveRequested, nameof(onPlayerSaveRequested));
            Debug.Assert(onPlayerSaveCompleted, nameof(onPlayerSaveCompleted));
            Debug.Assert(onPlayerLoadRequested, nameof(onPlayerLoadRequested));
            Debug.Assert(onPlayerLoadCompleted, nameof(onPlayerLoadCompleted));

            Debug.Assert(onSettingsSaveRequested, nameof(onSettingsSaveRequested));
            Debug.Assert(onSettingsSaveCompleted, nameof(onSettingsSaveCompleted));
            Debug.Assert(onSettingsLoadRequested, nameof(onSettingsLoadRequested));
            Debug.Assert(onSettingsLoadCompleted, nameof(onSettingsLoadCompleted));
        }

        public void Install()
        {
            Validate();
            GameEvents.MarkInstalled();
            
            // Player
            GameEvents.OnPlayerDamaged = onPlayerDamaged;
            GameEvents.OnGoldIncreased = onGoldIncreased;
            GameEvents.OnExperienceIncreased = onExperienceIncreased;
            GameEvents.OnLevelIncreased = onLevelIncreased;

            // Audio
            GameEvents.OnMusicRequested = onMusicRequested;
            GameEvents.OnMusicFadeRequested = onMusicFadeRequested;
            GameEvents.OnAmbienceRequested = onAmbienceRequested;
            GameEvents.OnSfxRequested = onSfxRequested;
            GameEvents.OnUISfxRequested = onUISfxRequested;

            // Arena
            GameEvents.OnEnemySpawned = onEnemySpawned;
            GameEvents.OnEnemyDespawned = onEnemyDespawned;
            GameEvents.OnArenaStateChanged = onArenaStateChanged;
            GameEvents.OnArenaStateChangeRequested = onArenaStateChangeRequested;
            GameEvents.OnPauseRequested = onPauseRequested;
            GameEvents.OnResumeRequested = onResumeRequested;
            GameEvents.OnGameOverSequenceRequested = onGameOverSequenceRequested;
            GameEvents.OnGameWonSequenceRequested = onGameWonSequenceRequested;

            // Game
            GameEvents.OnGameStateChanged = onGameStateChanged;
            GameEvents.OnGameStateChangeRequested = onGameStateChangeRequested;
            GameEvents.OnLocaleChangeRequested = onLocaleChangeRequested;

            // Save
            GameEvents.OnPlayerSaveRequested = onPlayerSaveRequested;
            GameEvents.OnPlayerSaveCompleted = onPlayerSaveCompleted;
            GameEvents.OnPlayerLoadRequested = onPlayerLoadRequested;
            GameEvents.OnPlayerLoadCompleted = onPlayerLoadCompleted;

            GameEvents.OnSettingsSaveRequested = onSettingsSaveRequested;
            GameEvents.OnSettingsSaveCompleted = onSettingsSaveCompleted;
            GameEvents.OnSettingsLoadRequested = onSettingsLoadRequested;
            GameEvents.OnSettingsLoadCompleted = onSettingsLoadCompleted;
        }

        #endregion
    }
}
