using System;
using Events.Registries;
using PrimeTween;
using UnityEngine;

namespace UI.Controllers
{
    public class FadeController : MonoBehaviour
    {
        private CanvasGroup _fadeToBlackCanvasGroup;
        
        private const int FadeInAlpha = 1;
        private const int FadeOutAlpha = 0;
        private const float FadeDuration = 1.5f;
        private const Ease FadeEase = Ease.Linear;

        private void FadeIn()
        {
            _fadeToBlackCanvasGroup.alpha = FadeOutAlpha;
            Tween.Alpha(_fadeToBlackCanvasGroup, FadeInAlpha, FadeDuration, FadeEase);
        }

        private void FadeOut()
        {
            _fadeToBlackCanvasGroup.alpha = FadeInAlpha;
            Tween.Alpha(_fadeToBlackCanvasGroup, FadeOutAlpha, FadeDuration, FadeEase);
        }

        private void InitCanvas()
        {
            _fadeToBlackCanvasGroup = GetComponent<CanvasGroup>();
            _fadeToBlackCanvasGroup.alpha = FadeInAlpha;
            _fadeToBlackCanvasGroup.blocksRaycasts = false;
            _fadeToBlackCanvasGroup.interactable = false;
        }

        private void SubscribeEvents()
        {
            UIEvents.FadeIn.Subscribe(FadeIn);
            UIEvents.FadeOut.Subscribe(FadeOut);
        }

        private void UnsubscribeEvents()
        {
            UIEvents.FadeIn.Unsubscribe(FadeIn);
            UIEvents.FadeOut.Unsubscribe(FadeOut);
        }
        
        private void Awake() => InitCanvas();
        
        private void OnEnable()
        {
            SubscribeEvents();
            FadeOut();
        }
        
        private void OnDisable() => UnsubscribeEvents();
    }
}