using PrimeTween;
using UI.Hosts;
using UnityEngine;

namespace Systems.Arena
{
    public class InterfaceManager : MonoBehaviour
    {
        #region Fields

        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup fadeToBlackCanvasGroup;
        
        [Header("Hosts")]
        [SerializeField] private ArenaIntroHost arenaIntroHost;
        [SerializeField] private BossIntroHost bossIntroHost;
        
        private const int FadeInAlpha = 1;
        private const int FadeOutAlpha = 0;
        private const float FadeDuration = 2f;
        private const Ease FadeEase = Ease.Linear;
        
        #endregion
        
        #region Methods
        
        public void FadeIn() => Tween.Alpha(fadeToBlackCanvasGroup, FadeInAlpha, FadeDuration, FadeEase);
        private void FadeOut() => Tween.Alpha(fadeToBlackCanvasGroup, FadeOutAlpha, FadeDuration, FadeEase);
        
        public void ShowArenaIntro() => arenaIntroHost.DisplayArenaIntro();
        public void HideArenaIntro() => arenaIntroHost.HideArenaIntro();
        public void HandleArenaIntroCompleted() => arenaIntroHost.gameObject.SetActive(false);
        
        public void ShowBossIntro() => bossIntroHost.DisplayBossIntro();
        public void HideBossIntro() => bossIntroHost.HideBossIntro();
        public void HandleBossIntroCompleted() => bossIntroHost.gameObject.SetActive(false);
        
        private void OnEnable()
        {
            fadeToBlackCanvasGroup.alpha = FadeInAlpha;
            FadeOut();
        }

        #endregion
        
    }
}