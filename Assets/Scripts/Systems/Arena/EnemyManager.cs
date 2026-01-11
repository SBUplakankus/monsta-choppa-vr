using System;
using System.Collections.Generic;
using Characters.Enemies;
using Events;
using Pooling;
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
        private GamePoolManager _gamePoolManager;

        public Action OnEnemyDeath;

        #endregion
        
        #region Properties
        
        public int ActiveEnemiesCount => _activeEnemies.Count;
        
        #endregion
        
        #region Methods

        public void CleanupEnemies()
        {
            foreach (var enemyController in _activeEnemies)
            {
                _gamePoolManager.ReturnEnemyPrefab(enemyController);
            }
        }
        
        #endregion
        
        #region Event Handlers

        private void HandleEnemyEnable(EnemyController enemyController) => _activeEnemies.Add(enemyController);

        private void HandleEnemyDisable(EnemyController enemyController)
        {
            _activeEnemies.Remove(enemyController);
            OnEnemyDeath?.Invoke();
        } 
        
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
            _gamePoolManager = GamePoolManager.Instance;
        }
        
        private void OnEnable()
        { 
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
            
            if(!_gamePoolManager)
                _gamePoolManager = GamePoolManager.Instance;
            
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
