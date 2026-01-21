using System;
using System.Collections;
using Events;
using Systems.Core;
using UnityEngine;

namespace Systems.Arena
{
    /// <summary>
    /// Enum representing the high-level states of the game.
    /// </summary>
    public enum ArenaState
    {
        /// <summary>Initial prelude before the first wave (intro, cutscene, etc.)</summary>
        ArenaPrelude,

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
        ArenaWon,

        /// <summary>Player defeated; game over sequence</summary>
        ArenaOver,

        /// <summary>Game is paused</summary>
        ArenaPaused
    }

    /// <summary>
    /// Manages the current game state and transitions between them.
    /// Raises events for listeners and triggers enter/exit logic for each state.
    /// </summary>
    public class ArenaStateManager : MonoBehaviour
    {
        #region Fields

        [Header("Events")]
        [SerializeField] private ArenaStateEventChannel onArenaStateChangeRequested;
        [SerializeField] private ArenaStateEventChannel onArenaStateChanged;
        [SerializeField] private VoidEventChannel onPauseRequested;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;

        private ArenaState _currentArenaState;
        private ArenaState _previousState;

        #endregion

        #region  Properties

        public ArenaState CurrentArenaState => _currentArenaState;
        public ArenaState PreviousArenaState => _previousState;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentArenaState = ArenaState.ArenaPrelude;
            EnterCurrentState();
        }

        private IEnumerator TestStart()
        {
            yield return new WaitForSeconds(1);
            onArenaStateChanged.Raise(_currentArenaState);
        }

        private void OnEnable()
        {
            onArenaStateChangeRequested?.Subscribe(HandleGameStateChangeRequest);
            StartCoroutine(TestStart());
        }

        private void OnDisable()
        {
            onArenaStateChangeRequested?.Unsubscribe(HandleGameStateChangeRequest);
        }

        #endregion

        #region State Management

        /// <summary>
        /// Determines if a transition from one state to another is valid.
        /// </summary>
        private static bool IsValidTransition(ArenaState from, ArenaState to)
        {
            Debug.Log($"GameStateManager: Checking if transition from {from} to {to} is valid.");
            return from switch
            {
                // Prelude -> WaveIntermission (game setup completed, start intermission)
                ArenaState.ArenaPrelude => to == ArenaState.WaveIntermission,

                // WaveIntermission -> WaveActive (ready to start a wave)
                ArenaState.WaveIntermission => to == ArenaState.WaveActive,

                // WaveActive -> (WaveComplete/BossIntermission/GameOver)
                ArenaState.WaveActive => to is ArenaState.WaveComplete or ArenaState.ArenaOver,

                // WaveComplete -> WaveIntermission/BossIntermission
                ArenaState.WaveComplete => to is ArenaState.WaveIntermission or ArenaState.BossIntermission,

                // BossIntermission -> BossActive (ready to start boss wave)
                ArenaState.BossIntermission => to == ArenaState.BossActive,

                // BossActive -> (BossComplete/GameOver)
                ArenaState.BossActive => to is ArenaState.BossComplete or ArenaState.ArenaOver,

                // BossComplete -> GameWon (boss defeated, transition to victory)
                ArenaState.BossComplete => to == ArenaState.ArenaWon,

                // Game state is final, no transitions possible
                ArenaState.ArenaWon => false, // Can't transition from victory
                ArenaState.ArenaOver => false, // Can't transition from game over

                // Paused -> True (can resume to any previous state)
                ArenaState.ArenaPaused => true,

                // Default case: transition not allowed
                _ => false
            };
        }

        /// <summary>
        /// Handles an external request to change the game state.
        /// </summary>
        private void HandleGameStateChangeRequest(ArenaState newArenaState)
        {
            if (_currentArenaState == newArenaState)
            {
                Debug.LogWarning($"GameStateManager: Ignoring redundant state change to {newArenaState}. Already in that state!");
                return;
            }

            if (!IsValidTransition(_currentArenaState, newArenaState))
            {
                Debug.LogWarning($"GameStateManager: Invalid transition from {_currentArenaState} to {newArenaState}.");
                return;
            }

            Debug.Log($"GameStateManager: Handling requested state transition to {newArenaState}.");
            HandleGameStateChanged(newArenaState);
        }

