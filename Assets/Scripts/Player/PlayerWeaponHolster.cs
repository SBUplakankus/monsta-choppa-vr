using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Weapons;

namespace Player
{
    public class PlayerWeaponHolster : MonoBehaviour
    {
        [Header("Holsters")]
        [SerializeField] private XRSocketInteractor socketInteractor;
        [SerializeField] private WeaponHolsterController holsterWeapon;
        
        private XRBaseInteractable _currentHolsteredWeapon;
        
        public WeaponHolsterController HolsterWeapon { get; set; }
        
        private void Start()
    {
        if (socketInteractor == null)
        {
            socketInteractor = GetComponent<XRSocketInteractor>();
        }

        SubscribeToSocketEvents();
    }

    private void SubscribeToSocketEvents()
    {
        // When weapon is placed in holster
        socketInteractor.selectEntered.AddListener((SelectEnterEventArgs args) =>
        {
            _currentHolsteredWeapon = args.interactableObject as XRBaseInteractable;

            if (_currentHolsteredWeapon == null) return;
            Debug.Log($"Weapon Holstered: {holsterWeapon.name}");

            if (holsterWeapon != null)
            {
                // holsterWeapon.
            }
        });

        socketInteractor.selectExited.AddListener((SelectExitEventArgs args) =>
        {
            Debug.Log($"Weapon Removed: {holsterWeapon.name}");

            if (holsterWeapon != null)
            {
                // holsterWeapon.OnUnholstered();
            }

            if (_currentHolsteredWeapon == (XRBaseInteractable)args.interactableObject)
            {
                _currentHolsteredWeapon = null;
            }
        });
    }

    public bool HasWeaponHolstered()
    {
        return _currentHolsteredWeapon != null;
    }

    public XRBaseInteractable GetHolsteredWeapon()
    {
        return _currentHolsteredWeapon;
    }
    }
}