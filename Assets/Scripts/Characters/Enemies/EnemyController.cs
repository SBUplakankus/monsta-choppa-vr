using System;
using Databases;
using Events;
using Pooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Enemies
{
    /// <summary>
    /// Main controller for enemy entities, coordinating health, movement, animation, and events.
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
        /// Gets or sets the enemy configuration data.
        /// </summary>
        /// <value>The <see cref="EnemyData"/> containing enemy stats and settings.</value>
        public EnemyData Data
        {
            get => enemyData;
            set => enemyData = value;
        }

        public Action OnDeath { get; set; }

        #endregion
        
        #region Class Functions

        public void DebugKillEnemy()
        {
            HandleEnemyDeath();
        }

        private void HandleEnemyDeath()
        {
            _gamePoolManager.ReturnEnemyPrefab(this);
            _gamePoolManager.GetWorldAudioPrefab(enemyData.DeathSfx, transform.position);
            OnDeath?.Invoke();
        }

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
        /// Initializes the enemy with specific configuration data when spawned from pool.
        /// </summary>
        /// <param name="data">The <see cref="EnemyData"/> to configure this enemy.</param>
        public void OnSpawn(EnemyData data)
        {
            enemyData = data;
            InitEnemy();
        }

        /// <summary>
        /// Cleans up the enemy when returning to the object pool.
        /// </summary>
        public void OnDespawn()
        {
            _enemyAnimator.OnDespawn();
            _enemyMovement.OnDespawn();
            _enemyHealth.OnDespawn(HandleEnemyDeath);
            
            _onEnemyDespawned.Raise(this);
        }

        /// <summary>
        /// High priority update method for critical enemy logic.
        /// </summary>
        public void HighPriorityUpdate()
        {
            
        }

        /// <summary>
        /// Medium priority update method for non-critical enemy logic.
        /// </summary>
        public void MediumPriorityUpdate()
        {
            
        }
    
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

            if (!_onEnemySpawned)
                _onEnemySpawned = GameEvents.OnEnemySpawned;

            if (!_onEnemyDespawned)
                _onEnemyDespawned = GameEvents.OnEnemyDespawned;
        }
    
        private void CacheComponents()
        {
            _enemyHealth = GetComponent<EnemyHealth>();
            _enemyId = GetComponent<EnemyId>();
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
            _gamePoolManager = GamePoolManager.Instance;
        }
        
        #endregion
        
        #region Unity Functions

        private void Awake()
        {
            ValidateRequiredComponents();
            CacheComponents();
        }

        private void OnEnable()
        {
            InitEnemy();
        }
        
        #endregion
    }
}