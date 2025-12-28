using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Weapons
{
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
        
            // Subscribe to select/deselect events
            _grabInteractable.selectEntered.AddListener(OnGrabbed);
            _grabInteractable.selectExited.AddListener(OnReleased);
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            // Weapon is now held, disable holster physics/constraints
            _isHolstered = false;
            EnablePhysics(true);
            // You could play a subtle "draw" sound effect here
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            // Check if it was released into a holster socket
            // This logic would connect to your socket interactor's events
            if (_rigidbody.isKinematic)
            {
                HolsterWeapon();
            }
        }

        private void HolsterWeapon()
        {
            _isHolstered = true;
            EnablePhysics(false);
            // Snap to holster position/rotation
            // Disable certain colliders to prevent interference
        }

        private void EnablePhysics(bool toggle)
        {
            _rigidbody.isKinematic = !toggle;
            foreach (var col in _colliders)
            {
                col.enabled = toggle;
            }
        }
    }
}