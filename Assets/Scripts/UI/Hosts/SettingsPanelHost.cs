using System.Collections.Generic;
using UI.Extensions;
using UI.Views;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class SettingsPanelHost : BasePanelHost
    {
        #region Fields

        [Header("Panel Hosts")]
        [SerializeField] private AudioSettingsPanelHost audioSettingsPanelHost;
        [SerializeField] private VideoSettingsPanelHost videoSettingsPanelHost;
        [SerializeField] private LocalizationPanelHost localizationPanelHost;

        private SettingsPanelView _settingsView;
        private AudioSettingsPanelView _audioSettingsView;
        private VideoSettingsPanelView _videoSettingsView;
        private LocalizationPanelView _languageView;

        private Button _audioTab;
        private Button _videoTab;
        private Button _languageTab;
        
        #endregion

        #region Methods

        public override void Dispose()
        {
            UnbindTabs();
            _settingsView?.Dispose();
            _settingsView = null;
        }

        private void DisposeTabViews()
        {
            audioSettingsPanelHost.Dispose();
            _audioSettingsView?.Dispose();
            _audioSettingsView = null;
            
            videoSettingsPanelHost.DisposeView();
            _videoSettingsView?.Dispose();
            _videoSettingsView = null;
            
            localizationPanelHost.DisposeView();
            _languageView?.Dispose();
            _languageView = null;
        }

        private void ShowAudioTab()
        {
            DisposeTabViews();
            _audioSettingsView = new AudioSettingsPanelView(ContentRoot, styleSheet);
            audioSettingsPanelHost.BindViewSliders(_audioSettingsView);
        }

        private void ShowVideoTab()
        {
            DisposeTabViews();
            _videoSettingsView = new VideoSettingsPanelView(ContentRoot, styleSheet);
            videoSettingsPanelHost.BindPanel(_videoSettingsView);
        }

        private void ShowLanguageTab()
        {
            DisposeTabViews();
            _languageView = new LocalizationPanelView(ContentRoot, styleSheet, LocalizationSettings.AvailableLocales.Locales);
            localizationPanelHost.BindPanel(_languageView);
        }

        public override void Generate()
        {
            Dispose();

            _settingsView = new SettingsPanelView(
                uiDocument.rootVisualElement,
                styleSheet
            );
            
            _audioTab = _settingsView.AudioTab;
            _videoTab = _settingsView.VideoTab;
            _languageTab = _settingsView.LanguageTab;
            ContentRoot = _settingsView.Content;
            
            BindTabs();
            Show();
        }

        private void BindTabs()
        {
            UnbindTabs();
            _audioTab.clicked += ShowAudioTab;
            _videoTab.clicked += ShowVideoTab;
            _languageTab.clicked += ShowLanguageTab;
        }

        private void UnbindTabs()
        {
            if(_audioTab == null) return;
            _audioTab.clicked -= ShowAudioTab;
            _videoTab.clicked -= ShowVideoTab;
            _languageTab.clicked -= ShowLanguageTab;
        }
        
        #endregion

        
    }
}
