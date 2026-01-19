using Data.Settings;
using Events;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Systems.Settings
{
    public class LanguageController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LanguageSettingsConfig languageSettingsConfig;
        
        [Header("Events")]
        [SerializeField] private LocaleEventChannel onLocaleChangeRequested;

        private void HandleLocaleChangeRequested(Locale locale)
        {
            languageSettingsConfig.Language = locale;
            SetLanguage();
        }

        private void SetLanguage()
        {
            if (LocalizationSettings.SelectedLocale == languageSettingsConfig.Language)
                return;

            LocalizationSettings.SelectedLocale = languageSettingsConfig.Language;
        }

        private void OnEnable()
        {
            SetLanguage();
            onLocaleChangeRequested.Subscribe(HandleLocaleChangeRequested);
        }

        private void OnDisable()
        {
            onLocaleChangeRequested.Unsubscribe(HandleLocaleChangeRequested);
        }
    }
}