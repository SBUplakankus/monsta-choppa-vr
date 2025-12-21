using Databases;
using Events;
using Pooling;
using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        #region Fields

        [Header("Components")] 
        private EnemyMovement _enemyMovement;
        private EnemyAnimator _enemyAnimator;
        private EnemyAttack _enemyAttack;
        private EnemyHealth _enemyHealth;
        private EnemyData _enemyData;
        private EnemyId _enemyId;

        [Header("Enemy Events")] 
        private EnemyEventChannel _onEnemySpawned;
        private EnemyEventChannel _onEnemyDespawned;

        #endregion
        
        #region Properties

        public EnemyData Data => _enemyData;
        
        #endregion
        
        #region Class Functions

        private void HandleEnemyDeath()
        {
            GamePoolManager.Instance.ReturnEnemyPrefab(this);
        }
        
        public void InitEnemy(EnemyData data)
        {
            _enemyData = data;
            
            _enemyId.ID = _enemyData.EnemyId;
            _enemyHealth.InitHealth(_enemyData.MaxHealth, HandleEnemyDeath);
            _enemyMovement.InitMovement(_enemyData.MoveSpeed);
            _enemyAttack.InitAttack(10, 2);
            _enemyAnimator.InitAnimator();
            
            _onEnemySpawned.Raise(this);
        }

        public void ResetEnemy()
        {
            _enemyAnimator.ResetAnimator();
            _enemyMovement.ResetMovement();
            _enemyAttack.ResetAttack();
            _enemyHealth.ResetHealth(HandleEnemyDeath);
            
            _onEnemyDespawned.Raise(this);
        }

        public void UpdateEnemy()
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
            
            if (!GetComponent<EnemyAttack>())
                gameObject.AddComponent<EnemyAttack>();
            
            if (!GetComponent<EnemyAnimator>())
                gameObject.AddComponent<EnemyAnimator>();
        }
    
        private void CacheComponents()
        {
            _enemyHealth = GetComponent<EnemyHealth>();
            _enemyId = GetComponent<EnemyId>();
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyAttack = GetComponent<EnemyAttack>();
            _enemyAnimator = GetComponent<EnemyAnimator>();
        }
        
        #endregion
        
        #region Unity Functions

        private void Awake()
        {
            ValidateRequiredComponents();
            CacheComponents();
        }
        
        #endregion
    }
}
