using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Attributes
{
    /// <summary>
    /// Scriptable object representing an integer attribute with data binding support.
    /// Implements <see cref="INotifyBindablePropertyChanged"/> for property change notifications.
    /// </summary>
    [CreateAssetMenu(fileName = "IntAttribute", menuName = "Scriptable Objects/Attributes/Int")]
    public class IntAttribute : ScriptableObject, INotifyBindablePropertyChanged
    {
        #region Fields
        
        [SerializeField] private int value;
        
        /// <inheritdoc cref="INotifyBindablePropertyChanged.propertyChanged"/>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        public event Action<int> OnValueChanged;

        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets or sets the integer value of this attribute.
        /// Triggers <see cref="propertyChanged"/> notification when value is modified.
        /// </summary>
        /// <value>The current integer value.</value>
        [CreateProperty]
        public int Value
        {
            get => value;
            set 
            {
                if (this.value == value) return;
                this.value = value;
                OnValueChanged?.Invoke(this.value);
                Notify();
            }
        }
        
        /// <summary>
        /// Gets the display name for this attribute.
        /// </summary>
        /// <value>The attribute name string.</value>
        public string AttributeName => name;
        
        #endregion
        
        #region Methods

        private void Notify([CallerMemberName] string property = "") => propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        
        /// <summary>
        /// Resets the attribute value to zero.
        /// </summary>
        public void Reset() => Value = 0;
        
        /// <summary>
        /// Forces a property change notification for the <see cref="Value"/> property.
        /// </summary>
        public void Refresh() => Notify(nameof(Value));
        
        /// <summary>
        /// Calculates the percentage of current value relative to a maximum.
        /// </summary>
        /// <param name="maxValue">The maximum value for percentage calculation.</param>
        /// <returns>The current value as a percentage (0.0 to 1.0).</returns>
        public float GetPercentage(int maxValue) => (float)Value / maxValue;
        
        /// <summary>
        /// Checks if the current value is at least the specified amount.
        /// </summary>
        /// <param name="amount">The minimum threshold value.</param>
        /// <returns>True if <see cref="Value"/> is greater than or equal to amount.</returns>
        public bool IsAtLeast(int amount) => Value >= amount;
        
        /// <summary>
        /// Checks if the current value is at most the specified amount.
        /// </summary>
        /// <param name="amount">The maximum threshold value.</param>
        /// <returns>True if <see cref="Value"/> is less than or equal to amount.</returns>
        public bool IsAtMost(int amount) => Value <= amount;
        
        /// <summary>
        /// Checks if the current value is within the specified range (inclusive).
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>True if <see cref="Value"/> is between min and max.</returns>
        public bool IsBetween(int min, int max) => Value >= min && Value <= max;
        
        /// <summary>
        /// Checks if the current value exactly matches the specified amount.
        /// </summary>
        /// <param name="amount">The exact value to compare against.</param>
        /// <returns>True if <see cref="Value"/> equals amount.</returns>
        public bool IsExactly(int amount) => Value == amount;
        
        #endregion
    }
}