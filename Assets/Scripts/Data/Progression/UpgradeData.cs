using Attributes;
using UnityEngine;

namespace Data.Progression
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/Progression/Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        #region Fields

        [Header("Upgrade Attributes")]
        [SerializeField] private FloatAttribute upgradeAttribute;
        
        [Header("Upgrade Parameters")]
        [SerializeField] private float upgradeValue;
        [SerializeField] private int upgradeCost;
        [SerializeField] private UpgradeData[] nextUpgrades;
        
        [Header("Upgrade UI Elements")]
        [SerializeField] private Sprite upgradeSprite;
        [SerializeField] private string upgradeNameKey;
        [SerializeField] private string upgradeDescriptionKey;

        #endregion
        
        #region Properties

        public string ID => name;
        public FloatAttribute Attribute => upgradeAttribute;
        
        public float Value => upgradeValue;
        public int Cost => upgradeCost;
        public UpgradeData[] NextUpgrades => nextUpgrades;
        
        public Sprite Sprite => upgradeSprite;
        public string NameKey => upgradeNameKey;
        public string DescriptionKey => upgradeDescriptionKey;
        
        #endregion
    }
}