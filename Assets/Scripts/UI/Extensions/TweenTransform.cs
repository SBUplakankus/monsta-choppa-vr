using PrimeTween;
using UnityEngine;

namespace UI.Extensions
{
    public class TweenTransform : MonoBehaviour, ITweenable
    {
        private const float DisplayScale = 1f;
        private const float DisplayStartScale = 0.75f;
        private const float Duration = 0.25f;

        private static readonly Vector3 StartScale = Vector3.one * DisplayStartScale;
        private static readonly Vector3 HiddenScale = Vector3.zero;

        private const Ease ShowEase = Ease.OutBack;
        private const Ease HideEase = Ease.InBack;
        private Tween _currentTween;

        public void Show()
        {
            StopTween();
            transform.localScale = StartScale;
            _currentTween = Tween.Scale(transform, DisplayScale, Duration, ShowEase);
        }

        public void Hide()
        {
            StopTween();
            _currentTween = Tween.Scale(transform, HiddenScale, Duration * 0.75f, HideEase);
        }

        public void StopTween()
        {
            if (_currentTween.isAlive)
                _currentTween.Complete();
        }

        public bool IsTweening => _currentTween.isAlive;
    }
}