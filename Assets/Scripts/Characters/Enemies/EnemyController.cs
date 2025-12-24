using Databases;
using Events;
using Pooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Enemies
{
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

        #endregion
        
        #region Properties

        public EnemyData Data
        {
            get => enemyData;
            set => enemyData = value;
        }

        #endregion
        
        #region Class Functions

        private void HandleEnemyDeath()
        {
            GamePoolManager.Instance.ReturnEnemyPrefab(this);
        }

        private void InitEnemy()
        {
            _enemyId.ID = enemyData.EnemyId;
            _enemyHealth.OnSpawn(enemyData.MaxHealth, HandleEnemyDeath);
            _enemyMovement.OnSpawn(enemyData.MoveSpeed);
            _enemyAnimator.OnSpawn();
            
            _onEnemySpawned.Raise(this);
        }
        
        public void OnSpawn(EnemyData data)
        {
            enemyData = data;
            ValidateRequiredComponents();
            InitEnemy();
        }

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
