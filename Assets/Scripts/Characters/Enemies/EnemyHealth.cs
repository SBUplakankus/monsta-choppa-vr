using System;
using Characters.Base;
using Events;
using UI.Game;
using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyHealth : HealthComponent
    {
        [SerializeField] private EnemyHealthBar  healthBar;
        
        private void OnEnable() => OnDamageTaken += HandleDamageTaken;
        private void OnDisable() => OnDamageTaken -= HandleDamageTaken;
        
        private void HandleDamageTaken()
        {
            healthBar.UpdateHealthBarValue(HealthBarValue);
        }
    }
}