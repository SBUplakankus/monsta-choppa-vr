using System.Collections.Generic;
using Audio;
using Data.Core;
using Databases;
using UnityEngine;
using Weapons;

namespace Data.Weapons
{
    /// <summary>
    /// ScriptableObject containing all configuration data for a weapon.
    /// Used by both player and enemy weapons for a unified, data-driven system.
    /// Supports elemental modifiers and various weapon types.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Data/Weapons/Weapon")]
    public class WeaponData : ScriptableObject
    {
        #region Fields

        [Header("Identity")]
        [SerializeField] private string weaponID;
        [SerializeField] private string displayName;
        [SerializeField] private WeaponCategory category;
        [SerializeField] private WeaponRarity rarity;
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private Sprite icon;

        [Header("Base Stats")]
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float range = 2;
        [SerializeField] private int staminaCost = 10;
        [SerializeField] private DamageType damageType;

        [Header("VR Settings")]
        [SerializeField] private Vector3 gripPositionOffset;
        [SerializeField] private Vector3 gripRotationOffset;
        [SerializeField] private float hapticStrength = 0.5f;
        [SerializeField] private float hapticDuration = 0.1f;

        [Header("Visual / Audio")] 
        [SerializeField] private WorldAudioData[] hitSfx;
        [SerializeField] private ParticleData hitVFX;
        [SerializeField] private GameObject trailEffect;
        [SerializeField] private Color trailColor = Color.white;

        [Header("Modifiers")]
        [SerializeField] private List<WeaponModifierData> activeModifiers = new();

        [Header("Economy")]
        [SerializeField] private int purchasePrice = 100;
        [SerializeField] private int sellPrice = 50;
        [SerializeField] private bool isPurchasable = true;

        #endregion

        #region Methods

        private WorldAudioData GetHitSfx()
        {
            if (hitSfx == null || hitSfx.Length == 0) return null;
            var sfx = Random.Range(0, hitSfx.Length);
            return hitSfx[sfx];
        }

        /// <summary>
        /// Gets the combined trail color from base and modifiers.
        /// </summary>
        public Color GetEffectiveTrailColor()
        {
            if (activeModifiers == null || activeModifiers.Count == 0)
                return trailColor;

            // Use the first modifier's trail color if available
            foreach (var mod in activeModifiers)
            {
                if (mod != null && mod.trailColor != Color.white)
                    return mod.trailColor;
            }
            return trailColor;
        }

        /// <summary>
        /// Gets all damage types this weapon deals (base + modifiers).
        /// </summary>
        public List<DamageType> GetAllDamageTypes()
        {
            var types = new List<DamageType> { damageType };
            
            if (activeModifiers != null)
            {
                foreach (var mod in activeModifiers)
                {
                    if (mod != null && !types.Contains(mod.addedDamageType))
                        types.Add(mod.addedDamageType);
                }
            }
            
            return types;
        }

        #endregion

        #region Properties

        // Identity
        public string WeaponID => weaponID;
        public string DisplayName => displayName;
        public WeaponCategory Category => category;
        public WeaponRarity Rarity => rarity;
        public GameObject WeaponPrefab => weaponPrefab;
        public Sprite Icon => icon;

        // Base Stats
        public int BaseDamage => baseDamage;
        public float AttackCooldown => attackCooldown;
        public float Range => range;
        public int StaminaCost => staminaCost;
        public DamageType DamageType => damageType;

        // VR
        public Vector3 GripPositionOffset => gripPositionOffset;
        public Vector3 GripRotationOffset => gripRotationOffset;
        public float HapticStrength => hapticStrength;
        public float HapticDuration => hapticDuration;

        // Visual / Audio
        public WorldAudioData HitSfx => GetHitSfx();
        public ParticleData HitVFX => hitVFX;
        public GameObject TrailEffect => trailEffect;
        public Color TrailColor => trailColor;

        // Modifiers
        public IReadOnlyList<WeaponModifierData> ActiveModifiers => activeModifiers;

        // Economy
        public int PurchasePrice => purchasePrice;
        public int SellPrice => sellPrice;
        public bool IsPurchasable => isPurchasable;

        /// <summary>
        /// Calculates total damage including all modifier bonuses.
        /// </summary>
        public int TotalDamage
        {
            get
            {
                int total = baseDamage;
                float multiplier = 1f;
                
                if (activeModifiers != null)
                {
                    foreach (var mod in activeModifiers)
                    {
                        if (mod == null) continue;
                        total += mod.damageBonus;
                        multiplier += mod.damageMultiplier;
                    }
                }

                return Mathf.RoundToInt(total * multiplier);
            }
        }

        /// <summary>
        /// Calculates effective attack cooldown with modifier reductions.
        /// </summary>
        public float EffectiveCooldown
        {
            get
            {
                float cooldown = attackCooldown;
                
                if (activeModifiers != null)
                {
                    foreach (var mod in activeModifiers)
                    {
                        if (mod == null) continue;
                        cooldown *= (1f - mod.cooldownReduction);
                    }
                }

                return Mathf.Max(0.1f, cooldown);
            }
        }

        #endregion
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            weaponID = weaponID?.Trim().ToLowerInvariant();
            baseDamage = Mathf.Max(0, baseDamage);
            staminaCost = Mathf.Max(0, staminaCost);
            attackCooldown = Mathf.Max(0.1f, attackCooldown);
            purchasePrice = Mathf.Max(0, purchasePrice);
            sellPrice = Mathf.Max(0, sellPrice);
        }
#endif
    }
}
