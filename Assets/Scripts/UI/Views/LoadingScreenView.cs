using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class LoadingScreenView : BasePanelView
    {
        private IStyle _loadingBarFillStyle;
        private VisualElement _loadingLabelContainer;
        
        public LoadingScreenView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);
            
            GenerateUI(root);
        }
        
        protected sealed override void GenerateUI(VisualElement root)
        {
            var loadingBar = UIToolkitFactory.CreateLoadingBar();
            _loadingBarFillStyle = loadingBar.Fill.style;
            
            Container.Add(loadingBar.Container);
            
            root.Add(Container);
        }

        public void UpdateLoadingBarValue(float value)
        {
            _loadingBarFillStyle.width = new Length(value, LengthUnit.Percent);
        }
    }
}