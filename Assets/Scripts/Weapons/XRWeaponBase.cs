using System.Collections;
using Characters.Base;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Weapons
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public abstract class XRWeaponBase : MonoBehaviour
    {
        #region Fields

        [Header("Weapon Data")]
        [SerializeField] protected WeaponData data;

        private XRGrabInteractable _grab;
        protected bool IsHeld;

        private float _lastAttackTime;

        #endregion

        #region Properties

        public bool IsActive => IsHeld;

        /// <summary>
        /// Returns true if enough time has passed to allow another attack
        /// </summary>
        public bool CanAttack => Time.time >= _lastAttackTime + data.AttackCooldown;

        #endregion

        #region Methods

        protected virtual void Awake()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _grab.selectEntered.AddListener(OnGrab);
            _grab.selectExited.AddListener(OnRelease);
        }

        protected virtual void OnDestroy()
        {
            _grab.selectEntered.RemoveListener(OnGrab);
            _grab.selectExited.RemoveListener(OnRelease);
        }

        protected virtual void OnGrab(SelectEnterEventArgs args)
        {
            IsHeld = true;
            OnEquipped();
        }

        protected virtual void OnRelease(SelectExitEventArgs args)
        {
            IsHeld = false;
            OnUnequipped();
        }

        /// <summary>
        /// Call this when performing an attack. Updates last attack time.
        /// </summary>
        private void RegisterAttack()
        {
            _lastAttackTime = Time.time;
        }

        public void ProcessHit(IDamageable target, Vector3 hitPoint)
        {
            if(!CanAttack) return;
            RegisterAttack();
            target.TakeDamage(data.TotalDamage);
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
