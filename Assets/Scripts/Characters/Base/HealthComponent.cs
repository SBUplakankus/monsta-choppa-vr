using System;
using Constants;
using Interfaces;
using UnityEngine;

namespace Characters.Base
{
    /// <summary>
    /// Base health component implementing damage handling with invincibility frames.
    /// VR-optimized with minimal allocations and event-driven architecture.
    /// </summary>
    public abstract class HealthComponent : MonoBehaviour, IDamageable
    {
        #region Fields

        protected event Action OnDeath;
        protected event Action OnDamageTaken;
        
        /// <summary>
        /// Event providing hit direction for directional feedback.
        /// </summary>
        protected event Action<Vector3> OnDamageTakenWithDirection;
        
        private int _currentHealth;
        private int _maxHealth;
        private float _lastDamageTime;
        
        [Header("Invincibility")]
        [SerializeField] private float invincibilityDuration = GameConstants.InvincibilityDuration;
        [SerializeField] private bool useInvincibility = true;

        // Cache last hit info for directional reactions
        private Vector3 _lastHitDirection;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the normalized health value (0 to 1) for UI display.
        /// </summary>
        public float HealthBarValue => Mathf.Clamp01((float)_currentHealth / _maxHealth);
        
        /// <summary>
        /// Gets the current health points.
        /// </summary>
        public int CurrentHealth => _currentHealth;
        
        /// <summary>
        /// Gets the maximum health points.
        /// </summary>
        public int MaxHealth => _maxHealth;

        /// <summary>
        /// Returns true if the entity is currently invincible.
        /// </summary>
        public bool IsInvincible => useInvincibility && Time.time < _lastDamageTime + invincibilityDuration;

        /// <summary>
        /// Returns true if the entity is dead.
        /// </summary>
        public bool IsDead => _currentHealth <= 0;

        /// <summary>
        /// Gets the direction of the last hit received.
        /// </summary>
        public Vector3 LastHitDirection => _lastHitDirection;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the Health Component when spawned.
        /// </summary>
        /// <param name="maxHealth">Maximum health points.</param>
        /// <param name="onDeathHandler">Callback when health reaches zero.</param>
        public virtual void OnSpawn(int maxHealth, Action onDeathHandler)
        {
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            _lastDamageTime = -invincibilityDuration; // Allow immediate damage
            _lastHitDirection = Vector3.zero;
            OnDeath += onDeathHandler;
        }

        /// <summary>
        /// Cleans up the Health Component when despawned.
        /// </summary>
        /// <param name="onDeathHandler">Callback to unsubscribe.</param>
        public virtual void OnDespawn(Action onDeathHandler)
        {
            _currentHealth = 0;
            _maxHealth = 0;
            OnDeath -= onDeathHandler;
        }

        #endregion

        #region Damage

        /// <summary>
        /// Applies damage to this entity. Respects invincibility frames.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        public void TakeDamage(int damage)
        {
            TakeDamage(damage, Vector3.zero);
        }

        /// <summary>
        /// Applies damage to this entity with hit direction for directional reactions.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        /// <param name="hitDirection">Direction the damage came from.</param>
        public void TakeDamage(int damage, Vector3 hitDirection)
        {
            // Check invincibility
            if (IsInvincible) return;
            
            // Already dead check
            if (IsDead) return;

            // Apply damage
            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);
            _lastDamageTime = Time.time;
            _lastHitDirection = hitDirection;

            // Notify listeners
            OnDamageTaken?.Invoke();
            
            if (hitDirection != Vector3.zero)
            {
                OnDamageTakenWithDirection?.Invoke(hitDirection);
            }

            // Check for death
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        /// <summary>
        /// Heals the entity by the specified amount.
        /// </summary>
        /// <param name="amount">Amount to heal.</param>
        public void Heal(int amount)
        {
            if (IsDead) return;
            
            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        }

        /// <summary>
        /// Sets health to a specific value. Use for initialization or special effects.
        /// </summary>
        /// <param name="health">New health value.</param>
        public void SetHealth(int health)
        {
            _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
            
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        #endregion
    }

    
}