using System;
using Constants;
using Factories;
using UI.Extensions;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class SettingsPanelView : BasePanelView
    {
        #region Fields
        
        public Button AudioTab { get; private set; }
        public Button VideoTab { get; private set; }
        public Button LanguageTab { get; private set; }

        public VisualElement Content { get; private set; }

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

        protected sealed override void GenerateUI(VisualElement root)
        {
            Container = UIToolkitFactory.CreateContainer(
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
            Container.Add(tabs);

            Content = UIToolkitFactory.CreateContainer(UIToolkitStyles.TabContent);
            Container.Add(Content);
            
            root.Add(Container);
        }

        #endregion
    }
}
