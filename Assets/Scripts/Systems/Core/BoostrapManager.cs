using System.Collections;
using System.Collections.Generic;
using Constants;
using Events;
using Pooling;
using Saves;
using UI.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

namespace Systems.Core
{
    public class BoostrapManager : MonoBehaviour
    {
        #region Fields

        [Header("Bootstrap Dependencies")]
        [SerializeField] private GameBootstrap gameBootstrap;
        [SerializeField] private LoadingScreenController loadingScreen;
        [SerializeField] private GamePoolManager gamePoolManager;
        [SerializeField] private SettingsSaveFileManager settingsSaveFileManager;
        
        [Header("Events")]
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;
        
        private const float MinimumLoadTime = 2f;
        private const float OperationThreshold = 0.9f;
        private readonly WaitForSeconds _minStepWait = new(0.5f);
        
        private bool _isInitialized;

        #endregion

        #region Properties

        public static BoostrapManager Instance { get; private set; }

        #endregion

        #region Routines

        private IEnumerator Start()
        {
            StartLoadingGame();

            yield return StartCoroutine(InitCoreSystemsAsync());
            yield return StartCoroutine(LoadUserDataAsync());
            yield return StartCoroutine(LoadAssetsAsync());
            yield return StartCoroutine(LoadStartMenuAsync());
            
            FinishLoadingGame();
        }
        
        private IEnumerator HandleSceneLoading(AsyncOperation operation)
        {
            if (operation == null) yield break;
            
            var startTime = Time.time;
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                if (operation.progress >= OperationThreshold)
                {
                    var elapsed = Time.time - startTime;
                    if (elapsed < MinimumLoadTime)
                    {
                        yield return new WaitForSeconds(MinimumLoadTime - elapsed);
                    }
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        private IEnumerator InitCoreSystemsAsync()
        {
            gameBootstrap?.Initialize();
            yield return UpdateAndWait(LocalizationKeys.InitializingCore);
            loadingScreen.UpdateProgress(LocalizationKeys.CoreReady);
            
        }

        private IEnumerator LoadUserDataAsync()
        {
            settingsSaveFileManager?.InitSettings();
            yield return UpdateAndWait(LocalizationKeys.PreloadingAssets);
            // loadingScreen.ReportProgress(_loadProgresses[LocalizationKeys.LoadingSaves]);
            loadingScreen.UpdateProgress(LocalizationKeys.UserDataLoaded);
        }

        private IEnumerator LoadAssetsAsync()
        {
            gamePoolManager?.Initialise();
            yield return UpdateAndWait(LocalizationKeys.PreloadingAssets);
            loadingScreen.UpdateProgress(LocalizationKeys.AssetsLoaded);
        }

        private IEnumerator LoadStartMenuAsync()
        {
            loadingScreen.UpdateProgress(LocalizationKeys.LoadingGame);
            
            var operation = SceneManager.LoadSceneAsync(GameConstants.StartMenu, LoadSceneMode.Additive);
            if (operation != null) operation.allowSceneActivation = false;
            yield return HandleSceneLoading(operation);

            loadingScreen.UpdateProgress(LocalizationKeys.LoadingComplete);
        }

        #endregion

        #region Class Methods

        private IEnumerator UpdateAndWait(string key)
        {
            loadingScreen.UpdateProgress(key);
            yield return _minStepWait;
        }
        
        private void StartLoadingGame()
        {
            loadingScreen.Show();
            loadingScreen.UpdateProgress(LocalizationKeys.Initializing);
            _isInitialized = false;
        }

        private void FinishLoadingGame()
        {
            loadingScreen.Hide();
            _isInitialized = true;
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion
        
        #region Public API
        
        /// <summary>
        /// Loads a new scene with a loading screen transition.
        /// Use this for all scene transitions.
        /// </summary>
        public void LoadScene(string sceneName, bool showLoading = true)
        {
            StartCoroutine(LoadSceneWithTransition(sceneName, showLoading));
        }
        
        private IEnumerator LoadSceneWithTransition(string sceneName, bool showLoading)
        {
            if (showLoading)
            {
                loadingScreen?.FadeIn();
                loadingScreen?.UpdateProgress(LocalizationKeys.LoadingScene);
                onGameStateChangeRequested.Raise(GameState.Loading);
            }
            
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != GameConstants.Bootstrapper)
            {
                yield return SceneManager.UnloadSceneAsync(currentScene);
            }
            
            // Clean up memory
            // Note: Resources.UnloadUnusedAssets() is async and handles most cleanup.
            // We call it here while on the loading screen to reclaim memory from the 
            // unloaded scene. Explicit GC.Collect() is omitted since Unity's asset
            // unloading already triggers collection internally.
            yield return Resources.UnloadUnusedAssets();
            yield return null;
            
            // Load new scene
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (operation != null)
            {
                operation.allowSceneActivation = false;
                yield return HandleSceneLoading(operation);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            if (!showLoading) yield break;
            loadingScreen?.UpdateProgress(LocalizationKeys.LoadingComplete);
            yield return _minStepWait;
            loadingScreen?.FadeOut();
        }
        
        #endregion
    }
}