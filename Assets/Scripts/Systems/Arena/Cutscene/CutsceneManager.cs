using System.Collections;
using Player;
using UnityEngine;

namespace Systems.Arena.Cutscene
{
    public class CutsceneManager : MonoBehaviour
    {
        #region Fields
        
        [Header("Arena Data")]
        [SerializeField] private ArenaData arenaData;
        
        [Header("References")]
        [SerializeField] private GameObject playerRig;
        [SerializeField] private CutsceneCameraController cameraController;
        [SerializeField] private CutsceneInterfaceController interfaceController;
        [SerializeField] private XRComponentController xrComponentController;
        
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
            interfaceController.ShowArenaIntro(arenaData);
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
            interfaceController.ShowBossIntro(arenaData);
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
        
        #endregion
    }
}