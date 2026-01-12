using Constants;
using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class BossIntroView : BasePanelView
    {
        public BossIntroView(VisualElement root, StyleSheet styleSheet, string bossKey)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root, bossKey);
        }

        private void GenerateUI(VisualElement root, string bossKey)
        {
            Container = UIToolkitFactory.CreateContainer(UIToolkitStyles.BossIntroContainer);
            
            var header = UIToolkitFactory.CreateLabel(LocalizationKeys.BossWarning,UIToolkitStyles.BossIntroHeader);
            Container.Add(header);
            
            var bossName = UIToolkitFactory.CreateLabel(bossKey, UIToolkitStyles.BossIntroName);
            Container.Add(bossName);
            
            root.Add(Container);
        }
    }
}