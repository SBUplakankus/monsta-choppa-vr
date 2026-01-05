using PrimeTween;
using UnityEngine;

namespace UI.Extensions
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenAlpha : MonoBehaviour, ITweenable
    {
        private CanvasGroup _canvasGroup;
        private const float Duration = 0.25f;
        private const float ShowAlpha = 1;
        private const float HideAlpha = 0;

        private const Ease ShowEase = Ease.OutBack;
        private const Ease HideEase = Ease.InBack;
        private Tween _currentTween;
        
        private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

        public void Show()
        {
            StopTween();
            _currentTween = Tween.Alpha(_canvasGroup, ShowAlpha, Duration, ShowEase);
        }

        public void Hide()
        {
            StopTween();
            _currentTween = Tween.Alpha(_canvasGroup, HideAlpha, Duration, HideEase);
        }

        public void StopTween()
        {
            if (_currentTween.isAlive)
                _currentTween.Complete();
        }

        public bool IsTweening => _currentTween.isAlive;
    }
}