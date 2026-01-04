using System.Collections.Generic;
using Constants;
using Factories;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class LocalizationPanelView
    {
        private VisualElement _container;
        private ScrollView _scrollView;

        private readonly List<Button> _languageButtons = new();

        public IReadOnlyList<Button> LanguageButtons => _languageButtons;

        public LocalizationPanelView(
            VisualElement root,
            StyleSheet styleSheet,
            List<Locale> languages)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root, languages);
        }

        private void GenerateUI(VisualElement root, List<Locale> languages)
        {
            _container = UIToolkitFactory.CreateContainer(
                UIToolkitStyles.Container);

            var header = UIToolkitFactory.CreateContainer(
                UIToolkitStyles.PanelHeader);

            var title = UIToolkitFactory.CreateLabel(
                LocalizationKeys.LanguageSettings,
                UIToolkitStyles.PanelTitle);

            header.Add(title);
            _container.Add(header);

            var content = UIToolkitFactory.CreateContainer(
                UIToolkitStyles.PanelContent);

            _scrollView = UIToolkitFactory.CreateScrollView(UIToolkitStyles.LanguageScroll);

            foreach (var language in languages)
            {
                var button = UIToolkitFactory.CreateButton(
                    language.LocaleName,
                    classNames: UIToolkitStyles.LanguageRow);

                _languageButtons.Add(button);
                _scrollView.Add(button);
            }

            content.Add(_scrollView);
            _container.Add(content);
            root.Add(_container);
        }

        public void Dispose()
        {
            _container?.RemoveFromHierarchy();
            _container = null;
        }
    }
}