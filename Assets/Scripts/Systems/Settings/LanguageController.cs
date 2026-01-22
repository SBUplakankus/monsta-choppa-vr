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
        private readonly LocaleEventChannel _onLocaleChangeRequested = GameEvents.OnLocaleChangeRequested;

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
            _onLocaleChangeRequested.Subscribe(HandleLocaleChangeRequested);
        }

        private void OnDisable()
        {
            _onLocaleChangeRequested.Unsubscribe(HandleLocaleChangeRequested);
        }
    }
}