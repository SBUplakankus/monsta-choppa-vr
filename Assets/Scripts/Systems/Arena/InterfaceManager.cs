using System;
using Events;
using PrimeTween;
using UI.Hosts;
using UnityEngine;
using Utilities;

namespace Systems.Arena
{
    public class ArenaInterfaceManager : MonoBehaviour, IUpdateable
    {
        #region Fields

        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup fadeToBlackCanvasGroup;
        
        [Header("Hosts")]
        [SerializeField] private ArenaIntroHost arenaIntroHost;
        [SerializeField] private BossIntroHost bossIntroHost;
        
        [Header("Events")]
        [SerializeField] private ArenaStateEventChannel onArenaStateChange;
        
        private readonly CountdownTimer _countdownTimer = new();

        private const int IntroDisplayDuration = 8;
        private const int FadeInAlpha = 1;
        private const int FadeOutAlpha = 0;
        private const float FadeDuration = 2f;
        private const Ease FadeEase = Ease.Linear;
        
        #endregion
        
        #region Methods

        private void FadeIn() => Tween.Alpha(fadeToBlackCanvasGroup, FadeInAlpha, FadeDuration, FadeEase);
        private void FadeOut() => Tween.Alpha(fadeToBlackCanvasGroup, FadeOutAlpha, FadeDuration, FadeEase);

        private void ShowArenaIntro()
        { 
            arenaIntroHost.DisplayArenaIntro();
            _countdownTimer.Start(IntroDisplayDuration, arenaIntroHost.HideArenaIntro);
        }
        private void ShowBossIntro()
        { 
            bossIntroHost.DisplayBossIntro();
            _countdownTimer.Start(IntroDisplayDuration, bossIntroHost.HideBossIntro);
        }

        private void HandleGameStateChange(ArenaState arenaState)
        {
            switch (arenaState)
            {
                case ArenaState.ArenaPrelude:
                    ShowArenaIntro();
                    break;
                case ArenaState.WaveActive:
                    break;
                case ArenaState.WaveIntermission:
                    break;
                case ArenaState.WaveComplete:
                    break;
                case ArenaState.BossIntermission:
                    ShowBossIntro();
                    break;
                case ArenaState.BossActive:
                    break;
                case ArenaState.BossComplete:
                    break;
                case ArenaState.ArenaWon:
                    FadeIn();
                    break;
                case ArenaState.ArenaOver:
                    FadeIn();
                    break;
                case ArenaState.ArenaPaused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arenaState), arenaState, null);
            }
        }
        
        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            onArenaStateChange.Subscribe(HandleGameStateChange);
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
            fadeToBlackCanvasGroup.alpha = FadeInAlpha;
            FadeOut();
        }

        private void OnDisable()
        {
            onArenaStateChange.Unsubscribe(HandleGameStateChange);
            GameUpdateManager.Instance.Unregister(this);
            _countdownTimer.Stop();
        }

        #endregion

        public void OnUpdate(float deltaTime)
        {
            _countdownTimer.Update(deltaTime);
        }
    }
}