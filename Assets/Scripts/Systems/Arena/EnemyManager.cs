using System;
using System.Collections.Generic;
using Characters.Enemies;
using Events;
using Pooling;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// Manages the lifecycle of active enemies in the game, including tracking, cleanup, and updates.
    /// Subscribes to enemy-related spawn and despawn events.
    /// </summary>
    public class EnemyManager : MonoBehaviour, IUpdateable
    {
        #region Fields

        [Header("Enemy Events")]
        private EnemyEventChannel _onEnemySpawned;
        private EnemyEventChannel _onEnemyDespawned;

        private readonly HashSet<EnemyController> _activeEnemies = new();
        // Reusable list to avoid allocations during cleanup
        private readonly List<EnemyController> _enemyCleanupBuffer = new();
        private GamePoolManager _gamePoolManager;

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets the current count of active enemies being tracked.
        /// </summary>
        public int ActiveEnemiesCount => _activeEnemies.Count;
        
        /// <summary>
        /// Event triggered when an enemy is killed.
        /// </summary>
        public Action OnEnemyDeath;

        #endregion
        
        #region Methods

        /// <summary>
        /// Cleans up all active enemies by killing each one and removing them from the tracked set.
        /// </summary>
        public void CleanupEnemies()
        {
            if (_activeEnemies.Count == 0) return;
    
            // Use reusable buffer to avoid allocation
            _enemyCleanupBuffer.Clear();
            _enemyCleanupBuffer.AddRange(_activeEnemies);
            
            foreach (var enemy in _enemyCleanupBuffer)
            {
                if (enemy != null)
                {
                    enemy.DebugKillEnemy(); // Only interact with valid enemies
                }
                else
                {
                    Debug.LogWarning("Encountered a null enemy during CleanupEnemies.");
                }
            }
            
            _enemyCleanupBuffer.Clear();
        }

        #endregion
        
        #region Event Handlers

        /// <summary>
        /// Adds an enemy to the manager's active enemy collection when it spawns.
        /// </summary>
        /// <param name="enemyController">The <see cref="EnemyController"/> being added to the manager.</param>
        private void HandleEnemyEnable(EnemyController enemyController) => _activeEnemies.Add(enemyController);

        /// <summary>
        /// Removes an enemy from the active enemy collection when it despawns.
        /// </summary>
        /// <param name="enemyController">The <see cref="EnemyController"/> being removed from the manager.</param>
        private void HandleEnemyDisable(EnemyController enemyController)
        {
            // Check if reference was already removed or is null
            if (enemyController == null)
            {
                Debug.LogWarning("Attempting to disable an enemy that is already null.");
                return;
            }

            // Remove from active list
            if (_activeEnemies.Remove(enemyController))
            {
                OnEnemyDeath?.Invoke(); // Trigger any death events
            }
            else
            {
                Debug.LogWarning("EnemyController not found in active enemies list.");
            }
        }

        #endregion
        
        #region Unity Methods

        /// <summary>
        /// Updates high-priority logic for all tracked enemies.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        public void OnUpdate(float deltaTime)
        {
            if (_activeEnemies.Count == 0) return;

            foreach (var enemy in _activeEnemies)
            {
                enemy.HighPriorityUpdate();
            }
        }

        /// <summary>
        /// Initializes required references and subscribes to relevant events for enemy management.
        /// </summary>
        private void Awake()
        {
            _gamePoolManager = GamePoolManager.Instance;

            _onEnemySpawned = GameEvents.OnEnemySpawned;
            _onEnemyDespawned = GameEvents.OnEnemyDespawned;
        }

        /// <summary>
        /// Subscribes to spawn and despawn events and registers to manage updates.
        /// </summary>
        private void OnEnable()
        {
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);

            if (!_gamePoolManager)
                _gamePoolManager = GamePoolManager.Instance;
            
            _onEnemySpawned.Subscribe(HandleEnemyEnable);
            _onEnemyDespawned.Subscribe(HandleEnemyDisable);
        }

        /// <summary>
        /// Unsubscribes from spawn and despawn events and unregisters from updates.
        /// </summary>
        private void OnDisable()
        {
            GameUpdateManager.Instance.Unregister(this);
            _onEnemySpawned.Unsubscribe(HandleEnemyEnable);
            _onEnemyDespawned.Unsubscribe(HandleEnemyDisable);
        }

        #endregion
    }
}