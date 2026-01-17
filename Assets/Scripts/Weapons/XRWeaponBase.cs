using Characters.Base;
using Pooling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Weapons
{
    /// <summary>
    /// Base class for all VR weapons. Handles XR interaction, cooldowns, and damage processing.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public abstract class XRWeaponBase : MonoBehaviour
    {
        #region Fields

        [Header("Weapon Data")]
        [SerializeField] protected WeaponData data;

        [Header("Haptic Feedback")]
        [SerializeField] protected bool enableHaptics = true;

        private XRGrabInteractable _grab;
        private XRBaseInteractor _currentInteractor;
        protected bool IsHeld;

        private float _lastAttackTime;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the weapon data ScriptableObject.
        /// </summary>
        public WeaponData Data => data;

        /// <summary>
        /// Returns true if the weapon is currently held by the player.
        /// </summary>
        public bool IsActive => IsHeld;

        /// <summary>
        /// Returns true if enough time has passed to allow another attack.
        /// </summary>
        public bool CanAttack => Time.time >= _lastAttackTime + data.AttackCooldown;

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _grab.selectEntered.AddListener(OnGrab);
            _grab.selectExited.AddListener(OnRelease);
        }

        protected virtual void OnDestroy()
        {
            if (_grab != null)
            {
                _grab.selectEntered.RemoveListener(OnGrab);
                _grab.selectExited.RemoveListener(OnRelease);
            }
        }

        protected virtual void OnGrab(SelectEnterEventArgs args)
        {
            IsHeld = true;
            _currentInteractor = args.interactorObject as XRBaseInteractor;
            OnEquipped();
        }

        protected virtual void OnRelease(SelectExitEventArgs args)
        {
            IsHeld = false;
            _currentInteractor = null;
            OnUnequipped();
        }

        /// <summary>
        /// Call this when performing an attack. Updates last attack time.
        /// </summary>
        private void RegisterAttack()
        {
            _lastAttackTime = Time.time;
        }

        /// <summary>
        /// Processes a hit on a damageable target with optional damage multiplier.
        /// </summary>
        /// <param name="target">The damageable target.</param>
        /// <param name="hitPoint">World position of the hit.</param>
        /// <param name="hitRotation">Rotation for VFX orientation.</param>
        /// <param name="damageMultiplier">Multiplier for damage (e.g., from swing velocity).</param>
        public void ProcessHit(IDamageable target, Vector3 hitPoint, Quaternion hitRotation, float damageMultiplier = 1f)
        {
            if (!CanAttack) return;
            
            RegisterAttack();
            
            // Calculate final damage with multiplier
            var finalDamage = Mathf.RoundToInt(data.TotalDamage * damageMultiplier);
            finalDamage = Mathf.Max(1, finalDamage); // Ensure at least 1 damage
            
            target.TakeDamage(finalDamage);
            
            // Spawn VFX and audio
            if (data.HitVFX != null)
                GamePoolManager.Instance?.GetParticlePrefab(data.HitVFX, hitPoint, hitRotation);
            
            if (data.HitSfx != null)
                GamePoolManager.Instance?.GetWorldAudioPrefab(data.HitSfx, hitPoint);

            // Haptic feedback
            TriggerHapticFeedback();
        }

        /// <summary>
        /// Triggers haptic feedback on the controller holding this weapon.
        /// </summary>
        protected void TriggerHapticFeedback()
        {
            if (!enableHaptics || data == null || _currentInteractor == null)
                return;

            // Try to get the XR controller for haptics
            if (_currentInteractor.TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>(out var inputInteractor))
            {
                inputInteractor.SendHapticImpulse(data.HapticStrength, data.HapticDuration);
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Called by input routing elsewhere (player, hand, etc.)
        /// </summary>
        public abstract void PrimaryAction();

        /// <summary>
        /// Called by input routing elsewhere (player, hand, etc.)
        /// </summary>
        public abstract void SecondaryAction();

        protected virtual void OnEquipped() { }
        protected virtual void OnUnequipped() { }

        #endregion
    }
}
