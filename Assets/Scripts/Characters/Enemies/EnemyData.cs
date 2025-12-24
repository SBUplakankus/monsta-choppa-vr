using UnityEngine;
using Weapons;

namespace Characters.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Characters/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        #region Fields
        
        [Header("Identity")]
        [SerializeField] private string enemyId;

        [Header("Stats")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float moveSpeed = 3.5f;

        [Header("Presentation")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private AudioClip deathSfx;

        [Header("Combat")] 
        [SerializeField] private WeaponData weapon;

        #endregion
        
        #region Properties

        // Public, read-only accessors
        public string EnemyId => enemyId;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public GameObject Prefab => prefab;
        public AudioClip DeathSfx => deathSfx;
        
        #endregion

#if UNITY_EDITOR

        private void OnValidate()
        {
            enemyId = enemyId?.Trim().ToLowerInvariant();
        }
        
#endif
    }
}
