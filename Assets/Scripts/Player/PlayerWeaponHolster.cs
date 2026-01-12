using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Weapons;

namespace Player
{
    /// <summary>
    /// Manages the player's weapon holsters and interaction with weapons.
    /// </summary>
    public class PlayerWeaponHolster : MonoBehaviour
    {
        [Header("Holsters")]
        [SerializeField] private XRSocketInteractor primaryHolster; // Holster 1
        [SerializeField] private XRSocketInteractor secondaryHolster; // Holster 2

        private XRBaseInteractable _primaryWeapon; // The currently holstered weapon in primary slot
        private XRBaseInteractable _secondaryWeapon; // The currently holstered weapon in secondary slot

        private void Start()
        {
            // Subscribe to holster events
            SubscribeToHolsterEvents(primaryHolster, true);
            SubscribeToHolsterEvents(secondaryHolster, false);
        }

        /// <summary>
        /// Subscribes to socket interactor events for weapon holstering.
        /// </summary>
        private void SubscribeToHolsterEvents(XRSocketInteractor socket, bool isPrimaryHolster)
        {
            socket.selectEntered.AddListener((SelectEnterEventArgs args) =>
            {
                XRBaseInteractable weapon = args.interactableObject as XRBaseInteractable;
                if (weapon == null) return;

                // Holster the weapon
                if (isPrimaryHolster)
                {
                    _primaryWeapon = weapon;
                    Debug.Log($"[Holster] Primary Weapon Holstered: {weapon.name}");
                }
                else
                {
                    _secondaryWeapon = weapon;
                    Debug.Log($"[Holster] Secondary Weapon Holstered: {weapon.name}");
                }
            });

            socket.selectExited.AddListener((SelectExitEventArgs args) =>
            {
                XRBaseInteractable weapon = args.interactableObject as XRBaseInteractable;
                if (weapon == null) return;

                // Remove the weapon from the holster
                if (isPrimaryHolster && _primaryWeapon == weapon)
                {
                    _primaryWeapon = null;
                    Debug.Log("[Holster] Primary Weapon Removed");
                }
                else if (!isPrimaryHolster && _secondaryWeapon == weapon)
                {
                    _secondaryWeapon = null;
                    Debug.Log("[Holster] Secondary Weapon Removed");
                }
            });
        }

        /// <summary>
        /// Checks if the player has a weapon holstered in the specified slot.
        /// </summary>
        /// <param name="isPrimary">True for primary, false for secondary holster.</param>
        /// <returns>True if there’s a weapon holstered, otherwise false.</returns>
        public bool HasWeaponHolstered(bool isPrimary)
        {
            return isPrimary ? _primaryWeapon != null : _secondaryWeapon != null;
        }

        /// <summary>
        /// Gets the holstered weapon in the specified slot.
        /// </summary>
        /// <param name="isPrimary">True for primary, false for secondary holster.</param>
        /// <returns>The holstered weapon or null if the slot is empty.</returns>
        public XRBaseInteractable GetHolsteredWeapon(bool isPrimary)
        {
            return isPrimary ? _primaryWeapon : _secondaryWeapon;
        }
    }
}