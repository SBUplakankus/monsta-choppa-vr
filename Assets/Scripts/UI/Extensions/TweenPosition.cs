using PrimeTween;
using UnityEngine;

namespace UI.Extensions
{
    public class TweenPosition : MonoBehaviour, ITweenable
    {
        [Header("Tween Settings")] 
        [SerializeField] private Transform startPosition;
        [SerializeField] private Transform endPosition;
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private Ease showEase = Ease.OutCubic;

        private Tween _currentTween;

        public void Show()
        {
            StopTween();
            transform.position = startPosition.position;
            _currentTween = Tween.LocalPosition(transform, endPosition.position, moveDuration, showEase);
        }

        public void Hide()
        {
            StopTween();
        }

        /// <summary>
        /// Stops any currently running tween.
        /// </summary>
        public void StopTween()
        {
            if (_currentTween.isAlive)
            {
                _currentTween.Complete();
            }
        }

        public bool IsTweening => _currentTween.isAlive;
    }
}