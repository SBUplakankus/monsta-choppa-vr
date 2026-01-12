using System;
using Databases;
using Events;
using Pooling;
using UnityEngine;

namespace Characters.Enemies
{
    /// <summary>
    /// Main controller for enemy entities, responsible for managing health, movement, and behavior.
    /// Coordinates with the object pool manager, enemy events, and associated components.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        #region Fields

        [Header("Components")]
        [SerializeField] private EnemyData enemyData;

        private EnemyMovement _enemyMovement;
        private EnemyAnimator _enemyAnimator;
        private EnemyHealth _enemyHealth;
        private EnemyId _enemyId;

        [Header("Enemy Events")]
        private EnemyEventChannel _onEnemySpawned;
        private EnemyEventChannel _onEnemyDespawned;

        private GamePoolManager _gamePoolManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets this enemy's configuration data.
        /// </summary>
        public EnemyData Data
        {
            get => enemyData;
            set => enemyData = value;
        }

        /// <summary>
        /// Event triggered when this enemy dies.
        /// </summary>
        public Action OnDeath { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Simulates immediate death for debugging purposes.
        /// </summary>
        public void DebugKillEnemy()
        {
            HandleEnemyDeath();
        }

        /// <summary>
        /// Executes logic triggered when the enemy dies.
        /// This includes despawning the enemy and invoking assigned death effects.
        /// </summary>
        private void HandleEnemyDeath()
        {
            if (this == null || gameObject == null || !enemyData || !_gamePoolManager)
            {
                Debug.LogWarning("EnemyController or required dependencies are null or destroyed. Skipping HandleEnemyDeath.");
                return;
            }

            _gamePoolManager.ReturnEnemyPrefab(this);
            
            if (enemyData?.DeathSfx != null && transform != null)
            {
                _gamePoolManager.GetWorldAudioPrefab(enemyData.DeathSfx, transform.position);
            }

            OnDeath?.Invoke();
        }

        /// <summary>
        /// Initializes the enemy with its components and assigns event listeners.
        /// </summary>
        private void InitEnemy()
        {
            _enemyId.ID = enemyData.EnemyId;
            _enemyHealth.OnSpawn(enemyData.MaxHealth, HandleEnemyDeath);
            _enemyHealth.DeathVFX = enemyData.DeathVFX;
            _enemyMovement.OnSpawn(enemyData.MoveSpeed);
            _enemyAnimator.OnSpawn();
            _onEnemySpawned.Raise(this);
        }

        /// <summary>
        /// Prepares this enemy for combat when spawned from the object pool.
        /// </summary>
        /// <param name="data">The <see cref="EnemyData"/> defining enemy stats.</param>
        public void OnSpawn(EnemyData data)
        {
            enemyData = data;
            InitEnemy();
        }

        /// <summary>
        /// Resets the enemy and removes associations before returning to the object pool.
        /// </summary>
        public void OnDespawn()
        {
            _enemyAnimator.OnDespawn();
            _enemyMovement.OnDespawn();
            _enemyHealth.OnDespawn(HandleEnemyDeath);
            _onEnemyDespawned.Raise(this);
        }

        public void HighPriorityUpdate()
        {
            
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Validates required components and assigns events on startup.
        /// </summary>
        private void Awake()
        {
            ValidateRequiredComponents();
            CacheComponents();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Ensures all necessary components are attached to this <see cref="GameObject"/>.
        /// </summary>
        private void ValidateRequiredComponents()
        {
            if (!GetComponent<EnemyHealth>())
                gameObject.AddComponent<EnemyHealth>();
            
            if (!GetComponent<EnemyId>())
                gameObject.AddComponent<EnemyId>();
            
            if (!GetComponent<EnemyMovement>())
                gameObject.AddComponent<EnemyMovement>();
            
            if (!GetComponent<EnemyAnimator>())
                gameObject.AddComponent<EnemyAnimator>();

            _onEnemySpawned ??= GameEvents.OnEnemySpawned;
            _onEnemyDespawned ??= GameEvents.OnEnemyDespawned;
        }

        /// <summary>
        /// Caches this enemy's components for runtime efficiency.
        /// </summary>
        private void CacheComponents()
        {
            _enemyHealth = GetComponent<EnemyHealth>();
            _enemyId = GetComponent<EnemyId>();
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
            _gamePoolManager = GamePoolManager.Instance;
        }

        #endregion
    }
}