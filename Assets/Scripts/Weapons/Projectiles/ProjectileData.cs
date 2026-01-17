using Audio;
using Constants;
using Databases;
using UnityEngine;

namespace Weapons.Projectiles
{
    /// <summary>
    /// ScriptableObject containing configuration data for projectiles (arrows, bolts, etc.).
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/Weapons/Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        #region Fields

        [Header("Identity")]
        [SerializeField] private string projectileId;
        [SerializeField] private string displayName;
        [SerializeField] private GameObject prefab;

        [Header("Stats")]
        [SerializeField] private int baseDamage = 15;
        [SerializeField] private float speed = GameConstants.DefaultProjectileSpeed;
        [SerializeField] private float lifetime = GameConstants.DefaultProjectileLifetime;
        [SerializeField] private float gravityMultiplier = 0.5f;
        [SerializeField] private DamageType damageType = DamageType.Physical;

        [Header("Visual / Audio")]
        [SerializeField] private WorldAudioData hitSfx;
        [SerializeField] private WorldAudioData flightSfx;
        [SerializeField] private ParticleData hitVFX;
        [SerializeField] private ParticleData trailVFX;

        #endregion

        #region Properties

        public string ProjectileId => projectileId;
        public string DisplayName => displayName;
        public GameObject Prefab => prefab;
        
        public int BaseDamage => baseDamage;
        public float Speed => speed;
        public float Lifetime => lifetime;
        public float GravityMultiplier => gravityMultiplier;
        public DamageType DamageType => damageType;
        
        public WorldAudioData HitSfx => hitSfx;
        public WorldAudioData FlightSfx => flightSfx;
        public ParticleData HitVFX => hitVFX;
        public ParticleData TrailVFX => trailVFX;

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            baseDamage = Mathf.Max(1, baseDamage);
            speed = Mathf.Max(1f, speed);
            lifetime = Mathf.Max(0.5f, lifetime);
            gravityMultiplier = Mathf.Max(0f, gravityMultiplier);
        }
#endif
    }
}
