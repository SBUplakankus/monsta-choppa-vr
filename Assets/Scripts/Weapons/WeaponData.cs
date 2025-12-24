using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapons/Data")]
    public class WeaponData : ScriptableObject
    {
        #region Fields

        [Header("Identity")]
        [SerializeField] private string weaponID;
        [SerializeField] private string displayName;
        [SerializeField] private WeaponCategory category;
        [SerializeField] private WeaponRarity rarity;
        [SerializeField] private GameObject weaponPrefab;

        [Header("Base Stats")]
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int range = 2;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private DamageType damageType;

        [Header("VR Settings")]
        [SerializeField] private Vector3 gripPositionOffset;
        [SerializeField] private Vector3 gripRotationOffset;
        [SerializeField] private float hapticStrength = 0.5f;
        [SerializeField] private float hapticDuration = 0.1f;

        [Header("Visual / Audio")]
        [SerializeField] private string swingSoundKey;
        [SerializeField] private string hitSoundKey;
        [SerializeField] private string hitVFXKey;

        [Header("Modifiers")]
        [SerializeField] private List<WeaponModifier> activeModifiers = new();

        #endregion

        #region Properties

        // Identity
        public string WeaponID => weaponID;
        public string DisplayName => displayName;
        public WeaponCategory Category => category;
        public WeaponRarity Rarity => rarity;
        public GameObject WeaponPrefab => weaponPrefab;

        // Base Stats
        public int BaseDamage => baseDamage;
        public float AttackCooldown => attackCooldown;
        public int Range => range;
        public int StaminaCost => staminaCost;
        public DamageType DamageType => damageType;

        // VR
        public Vector3 GripPositionOffset => gripPositionOffset;
        public Vector3 GripRotationOffset => gripRotationOffset;
        public float HapticStrength => hapticStrength;
        public float HapticDuration => hapticDuration;

        // Visual / Audio
        public string SwingSoundKey => swingSoundKey;
        public string HitSoundKey => hitSoundKey;
        public string HitVFXKey => hitVFXKey;

        // Modifiers
        public IReadOnlyList<WeaponModifier> ActiveModifiers => activeModifiers;

        // Calculated
        public int TotalDamage
        {
            get
            {
                int total = baseDamage;
                foreach (var mod in activeModifiers)
                    total += mod.damageBonus;

                return total;
            }
        }

        #endregion
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            weaponID = weaponID?.Trim().ToLowerInvariant();
            baseDamage = Mathf.Max(0, baseDamage);
            staminaCost = Mathf.Max(0, staminaCost);
        }
#endif
    }
}
