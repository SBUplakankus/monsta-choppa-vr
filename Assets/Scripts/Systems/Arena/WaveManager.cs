using System;
using Constants;
using Data.Arena;
using Data.Waves;
using Events;
using Events.Registries;
using UnityEngine;
using Utilities;

namespace Systems.Arena
{
    /// <summary>
    /// Manages wave flow, countdowns, and game progression for the arena combat system.
    /// This component listens for <see cref="ArenaState"/> changes and coordinates
    /// spawning waves and bosses while managing intermissions and state transitions.
    /// </summary>
    /// <remarks>
    /// This class works closely with <see cref="WaveSpawner"/>, <see cref="CountdownTimer"/>,
    /// </remarks>
    [RequireComponent(typeof(WaveSpawner))]
    public class WaveManager : MonoBehaviour, IUpdateable
    {
        #region Fields

        [Header("Arena Data")]
        [SerializeField] private ArenaWavesData arenaWavesData;

        private CountdownTimer _countdownTimer;
        private WaveSpawner _waveSpawner;
        private int _currentWaveIndex;

        #endregion
        
        #region Wave Methods

        /// <summary>
        /// Handles the prelude before any wave starts.
        /// Starts a countdown timer to transition into <see cref="ArenaState.WaveIntermission"/>.
        /// </summary>
        private void HandleWavePrelude()
        {
            StartCountdown(GameConstants.PreludeDuration, ArenaState.WaveIntermission);
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
        /// If all waves are completed, transitions to <see cref="ArenaState.BossIntermission"/>.
        /// Otherwise, transitions back to <see cref="ArenaState.WaveIntermission"/>.
        /// </summary>
        private void HandleWaveCompletion()
        {
            _currentWaveIndex++;
            var allWavesCompleted = _currentWaveIndex >= arenaWavesData.Waves.Count;
            
            GameplayEvents.ArenaStateChangeRequested.Raise(allWavesCompleted
                ? ArenaState.BossIntermission
                : ArenaState.WaveIntermission);
        }

        /// <summary>
        /// Handles events triggered when all enemies in a wave are defeated.
        /// Requests a transition to <see cref="ArenaState.WaveComplete"/>.
        /// </summary>
        private void HandleAllWaveEnemiesDefeated()
        {
            GameplayEvents.ArenaStateChangeRequested.Raise(ArenaState.WaveComplete);
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
        /// Handles the boss defeat and transitions to the <see cref="ArenaState.BossComplete"/> state.
        /// </summary>
        private void HandleBossDefeated() => GameplayEvents.ArenaStateChangeRequested.Raise(ArenaState.BossComplete);
        
        /// <summary>
        /// Handles the boss completion and requests a transition to <see cref="ArenaState.ArenaVictory"/>.
        /// </summary>
        private void HandleBossCompletion() => GameplayEvents.ArenaStateChangeRequested.Raise(ArenaState.ArenaVictory);
        
        /// <summary>
        /// Manages intermission periods between waves or before the boss fight.
        /// </summary>
        /// <param name="duration">The duration of the intermission in seconds.</param>
        /// <param name="nextState">The <see cref="ArenaState"/> to transition to after the intermission.</param>
        private void HandleIntermission(float duration, ArenaState nextState) => StartCountdown(duration, nextState);
        
        /// <summary>
        /// Stops the countdown timer if it is currently running.
        /// </summary>
        private void StopTimer() => _countdownTimer.Stop();
        
        #endregion
        
        #region Event Handlers

        /// <summary>
        /// Handles changes to the <see cref="ArenaState"/> and triggers appropriate wave or state logic.
        /// </summary>
        /// <param name="arenaState">The new game state to handle.</param>
        private void HandleGameStateChange(ArenaState arenaState)
        {
            Debug.Log($"WaveManager: State changed to {arenaState}.");
            switch (arenaState)
            {
                case ArenaState.ArenaPrelude:
                    HandleWavePrelude();
                    break;
                case ArenaState.WaveIntermission:
                    HandleIntermission(GameConstants.WaveIntermissionDuration, ArenaState.WaveActive);
                    break;
                case ArenaState.WaveActive:
                    HandleWaveStart();
                    break;
                case ArenaState.WaveComplete:
                    HandleWaveCompletion();
                    break;
                case ArenaState.BossIntermission:
                    HandleIntermission(GameConstants.BossIntermissionDuration, ArenaState.BossActive);
                    break;
                case ArenaState.BossActive:
                    HandleBossSpawn();
                    break;
                case ArenaState.BossComplete:
                    HandleBossCompletion();
                    break;
                case ArenaState.ArenaDefeat:
                case ArenaState.ArenaVictory:
                case ArenaState.ArenaPaused:
                    StopTimer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arenaState), arenaState, null);
            }
        }
        
        /// <summary>
        /// Starts a countdown timer and triggers a transition to the specified <see cref="ArenaState"/>.
        /// </summary>
        /// <param name="duration">Duration of the countdown in seconds.</param>
        /// <param name="nextState">The <see cref="ArenaState"/> to transition to when the countdown ends.</param>
        private void StartCountdown(float duration, ArenaState nextState)
        {
            _countdownTimer.Start(duration, () => GameplayEvents.ArenaStateChangeRequested.Raise(nextState));
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
            GameplayEvents.ArenaStateChanged.Subscribe(HandleGameStateChange);
            _waveSpawner.OnWaveEnemiesDefeated += HandleAllWaveEnemiesDefeated;
            _waveSpawner.OnBossDefeated += HandleBossDefeated;

            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }

        /// <summary>
        /// Unsubscribes from events and unregisters the manager from <see cref="GameUpdateManager"/>.
        /// </summary>
        private void OnDisable()
        {
            GameplayEvents.ArenaStateChanged.Unsubscribe(HandleGameStateChange);
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