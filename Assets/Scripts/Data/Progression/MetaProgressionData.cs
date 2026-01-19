using Attributes;
using UnityEngine;

namespace Data.Progression
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/Progression/Meta Progression")]
    public class MetaProgressionData : ScriptableObject
    {
        #region Fields

        [Header("Meta Progression Attributes")] 
        [SerializeField] private FloatAttribute expGain;
        [SerializeField] private FloatAttribute goldGain;
        [SerializeField] private FloatAttribute health;
        [SerializeField] private FloatAttribute armour;

        #endregion
        
        #region Properties
        
        public float ExpGain => expGain.Value;
        public float GoldGain => goldGain.Value;
        public int Health => (int)health.Value;
        public float Armour => armour.Value;
        
        #endregion
        
    }
}