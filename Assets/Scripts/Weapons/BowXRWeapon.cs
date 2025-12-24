using UnityEngine;

namespace Weapons
{
    public class BowXRWeapon : XRWeaponBase
    {
        [Header("Bow Settings")]
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private Transform arrowSpawn;

        private float _drawStrength;

        public override void PrimaryAction()
        {
            _drawStrength = Mathf.Clamp01(_drawStrength + Time.deltaTime);
        }

        public override void SecondaryAction()
        {
            FireArrow();
        }

        private void FireArrow()
        {
            var arrow = Instantiate(arrowPrefab, arrowSpawn.position, arrowSpawn.rotation);
            // Apply force based on drawStrength
            _drawStrength = 0f;
        }
    }
}
