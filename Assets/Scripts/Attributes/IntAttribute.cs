using UnityEngine;
using UnityEngine.Serialization;

namespace Attributes
{
    [CreateAssetMenu(fileName = "IntAttribute", menuName = "Scriptable Objects/Attributes/Int")]
    public class IntAttribute : ScriptableObject
    {
        #region Fields
        
        [SerializeField] private int value;

        #endregion
        
        #region Properties

        public int Value
        {
            get => value;
            set => this.value = value;
        }
        public int Add(int amount) => value + amount;
        public int Subtract(int amount) => value - amount;
        public float GetPercentage(int maxValue) => (float)value / maxValue;
        
        public bool IsAtLeast(int amount) => value >= amount;
        public bool IsAtMost(int amount) => value <= amount;
        public bool IsBetween(int min, int max) => value >= min && value <= max;
        public bool IsExactly(int amount) => value == amount;
        
        #endregion
    }
}
