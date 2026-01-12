using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Weapons
{
    /// <summary>
    /// Handles weapon behavior when holstered or equipped.
    /// </summary>
    public class WeaponHolsterController : MonoBehaviour
    {
        private XRGrabInteractable _grabInteractable;
        private Rigidbody _rigidbody;
        private Collider[] _colliders;
        private bool _isHolstered = false;

        private void Awake()
        {
            _grabInteractable = GetComponent<XRGrabInteractable>();
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>(true);

            // Subscribe to grab and release events
            _grabInteractable.selectEntered.AddListener(OnGrabbed);
            _grabInteractable.selectExited.AddListener(OnReleased);
        }

        /// <summary>
        /// Handles the weapon being grabbed by a player.
        /// </summary>
        /// <param name="args">Data about the selected object.</param>
        private void OnGrabbed(SelectEnterEventArgs args)
        {
            Debug.Log($"[Weapon] Grabbed: {name}");
            _isHolstered = false;
            EnableHolsterPhysics(true); // Restore physics for grabbing
        }

        /// <summary>
        /// Handles the weapon being released by a player.
        /// Checks for holster placement.
        /// </summary>
        /// <param name="args">Data about the deselected object.</param>
        private void OnReleased(SelectExitEventArgs args)
        {
            Debug.Log($"[Weapon] Released: {name}");

            // If released into a holster socket
            if (_rigidbody.isKinematic)
            {
                HolsterWeapon();
            }
        }

        /// <summary>
        /// Marks the weapon as holstered, disabling its physics.
        /// </summary>
        private void HolsterWeapon()
        {
            Debug.Log($"[Weapon] Holstered: {name}");
            _isHolstered = true;
            EnableHolsterPhysics(false);
        }

        /// <summary>
        /// Enables or disables weapon physics dynamically.
        /// </summary>
        /// <param name="toggle">True to enable physics, false to disable.</param>
        private void EnableHolsterPhysics(bool toggle)
        {
            _rigidbody.isKinematic = !toggle;
            foreach (var col in _colliders)
            {
                col.enabled = toggle;
            }
        }
    }
}