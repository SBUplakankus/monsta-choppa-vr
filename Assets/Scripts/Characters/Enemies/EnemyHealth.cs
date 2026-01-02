using System;
using Characters.Base;
using Databases;
using Events;
using Pooling;
using UI.Game;
using UnityEngine;

namespace Characters.Enemies
{
    /// <summary>
    /// Health component for enemy characters with visual feedback for damage and death.
    /// Extends <see cref="HealthComponent"/> with enemy-specific behavior.
    /// </summary>
    public class EnemyHealth : HealthComponent
    {
        [SerializeField] private EnemyHealthBar  healthBar;
        
        /// <summary>
        /// Gets or sets the visual effect to play when this enemy dies.
        /// </summary>
        /// <value>The <see cref="ParticleData"/> for death effects.</value>
        public ParticleData DeathVFX {get; set;}
        
        private void OnEnable()
        {
            OnDeath += HandleDeath;
            OnDamageTaken += HandleDamageTaken;
        }

        private void OnDisable()
        {
            OnDeath -= HandleDeath;
            OnDamageTaken -= HandleDamageTaken;
        } 
        
        private void HandleDamageTaken()
        {
            healthBar.UpdateHealthBarValue(HealthBarValue);
        }

        private void HandleDeath()
        {
            GamePoolManager.Instance.GetParticlePrefab(DeathVFX, transform.position, transform.rotation);
        }
    }
}