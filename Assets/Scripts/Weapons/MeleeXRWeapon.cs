using Characters.Base;
using UnityEngine;

namespace Weapons
{
    public class MeleeXRWeapon : XRWeaponBase
    {
        private Rigidbody _rb;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
        }

        public override void PrimaryAction() { }
        public override void SecondaryAction() { }
    }
}
