using Data.Weapons;
using UnityEngine;

namespace Data.Core
{
    /// <summary>
    /// ScriptableObject containing configuration data for enemy characters.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Data/Core/Enemy")]
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

        [SerializeField] private WorldAudioData[] ambientSfx;
        [SerializeField] private WorldAudioData[] hitSfx;
        [SerializeField] private WorldAudioData[] deathSfx;
        [SerializeField] private ParticleData deathVFX;

        [Header("Combat")] 
        [SerializeField] private WeaponData weapon;

        #endregion
        
        #region Methods
        
        private WorldAudioData GetAmbientSfx()
        {
            if(ambientSfx == null) return null;
            var sfx = Random.Range(0, ambientSfx.Length);
            return ambientSfx[sfx];
        }

        private WorldAudioData GetHitSfx()
        {
            if(hitSfx == null) return null;
            var sfx = Random.Range(0, hitSfx.Length);
            return hitSfx[sfx];
        }
        
        private WorldAudioData GetDeathSfx()
        {
            if(deathSfx == null) return null;
            var sfx = Random.Range(0, deathSfx.Length);
            return deathSfx[sfx];
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Gets the unique identifier for this enemy type.
        /// </summary>
        /// <value>The enemy ID string.</value>
        public string EnemyId => enemyId;
        
        /// <summary>
        /// Gets the maximum health value for this enemy.
        /// </summary>
        /// <value>The health points.</value>
        public int MaxHealth => maxHealth;
        
        /// <summary>
        /// Gets the movement speed for this enemy.
        /// </summary>
        /// <value>Speed in world units per second.</value>
        public float MoveSpeed => moveSpeed;
        
        /// <summary>
        /// Gets the prefab GameObject for this enemy.
        /// </summary>
        /// <value>The enemy prefab.</value>
        public GameObject Prefab => prefab;
        
        /// <summary>
        /// Gets a random ambient sound effect for this enemy.
        /// </summary>
        /// <value>A <see cref="WorldAudioData"/> or null if none configured.</value>
        public WorldAudioData AmbientSfx => GetAmbientSfx();
        
        /// <summary>
        /// Gets a random hit sound effect for this enemy.
        /// </summary>
        /// <value>A <see cref="WorldAudioData"/> or null if none configured.</value>
        public WorldAudioData HitSfx => GetHitSfx();
        
        /// <summary>
        /// Gets a random death sound effect for this enemy.
        /// </summary>
        /// <value>A <see cref="WorldAudioData"/> or null if none configured.</value>
        public WorldAudioData DeathSfx => GetDeathSfx();
        
        /// <summary>
        /// Gets the visual effect to play when this enemy dies.
        /// </summary>
        /// <value>The <see cref="ParticleData"/> for death effects.</value>
        public ParticleData DeathVFX => deathVFX;
        
        /// <summary>
        /// Gets the weapon data for this enemy's attacks.
        /// </summary>
        /// <value>The <see cref="WeaponData"/> for combat.</value>
        public WeaponData Weapon => weapon;
        
        #endregion
    }
}