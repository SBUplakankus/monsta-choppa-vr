using System;
using Constants;
using Events;
using UI.Extensions;
using UI.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UI.Panels
{
    public class StartMenuPanelHost : MonoBehaviour
    {
        #region Fields

        [Header("UI Toolkit")] 
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet styleSheet;
        
        public event Action OnPlayClicked;
        public event Action OnControlsClicked;
        public event Action OnAudioSettingsClicked;
        public event Action OnVideoSettingsClicked;
        public event Action OnQuitClicked;
        
        private StartMenuPanelView _view;

        #endregion

        #region Methods
        
        private void OnPlay() => OnPlayClicked?.Invoke();
        private void OnControls() => OnControlsClicked?.Invoke();
        private void OnAudioSettings() => OnAudioSettingsClicked?.Invoke();
        private void OnVideoSettings() => OnVideoSettingsClicked?.Invoke();
        private void OnQuit() => OnQuitClicked?.Invoke();

        private void SubscribeEvents()
        {
            if(_view == null) return;
            _view.OnPlayClicked += OnPlay;
            _view.OnControlsClicked += OnControls;
            _view.OnAudioSettingsClicked += OnAudioSettings;
            _view.OnVideoSettingsClicked += OnVideoSettings;
            _view.OnQuitClicked += OnQuit;
        }

        private void UnsubscribeEvents()
        {
            if(_view == null) return;
            _view.OnPlayClicked -= OnPlay;
            _view.OnControlsClicked -= OnControls;
            _view.OnAudioSettingsClicked -= OnAudioSettings;
            _view.OnVideoSettingsClicked -= OnVideoSettings;
            _view.OnQuitClicked -= OnQuit;
        }
        
        private void Generate()
        {
            DisposeView();
            
            _view = new StartMenuPanelView(
                uiDocument.rootVisualElement,
                styleSheet
            );

            foreach (var viewButton in _view.Buttons)
                viewButton.AddAudioEvents();
            
            SubscribeEvents();
        }

        private void DisposeView()
        {
            UnsubscribeEvents();
            _view?.Dispose();
            _view = null;
        }

        #endregion

        #region Unity Methods

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
