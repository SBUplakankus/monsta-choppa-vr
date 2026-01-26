using Attributes;

using Events;
using Interfaces;
using Systems.Arena;
using UnityEngine;

namespace Player
{
    public class PlayerArenaController : MonoBehaviour, IDamageable
    {
        [Header("Arena Attributes")] 
        [SerializeField] private IntAttribute maxHealth;
        [SerializeField] private IntAttribute currentHealth;
        [SerializeField] private IntAttribute maxShield;
        [SerializeField] private IntAttribute currentShield;

        [Header("Events")] 
        [SerializeField] private ArenaStateEventChannel onArenaStateChangeRequested;
        
        private bool _isInitialized;

        public void Init()
        {
            currentHealth.Value = maxHealth.Value;
            currentShield.Value = maxShield.Value;
            _isInitialized = true;
        }

        public void TakeDamage(int damage)
        {
            if (!_isInitialized)
            {
                Debug.LogError("PlayerArenaController has not been initialized!");
                return;
            }

            if (currentShield.Value > 0)
            {
                var remainingDamage = damage - currentShield.Value;
                currentShield.Value = Mathf.Max(0, currentShield.Value - damage);

                if (remainingDamage > 0)
                {
                    currentHealth.Value = Mathf.Clamp(currentHealth.Value - remainingDamage, 0, maxHealth.Value);
                }
            }
            else
            {
                currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, maxHealth.Value);
            }

            if (currentHealth.Value <= 0)
                HandleDeath();
        }

        private void HandleDeath()
        {
            onArenaStateChangeRequested?.Raise(ArenaState.ArenaOver); 
        }
    }
}