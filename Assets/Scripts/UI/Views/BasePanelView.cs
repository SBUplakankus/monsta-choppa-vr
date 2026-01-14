using System;
using UnityEngine.UIElements;

namespace UI.Views
{
    public abstract class BasePanelView : IDisposable
    {
        protected VisualElement Container;
        
        protected virtual void GenerateUI(VisualElement root) {}
        
        public virtual void Dispose()
        {
            Container?.Clear();
            Container?.RemoveFromHierarchy();
            Container = null;
        }
    }
}