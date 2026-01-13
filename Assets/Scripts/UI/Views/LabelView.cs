using Constants;
using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class LabelView : BasePanelView
    {
        public LabelView(VisualElement root, StyleSheet styleSheet)
        {
            if(!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);
            
            GenerateUI(root);
        }
        
        protected sealed override void GenerateUI(VisualElement root)
        {
            Container = UIToolkitFactory.CreateContainer(UIToolkitStyles.BossIntroContainer);
            var label = UIToolkitFactory.CreateLabel(LocalizationKeys.BossWarning, UIToolkitStyles.BossIntroHeader);
            Container.Add(label);
            root.Add(Container);
        }
    }
}