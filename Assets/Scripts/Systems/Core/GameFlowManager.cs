using System;
using Events;
using UnityEngine;

namespace Systems.Core
{
    public enum GameState
    {
        StartMenu,
        Loading,
        Hub,
        Arena,
        ArenaVictory,
        ArenaDefeat,
        ArenaPaused
    }
    /// <summary>
    /// Manages the flow of the game, handling transitions between major states.
    /// </summary>
    public class GameFlowManager : MonoBehaviour
    {
        #region Fields

        [Header("Game State Events")]
        [SerializeField] private GameStateEventChannel _onGameStateChangeRequested;
        [SerializeField] private GameStateEventChannel _onGameStateChanged;
        
        [Header("Save Events")]
        [SerializeField] private VoidEventChannel _onSettingsSaveRequested;
        [SerializeField] private VoidEventChannel _onSettingsLoadRequested;
        [SerializeField] private VoidEventChannel _onPlayerSaveRequested;
        [SerializeField] private VoidEventChannel _onPlayerLoadRequested;

        [Header("Pause Settings")]
        [SerializeField] private VoidEventChannel _onPauseRequested;
        [SerializeField] private VoidEventChannel _onResumeRequested;

        private GameState _currentGameState;
        private GameState _previousGameState;

        #endregion

        #region Properties

        public GameState CurrentGameState => _currentGameState;
        public GameState PreviousGameState => _previousGameState;

        #endregion

        #region Unity Methods

        private void BindEvents()
        {
            _onSettingsSaveRequested = GameEvents.OnSettingsSaveRequested;
            _onSettingsLoadRequested = GameEvents.OnSettingsLoadRequested;
            _onPlayerSaveRequested = GameEvents.OnPlayerSaveRequested;
            _onPlayerLoadRequested = GameEvents.OnPlayerLoadRequested;
            _onPauseRequested = GameEvents.OnPauseRequested;
            _onResumeRequested = GameEvents.OnResumeRequested;
            _onGameStateChanged = GameEvents.OnGameStateChanged;
            _onGameStateChangeRequested = GameEvents.OnGameStateChangeRequested;
        }

        private void Awake()
        {
            _currentGameState = GameState.StartMenu;
            EnterCurrentState();
        }

        private void OnEnable()
        {
            _onGameStateChangeRequested?.Subscribe(HandleGameStateChangeRequest);
            _onPauseRequested?.Subscribe(TogglePause);
            _onResumeRequested?.Subscribe(TogglePause);
        }

        private void OnDisable()
        {
            _onGameStateChangeRequested?.Unsubscribe(HandleGameStateChangeRequest);
            _onPauseRequested?.Unsubscribe(TogglePause);
            _onResumeRequested?.Unsubscribe(TogglePause);
        }

        #endregion

        #region State Management

        /// <summary>
        /// Determines if a transition from one game state to another is valid.
        /// </summary>
        private static bool IsValidTransition(GameState from, GameState to)
        {
            Debug.Log($"GameFlowManager: Checking transition from {from} to {to}.");

            return from switch
            {
                // StartMenu -> Loading
                GameState.StartMenu => to == GameState.Loading,

                // Loading -> Hub or Arena
                GameState.Loading => to is GameState.Hub or GameState.Arena,

                // Hub -> Loading or Arena
                GameState.Hub => to is GameState.Loading or GameState.Arena,

                // Arena -> Victory, Defeat, or Pause
                GameState.Arena => to is GameState.ArenaVictory or GameState.ArenaDefeat or GameState.ArenaPaused,

                // Victory or Defeat -> Hub or StartMenu
                GameState.ArenaVictory => to is GameState.Hub or GameState.StartMenu,
                GameState.ArenaDefeat => to is GameState.Hub or GameState.StartMenu,

                // Pause -> Resume (any state can technically be paused)
                GameState.ArenaPaused => true,

                _ => false
            };
        }

