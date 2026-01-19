using Data.Core;
using Databases;
using UnityEngine;
using Weapons;

namespace Data.Weapons
{
    /// <summary>
    /// Weapon modifier ScriptableObject for adding elemental effects and stat bonuses.
    /// Modifiers can be stacked on weapons to create unique combinations.
    /// Works with both player and enemy weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponModifier", menuName = "Scriptable Objects/Data/Weapons/Weapon Modifier")]
    public class WeaponModifierData : ScriptableObject
    {
        #region Fields

        [Header("Identity")]
        [SerializeField] private string modifierId;
        public string modifierName;
        
        [Header("Damage")]
        public DamageType addedDamageType;
        public int damageBonus = 0;
        [Range(0f, 1f)]
        public float damageMultiplier = 0f;
        
        [Header("Speed")]
        public float speedBonus = 0f;
        [Range(-0.5f, 0.5f)]
        public float cooldownReduction = 0f;
        
        [Header("Visual Effects")]
        public GameObject visualEffect;
        public Color trailColor = Color.white;
        public Color glowColor = Color.white;
        [Range(0f, 2f)]
        public float glowIntensity = 1f;
        
        [Header("Hit Effects")]
        public ParticleData onHitVFX;
        public WorldAudioData onHitSfx;
        
        [Header("Description")]
        [TextArea]
        public string description;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the unique identifier for this modifier.
        /// </summary>
        public string ModifierId => modifierId;

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(modifierId))
            {
                modifierId = modifierName?.ToLowerInvariant().Replace(" ", "_");
            }
            damageBonus = Mathf.Max(0, damageBonus);
        }
#endif
    }
}
