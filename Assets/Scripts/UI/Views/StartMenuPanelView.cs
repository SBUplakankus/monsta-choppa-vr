using System;
using System.Collections.Generic;
using Constants;
using Factories;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class StartMenuPanelView : IDisposable
    {
        #region Fields

        private VisualElement _container;
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

        private void CreateButton(string key, Action onClick)
        {
            var button = UIToolkitFactory.CreateButton(key,() => onClick?.Invoke());
            _buttonContainer.Add(button);
        }

        private void GenerateUI(VisualElement root)
        {
            _container = UIToolkitFactory.CreateContainer(UIToolkitStyles.Container, UIToolkitStyles.PanelBody);

            var gameTitle = UIToolkitFactory.CreateLabel(LocalizationKeys.GameTitle, UIToolkitStyles.Header);
            _container.Add(gameTitle);
            
            _buttonContainer = UIToolkitFactory.CreateContainer(UIToolkitStyles.ButtonContainer);
            
            CreateButton(LocalizationKeys.Play, OnPlayClicked);
            CreateButton(LocalizationKeys.Settings, OnSettingsClicked);
            CreateButton(LocalizationKeys.Controls, OnControlsClicked);
            CreateButton(LocalizationKeys.Quit, OnQuitClicked);
            
            _container.Add(_buttonContainer);
            root.Add(_container);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_container != null)
            {
                _container.RemoveFromHierarchy();
                _container = null;
            }

            if (_buttonContainer != null)
            {
                _buttonContainer.RemoveFromHierarchy();
                _buttonContainer = null;
            }
        }

        #endregion
    }
}
