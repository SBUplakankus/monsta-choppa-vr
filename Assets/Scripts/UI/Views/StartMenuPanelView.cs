using System;
using System.Collections.Generic;
using Constants;
using Factories;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class StartMenuPanelView : BasePanelView
    {
        #region Fields

        private VisualElement _buttonContainer;
        
        public event Action OnPlayClicked;
        public event Action OnControlsClicked;
        public event Action OnSettingsClicked;
        public event Action OnQuitClicked;

        #endregion
        
        #region Properties
        
        public IReadOnlyList<Button> Buttons => _buttonContainer.Query<Button>().ToList();
        
        #endregion

        #region Constructors

        public StartMenuPanelView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root);
        }

        #endregion

        #region Methods

        private Button CreateButton(string key)
        {
            var button = UIToolkitFactory.CreateButton(key, null);
            _buttonContainer.Add(button);
            return button;
        }

        private void CreateButtons()
        {
            CreateButton(LocalizationKeys.Play)
                .clicked += () => OnPlayClicked?.Invoke();

            CreateButton(LocalizationKeys.Settings)
                .clicked += () => OnSettingsClicked?.Invoke();

            CreateButton(LocalizationKeys.Controls)
                .clicked += () => OnControlsClicked?.Invoke();

            CreateButton(LocalizationKeys.Quit)
                .clicked += () => OnQuitClicked?.Invoke();
        }

        protected sealed override void GenerateUI(VisualElement root)
        {
            Container = UIToolkitFactory.CreateContainer(UIToolkitStyles.Container, UIToolkitStyles.PanelBody);

            var gameTitle = UIToolkitFactory.CreateLabel(LocalizationKeys.GameTitle, UIToolkitStyles.Header);
            Container.Add(gameTitle);
            
            _buttonContainer = UIToolkitFactory.CreateContainer(UIToolkitStyles.ButtonContainer);
            
            CreateButtons();
            
            Container.Add(_buttonContainer);
            root.Add(Container);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            if (_buttonContainer == null) return;
            _buttonContainer.RemoveFromHierarchy();
            _buttonContainer = null;
        }

        #endregion
    }
}
