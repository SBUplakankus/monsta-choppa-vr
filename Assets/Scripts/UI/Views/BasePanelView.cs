using System;
using UnityEngine.UIElements;

namespace UI.Views
{
    public abstract class BasePanelView : IDisposable
    {
        protected VisualElement Container;
        
        protected abstract void GenerateUI(VisualElement root);
        
        public virtual void Dispose()
        {
            Container?.RemoveFromHierarchy();
            Container = null;
        }
    }
}