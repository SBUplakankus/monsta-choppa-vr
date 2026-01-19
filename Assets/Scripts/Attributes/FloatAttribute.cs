using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Attributes
{
    /// <summary>
    /// Scriptable object representing a float attribute with data binding support.
    /// Value is clamped between 0 and 1, suitable for sliders or normalized settings.
    /// Implements <see cref="INotifyBindablePropertyChanged"/> for property change notifications.
    /// </summary>
    [CreateAssetMenu(fileName = "FloatAttribute", menuName = "Scriptable Objects/Attributes/Float")]
    public class FloatAttribute : ScriptableObject, INotifyBindablePropertyChanged
    {
        #region Fields

        private float value;

        /// <inheritdoc cref="INotifyBindablePropertyChanged.propertyChanged"/>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        public event Action<float> OnValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the normalized float value of this attribute (0..1).
        /// Triggers <see cref="propertyChanged"/> when modified.
        /// </summary>
        [CreateProperty]
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged?.Invoke(this.value);
                Notify();
            }
        }

        /// <summary>
        /// Display name for this attribute.
        /// </summary>
        public string AttributeName => name;

        #endregion

        #region Methods

        private void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Sets the value and triggers a notification even if the value hasn't changed.
        /// </summary>
        public void Refresh() => Notify(nameof(Value));

        /// <summary>
        /// Converts the current value to a percentage (0.0 to 1.0, clamped).
        /// </summary>
        public float GetPercentage() => Mathf.Clamp01(Value);

        /// <summary>
        /// Sets the value directly via percentage (0..1).
        /// </summary>
        /// <param name="percentage"></param>
        public void SetPercentage(float percentage) => Value = Mathf.Clamp01(percentage);

        #endregion
    }
}
