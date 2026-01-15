using PrimeTween;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Extensions
{
    public class TweenTransform : MonoBehaviour, ITweenable
    {
        #region Fields
        
        [Header("Display Settings")]
        [SerializeField] private float displayScale = 1f;
        [SerializeField] private float displayStartScale = 0.75f;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private bool startHidden = true;
        
        [Header("Tween Settings")]
        [SerializeField] private Ease showEase = Ease.OutCubic;
        [SerializeField] private Ease hideEase = Ease.InCubic;
        
        private static readonly Vector3 StartScale = Vector3.one;
        private static readonly Vector3 HiddenScale = Vector3.zero;
        private Tween _currentTween;

        #endregion
        
        #region Properties
        
        public bool IsTweening => _currentTween.isAlive;
        
        #endregion
        
        #region Methods
        
        public void Show()
        {
            StopTween();
            transform.localScale = StartScale *  displayStartScale;
            _currentTween = Tween.Scale(transform, displayScale, duration, showEase);
        }

        public void Hide()
        {
            StopTween();
            _currentTween = Tween.Scale(transform, HiddenScale, duration / 2, hideEase);
        }

        public void StopTween()
        {
            if (_currentTween.isAlive)
                _currentTween.Complete();
        }
        
        private void OnEnable()
        { 
            transform.localScale = startHidden ? HiddenScale : StartScale;   
        }
        
        #endregion
    }
}