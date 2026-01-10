using System.Collections.Generic;
using Characters.Enemies;
using Events;
using UnityEngine;

namespace Systems
{
    public class EnemyManager : MonoBehaviour, IUpdateable
    {
        #region Fields
        
        [Header("Enemy Events")]
        private EnemyEventChannel _onEnemySpawned;
        private EnemyEventChannel _onEnemyDespawned;
        
        private readonly HashSet<EnemyController> _activeEnemies = new();

        #endregion
        
        #region Event Handlers

        private void HandleEnemyEnable(EnemyController enemyController) => _activeEnemies.Add(enemyController);
        private void HandleEnemyDisable(EnemyController enemyController) => _activeEnemies.Remove(enemyController);
        
        #endregion
        
        
        #region Unity Functions

        public void OnUpdate(float deltaTime)
        {
            if(_activeEnemies.Count == 0) return;
            
            foreach (var enemy in _activeEnemies)
                enemy.HighPriorityUpdate();
        }

        private void Awake()
        {
            _onEnemySpawned = GameEvents.OnEnemySpawned;
            _onEnemyDespawned = GameEvents.OnEnemyDespawned;
            
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }
        
        private void OnEnable()
        { 
            _onEnemySpawned.Subscribe(HandleEnemyEnable);
            _onEnemyDespawned.Subscribe(HandleEnemyDisable);
        }
        private void OnDisable()
        { 
            GameUpdateManager.Instance.Unregister(this);
            
            _onEnemySpawned.Unsubscribe(HandleEnemyEnable);
            _onEnemyDespawned.Unsubscribe(HandleEnemyDisable);
        }

        #endregion
        
        
    }
}
