using Constants;
using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class ArenaIntroView : BasePanelView
    {
        public ArenaIntroView(VisualElement root, StyleSheet styleSheet, string arenaKey, string difficultyKey)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);
            
            GenerateUI(root, arenaKey, difficultyKey);
        }
        
        private void GenerateUI(VisualElement root, string arenaKey, string difficultyKey)
        {
            Container = UIToolkitFactory.CreateContainer(UIToolkitStyles.ArenaIntroContainer);
            
            var arenaName = UIToolkitFactory.CreateLabel(arenaKey, UIToolkitStyles.ArenaIntroName);
            Container.Add(arenaName);
            
            var arenaDifficulty = UIToolkitFactory.CreateLabel(arenaKey, UIToolkitStyles.ArenaIntroTime);
            Container.Add(arenaDifficulty);
            
            root.Add(Container);
        }
    }
}