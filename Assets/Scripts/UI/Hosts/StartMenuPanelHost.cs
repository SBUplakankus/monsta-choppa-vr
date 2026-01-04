using System;
using UI.Extensions;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class StartMenuPanelHost : MonoBehaviour
    {
        #region Fields

        [Header("UI Toolkit")] 
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet styleSheet;
        
        public event Action OnPlayClicked;
        public event Action OnControlsClicked;
        public event Action OnSettingsClicked;
        public event Action OnQuitClicked;
        
        private StartMenuPanelView _view;

        #endregion

        #region Methods
        
        private void OnPlay() => OnPlayClicked?.Invoke();
        private void OnControls() => OnControlsClicked?.Invoke();
        private void OnSettings() => OnSettingsClicked?.Invoke();
        private void OnQuit() => OnQuitClicked?.Invoke();

        public void SubscribeEvents()
        {
            if(_view == null) return;
            _view.OnPlayClicked += OnPlay;
            _view.OnControlsClicked += OnControls;
            _view.OnSettingsClicked += OnSettings;
            _view.OnQuitClicked += OnQuit;
        }

        private void UnsubscribeEvents()
        {
            if(_view == null) return;
            _view.OnPlayClicked -= OnPlay;
            _view.OnControlsClicked -= OnControls;
            _view.OnSettingsClicked -= OnSettings;
            _view.OnQuitClicked -= OnQuit;
        }
        
        public void Generate()
        {
            DisposeView();
            
            _view = new StartMenuPanelView(
                uiDocument.rootVisualElement,
                styleSheet
            );
            
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
