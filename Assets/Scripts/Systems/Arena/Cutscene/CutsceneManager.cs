using System.Collections;
using Events;
using Player;
using UnityEngine;

namespace Systems.Arena.Cutscene
{
    public class CutsceneManager : MonoBehaviour
    {
        #region Fields
        
        [Header("References")]
        [SerializeField] private CutsceneCameraController cameraController;
        [SerializeField] private CutsceneInterfaceController interfaceController;
        [SerializeField] private XRComponentController xrComponentController;

        [Header("Events")] 
        [SerializeField] private GameStateEventChannel onGameStateChange;
        
        private const int ArenaDisplayDuration = 5;
        private const int BossDisplayDuration = 4;
        private const int FadeDuration = 2;
        private const int WaitDuration = 1;
        
        private readonly WaitForSeconds _fadeTimer = new(FadeDuration + WaitDuration);
        private readonly WaitForSeconds _waitTimer = new(WaitDuration);
        private readonly WaitForSeconds _arenaTimer = new(ArenaDisplayDuration);
        private readonly WaitForSeconds _bossTimer = new(BossDisplayDuration);

        #endregion
        
        #region Routines

        private IEnumerator ArenaCutsceneRoutine()
        {
            HandleArenaStart();
            yield return _fadeTimer;
            interfaceController.ShowArenaIntro();
            yield return _arenaTimer;
            interfaceController.HideArenaIntro();
            yield return _waitTimer;
            interfaceController.FadeIn();
            yield return _fadeTimer;
            HandleArenaCompletion();
        }

        private IEnumerator BossCutsceneRoutine()
        {
            HandleBossStart();
            yield return _fadeTimer;
            interfaceController.FadeOut();
            yield return _fadeTimer;
            interfaceController.ShowBossIntro();
            yield return _bossTimer;
            interfaceController.HideBossIntro();
            yield return _waitTimer;
            interfaceController.FadeIn();
            yield return _fadeTimer;
            HandleBossCompletion();
        }
        
        #endregion
        
        #region Methods

        private void HandleArenaStart()
        {
            interfaceController.FadeOut();
            xrComponentController.DisableComponents();
        }
        
        private void HandleArenaCompletion()
        {
            interfaceController.HandleArenaIntroCompleted();
            interfaceController.FadeOut();
            xrComponentController.EnableComponents();
        }

        private void HandleBossStart()
        {
            interfaceController.FadeIn();
            xrComponentController.DisableComponents();
        }

        private void HandleBossCompletion()
        {
            interfaceController.HandleBossIntroCompleted();
            interfaceController.FadeOut();
            xrComponentController.EnableComponents();
        }

        private void TriggerArenaCutscene()
        {
            StartCoroutine(ArenaCutsceneRoutine());
        }

        private void TriggerBossCutscene()
        {
            StartCoroutine(BossCutsceneRoutine());
        }

        private void HandleGameStateChange(GameState gameState)
        {
            if(gameState == GameState.GamePrelude)
                TriggerArenaCutscene();
            else if (gameState == GameState.BossIntermission)
                TriggerBossCutscene();
        }
        
        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            onGameStateChange.Subscribe(HandleGameStateChange);
        }

        private void OnDisable()
        {
            onGameStateChange.Unsubscribe(HandleGameStateChange);
        }
        #endregion
    }
}