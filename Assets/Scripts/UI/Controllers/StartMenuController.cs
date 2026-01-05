using Constants;
using UI.Hosts;
using UI.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Controllers
{
    public class StartMenuController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private StartMenuPanelHost startMenuPanelHost;
        [SerializeField] private SettingsPanelHost settingsPanelHost;

        private bool _settingsActive;
        
        #endregion
        
        #region Methods

        private void ToggleSettings()
        {
            _settingsActive = !_settingsActive;
            
            if(!_settingsActive)
                settingsPanelHost.Hide();
            else
                settingsPanelHost.Generate();
        }
        
        private void BindButtons()
        {
            startMenuPanelHost.SubscribeEvents();
            startMenuPanelHost.OnPlayClicked += HandlePlay;
            startMenuPanelHost.OnSettingsClicked += HandleSettings;
            startMenuPanelHost.OnControlsClicked += HandleControls;
            startMenuPanelHost.OnQuitClicked += HandleQuit;
        }

        private void UnbindButtons()
        {
            startMenuPanelHost.OnPlayClicked -= HandlePlay;
            startMenuPanelHost.OnSettingsClicked -= HandleSettings;
            startMenuPanelHost.OnControlsClicked -= HandleControls;
            startMenuPanelHost.OnQuitClicked -= HandleQuit;
        }
        
        #endregion
        
        #region Event Handlers
        
        private void HandlePlay()
        {
            SceneManager.LoadScene(GameConstants.Hub);
        }

        private void HandleSettings()
        {
            ToggleSettings();
        }

        private void HandleControls()
        {
            
        }

        private void HandleQuit()
        {
            Application.Quit();
        }
        
        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            startMenuPanelHost.Generate();
            BindButtons();
        }

        private void OnDisable()
        {
            UnbindButtons();
        }
        
        #endregion
        
    }
}