using System;
using Constants;
using Events;
using UnityEngine;
using Utilities;
using Waves;

namespace Systems.Arena
{
    /// <summary>
    /// Manages wave flow, countdowns, and game progression for the arena combat system.
    /// This component listens for <see cref="GameState"/> changes and coordinates
    /// spawning waves and bosses while managing intermissions and state transitions.
    /// </summary>
    /// <remarks>
    /// This class works closely with <see cref="WaveSpawner"/>, <see cref="CountdownTimer"/>,
    /// and state-related event channels like <see cref="GameStateEventChannel"/>.
    /// </remarks>
    [RequireComponent(typeof(WaveSpawner))]
    public class WaveManager : MonoBehaviour, IUpdateable
    {
        #region Fields

        [Header("Arena Data")]
        [SerializeField] private ArenaWavesData arenaWavesData;

        [Header("Event Channels")]
        [SerializeField] private GameStateEventChannel onGameStateChanged;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequest;

        private CountdownTimer _countdownTimer;
        private WaveSpawner _waveSpawner;
        private int _currentWaveIndex;

        #endregion
        
        #region Wave Methods

        /// <summary>
        /// Handles the prelude before any wave starts.
        /// Starts a countdown timer to transition into <see cref="GameState.WaveIntermission"/>.
        /// </summary>
        private void HandleWavePrelude()
        {
            StartCountdown(GameConstants.PreludeDuration, GameState.WaveIntermission);
        }
        
        /// <summary>
        /// Spawns the next wave of enemies using <see cref="WaveSpawner.SpawnWave"/>.
        /// </summary>
        private void HandleWaveStart()
        {
            var waveData = arenaWavesData.Waves[_currentWaveIndex];
            _waveSpawner.SpawnWave(waveData);
        }

        /// <summary>
        /// Handles wave completion logic and transitions to the next game state.
        /// If all waves are completed, transitions to <see cref="GameState.BossIntermission"/>.
        /// Otherwise, transitions back to <see cref="GameState.WaveIntermission"/>.
        /// </summary>
        private void HandleWaveCompletion()
        {
            _currentWaveIndex++;
            var allWavesCompleted = _currentWaveIndex >= arenaWavesData.Waves.Count;

            onGameStateChangeRequest?.Raise(allWavesCompleted
                ? GameState.BossIntermission
                : GameState.WaveIntermission);
        }

        /// <summary>
        /// Handles events triggered when all enemies in a wave are defeated.
        /// Requests a transition to <see cref="GameState.WaveComplete"/>.
        /// </summary>
        private void HandleAllWaveEnemiesDefeated()
        {
            onGameStateChangeRequest?.Raise(GameState.WaveComplete);
        }

        /// <summary>
        /// Spawns the boss wave using <see cref="WaveSpawner.SpawnBoss"/>.
        /// </summary>
        private void HandleBossSpawn()
        {
            var bossWave = arenaWavesData.Boss;
            _waveSpawner.SpawnBoss(bossWave);
        }
        
        /// <summary>
        /// Handles the boss defeat and transitions to the <see cref="GameState.BossComplete"/> state.
        /// </summary>
        private void HandleBossDefeated() => onGameStateChangeRequest?.Raise(GameState.BossComplete);
        
        /// <summary>
        /// Handles the boss completion and requests a transition to <see cref="GameState.GameWon"/>.
        /// </summary>
        private void HandleBossCompletion() => onGameStateChangeRequest?.Raise(GameState.GameWon);
        
        /// <summary>
        /// Manages intermission periods between waves or before the boss fight.
        /// </summary>
        /// <param name="duration">The duration of the intermission in seconds.</param>
        /// <param name="nextState">The <see cref="GameState"/> to transition to after the intermission.</param>
        private void HandleIntermission(float duration, GameState nextState) => StartCountdown(duration, nextState);
        
        /// <summary>
        /// Stops the countdown timer if it is currently running.
        /// </summary>
        private void StopTimer() => _countdownTimer.Stop();
        
        #endregion
        
        #region Event Handlers

        /// <summary>
        /// Handles changes to the <see cref="GameState"/> and triggers appropriate wave or state logic.
        /// </summary>
        /// <param name="gameState">The new game state to handle.</param>
        private void HandleGameStateChange(GameState gameState)
        {
            Debug.Log($"WaveManager: State changed to {gameState}.");
            switch (gameState)
            {
                case GameState.GamePrelude:
                    HandleWavePrelude();
                    break;
                case GameState.WaveIntermission:
                    HandleIntermission(GameConstants.WaveIntermissionDuration, GameState.WaveActive);
                    break;
                case GameState.WaveActive:
                    HandleWaveStart();
                    break;
                case GameState.WaveComplete:
                    HandleWaveCompletion();
                    break;
                case GameState.BossIntermission:
                    HandleIntermission(GameConstants.BossIntermissionDuration, GameState.BossActive);
                    break;
                case GameState.BossActive:
                    HandleBossSpawn();
                    break;
                case GameState.BossComplete:
                    HandleBossCompletion();
                    break;
                case GameState.GameOver:
                case GameState.GameWon:
                case GameState.GamePaused:
                    StopTimer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }
        
        /// <summary>
        /// Starts a countdown timer and triggers a transition to the specified <see cref="GameState"/>.
        /// </summary>
        /// <param name="duration">Duration of the countdown in seconds.</param>
        /// <param name="nextState">The <see cref="GameState"/> to transition to when the countdown ends.</param>
        private void StartCountdown(float duration, GameState nextState)
        {
            _countdownTimer.Start(duration, () => onGameStateChangeRequest?.Raise(nextState));
        }
        
        #endregion

        #region Unity Methods

        /// <summary>
        /// Initializes timer and spawner components.
        /// </summary>
        private void Awake()
        {
            _countdownTimer = new CountdownTimer();
            _waveSpawner = GetComponent<WaveSpawner>();
        }

        /// <summary>
        /// Subscribes to events and registers the manager with the <see cref="GameUpdateManager"/>.
        /// </summary>
        private void OnEnable()
        {
            onGameStateChanged?.Subscribe(HandleGameStateChange);
            _waveSpawner.OnWaveEnemiesDefeated += HandleAllWaveEnemiesDefeated;
            _waveSpawner.OnBossDefeated += HandleBossDefeated;

            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }

        /// <summary>
        /// Unsubscribes from events and unregisters the manager from <see cref="GameUpdateManager"/>.
        /// </summary>
        private void OnDisable()
        {
            onGameStateChanged?.Unsubscribe(HandleGameStateChange);
            _waveSpawner.OnWaveEnemiesDefeated -= HandleAllWaveEnemiesDefeated;
            _waveSpawner.OnBossDefeated -= HandleBossDefeated;

            GameUpdateManager.Instance.Unregister(this);
        }

        /// <summary>
        /// Updates the countdown timer every frame.
        /// Invoked as part of the <see cref="IUpdateable"/> interface.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        public void OnUpdate(float deltaTime) => _countdownTimer.Update(deltaTime);
        
        #endregion
    }
}