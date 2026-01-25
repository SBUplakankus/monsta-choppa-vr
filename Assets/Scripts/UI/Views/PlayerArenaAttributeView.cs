using Attributes;
using Factories;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class PlayerArenaAttributeView : BasePanelView
    {
        public PlayerArenaAttributeView(
            VisualElement root, 
            StyleSheet styleSheet, 
            Sprite sprite,
            IntAttribute attribute)
        {
            if(!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);
            
            GenerateUI(root, sprite, attribute);
        }
        
        private void GenerateUI(VisualElement root, Sprite sprite, IntAttribute attribute)
        {
            Dispose();

            Container = UIToolkitFactory.CreateContainer();
            
            var icon = UIToolkitFactory.CreateIcon(sprite);
            Container.Add(icon);
            
            var num = UIToolkitFactory.CreateBoundLabel(attribute, nameof(attribute.Value));
            Container.Add(num);
            
            root.Add(Container);
        }
    }
}