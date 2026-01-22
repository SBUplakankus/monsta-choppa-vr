using Constants;
using Factories;
using UI.Controllers;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class LoadingScreenView : BasePanelView
    {
        private IStyle _loadingBarFillStyle;
        private LocalizedString _labelText;
        private Label _loadingLabel;
        
        public LoadingScreenView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);
            
            GenerateUI(root);
        }
        
        protected sealed override void GenerateUI(VisualElement root)
        {
            Dispose();
            
            Container = UIToolkitFactory.CreateContainer(UIToolkitStyles.LoadingScreenContainer);
            
            var loadingLabelContainer = UIToolkitFactory.CreateContainer(UIToolkitStyles.LoadingLabelContainer);
            _loadingLabel = UIToolkitFactory.CreateLabel(LocalizationKeys.Initializing,UIToolkitStyles.LoadingLabel);
            loadingLabelContainer.Add(_loadingLabel);
            Container.Add(loadingLabelContainer);
            
            var loadingBar = UIToolkitFactory.CreateLoadingBar();
            _loadingBarFillStyle = loadingBar.Fill.style;
            Container.Add(loadingBar.Container);
            
            root.Add(Container);
        }

        public override void Dispose()
        {
            _loadingBarFillStyle = null;
            _labelText = null;
            _loadingLabel = null;
            base.Dispose();
        }
        
        public void UpdateLoadingBar(LoadProgress loadProgress)
        {
            _loadingBarFillStyle.width = new Length(loadProgress.LoadPercentage * 100, LengthUnit.Percent);
            _labelText = LocalizationFactory.CreateString(loadProgress.LoadingMessageKey);
            _loadingLabel.SetBinding(UIToolkitStyles.LabelTextBind, _labelText);
        }
        
    }
}