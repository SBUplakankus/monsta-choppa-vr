using System;
using UnityEngine;
using Weapons;

namespace Characters.Base
{
    public abstract class HealthComponent : MonoBehaviour, IDamageable
    {
        #region Fields

        protected event Action OnDeath;
        protected event Action OnDamageTaken;
        private int _currentHealth;
        private int _maxHealth;

        #endregion
        

        #region Properties

        public float HealthBarValue =>  Mathf.Clamp01((float)_currentHealth / _maxHealth);
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        #endregion
        
        /// <summary>
        /// Initialise the Health Component
        /// </summary>
        /// <param name="maxHealth">Max Health</param>
        /// <param name="onDeathHandler">Function to call on death</param>
        public virtual void OnSpawn(int maxHealth, Action onDeathHandler)
        {
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            OnDeath += onDeathHandler;
        }

        public virtual void OnDespawn(Action onDeathHandler)
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
            Debug.Log($"Health: {_currentHealth} / {_maxHealth}");
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }
    }
    
    public interface IDamageable
    {
        void TakeDamage(int damage);
    }
}