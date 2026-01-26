using Interfaces;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Shield weapon for blocking attacks.
    /// Can be used by both player and enemies.
    /// VR-optimized with collision-based blocking and non-allocating physics.
    /// </summary>
    public class ShieldXRWeapon : XRWeaponBase
    {
        #region Fields

        [Header("Shield Settings")]
        [SerializeField] private float blockAngle = 60f;
        [SerializeField] private float blockDamageReduction = 0.75f;
        [SerializeField] private float perfectBlockWindow = 0.2f;
        [SerializeField] private float blockStaminaCost = 5f;
        
        [Header("Shield Bash")]
        [SerializeField] private float bashDamage = 10f;
        [SerializeField] private float bashCooldown = 1f;
        [SerializeField] private float bashForce = 500f;
        [SerializeField] private float bashRadius = 0.5f;

        private bool _isBlocking;
        private float _blockStartTime;
        private float _lastBashTime;

        // Cached
        private Transform _transform;
        
        // VR Optimization: Pre-allocated array for physics queries
        private const int MaxBashTargets = 4;
        private readonly Collider[] _bashBuffer = new Collider[MaxBashTargets];

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the shield is actively blocking.
        /// </summary>
        public bool IsBlocking => _isBlocking;

        /// <summary>
        /// Returns true if within the perfect block window for parrying.
        /// </summary>
        public bool IsPerfectBlock => _isBlocking && (Time.time - _blockStartTime) < perfectBlockWindow;

        /// <summary>
        /// Gets the current damage reduction multiplier.
        /// </summary>
        public float DamageReduction => IsPerfectBlock ? 1f : blockDamageReduction;

        /// <summary>
        /// Returns true if bash is available.
        /// </summary>
        public bool CanBash => Time.time >= _lastBashTime + bashCooldown;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
        }

        #endregion

        #region Blocking

        /// <summary>
        /// Checks if an incoming attack can be blocked based on angle.
        /// </summary>
        /// <param name="attackDirection">Direction of the incoming attack.</param>
        /// <returns>True if the attack is within block angle.</returns>
        public bool CanBlockAttack(Vector3 attackDirection)
        {
            if (!_isBlocking) return false;

            var shieldForward = _transform.forward;
            var angle = Vector3.Angle(-attackDirection, shieldForward);
            
            return angle <= blockAngle;
        }

        /// <summary>
        /// Calculates damage after shield block.
        /// </summary>
        /// <param name="incomingDamage">Original damage amount.</param>
        /// <returns>Reduced damage amount.</returns>
        public int CalculateBlockedDamage(int incomingDamage)
        {
            if (IsPerfectBlock)
            {
                // Perfect block negates all damage
                return 0;
            }

            return Mathf.RoundToInt(incomingDamage * (1f - blockDamageReduction));
        }

        /// <summary>
        /// Starts blocking.
        /// </summary>
        public void StartBlock()
        {
            _isBlocking = true;
            _blockStartTime = Time.time;
        }

        /// <summary>
        /// Stops blocking.
        /// </summary>
        public void StopBlock()
        {
            _isBlocking = false;
        }

        #endregion

        #region Shield Bash

        /// <summary>
        /// Performs a shield bash attack.
        /// VR-optimized: Uses non-allocating physics query.
        /// </summary>
        public void ShieldBash()
        {
            if (!CanBash) return;

            _lastBashTime = Time.time;

            // Detect enemies in front using non-allocating query
            var bashPoint = _transform.position + _transform.forward * 0.5f;
            var hitCount = Physics.OverlapSphereNonAlloc(bashPoint, bashRadius, _bashBuffer);

            for (int i = 0; i < hitCount; i++)
            {
                var col = _bashBuffer[i];
                
                // Apply damage
                if (col.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(Mathf.RoundToInt(bashDamage));
                }

                // Apply knockback
                if (col.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.AddForce(_transform.forward * bashForce, ForceMode.Impulse);
                }
            }

            TriggerHapticFeedback();
        }

        #endregion

        #region Actions

        public override void PrimaryAction()
        {
            if (!_isBlocking)
            {
                StartBlock();
            }
        }

        public override void SecondaryAction()
        {
            ShieldBash();
        }

        #endregion

        #region Lifecycle

        protected override void OnEquipped()
        {
            base.OnEquipped();
            _isBlocking = false;
        }

        protected override void OnUnequipped()
        {
            base.OnUnequipped();
            _isBlocking = false;
        }

        #endregion
    }
}
