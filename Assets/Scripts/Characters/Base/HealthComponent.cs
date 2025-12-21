using System;
using UnityEngine;

namespace Characters.Base
{
    public abstract class HealthComponent : MonoBehaviour
    {
        protected event Action OnDeath;
        private int _currentHealth;
        private int _maxHealth;

        /// <summary>
        /// Initialise the Health Component
        /// </summary>
        /// <param name="maxHealth">Max Health</param>
        /// <param name="onDeathHandler">Function to call on death</param>
        public virtual void InitHealth(int maxHealth, Action onDeathHandler)
        {
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            OnDeath += onDeathHandler;
        }

        public virtual void ResetHealth(Action onDeathHandler)
        {
            _currentHealth = 0;
            _maxHealth = 0;
            OnDeath -= onDeathHandler;
        }
        
        /// <summary>
        /// Damage the health component and check to see if dead
        /// </summary>
        /// <param name="damage">Damage to be done</param>
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }
    }
}