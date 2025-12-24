using Characters.Base;
using UnityEngine;

namespace Weapons
{
    public class WeaponHitbox : MonoBehaviour
    {
        [SerializeField] private XRWeaponBase weapon;

        private void Reset()
        {
            weapon = GetComponentInParent<XRWeaponBase>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!weapon || !weapon.IsActive || !weapon.CanAttack) return;

            if (other.TryGetComponent<IDamageable>(out var target))
            {
                weapon.ProcessHit(target, transform.position);
            }
        }
    }
}
