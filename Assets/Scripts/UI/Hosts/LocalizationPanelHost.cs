using System;
using System.Collections.Generic;
using Events;
using UI.Views;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class LocalizationPanelHost : MonoBehaviour
    {
        #region Fields

        [SerializeField] private LocaleEventChannel onLocaleChangeRequested;
        
        private Locale[] _availableLocales;
        private Action _unbindAll;

        #endregion

        #region Methods
        
        public void BindPanel(LocalizationPanelView view)
        {
            DisposeView();

            _availableLocales = LocalizationSettings.AvailableLocales.Locales.ToArray();

            BindLanguages(view);
        }

        private void BindLanguages(LocalizationPanelView view)
        {
            var unbinders = new List<Action>();

            for (int i = 0; i < view.LanguageButtons.Count; i++)
            {
                var index = i;
                var button = view.LanguageButtons[i];
                var locale = _availableLocales[i];

                button.clicked += OnClicked;
                unbinders.Add(() => button.clicked -= OnClicked);
                continue;

                void OnClicked()
                {
                    SetLocale(locale);
                }
            }

            _unbindAll = () =>
            {
                foreach (var unbind in unbinders)
                    unbind();
            };
        }

        private void SetLocale(Locale locale)
        {
            onLocaleChangeRequested.Raise(locale);
        }
        
        #endregion
        
        #region Unity Lifecycle

        private void OnDisable() => DisposeView();

        #endregion

        #region Dispose

        public void DisposeView()
        {
            _unbindAll?.Invoke();
            _unbindAll = null;
        }

        #endregion
    }
}
