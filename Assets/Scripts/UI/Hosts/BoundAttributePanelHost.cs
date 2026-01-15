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
            Dispose();
            _boundAttributePanelView = new BoundAttributePanelView(uiDocument.rootVisualElement,  styleSheet, attribute);
            attribute.Refresh();
        }

        protected override void Dispose()
        {
            _boundAttributePanelView?.Dispose();
            _boundAttributePanelView = null;
        }

        #endregion
        
        #region Unity Functions
        
        private void OnEnable() => Generate();
        
        #endregion
    }
}
