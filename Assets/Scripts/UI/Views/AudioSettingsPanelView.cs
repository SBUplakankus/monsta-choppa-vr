using System;
using Constants;
using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class AudioSettingsPanelView : IDisposable
    {
        #region Fields

        private VisualElement _container;

        #endregion

        #region Constructors

        public AudioSettingsPanelView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root);
        }

        #endregion

        #region Methods

        private void GenerateUI(VisualElement root)
        {
            _container = UIToolkitFactory.CreateContainer(
                UIToolkitStyles.Container,
                UIToolkitStyles.PanelBody
            );

            // TODO: Build UI here
            

            root.Add(_container);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_container == null) return;

            _container.RemoveFromHierarchy();
            _container = null;
        }

        #endregion
    }
}
