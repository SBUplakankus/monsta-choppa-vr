using Attributes;
using UI.Views;
using UnityEngine;

namespace UI.Hosts
{
    public class PlayerArenaAttributeHost : BasePanelHost
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private IntAttribute attribute;
        
        private PlayerArenaAttributeView _view;
        
        public override void Generate()
        {
            Dispose();
            
            _view = new PlayerArenaAttributeView(
                uiDocument.rootVisualElement,
                styleSheet,
                icon,
                attribute);
        }

        protected override void Dispose()
        {
            _view?.Dispose();
            _view = null;
        }
    }
}