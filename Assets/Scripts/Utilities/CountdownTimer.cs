using System;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Utility class for countdown timer functionality.
    /// </summary>
    public class CountdownTimer
    {
        private float _duration;
        private float _remainingTime;
        private Action _onComplete;
        private bool _isActive;

        /// <summary>
        /// Starts the countdown timer.
        /// </summary>
        /// <param name="duration">Duration of the countdown in seconds.</param>
        /// <param name="onComplete">Callback to invoke when countdown completes.</param>
        public void Start(float duration, Action onComplete)
        {
            _duration = duration;
            _remainingTime = duration;
            _onComplete = onComplete;
            _isActive = true;
        }

        /// <summary>
        /// Stops the countdown timer.
        /// </summary>
        public void Stop()
        {
            _isActive = false;
            _onComplete = null;
        }

        /// <summary>
        /// Updates the timer. Should be called by the driving update system (e.g., GameUpdateManager).
        /// </summary>
        /// <param name="deltaTime">Elapsed time since the last update call.</param>
        public void Update(float deltaTime)
        {
            if (!_isActive) return;

            _remainingTime -= deltaTime;
            if (_remainingTime <= 0f)
            {
                _isActive = false;
                _onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Indicates if the timer is currently active.
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// Gets the remaining time in the countdown.
        /// </summary>
        public float RemainingTime => Mathf.Max(_remainingTime, 0f); // Clamp to avoid negative values
    }
}