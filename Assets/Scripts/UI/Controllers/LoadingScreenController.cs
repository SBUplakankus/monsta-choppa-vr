using System.Collections.Generic;
using Constants;
using UI.Hosts;
using UnityEngine;

namespace UI.Controllers
{
    public struct LoadProgress
    {
        public readonly float LoadPercentage;
        public readonly string LoadingMessageKey;

        public LoadProgress(float loadPercentage, string loadingMessageKey)
        {
            LoadPercentage = loadPercentage;
            LoadingMessageKey = loadingMessageKey;
        }
    }
    
    public class LoadingScreenController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private LoadingScreenHost loadingScreenHost;
        
        private readonly Dictionary<string, LoadProgress> _loadProgresses = new()
        {
            { LocalizationKeys.Initializing, new LoadProgress(0f, LocalizationKeys.Initializing) },
            { LocalizationKeys.InitializingCore, new LoadProgress(0.1f, LocalizationKeys.InitializingCore) },
            { LocalizationKeys.CoreReady, new LoadProgress(0.2f, LocalizationKeys.CoreReady) },
            { LocalizationKeys.LoadingSettings, new LoadProgress(0.3f, LocalizationKeys.LoadingSettings) },
            { LocalizationKeys.LoadingSaves, new LoadProgress(0.4f, LocalizationKeys.LoadingSaves) },
            { LocalizationKeys.UserDataLoaded, new LoadProgress(0.5f, LocalizationKeys.UserDataLoaded) },
            { LocalizationKeys.PreloadingAssets, new LoadProgress(0.6f, LocalizationKeys.PreloadingAssets) },
            { LocalizationKeys.AssetsLoaded, new LoadProgress(0.8f, LocalizationKeys.AssetsLoaded) },
            { LocalizationKeys.LoadingGame, new LoadProgress(0.9f, LocalizationKeys.LoadingGame) },
            { LocalizationKeys.LoadingComplete, new LoadProgress(1f, LocalizationKeys.LoadingComplete) },
            { LocalizationKeys.LoadingScene, new LoadProgress(0f, LocalizationKeys.LoadingScene) },
            { LocalizationKeys.LoadingSceneComplete, new LoadProgress(1f, LocalizationKeys.LoadingSceneComplete) },
        };

        public void UpdateProgress(string key)
        {
            loadingScreenHost.UpdateLoadingScreen(_loadProgresses[key]);
        }

        public void Show(string key)
        {
            loadingScreenHost.DisplayLoadingScreen(_loadProgresses[key]);
        }

        public void Hide()
        {
            loadingScreenHost.HideLoadingScreen();
        }
    }
}