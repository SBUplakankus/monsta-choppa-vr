using System;
using Characters.Base;
using Databases;
using Pooling;
using UI.Game;
using UnityEngine;

namespace Characters.Enemies
{
    /// <summary>
    /// Health component for enemy characters with visual feedback for damage and death.
    /// Extends <see cref="HealthComponent"/> with enemy-specific behavior and VR-optimized effects.
    /// </summary>
    public class EnemyHealth : HealthComponent
    {
        #region Fields

        [Header("UI")]
        [SerializeField] private EnemyHealthBar healthBar;
        
        [Header("Hit Effects")]
        [SerializeField] private ParticleData hitVFX;
        
        private EnemyAnimator _animator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the visual effect to play when this enemy dies.
        /// </summary>
        public ParticleData DeathVFX { get; set; }
        
        /// <summary>
        /// Gets or sets the visual effect to play when hit.
        /// </summary>
        public ParticleData HitVFX
        {
            get => hitVFX;
            set => hitVFX = value;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the enemy health with animator reference for hit reactions.
        /// </summary>
        /// <param name="animator">The enemy's animator component.</param>
        public void SetAnimator(EnemyAnimator animator)
        {
            _animator = animator;
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            OnDeath += HandleDeath;
            OnDamageTaken += HandleDamageTaken;
            OnDamageTakenWithDirection += HandleDirectionalDamage;
        }

        private void OnDisable()
        {
            OnDeath -= HandleDeath;
            OnDamageTaken -= HandleDamageTaken;
            OnDamageTakenWithDirection -= HandleDirectionalDamage;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles damage taken - updates health bar UI.
        /// </summary>
        private void HandleDamageTaken()
        {
            if (healthBar != null)
            {
                healthBar.UpdateHealthBarValue(HealthBarValue);
            }
        }

        /// <summary>
        /// Handles directional damage - plays hit reaction animation and spawns hit VFX.
        /// </summary>
        /// <param name="hitDirection">Direction the hit came from.</param>
        private void HandleDirectionalDamage(Vector3 hitDirection)
        {
            // Play directional hit reaction animation (upper body layer - Mixamo)
            if (_animator != null)
            {
                _animator.PlayHitReaction(hitDirection);
            }

            // Spawn hit VFX at impact location
            if (hitVFX != null)
            {
                var hitPosition = transform.position + Vector3.up; // Approximate chest height
                var hitRotation = Quaternion.LookRotation(-hitDirection);
                GamePoolManager.Instance?.GetParticlePrefab(hitVFX, hitPosition, hitRotation);
            }
        }

        /// <summary>
        /// Handles death - spawns death particle effect (poof).
        /// </summary>
        private void HandleDeath()
        {
            if (DeathVFX != null)
            {
                GamePoolManager.Instance?.GetParticlePrefab(DeathVFX, transform.position, transform.rotation);
            }
        }

        #endregion
    }
}