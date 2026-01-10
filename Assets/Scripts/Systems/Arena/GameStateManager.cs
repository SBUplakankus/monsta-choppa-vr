using System;
using Events;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// Enum representing the high-level states of the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>Initial prelude before the first wave (intro, cutscene, etc.)</summary>
        GamePrelude,

        /// <summary>Wave is actively spawning and enemies are alive</summary>
        WaveActive,

        /// <summary>Time between waves for countdowns, shop, or prep</summary>
        WaveIntermission,

        /// <summary>Wave completed; triggers wave-complete sequence</summary>
        WaveComplete,

        /// <summary>Time before a boss wave; may include cutscene or preparation</summary>
        BossIntermission,

        /// <summary>Boss wave is active</summary>
        BossActive,

        /// <summary>Boss defeated; triggers win or post-boss sequence</summary>
        BossComplete,

        /// <summary>All waves/bosses completed; game victory</summary>
        GameWon,

        /// <summary>Player defeated; game over sequence</summary>
        GameOver,

        /// <summary>Game is paused</summary>
        GamePaused
    }

    /// <summary>
    /// Manages the current game state and transitions between them.
    /// Raises events for listeners and triggers enter/exit logic for each state.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        #region Fields

        [Header("Events")]
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;
        [SerializeField] private GameStateEventChannel onGameStateChanged;
        [SerializeField] private StringEventChannel onSfxRequested;
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private VoidEventChannel onPauseRequested;
        [SerializeField] private VoidEventChannel onGameOverSequenceRequested;
        [SerializeField] private VoidEventChannel onGameWonSequenceRequested;

        private GameState _currentGameState;
        private GameState _previousState;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentGameState = GameState.GamePrelude;
            EnterCurrentState();
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        #endregion

        #region State Management

        /// <summary>
        /// Determines if a transition from one state to another is valid.
        /// </summary>
        private static bool IsValidTransition(GameState from, GameState to)
        {
            return from switch
            {
                GameState.GamePrelude => to == GameState.WaveIntermission,
                GameState.WaveIntermission => to == GameState.WaveActive,
                GameState.WaveActive => to is GameState.WaveComplete or GameState.GameOver or GameState.BossIntermission,
                GameState.WaveComplete => to is GameState.WaveIntermission or GameState.BossIntermission,
                GameState.BossIntermission => to == GameState.BossActive,
                GameState.BossActive => to is GameState.BossComplete or GameState.GameOver,
                GameState.BossComplete => to == GameState.GameWon,
                GameState.GameWon => false,
                GameState.GameOver => false,
                GameState.GamePaused => true, // can always exit pause
                _ => false
            };
        }

        /// <summary>
        /// Handles an external request to change the game state.
        /// </summary>
        private void HandleGameStateChangeRequest(GameState newGameState)
        {
            if (_currentGameState == newGameState) return;
            if (!IsValidTransition(_currentGameState, newGameState)) return;

            HandleGameStateChanged(newGameState);
        }

        /// <summary>
        /// Handles internal logic for changing the game state.
        /// Calls exit on the old state, sets the new state, enters the new state, and raises events.
        /// </summary>
        private void HandleGameStateChanged(GameState newGameState)
        {
            ExitCurrentState();
            _currentGameState = newGameState;
            EnterCurrentState();
            onGameStateChanged.Raise(_currentGameState);
        }

        /// <summary>
        /// Toggles the pause state, preserving the previous state.
        /// </summary>
        private void TogglePause()
        {
            if (_currentGameState == GameState.GamePaused)
            {
                HandleGameStateChanged(_previousState);
            }
            else
            {
                _previousState = _currentGameState;
                HandleGameStateChanged(GameState.GamePaused);
            }
        }

        #endregion

        #region State Enter / Exit

        private void EnterCurrentState()
        {
            switch (_currentGameState)
            {
                case GameState.GamePrelude: HandleGamePreludeEnter(); break;
                case GameState.WaveIntermission: HandleWaveIntermissionEnter(); break;
                case GameState.WaveActive: HandleWaveActiveEnter(); break;
                case GameState.WaveComplete: HandleWaveCompleteEnter(); break;
                case GameState.BossIntermission: HandleBossIntermissionEnter(); break;
                case GameState.BossActive: HandleBossActiveEnter(); break;
                case GameState.BossComplete: HandleBossCompleteEnter(); break;
                case GameState.GameWon: HandleGameWonEnter(); break;
                case GameState.GameOver: HandleGameOverEnter(); break;
                case GameState.GamePaused: HandleGamePausedEnter(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExitCurrentState()
        {
            switch (_currentGameState)
            {
                case GameState.GamePrelude: HandleGamePreludeExit(); break;
                case GameState.WaveIntermission: HandleWaveIntermissionExit(); break;
                case GameState.WaveActive: HandleWaveActiveExit(); break;
                case GameState.WaveComplete: HandleWaveCompleteExit(); break;
                case GameState.BossIntermission: HandleBossIntermissionExit(); break;
                case GameState.BossActive: HandleBossActiveExit(); break;
                case GameState.BossComplete: HandleBossCompleteExit(); break;
                case GameState.GameWon: HandleGameWonExit(); break;
                case GameState.GameOver: HandleGameOverExit(); break;
                case GameState.GamePaused: HandleGamePausedExit(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region State Logic Stubs

        private void HandleGamePreludeEnter()
        {
            // TODO: Play intro music, show cinematic, prepare first wave
            onMusicRequested.Raise("PreludeMusic");
        }
        private void HandleGamePreludeExit() { }

        private void HandleWaveIntermissionEnter()
        {
            // TODO: Start wave countdown, show shop or UI
            onMusicRequested.Raise("IntermissionMusic");
        }
        private void HandleWaveIntermissionExit() { }

        private void HandleWaveActiveEnter()
        {
            // TODO: Spawn enemies, play combat music, update UI
            onSfxRequested.Raise("WaveStartHorn");
            onMusicRequested.Raise("CombatMusic");
        }
        private void HandleWaveActiveExit() { }

        private void HandleWaveCompleteEnter()
        {
            // TODO: Show wave complete UI, reward player, play fanfare
            onSfxRequested.Raise("WaveCompleteFanfare");
        }
        private void HandleWaveCompleteExit() { }

        private void HandleBossIntermissionEnter()
        {
            // TODO: Play boss intro music, cutscene, countdown
            onMusicRequested.Raise("BossIntroMusic");
        }
        private void HandleBossIntermissionExit() { }

        private void HandleBossActiveEnter()
        {
            // TODO: Spawn boss, update UI, play boss music
            onSfxRequested.Raise("BossHorn");
            onMusicRequested.Raise("BossMusic");
        }
        private void HandleBossActiveExit() { }

        private void HandleBossCompleteEnter()
        {
            // TODO: Show boss defeat sequence, play victory music
            onSfxRequested.Raise("BossDefeatedFanfare");
        }
        private void HandleBossCompleteExit() { }

        private void HandleGameWonEnter()
        {
            // TODO: Play victory music, show end screen
            onMusicRequested.Raise("VictoryMusic");
            onGameWonSequenceRequested.Raise();
        }
        private void HandleGameWonExit() { }

        private void HandleGameOverEnter()
        {
            // TODO: Play defeat music, show game over screen
            onMusicRequested.Raise("DefeatMusic");
            onGameOverSequenceRequested.Raise();
        }
        private void HandleGameOverExit() { }

        private void HandleGamePausedEnter()
        {
            // TODO: Pause gameplay, show pause UI
            Time.timeScale = 0f;
        }
        private void HandleGamePausedExit()
        {
            // Resume gameplay
            Time.timeScale = 1f;
        }

        #endregion
    }
}
