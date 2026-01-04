using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class SettingsPanelHost : MonoBehaviour
    {
        #region Fields

        [Header("UI Toolkit")] 
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet styleSheet;
        
        [Header("Panel Hosts")]
        [SerializeField] private AudioSettingsPanelHost audioSettingsPanelHost;
        [SerializeField] private VideoSettingsPanelHost videoSettingsPanelHost;

        private SettingsPanelView _settingsView;
        private AudioSettingsPanelView _audioSettingsView;
        private VideoSettingsPanelView _videoSettingsView;

        private Button _audioTab;
        private Button _videoTab;
        private Button _languageTab;
        private VisualElement _contentRoot;

        #endregion

        #region Methods

        private void DisposeView()
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
        }

        private void ShowAudioTab()
        {
            DisposeTabViews();
            _audioSettingsView = new AudioSettingsPanelView(_contentRoot, styleSheet);
            audioSettingsPanelHost.BindViewSliders(_audioSettingsView);
        }

        private void ShowVideoTab()
        {
            DisposeTabViews();
            _videoSettingsView = new VideoSettingsPanelView(_contentRoot, styleSheet);
            videoSettingsPanelHost.BindPanel(_videoSettingsView);
        }

        private void ShowLanguageTab()
        {
            DisposeTabViews();
        }

        private void Generate()
        {
            DisposeView();

            _settingsView = new SettingsPanelView(
                uiDocument.rootVisualElement,
                styleSheet
            );
            
            _audioTab = _settingsView.AudioTab;
            _videoTab = _settingsView.VideoTab;
            _languageTab = _settingsView.LanguageTab;
            _contentRoot = _settingsView.Content;
            
            BindTabs();
            ShowVideoTab();
        }

        private void BindTabs()
        {
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

        #region Unity Lifecycle

        private void OnEnable() => Generate();

        private void OnDisable() => DisposeView();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (uiDocument == null) return;
            if (uiDocument.rootVisualElement == null) return;

            Generate();
        }
#endif

        #endregion
    }
}
