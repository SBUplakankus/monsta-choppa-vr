using System;
using Constants;
using Events;
using UnityEngine;
using Utilities;
using Waves;

namespace Systems.Arena
{
    /// <summary>
    /// Manages wave flow, countdowns, and progression.
    /// Listens to GameState changes and requests transitions.
    /// </summary>
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

        private void HandleWavePrelude()
        {
            Debug.Log($"WaveManager: Starting Wave Prelude. Duration: {GameConstants.PreludeDuration}s");
            if (GameConstants.PreludeDuration <= 0)
            {
                Debug.LogError("WaveManager: Invalid Prelude Duration. Must be greater than 0.");
                return;
            }
            StartCountdown(GameConstants.PreludeDuration, GameState.WaveIntermission);
        }
        
        /// <summary>
        /// Spawns the next wave of enemies.
        /// </summary>
        private void HandleWaveStart()
        {
            var waveData = arenaWavesData.Waves[_currentWaveIndex];
            _waveSpawner.SpawnWave(waveData);
        }

        /// <summary>
        /// Handles wave completion and progresses to the next state.
        /// </summary>
        private void HandleWaveCompletion()
        {
            _currentWaveIndex++;
            var allWavesCompleted = _currentWaveIndex >= arenaWavesData.Waves.Count;

            onGameStateChangeRequest?.Raise(allWavesCompleted
                ? GameState.BossIntermission
                : GameState.WaveIntermission
            );
        }

        /// <summary>
        /// Spawns the boss wave.
        /// </summary>
        private void HandleBossSpawn()
        {
            var bossWave = arenaWavesData.Boss;
            _waveSpawner.SpawnBoss(bossWave);
        }

        /// <summary>
        /// Handles the boss completion state and transitions to the final state.
        /// </summary>
        private void HandleBossCompletion() => onGameStateChangeRequest?.Raise(GameState.GameWon);
        
        private void HandleIntermission(float duration, GameState nextState) => StartCountdown(duration, nextState);
        
        private void StopTimer() => _countdownTimer.Stop();
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handles changes to the game state and takes action.
        /// </summary>
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
                    HandleIntermission(GameConstants.WaveIntermissionDuration, GameState.BossActive);
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
        /// Starts a countdown timer and triggers a transition when completed.
        /// </summary>
        private void StartCountdown(float duration, GameState nextState)
        {
            _countdownTimer.Start(duration, () => onGameStateChangeRequest?.Raise(nextState));
        }
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            _countdownTimer = new CountdownTimer();
            _waveSpawner = GetComponent<WaveSpawner>();
        }

        private void OnEnable()
        {
            onGameStateChanged?.Subscribe(HandleGameStateChange);
            _waveSpawner.OnWaveCompleted += HandleWaveCompletion;
            _waveSpawner.OnBossCompleted += HandleBossCompletion;

            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }

        private void OnDisable()
        {
            onGameStateChanged?.Unsubscribe(HandleGameStateChange);
            _waveSpawner.OnWaveCompleted -= HandleWaveCompletion;
            _waveSpawner.OnBossCompleted -= HandleBossCompletion;

            GameUpdateManager.Instance.Unregister(this);
        }

        public void OnUpdate(float deltaTime) => _countdownTimer.Update(deltaTime);
        
        #endregion
    }
}