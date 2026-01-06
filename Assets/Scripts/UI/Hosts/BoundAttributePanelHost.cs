using Attributes;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class BoundAttributePanelHost : BasePanelHost
    {
        #region Fields
        
        [Header("Attribute")]
        [SerializeField] private IntAttribute attribute;
        
        private BoundAttributePanelView _boundAttributePanelView;
        
        #endregion
        
        #region Class Functions

        public override void Generate()
        {
            _boundAttributePanelView = new BoundAttributePanelView(uiDocument.rootVisualElement,  styleSheet, attribute);
            attribute.Refresh();
        }

        public override void Dispose()
        {
            
        }

        #endregion
        
        #region Unity Functions
        
        private void OnEnable() => Generate();
        
        #endregion
    }
}
