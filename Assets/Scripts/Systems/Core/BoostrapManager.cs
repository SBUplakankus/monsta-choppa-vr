using System.Collections;
using Constants;
using Data.Registries;
using Databases;
using Events;
using Pooling;
using Saves;
using UI.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Core
{
    public class BootstrapManager : MonoBehaviour
    {
        #region Fields

        [Header("Core Dependencies")]
        [SerializeField] private GameEventRegistry gameEventRegistry;
        [SerializeField] private GameDatabaseRegistry gameDatabaseRegistry;
        [SerializeField] private LoadingScreenController loadingScreen;
        [SerializeField] private GamePoolManager gamePoolManager;
        [SerializeField] private SettingsSaveFileManager settingsSaveFileManager;

        [Header("Configuration")]
        [SerializeField] private float minimumLoadTime = 3f;
        [SerializeField] private float fadeWaitTime = 1.5f;
        [SerializeField] private float stepDelay = 0.5f;

        private WaitForSeconds _stepWait;
        private WaitForSeconds _minimumLoadWait;
        private WaitForSeconds _fadeWait;
        private bool _isInitialized;

        #endregion

        #region Properties

        public static BootstrapManager Instance { get; private set; }

        #endregion

        #region Bootstrap Sequence

        private IEnumerator BootstrapSequence()
        {
            Debug.Log("BootstrapManager: Starting bootstrap sequence");

            StartBootstrapSequence();

            yield return InitializeCoreRegistriesAsync();
            yield return InitializeSaveSystemsAsync();
            yield return InitializePoolingSystemsAsync();
            yield return LoadStartMenuAsync();

            CompleteBootstrapSequence();
        }

        private IEnumerator InitializeCoreRegistriesAsync()
        {
            UpdateLoadingProgress(LocalizationKeys.InitializingCore);
            yield return _stepWait;

            if (!ValidateCoreRegistries())
                yield break;

            gameEventRegistry.Validate();
            gameEventRegistry.Install();

            gameDatabaseRegistry.Validate();
            gameDatabaseRegistry.Install();

            UpdateLoadingProgress(LocalizationKeys.CoreReady);
            yield return _stepWait;
        }

        private IEnumerator InitializeSaveSystemsAsync()
        {
            UpdateLoadingProgress(LocalizationKeys.LoadingSettings);
            yield return _stepWait;

            if (settingsSaveFileManager)
            {
                settingsSaveFileManager.enabled = true;
                settingsSaveFileManager.InitSettings();
            }

            UpdateLoadingProgress(LocalizationKeys.UserDataLoaded);
            yield return _stepWait;
        }

        private IEnumerator InitializePoolingSystemsAsync()
        {
            UpdateLoadingProgress(LocalizationKeys.PreloadingAssets);
            yield return _stepWait;

            gamePoolManager?.Initialise();

            UpdateLoadingProgress(LocalizationKeys.AssetsLoaded);
            yield return _stepWait;
        }

        private IEnumerator LoadStartMenuAsync()
        {
            UpdateLoadingProgress(LocalizationKeys.LoadingGame);
            yield return _stepWait;
            UpdateLoadingProgress(LocalizationKeys.LoadingComplete);
            yield return _fadeWait;
            SceneManager.LoadScene(GameConstants.StartMenu);
        }

        #endregion

        #region Scene Management

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            ShowLoadingScreen(LocalizationKeys.LoadingScene);
            GameEvents.OnGameStateChangeRequested?.Raise(GameState.Loading);

            // Fake loading for minimum time
            yield return _minimumLoadWait;

            UpdateLoadingProgress(LocalizationKeys.LoadingSceneComplete);
            HideLoadingScreen();

            // Wait a bit before switching scene for smooth fade-out
            yield return _fadeWait;

            // Load new scene synchronously
            SceneManager.LoadScene(sceneName);
        }

        #endregion

        #region Utility

        private bool ValidateCoreRegistries()
        {
            if (!gameEventRegistry)
            {
                Debug.LogError("GameEventRegistry not assigned!");
                return false;
            }

            if (!gameDatabaseRegistry)
            {
                Debug.LogError("GameDatabaseRegistry not assigned!");
                return false;
            }

            return true;
        }

        private void StartBootstrapSequence()
        {
            _isInitialized = false;
            ShowLoadingScreen(LocalizationKeys.Initializing);
            GameEvents.OnGameStateChangeRequested?.Raise(GameState.Loading);
        }

        private void CompleteBootstrapSequence()
        {
            _isInitialized = true;
            HideLoadingScreen();
            GameEvents.OnGameStateChangeRequested?.Raise(GameState.StartMenu);
        }

        private void ShowLoadingScreen(string message)
        {
            loadingScreen?.Show(message);
        }

        private void UpdateLoadingProgress(string message)
        {
            loadingScreen?.UpdateProgress(message);
        }

        private void HideLoadingScreen()
        {
            loadingScreen?.Hide();
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeSingleton();

            // Cache WaitForSeconds to reduce GC allocations
            _stepWait = new WaitForSeconds(stepDelay);
            _minimumLoadWait = new WaitForSeconds(minimumLoadTime);
            _fadeWait = new WaitForSeconds(fadeWaitTime);

            settingsSaveFileManager.enabled = false;
        }

        private void Start()
        {
            StartCoroutine(BootstrapSequence());
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region Singleton

        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Cleanup()
        {
            if (Instance != this) return;

            GameDatabases.Clear();
            GameEvents.Clear();

            Instance = null;
        }

        #endregion
    }
}
