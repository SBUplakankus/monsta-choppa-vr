using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class SettingsPanelHost : MonoBehaviour
    {
        #region Fields

        [Header("UI Toolkit")] [SerializeField]
        private UIDocument uiDocument;

        [SerializeField] private StyleSheet styleSheet;

        #endregion

        #region View

        private SettingsPanelView _settingsView;

        #endregion

        #region Private Methods

        private void Generate()
        {
            DisposeView();

            _settingsView = new SettingsPanelView(
                uiDocument.rootVisualElement,
                styleSheet
            );
        }

        private void DisposeView()
        {
            _settingsView?.Dispose();
            _settingsView = null;
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
