using Constants;
using Factories;
using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game
{
    /// <summary>
    /// Manages the enemy health bar UI with visual feedback effects.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        #region Fields

        [Header("UI Elements")] 
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet styleSheet;
        [SerializeField] private ShakeSettings shakeSettings;
        
        private VisualElement _healthBarContainer;
        private VisualElement _healthBarBg;
        private VisualElement _healthBarFill;
        private IStyle _healthBarFillStyle;
        private Tween _shakeTween;
        
        #endregion

        #region Class Functions

        private void ShakeHealthBar()
        {
            if(_shakeTween.isAlive)
                _shakeTween.Complete();
            
            _shakeTween = Tween.ShakeLocalPosition(transform, shakeSettings);
        }
        
        /// <summary>
        /// Updates the health bar fill amount with visual feedback.
        /// </summary>
        /// <param name="value">Health percentage (0.0 to 1.0).</param>
        public void UpdateHealthBarValue(float value)
        {
            _healthBarFillStyle.width = new Length(value * 100f, LengthUnit.Percent);
            ShakeHealthBar();
        }
        
        private void Generate()
        {
            var root = uiDocument.rootVisualElement;
            root.Clear();

            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            var healthBar = UIToolkitFactory.CreateHealthBar();

            _healthBarContainer = healthBar.Container;
            _healthBarBg = healthBar.Background;
            _healthBarFill = healthBar.Fill;
            _healthBarFillStyle = _healthBarFill.style;

            root.Add(_healthBarContainer);
        }

        #endregion

        #region Unity Functions

        private void OnEnable()
        {
            Generate();
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (uiDocument == null) return;
            if (uiDocument.rootVisualElement == null) return;

            Generate();
        }

        private void OnDisable()
        {
            if(_shakeTween.isAlive)
                _shakeTween.Stop();
        }

        #endregion
    }
}