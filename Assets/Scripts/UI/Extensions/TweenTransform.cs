using PrimeTween;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Extensions
{
    public class TweenTransform : MonoBehaviour, ITweenable
    {
        [SerializeField] private float displayScale = 1f;
        [SerializeField] private float displayStartScale = 0.75f;
        [SerializeField] private float duration = 0.25f;

        private static readonly Vector3 StartScale = Vector3.one;
        private static readonly Vector3 HiddenScale = Vector3.zero;

        private const Ease ShowEase = Ease.OutCubic;
        private const Ease HideEase = Ease.InCubic;
        private Tween _currentTween;

        public void Show()
        {
            StopTween();
            transform.localScale = StartScale *  displayStartScale;
            _currentTween = Tween.Scale(transform, displayScale, duration, ShowEase);
        }

        public void Hide()
        {
            StopTween();
            _currentTween = Tween.Scale(transform, HiddenScale, duration * 0.75f, HideEase);
        }

        public void StopTween()
        {
            if (_currentTween.isAlive)
                _currentTween.Complete();
        }

        public bool IsTweening => _currentTween.isAlive;
        private void OnEnable() => transform.localScale = HiddenScale;
    }
}