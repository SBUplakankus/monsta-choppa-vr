using System;
using Constants;
using Factories;
using UI.Extensions;
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
            
            var tabs = UIToolkitFactory.CreateContainer(UIToolkitStyles.TabBar);
            AudioTab = UIToolkitFactory.CreateButton(LocalizationKeys.Audio, classNames: UIToolkitStyles.Tab);
            VideoTab = UIToolkitFactory.CreateButton(LocalizationKeys.Video, classNames: UIToolkitStyles.Tab);
            LanguageTab = UIToolkitFactory.CreateButton(LocalizationKeys.Language, classNames: UIToolkitStyles.Tab);

            tabs.Add(AudioTab);
            tabs.Add(VideoTab);
            tabs.Add(LanguageTab);
            _container.Add(tabs);

            Content = UIToolkitFactory.CreateContainer(UIToolkitStyles.TabContent);
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
