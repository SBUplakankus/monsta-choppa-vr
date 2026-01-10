using System;
using Constants;
using Events;
using UnityEngine;

namespace Systems.Arena
{
    /// <summary>
    /// Manages wave flow, countdowns, and progression.
    /// Listens to GameState changes and requests transitions.
    /// </summary>
    public class WaveManager : MonoBehaviour, IUpdateable
    {
        #region Fields

        [Header("Events")]
        [SerializeField] private GameStateEventChannel onGameStateChanged;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;

        [Header("Wave Timings")] 
        private const float PreludeDelay = GameConstants.PreludeDuration;
        private const float IntermissionDelay = GameConstants.WaveIntermissionDuration;
        private const float BossIntermissionDelay = GameConstants.BosIntermissionDuration;

        private float _countDownTimer;
        private bool _countDownActive;

        private int _currentWaveIndex;
        private bool _isBossWave;

        #endregion

        #region Game State Handlers

        private void HandleGamePrelude()
        {
            StartCountdown(PreludeDelay, GameState.WaveIntermission);
        }

        private void HandleWaveIntermission()
        {
            _isBossWave = IsBossWave(_currentWaveIndex);
            StartCountdown(
                _isBossWave ? BossIntermissionDelay : IntermissionDelay,
                _isBossWave ? GameState.BossIntermission : GameState.WaveActive
            );
        }

        private void HandleWaveActive()
        {
            // TODO: Trigger enemy spawning (via SpawnManager)
            // TODO: Reset alive enemy count
            Debug.Log($"Wave {_currentWaveIndex} started.");
        }

        private void HandleWaveComplete()
        {
            _currentWaveIndex++;

            if (IsLastWave())
            {
                RaiseGameStateChangeRequest(GameState.GameWon);
            }
            else
            {
                RaiseGameStateChangeRequest(GameState.WaveIntermission);
            }
        }

        private void HandleBossIntermission()
        {
            StartCountdown(BossIntermissionDelay, GameState.BossActive);
        }

        private void HandleBossActive()
        {
            // TODO: Spawn boss
            Debug.Log("Boss wave started.");
        }

        private void HandleBossComplete()
        {
            RaiseGameStateChangeRequest(GameState.GameWon);
        }

        private void HandleGameOver()
        {
            StopCountdown();
        }

        private void HandleGamePaused()
        {
            // Timer intentionally NOT paused (unscaled delta recommended)
        }

        #endregion

        #region Countdown Logic

        private void StartCountdown(float duration, GameState nextState)
        {
            _countDownTimer = duration;
            _countDownActive = true;
            _nextStateAfterCountdown = nextState;
        }

        private void StopCountdown()
        {
            _countDownActive = false;
        }

        private GameState _nextStateAfterCountdown;

        #endregion

        #region Utility Methods

        private void RaiseGameStateChangeRequest(GameState gameState)
        {
            onGameStateChangeRequested.Raise(gameState);
        }

        private bool IsBossWave(int waveIndex)
        {
            // Example: every 5 waves is a boss
            return (waveIndex + 1) % 5 == 0;
        }

        private bool IsLastWave()
        {
            // TODO: Replace with wave definition count
            return _currentWaveIndex >= 10;
        }

        #endregion

        #region Game State Change Listener

        private void HandleGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.GamePrelude:
                    HandleGamePrelude();
                    break;

                case GameState.WaveIntermission:
                    HandleWaveIntermission();
                    break;

                case GameState.WaveActive:
                    HandleWaveActive();
                    break;

                case GameState.WaveComplete:
                    HandleWaveComplete();
                    break;

                case GameState.BossIntermission:
                    HandleBossIntermission();
                    break;

                case GameState.BossActive:
                    HandleBossActive();
                    break;

                case GameState.BossComplete:
                    HandleBossComplete();
                    break;

                case GameState.GameOver:
                    HandleGameOver();
                    break;

                case GameState.GamePaused:
                    HandleGamePaused();
                    break;
            }
        }

        #endregion

        #region IUpdateable

        public void OnUpdate(float deltaTime)
        {
            if (!_countDownActive)
                return;

            _countDownTimer -= deltaTime;

            if (_countDownTimer <= 0f)
            {
                _countDownActive = false;
                RaiseGameStateChangeRequest(_nextStateAfterCountdown);
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
            onGameStateChanged.Subscribe(HandleGameStateChange);
        }

        private void OnDisable()
        {
            GameUpdateManager.Instance.Unregister(this);
            onGameStateChanged.Unsubscribe(HandleGameStateChange);
        }

        #endregion
    }
}
