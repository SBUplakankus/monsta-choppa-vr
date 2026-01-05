using UI.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public abstract class BasePanelHost : MonoBehaviour
    {
        #region Fields
        
        [Header("UI Toolkit")] 
        [SerializeField] protected UIDocument uiDocument;
        [SerializeField] protected StyleSheet styleSheet;
        
        
        protected VisualElement ContentRoot;
        private ITweenable[] _tweenables;
        
        #endregion
        
        #region Class Methods

        public virtual void Show()
        {
            foreach (var tween in _tweenables)
                tween?.Show();
        }

        public virtual void Hide()
        {
            foreach (var tween in _tweenables)
                tween?.Hide();
        }

        public abstract void Generate();
        public abstract void Dispose();
        
        #endregion

        #region Unity Methods

        private void Awake() => _tweenables = GetComponentsInChildren<ITweenable>(true);
        private void OnDisable() => Dispose();
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (uiDocument == null) return;
            if (uiDocument.rootVisualElement == null) return;

            Generate();
        }
#endif

        #endregion
        
        
    }
}