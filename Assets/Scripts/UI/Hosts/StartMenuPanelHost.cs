using System;
using UI.Extensions;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class StartMenuPanelHost : BasePanelHost
    {
        #region Fields
        
        public event Action OnPlayClicked;
        public event Action OnControlsClicked;
        public event Action OnSettingsClicked;
        public event Action OnQuitClicked;
        
        private StartMenuPanelView _view;

        #endregion

        #region Class Methods
        
        private void OnPlay() => OnPlayClicked?.Invoke();
        private void OnControls() => OnControlsClicked?.Invoke();
        private void OnSettings() => OnSettingsClicked?.Invoke();
        private void OnQuit() => OnQuitClicked?.Invoke();

        public void SubscribeEvents()
        {
            if(_view == null) return;
            UnsubscribeEvents();
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
        
        public override void Generate()
        {
            Dispose();
            
            _view = new StartMenuPanelView(
                uiDocument.rootVisualElement,
                styleSheet
            );
            
            SubscribeEvents();
            Show();
        }
        
        #endregion
        
        #region Unity Methods

        private void OnEnable() => Generate();

        #endregion
        
        #region IDisposable

        protected override void Dispose()
        {
            UnsubscribeEvents();
            _view?.Dispose();
            _view = null;
        }

        #endregion
    }
}
