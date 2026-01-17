using Constants;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Melee weapon implementation for VR combat.
    /// Tracks swing velocity for damage multipliers and provides haptic feedback.
    /// </summary>
    public class MeleeXRWeapon : XRWeaponBase
    {
        #region Fields

        [Header("Melee Settings")]
        [SerializeField] private float minSwingVelocity = GameConstants.MinSwingVelocity;
        [SerializeField] private float maxSwingVelocity = GameConstants.MaxSwingVelocity;

        private Rigidbody _rb;
        private Vector3 _previousPosition;
        private float _currentVelocity;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current swing velocity magnitude in m/s.
        /// </summary>
        public float SwingVelocity => _currentVelocity;

        /// <summary>
        /// Returns true if the weapon is being swung fast enough to deal damage.
        /// </summary>
        public bool IsSwinging => _currentVelocity >= minSwingVelocity;

        /// <summary>
        /// Gets the damage multiplier based on current swing velocity.
        /// Range from MinVelocityDamageMultiplier to MaxVelocityDamageMultiplier.
        /// </summary>
        public float VelocityDamageMultiplier
        {
            get
            {
                if (_currentVelocity < minSwingVelocity)
                    return 0f;

                var normalizedVelocity = Mathf.InverseLerp(minSwingVelocity, maxSwingVelocity, _currentVelocity);
                return Mathf.Lerp(
                    GameConstants.MinVelocityDamageMultiplier,
                    GameConstants.MaxVelocityDamageMultiplier,
                    normalizedVelocity
                );
            }
        }

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            _previousPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (!IsHeld)
            {
                _currentVelocity = 0f;
                return;
            }

            // Calculate velocity from position change (more reliable than rigidbody velocity in VR)
            var currentPosition = transform.position;
            _currentVelocity = (currentPosition - _previousPosition).magnitude / Time.fixedDeltaTime;
            _previousPosition = currentPosition;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Primary action for melee weapons - currently handled by physics-based swinging.
        /// Override for special attack moves.
        /// </summary>
        public override void PrimaryAction()
        {
            // Melee weapons rely on physics-based swinging
            // This can be used for special attack moves if needed
        }

        /// <summary>
        /// Secondary action for melee weapons.
        /// Can be used for blocking, parrying, or special moves.
        /// </summary>
        public override void SecondaryAction()
        {
            // Can be used for blocking or special moves
        }

        #endregion

        #region Overrides

        protected override void OnEquipped()
        {
            base.OnEquipped();
            _previousPosition = transform.position;
            _currentVelocity = 0f;
        }

        protected override void OnUnequipped()
        {
            base.OnUnequipped();
            _currentVelocity = 0f;
        }

        #endregion
    }
}
