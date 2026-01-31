using System;
using Events;
using Events.Registries;
using PrimeTween;
using Systems.Core;
using UI.Hosts;
using UnityEngine;
using Utilities;

namespace Systems.Arena
{
    public class ArenaInterfaceManager : MonoBehaviour, IUpdateable
    {
        #region Fields
        
        [Header("Hosts")]
        [SerializeField] private ArenaIntroHost arenaIntroHost;
        [SerializeField] private BossIntroHost bossIntroHost;
        
        private readonly CountdownTimer _countdownTimer = new();

        private const int IntroDisplayDuration = 8;
        private const int FadeInAlpha = 1;
        
        #endregion
        
        #region Methods

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
                case ArenaState.ArenaVictory:
                    UIEvents.FadeIn.Raise();
                    break;
                case ArenaState.ArenaDefeat:
                    UIEvents.FadeIn.Raise();
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
            GameplayEvents.ArenaStateChanged.Subscribe(HandleGameStateChange);
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }

        private void OnDisable()
        {
            GameplayEvents.ArenaStateChanged.Unsubscribe(HandleGameStateChange);
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