        /// <summary>
        /// Handles internal logic for changing the game state.
        /// Calls exit on the old state, sets the new state, enters the new state, and raises events.
        /// </summary>
        private void HandleGameStateChanged(ArenaState newArenaState)
        {
            Debug.Log($"GameStateManager: Exiting {_currentArenaState}, transitioning to {newArenaState}.");
            ExitCurrentState();
            _currentArenaState = newArenaState;
            EnterCurrentState();
            Debug.Log($"GameStateManager: Entered new state {newArenaState}.");
            onArenaStateChanged?.Raise(_currentArenaState);
        }

        /// <summary>
        /// Toggles the pause state, preserving the previous state.
        /// </summary>
        private void TogglePause()
        {
            if (_currentArenaState == ArenaState.ArenaPaused)
            {
                Debug.Log("GameStateManager: Resuming from paused state.");
                HandleGameStateChanged(_previousState);
            }
            else
            {
                Debug.Log($"GameStateManager: Pausing game. Previous state: {_currentArenaState}.");
                _previousState = _currentArenaState;
                HandleGameStateChanged(ArenaState.ArenaPaused);
            }
        }

        #endregion

        #region State Enter / Exit

        private void EnterCurrentState()
        {
            Debug.Log($"GameStateManager: Entering state {_currentArenaState}.");
            switch (_currentArenaState)
            {
                case ArenaState.ArenaPrelude: HandleGamePreludeEnter(); break;
                case ArenaState.WaveIntermission: HandleWaveIntermissionEnter(); break;
                case ArenaState.WaveActive: HandleWaveActiveEnter(); break;
                case ArenaState.WaveComplete: HandleWaveCompleteEnter(); break;
                case ArenaState.BossIntermission: HandleBossIntermissionEnter(); break;
                case ArenaState.BossActive: HandleBossActiveEnter(); break;
                case ArenaState.BossComplete: HandleBossCompleteEnter(); break;
                case ArenaState.ArenaWon: HandleGameWonEnter(); break;
                case ArenaState.ArenaOver: HandleGameOverEnter(); break;
                case ArenaState.ArenaPaused: HandleGamePausedEnter(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExitCurrentState()
        {
            Debug.Log($"GameStateManager: Exiting state {_currentArenaState}.");
            switch (_currentArenaState)
            {
                case ArenaState.ArenaPrelude: HandleGamePreludeExit(); break;
                case ArenaState.WaveIntermission: HandleWaveIntermissionExit(); break;
                case ArenaState.WaveActive: HandleWaveActiveExit(); break;
                case ArenaState.WaveComplete: HandleWaveCompleteExit(); break;
                case ArenaState.BossIntermission: HandleBossIntermissionExit(); break;
                case ArenaState.BossActive: HandleBossActiveExit(); break;
                case ArenaState.BossComplete: HandleBossCompleteExit(); break;
                case ArenaState.ArenaWon: HandleGameWonExit(); break;
                case ArenaState.ArenaOver: HandleGameOverExit(); break;
                case ArenaState.ArenaPaused: HandleGamePausedExit(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region State Logic Stubs

        private void HandleGamePreludeEnter()
        { 
            onGameStateChangeRequested.Raise(GameState.Arena);
        }
        
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

        private void HandleGameWonEnter()
        { 
            onGameStateChangeRequested.Raise(GameState.ArenaVictory);
        }
        private void HandleGameWonExit() => Debug.Log("GameStateManager: Exiting GameWon State.");

        private void HandleGameOverEnter()
        { 
            onGameStateChangeRequested.Raise(GameState.ArenaDefeat);
        }
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