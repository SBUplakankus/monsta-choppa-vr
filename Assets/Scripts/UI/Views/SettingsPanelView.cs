using System;
using Constants;
using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class SettingsPanelView : IDisposable
    {
        #region Fields
        
        public Button CloseButton { get; private set; }
        public Button AudioTab { get; private set; }
        public Button VideoTab { get; private set; }
        public Button LanguageTab { get; private set; }

        public VisualElement Content { get; private set; }

        private VisualElement _container;

        #endregion

        #region Constructors

        public SettingsPanelView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root);
        }

        #endregion

        #region Methods
        

        private void GenerateUI(VisualElement root)
        {
            _container = UIToolkitFactory.CreateContainer(
                UIToolkitStyles.Container,
                UIToolkitStyles.PanelBody
            );

            // Header
            var header = UIToolkitFactory.CreateContainer(UIToolkitStyles.PanelHeader);
            header.Add(UIToolkitFactory.CreateLabel(
                LocalizationFactory.CreateString(LocalizationKeys.Settings),
                UIToolkitStyles.PanelTitle));
            
            CloseButton = UIToolkitFactory.CreateButton(LocalizationFactory.CreateString(LocalizationKeys.Close));
            header.Add(CloseButton);

            // Tabs
            var tabs = UIToolkitFactory.CreateContainer(UIToolkitStyles.TabBar);
            AudioTab = UIToolkitFactory.CreateButton(LocalizationFactory.CreateString(LocalizationKeys.Audio));
            VideoTab = UIToolkitFactory.CreateButton(LocalizationFactory.CreateString(LocalizationKeys.Video));
            LanguageTab = UIToolkitFactory.CreateButton(LocalizationFactory.CreateString(LocalizationKeys.Language));

            tabs.Add(AudioTab);
            tabs.Add(VideoTab);
            tabs.Add(LanguageTab);

            // Content
            Content = UIToolkitFactory.CreateContainer(UIToolkitStyles.PanelContent);

            _container.Add(header);
            _container.Add(tabs);
            _container.Add(Content);
            
            root.Add(_container);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_container == null) return;

            _container.RemoveFromHierarchy();
            _container = null;
        }

        #endregion
    }
}
