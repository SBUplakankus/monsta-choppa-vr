using System;
using System.Collections;
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
        [SerializeField] private VoidEventChannel onPauseRequested;

        private GameState _currentGameState;
        private GameState _previousState;

        #endregion

        #region  Properties

        public GameState CurrentGameState => _currentGameState;
        public GameState PreviousGameState => _previousState;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentGameState = GameState.GamePrelude;
            EnterCurrentState();
        }

        private IEnumerator TestStart()
        {
            yield return new WaitForSeconds(1);
            onGameStateChanged.Raise(_currentGameState);
        }

        private void OnEnable()
        {
            onGameStateChangeRequested?.Subscribe(HandleGameStateChangeRequest);
            StartCoroutine(TestStart());
        }

        private void OnDisable()
        {
            onGameStateChangeRequested?.Unsubscribe(HandleGameStateChangeRequest);
        }

        #endregion

        #region State Management

        /// <summary>
        /// Determines if a transition from one state to another is valid.
        /// </summary>
        private static bool IsValidTransition(GameState from, GameState to)
        {
            Debug.Log($"GameStateManager: Checking if transition from {from} to {to} is valid.");
            return from switch
            {
                // Prelude -> WaveIntermission (game setup completed, start intermission)
                GameState.GamePrelude => to == GameState.WaveIntermission,

                // WaveIntermission -> WaveActive (ready to start a wave)
                GameState.WaveIntermission => to == GameState.WaveActive,

                // WaveActive -> (WaveComplete/BossIntermission/GameOver)
                GameState.WaveActive => to is GameState.WaveComplete or GameState.GameOver,

                // WaveComplete -> WaveIntermission/BossIntermission
                GameState.WaveComplete => to is GameState.WaveIntermission or GameState.BossIntermission,

                // BossIntermission -> BossActive (ready to start boss wave)
                GameState.BossIntermission => to == GameState.BossActive,

                // BossActive -> (BossComplete/GameOver)
                GameState.BossActive => to is GameState.BossComplete or GameState.GameOver,

                // BossComplete -> GameWon (boss defeated, transition to victory)
                GameState.BossComplete => to == GameState.GameWon,

                // Game state is final, no transitions possible
                GameState.GameWon => false, // Can't transition from victory
                GameState.GameOver => false, // Can't transition from game over

                // Paused -> True (can resume to any previous state)
                GameState.GamePaused => true,

                // Default case: transition not allowed
                _ => false
            };
        }

        /// <summary>
        /// Handles an external request to change the game state.
        /// </summary>
        private void HandleGameStateChangeRequest(GameState newGameState)
        {
            if (_currentGameState == newGameState)
            {
                Debug.LogWarning($"GameStateManager: Ignoring redundant state change to {newGameState}. Already in that state!");
                return;
            }

            if (!IsValidTransition(_currentGameState, newGameState))
            {
                Debug.LogWarning($"GameStateManager: Invalid transition from {_currentGameState} to {newGameState}.");
                return;
            }

            Debug.Log($"GameStateManager: Handling requested state transition to {newGameState}.");
            HandleGameStateChanged(newGameState);
        }

        /// <summary>
        /// Handles internal logic for changing the game state.
        /// Calls exit on the old state, sets the new state, enters the new state, and raises events.
        /// </summary>
        private void HandleGameStateChanged(GameState newGameState)
        {
            Debug.Log($"GameStateManager: Exiting {_currentGameState}, transitioning to {newGameState}.");
            ExitCurrentState();
            _currentGameState = newGameState;
            EnterCurrentState();
            Debug.Log($"GameStateManager: Entered new state {newGameState}.");
            onGameStateChanged?.Raise(_currentGameState);
        }

        /// <summary>
        /// Toggles the pause state, preserving the previous state.
        /// </summary>
        private void TogglePause()
        {
            if (_currentGameState == GameState.GamePaused)
            {
                Debug.Log("GameStateManager: Resuming from paused state.");
                HandleGameStateChanged(_previousState);
            }
            else
            {
                Debug.Log($"GameStateManager: Pausing game. Previous state: {_currentGameState}.");
                _previousState = _currentGameState;
                HandleGameStateChanged(GameState.GamePaused);
            }
        }

        #endregion

        #region State Enter / Exit

        private void EnterCurrentState()
        {
            Debug.Log($"GameStateManager: Entering state {_currentGameState}.");
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
            Debug.Log($"GameStateManager: Exiting state {_currentGameState}.");
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

        private void HandleGamePreludeEnter() => Debug.Log("GameStateManager: Entering GamePrelude State.");
        private void HandleGamePreludeExit() => Debug.Log("GameStateManager: Exiting GamePrelude State.");

        private void HandleWaveIntermissionEnter() => Debug.Log("GameStateManager: Entering WaveIntermission State.");
        private void HandleWaveIntermissionExit() => Debug.Log("GameStateManager: Exiting WaveIntermission State.");

        private void HandleWaveActiveEnter() => Debug.Log("GameStateManager: Entering WaveActive State.");
        private void HandleWaveActiveExit() => Debug.Log("GameStateManager: Exiting WaveActive State.");

        private void HandleWaveCompleteEnter() => Debug.Log("GameStateManager: Entering WaveComplete State.");
        private void HandleWaveCompleteExit() => Debug.Log("GameStateManager: Exiting WaveComplete State.");

        private void HandleBossIntermissionEnter() => Debug.Log("GameStateManager: Entering BossIntermission State.");
        private void HandleBossIntermissionExit() => Debug.Log("GameStateManager: Exiting BossIntermission State.");

        private void HandleBossActiveEnter() => Debug.Log("GameStateManager: Entering BossActive State.");
        private void HandleBossActiveExit() => Debug.Log("GameStateManager: Exiting BossActive State.");

        private void HandleBossCompleteEnter() => Debug.Log("GameStateManager: Entering BossComplete State.");
        private void HandleBossCompleteExit() => Debug.Log("GameStateManager: Exiting BossComplete State.");

        private void HandleGameWonEnter() => Debug.Log("GameStateManager: Entering GameWon State.");
        private void HandleGameWonExit() => Debug.Log("GameStateManager: Exiting GameWon State.");

        private void HandleGameOverEnter() => Debug.Log("GameStateManager: Entering GameOver State.");
        private void HandleGameOverExit() => Debug.Log("GameStateManager: Exiting GameOver State.");

        private void HandleGamePausedEnter() => Debug.Log("GameStateManager: Game Paused. Setting Time.timeScale to 0.");
        private void HandleGamePausedExit()
        {
            Debug.Log("GameStateManager: Game Resumed. Restoring Time.timeScale to 1.");
            Time.timeScale = 1f;
        }

        #endregion
    }
}