        /// <summary>
        /// Handles a request to change the game state.
        /// </summary>
        private void HandleGameStateChangeRequest(GameState newGameState)
        {
            if (_currentGameState == newGameState)
            {
                Debug.LogWarning($"GameFlowManager: Redundant state change to {newGameState}");
                return;
            }

            if (!IsValidTransition(_currentGameState, newGameState))
            {
                Debug.LogWarning($"GameFlowManager: Invalid transition from {_currentGameState} to {newGameState}");
                return;
            }

            Debug.Log($"GameFlowManager: Request transition to {newGameState}");
            HandleGameStateChanged(newGameState);
        }

        /// <summary>
        /// Changes the game state, triggers exit/enter logic, and fires events.
        /// </summary>
        private void HandleGameStateChanged(GameState newGameState)
        {
            Debug.Log($"GameFlowManager: Exiting {_currentGameState}, entering {newGameState}");
            ExitCurrentState();
            _currentGameState = newGameState;
            EnterCurrentState();
            _onGameStateChanged?.Raise(_currentGameState);
        }

        /// <summary>
        /// Toggles pause and unpause.
        /// </summary>
        private void TogglePause()
        {
            if (_currentGameState == GameState.ArenaPaused)
            {
                Debug.Log("GameFlowManager: Resuming from pause.");
                HandleGameStateChanged(_previousGameState);
            }
            else
            {
                Debug.Log($"GameFlowManager: Pausing. Previous state: {_currentGameState}");
                _previousGameState = _currentGameState;
                HandleGameStateChanged(GameState.ArenaPaused);
            }
        }

        #endregion

        #region State Enter/Exit Logic

        private void EnterCurrentState()
        {
            Debug.Log($"GameFlowManager: Entering state {_currentGameState}");

            switch (_currentGameState)
            {
                case GameState.StartMenu: HandleStartMenuEnter(); break;
                case GameState.Loading: HandleLoadingEnter(); break;
                case GameState.Hub: HandleHubEnter(); break;
                case GameState.Arena: HandleArenaEnter(); break;
                case GameState.ArenaVictory: HandleVictoryEnter(); break;
                case GameState.ArenaDefeat: HandleDefeatEnter(); break;
                case GameState.ArenaPaused: HandlePausedEnter(); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void ExitCurrentState()
        {
            Debug.Log($"GameFlowManager: Exiting state {_currentGameState}");

            switch (_currentGameState)
            {
                case GameState.StartMenu: HandleStartMenuExit(); break;
                case GameState.Loading: HandleLoadingExit(); break;
                case GameState.Hub: HandleHubExit(); break;
                case GameState.Arena: HandleArenaExit(); break;
                case GameState.ArenaVictory: HandleVictoryExit(); break;
                case GameState.ArenaDefeat: HandleDefeatExit(); break;
                case GameState.ArenaPaused: HandlePausedExit(); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region State Logic Stubs

        private void HandleStartMenuEnter()
        { 
            _onSettingsLoadRequested?.Raise();
        }
        private void HandleStartMenuExit()
        { 
            _onSettingsSaveRequested?.Raise();
        }

        private void HandleLoadingEnter() => Debug.Log("GameFlowManager: Enter Loading");
        private void HandleLoadingExit() => Debug.Log("GameFlowManager: Exit Loading");

        private void HandleHubEnter()
        { 
            _onPlayerLoadRequested?.Raise();
        }

        private void HandleHubExit()
        {
            _onPlayerSaveRequested?.Raise();
        }

        private void HandleArenaEnter() => Debug.Log("GameFlowManager: Enter Arena");
        private void HandleArenaExit()
        { 
            
        }

        private void HandleVictoryEnter() => Debug.Log("GameFlowManager: Enter Victory");
        private void HandleVictoryExit()
        { 
            _onPlayerSaveRequested?.Raise();
        }

        private void HandleDefeatEnter() => Debug.Log("GameFlowManager: Enter Defeat");
        private void HandleDefeatExit()
        { 
            _onPlayerSaveRequested?.Raise();
        }

        private void HandlePausedEnter() => Debug.Log("GameFlowManager: Enter Pause");
        private void HandlePausedExit()
        {
            Debug.Log("GameFlowManager: Exit Pause");
        }

        #endregion
    }
}