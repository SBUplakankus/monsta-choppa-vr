using System;
using System.Collections;
using PrimeTween;
using UI.Hosts;
using UnityEngine;
using Utilities;

namespace Systems.Arena.Cutscene
{
    public class CutsceneInterfaceController : MonoBehaviour
    {
        #region Fields

        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup fadeToBlackCanvasGroup;
        
        [Header("Hosts")]
        [SerializeField] private ArenaIntroHost arenaIntroHost;
        [SerializeField] private BossIntroHost bossIntroHost;
        
        private const int FadeInAlpha = 1;
        private const int FadeOutAlpha = 0;
        private const int FadeDuration = 2;
        private const Ease FadeEase = Ease.OutCubic;
        
        #endregion
        
        #region Methods
        
        public void FadeIn() => Tween.Alpha(fadeToBlackCanvasGroup, FadeInAlpha, FadeDuration, FadeEase);
        public void FadeOut() => Tween.Alpha(fadeToBlackCanvasGroup, FadeOutAlpha, FadeDuration, FadeEase);
        
        public void ShowArenaIntro(ArenaData arenaData) => arenaIntroHost.DisplayArenaIntro(arenaData.Location, arenaData.Difficulty);
        public void HideArenaIntro() => arenaIntroHost.HideArenaIntro();
        public void HandleArenaIntroCompleted() => arenaIntroHost.gameObject.SetActive(false);
        
        public void ShowBossIntro(ArenaData arenaData) => bossIntroHost.DisplayBossIntro(arenaData.Boss);
        public void HideBossIntro() => bossIntroHost.HideBossIntro();
        public void HandleBossIntroCompleted() => bossIntroHost.gameObject.SetActive(false);
        
        private void OnEnable() => fadeToBlackCanvasGroup.alpha = FadeInAlpha;
        
        #endregion
    }
}