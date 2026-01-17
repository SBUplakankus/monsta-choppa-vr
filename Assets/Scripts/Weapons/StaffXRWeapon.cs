using Constants;
using UnityEngine;
using Weapons.Projectiles;

namespace Weapons
{
    /// <summary>
    /// Staff/Wand weapon for casting spells (fireballs, etc.).
    /// VR-optimized with spell projectile pooling.
    /// Works for both player and enemy mages.
    /// </summary>
    public class StaffXRWeapon : XRWeaponBase
    {
        #region Fields

        [Header("Spell Settings")]
        [SerializeField] private ProjectileData spellProjectile;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float castCooldown = 1f;
        [SerializeField] private float chargeTime = 0.5f;
        
        [Header("Spell Pool")]
        [SerializeField] private int spellPoolSize = 10;

        private float _chargeProgress;
        private bool _isCharging;
        
        // Object pool for spell projectiles - VR performance optimization
        private Projectile[] _spellPool;
        private int _currentSpellIndex;

        // Cached references
        private Transform _transform;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current charge progress (0 to 1).
        /// </summary>
        public float ChargeProgress => _chargeProgress;

        /// <summary>
        /// Returns true if a spell is being charged.
        /// </summary>
        public bool IsCharging => _isCharging;

        /// <summary>
        /// Returns true if the spell is fully charged.
        /// </summary>
        public bool IsFullyCharged => _chargeProgress >= 1f;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
            InitializeSpellPool();
        }

        private void Update()
        {
            if (!IsHeld) return;

            if (_isCharging)
            {
                _chargeProgress = Mathf.Clamp01(_chargeProgress + Time.deltaTime / chargeTime);
            }
        }

        #endregion

        #region Spell Pool

        /// <summary>
        /// Initializes the spell projectile pool for VR performance.
        /// </summary>
        private void InitializeSpellPool()
        {
            if (spellProjectile == null || spellProjectile.Prefab == null) return;

            _spellPool = new Projectile[spellPoolSize];
            
            for (int i = 0; i < spellPoolSize; i++)
            {
                var spellGO = Instantiate(spellProjectile.Prefab, _transform);
                spellGO.SetActive(false);
                
                var projectile = spellGO.GetComponent<Projectile>();
                if (projectile == null)
                    projectile = spellGO.AddComponent<Projectile>();
                
                projectile.Initialize(spellProjectile);
                _spellPool[i] = projectile;
            }
        }

        /// <summary>
        /// Gets the next available spell from the pool.
        /// </summary>
        private Projectile GetSpellFromPool()
        {
            if (_spellPool == null || _spellPool.Length == 0) return null;

            for (int i = 0; i < spellPoolSize; i++)
            {
                _currentSpellIndex = (_currentSpellIndex + 1) % spellPoolSize;
                var spell = _spellPool[_currentSpellIndex];
                
                if (spell != null && !spell.gameObject.activeInHierarchy)
                {
                    return spell;
                }
            }

            // Force recycle oldest
            _currentSpellIndex = (_currentSpellIndex + 1) % spellPoolSize;
            var oldestSpell = _spellPool[_currentSpellIndex];
            oldestSpell?.OnDespawn();
            return oldestSpell;
        }

        #endregion

        #region Casting

        /// <summary>
        /// Starts charging a spell.
        /// </summary>
        public void StartCharge()
        {
            if (!IsHeld || !CanAttack) return;
            
            _isCharging = true;
            _chargeProgress = 0f;
        }

        /// <summary>
        /// Releases the charged spell.
        /// </summary>
        public void ReleaseSpell()
        {
            if (!_isCharging) return;
            
            if (_chargeProgress > 0.2f) // Minimum charge threshold
            {
                CastSpell();
            }
            
            _isCharging = false;
            _chargeProgress = 0f;
        }

        /// <summary>
        /// Casts the spell projectile.
        /// </summary>
        private void CastSpell()
        {
            if (castPoint == null) return;

            var spell = GetSpellFromPool();
            if (spell == null) return;

            spell.transform.SetParent(null);
            
            // Damage multiplier based on charge
            var damageMultiplier = 0.5f + (_chargeProgress * 0.5f);
            spell.Launch(castPoint.position, castPoint.forward, damageMultiplier);

            TriggerHapticFeedback();
        }

        /// <summary>
        /// Quick cast without charging (for enemies or quick attacks).
        /// </summary>
        public void QuickCast()
        {
            if (!CanAttack) return;
            
            _chargeProgress = 1f;
            CastSpell();
            _chargeProgress = 0f;
        }

        #endregion

        #region Actions

        public override void PrimaryAction()
        {
            if (!_isCharging)
            {
                StartCharge();
            }
        }

        public override void SecondaryAction()
        {
            ReleaseSpell();
        }

        #endregion

        #region Lifecycle

        protected override void OnEquipped()
        {
            base.OnEquipped();
            _chargeProgress = 0f;
            _isCharging = false;
        }

        protected override void OnUnequipped()
        {
            base.OnUnequipped();
            _chargeProgress = 0f;
            _isCharging = false;
        }

        #endregion
    }
}
