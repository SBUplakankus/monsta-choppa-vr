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
        [SerializeField] private GameStateEventChannel onGameStateChange;
        
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

        private void HandleGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.GamePrelude:
                    ShowArenaIntro();
                    break;
                case GameState.WaveActive:
                    break;
                case GameState.WaveIntermission:
                    break;
                case GameState.WaveComplete:
                    break;
                case GameState.BossIntermission:
                    ShowBossIntro();
                    break;
                case GameState.BossActive:
                    break;
                case GameState.BossComplete:
                    break;
                case GameState.GameWon:
                    FadeIn();
                    break;
                case GameState.GameOver:
                    FadeIn();
                    break;
                case GameState.GamePaused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }
        
        #endregion
        
        #region Unity Methods

        private void OnEnable()
        {
            onGameStateChange.Subscribe(HandleGameStateChange);
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
            fadeToBlackCanvasGroup.alpha = FadeInAlpha;
            FadeOut();
        }

        private void OnDisable()
        {
            onGameStateChange.Unsubscribe(HandleGameStateChange);
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