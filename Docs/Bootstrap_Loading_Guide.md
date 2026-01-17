# 🚀 Bootstrap, Loading Screens & Async Loading for VR

A comprehensive guide to proper game initialization, loading screens, and async scene loading for smooth VR experiences.

---

## Table of Contents

1. [Why Bootstrap Matters in VR](#why-bootstrap-matters-in-vr)
2. [Bootstrap Architecture](#bootstrap-architecture)
3. [The Bootstrap Sequence](#the-bootstrap-sequence)
4. [Loading Screens for VR](#loading-screens-for-vr)
5. [Async Scene Loading](#async-scene-loading)
6. [Addressables Integration](#addressables-integration)
7. [Pool Pre-warming](#pool-pre-warming)
8. [Performance Optimization](#performance-optimization)
9. [Example Implementation](#example-implementation)
10. [Troubleshooting](#troubleshooting)

---

## Why Bootstrap Matters in VR

### The VR Freeze Problem

In VR, any frame drop or freeze is immediately noticeable and uncomfortable:

```
Traditional Game Loading:
Load assets... [50ms freeze]... Initialize systems... [100ms freeze]... Ready!
Player experience: Minor annoyance

VR Loading:
Load assets... [50ms freeze]... Initialize systems... [100ms freeze]... Ready!
Player experience: Tracking lost → Disorientation → Motion sickness

VR Requirement: NEVER drop below 72 FPS during gameplay
```

### What Bootstrap Solves

1. **Pre-initialization**: Set up systems before gameplay begins
2. **Controlled Loading**: Show loading UI while work happens
3. **Graceful Transitions**: Fade in/out to hide loading
4. **Fail-safe Defaults**: Handle missing data gracefully
5. **Async Operations**: Load without freezing the frame

### Current Project State

Your project has a basic bootstrap system mentioned in `Architecture.md`:

```csharp
// GameBootstrap initializes:
// - Event Channels → static GameEvents
// - Databases → static GameDatabases
// - Ensures systems are ready before gameplay
```

This guide expands on that foundation.

---

## Bootstrap Architecture

### Scene Structure

```
Recommended Scene Organization:
┌─────────────────────────────────────────────────────────────┐
│ Build Index 0: Bootstrapper                                  │
│   - Bootstrap systems                                        │
│   - Persistent managers                                      │
│   - Loading screen                                           │
│   - Loads MainMenu scene additively                          │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ Build Index 1: MainMenu                                      │
│   - Start menu UI                                            │
│   - Settings access                                          │
│   - Save slot selection                                      │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ Build Index 2: Hub                                           │
│   - Player's home base                                       │
│   - Upgrade shop                                             │
│   - Arena portals                                            │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ Build Index 3+: Arena_01, Arena_02, etc.                     │
│   - Combat arenas                                            │
│   - Loaded/unloaded as needed                                │
└─────────────────────────────────────────────────────────────┘
```

### Persistent Objects

Objects that survive scene loads:

```csharp
// Mark with DontDestroyOnLoad
public class BootstrapManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

// Common persistent objects:
// - GameBootstrap (event channels, databases)
// - AudioManager (music continuation)
// - SaveDataManager (player data)
// - XR Rig (player position, hands)
// - LoadingScreen (UI during transitions)
// - SpaceWarpManager (performance settings)
```

---

## The Bootstrap Sequence

### Recommended Initialization Order

```
Phase 1: Core Systems (synchronous, fast)
1. Create singleton managers
2. Initialize event channels
3. Load databases
4. Initialize audio system

Phase 2: User Data (may require async)
5. Load player settings
6. Apply graphics/audio settings
7. Load save file (or create new)
8. Initialize player attributes

Phase 3: Pre-loading (async with loading screen)
9. Pre-warm object pools
10. Load commonly used assets
11. Initialize XR systems
12. Connect to online services (if any)

Phase 4: Scene Ready
13. Hide loading screen
14. Enable input
15. Start gameplay
```

### Initialization Manager

```csharp
// BootstrapManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Core
{
    /// <summary>
    /// Manages the bootstrap sequence for the game.
    /// This is the first thing that runs and sets up all systems.
    /// </summary>
    public class BootstrapManager : MonoBehaviour
    {
        #region Singleton
        
        public static BootstrapManager Instance { get; private set; }
        
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
        
        #region Fields
        
        [Header("Bootstrap Dependencies")]
        [SerializeField] private GameBootstrap gameBootstrap;
        [SerializeField] private LoadingScreenController loadingScreen;
        
        [Header("Settings")]
        [SerializeField] private string firstSceneToLoad = "MainMenu";
        [SerializeField] private float minimumLoadTime = 2f; // Min time to show loading
        
        // Events
        public event Action<float, string> OnLoadProgress;
        public event Action OnBootstrapComplete;
        
        private bool _isInitialized;
        
        #endregion
        
        #region Properties
        
        public bool IsInitialized => _isInitialized;
        
        #endregion
        
        #region Initialization
        
        private IEnumerator Start()
        {
            // Show loading screen immediately
            loadingScreen?.Show("Initializing...");
            
            // Phase 1: Core Systems
            yield return StartCoroutine(InitializeCoreSystemsAsync());
            
            // Phase 2: User Data
            yield return StartCoroutine(LoadUserDataAsync());
            
            // Phase 3: Pre-loading
            yield return StartCoroutine(PrewarmAssetsAsync());
            
            // Phase 4: Load first scene
            yield return StartCoroutine(LoadFirstSceneAsync());
            
            // Complete
            _isInitialized = true;
            OnBootstrapComplete?.Invoke();
            
            // Hide loading screen
            loadingScreen?.Hide();
        }
        
        #endregion
        
        #region Phase Implementations
        
        private IEnumerator InitializeCoreSystemsAsync()
        {
            ReportProgress(0.1f, "Initializing core systems...");
            
            // Initialize GameBootstrap (event channels, databases)
            gameBootstrap?.Initialize();
            
            // Small yield to prevent freeze
            yield return null;
            
            ReportProgress(0.2f, "Core systems ready");
        }
        
        private IEnumerator LoadUserDataAsync()
        {
            ReportProgress(0.3f, "Loading settings...");
            
            // Load settings from disk
            // SettingsManager.Instance?.LoadSettings();
            yield return null;
            
            ReportProgress(0.4f, "Loading save data...");
            
            // Load or create save file
            // SaveDataManager.Instance?.LoadOrCreateSave();
            yield return null;
            
            ReportProgress(0.5f, "User data loaded");
        }
        
        private IEnumerator PrewarmAssetsAsync()
        {
            ReportProgress(0.6f, "Pre-loading assets...");
            
            // Pre-warm pools
            // This should be done in small batches to avoid freezes
            // yield return GamePoolManager.Instance?.PrewarmPoolsAsync(progress);
            
            yield return null;
            
            ReportProgress(0.8f, "Assets ready");
        }
        
        private IEnumerator LoadFirstSceneAsync()
        {
            ReportProgress(0.9f, "Loading game...");
            
            // Load first scene additively
            var operation = SceneManager.LoadSceneAsync(firstSceneToLoad, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            
            while (!operation.isDone)
            {
                // Loading progress is 0-0.9 for loading, 0.9-1.0 for activation
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                ReportProgress(0.9f + (progress * 0.1f), "Loading game...");
                
                // Wait until ready
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            ReportProgress(1f, "Ready!");
        }
        
        #endregion
        
        #region Progress Reporting
        
        private void ReportProgress(float progress, string message)
        {
            OnLoadProgress?.Invoke(progress, message);
            loadingScreen?.UpdateProgress(progress, message);
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
                // Fade to loading screen
                yield return loadingScreen?.FadeIn(0.3f);
                loadingScreen?.UpdateProgress(0, "Loading...");
            }
            
            float startTime = Time.time;
            
            // Unload current gameplay scene
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != "Bootstrapper")
            {
                yield return SceneManager.UnloadSceneAsync(currentScene);
            }
            
            // Clean up memory
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            yield return null;
            
            // Load new scene
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;
            
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingScreen?.UpdateProgress(progress, $"Loading {sceneName}...");
                
                if (operation.progress >= 0.9f)
                {
                    // Ensure minimum load time for visual consistency
                    float elapsed = Time.time - startTime;
                    if (elapsed < minimumLoadTime)
                    {
                        yield return new WaitForSeconds(minimumLoadTime - elapsed);
                    }
                    
                    operation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            // Set as active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            
            if (showLoading)
            {
                loadingScreen?.UpdateProgress(1, "Ready!");
                yield return new WaitForSeconds(0.2f);
                yield return loadingScreen?.FadeOut(0.3f);
            }
        }
        
        #endregion
    }
}
```

---

## Loading Screens for VR

### VR Loading Screen Requirements

```
Must Have:
✓ Always visible (never black screen)
✓ Static or minimal movement (prevent sickness)
✓ Progress indicator (player knows it's working)
✓ Comfortable viewing distance

Should Have:
✓ Fade transitions (smooth, not jarring)
✓ Loading tips or hints
✓ Estimated time remaining
✓ Cancel option for long loads

Avoid:
✗ 3D loading scenes (complex = more loading time)
✗ Animations that require tracking (disorients player)
✗ Bright flashing elements
✗ Small text (hard to read in VR)
```

### Loading Screen Controller

```csharp
// LoadingScreenController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Core
{
    /// <summary>
    /// VR-safe loading screen using UI Toolkit.
    /// </summary>
    public class LoadingScreenController : MonoBehaviour
    {
        #region Fields
        
        [Header("UI")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet styleSheet;
        
        [Header("Settings")]
        [SerializeField] private float fadeSpeed = 3f;
        [SerializeField] private bool attachToCamera = true;
        [SerializeField] private float distanceFromCamera = 2f;
        
        // UI Elements
        private VisualElement _root;
        private VisualElement _background;
        private Label _messageLabel;
        private VisualElement _progressBarFill;
        private Label _progressLabel;
        private VisualElement _spinner;
        
        // State
        private bool _isVisible;
        private float _currentAlpha;
        private Coroutine _fadeCoroutine;
        
        #endregion
        
        #region Initialization
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            if (uiDocument == null) return;
            
            _root = uiDocument.rootVisualElement;
            _root.styleSheets.Add(styleSheet);
            
            // Create loading screen layout
            _background = new VisualElement();
            _background.AddToClassList("loading-background");
            _background.style.opacity = 0;
            
            var container = new VisualElement();
            container.AddToClassList("loading-container");
            
            // Spinner/logo
            _spinner = new VisualElement();
            _spinner.AddToClassList("loading-spinner");
            container.Add(_spinner);
            
            // Message
            _messageLabel = new Label("Loading...");
            _messageLabel.AddToClassList("loading-message");
            container.Add(_messageLabel);
            
            // Progress bar
            var progressContainer = new VisualElement();
            progressContainer.AddToClassList("loading-progress-container");
            
            _progressBarFill = new VisualElement();
            _progressBarFill.AddToClassList("loading-progress-fill");
            progressContainer.Add(_progressBarFill);
            
            container.Add(progressContainer);
            
            // Progress percentage
            _progressLabel = new Label("0%");
            _progressLabel.AddToClassList("loading-progress-text");
            container.Add(_progressLabel);
            
            _background.Add(container);
            _root.Add(_background);
            
            // Start hidden
            _background.style.display = DisplayStyle.None;
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Shows the loading screen immediately.
        /// </summary>
        public void Show(string message = "Loading...")
        {
            _background.style.display = DisplayStyle.Flex;
            _background.style.opacity = 1;
            _currentAlpha = 1;
            _isVisible = true;
            _messageLabel.text = message;
        }
        
        /// <summary>
        /// Hides the loading screen immediately.
        /// </summary>
        public void Hide()
        {
            _background.style.display = DisplayStyle.None;
            _background.style.opacity = 0;
            _currentAlpha = 0;
            _isVisible = false;
        }
        
        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        public IEnumerator FadeIn(float duration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            
            _background.style.display = DisplayStyle.Flex;
            _isVisible = true;
            
            float startAlpha = _currentAlpha;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _currentAlpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
                _background.style.opacity = _currentAlpha;
                yield return null;
            }
            
            _currentAlpha = 1;
            _background.style.opacity = 1;
        }
        
        /// <summary>
        /// Fades out the loading screen.
        /// </summary>
        public IEnumerator FadeOut(float duration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            
            float startAlpha = _currentAlpha;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _currentAlpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                _background.style.opacity = _currentAlpha;
                yield return null;
            }
            
            _currentAlpha = 0;
            _background.style.opacity = 0;
            _background.style.display = DisplayStyle.None;
            _isVisible = false;
        }
        
        /// <summary>
        /// Updates progress bar and message.
        /// </summary>
        public void UpdateProgress(float progress, string message = null)
        {
            // Update progress bar (0-100%)
            _progressBarFill.style.width = new Length(progress * 100, LengthUnit.Percent);
            _progressLabel.text = $"{Mathf.RoundToInt(progress * 100)}%";
            
            // Update message if provided
            if (!string.IsNullOrEmpty(message))
            {
                _messageLabel.text = message;
            }
        }
        
        #endregion
        
        #region Camera Following
        
        private void LateUpdate()
        {
            if (!attachToCamera || !_isVisible) return;
            
            // Position loading screen in front of camera
            var camera = Camera.main;
            if (camera == null) return;
            
            var targetPos = camera.transform.position + camera.transform.forward * distanceFromCamera;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
            transform.LookAt(camera.transform);
            transform.Rotate(0, 180, 0); // Face camera
        }
        
        #endregion
    }
}
```

### Loading Screen Styles (USS)

```css
/* LoadingScreen.uss */
.loading-background {
    position: absolute;
    width: 100%;
    height: 100%;
    background-color: rgba(10, 10, 15, 0.95);
    align-items: center;
    justify-content: center;
}

.loading-container {
    align-items: center;
    justify-content: center;
    padding: 40px;
    background-color: rgba(30, 30, 40, 0.8);
    border-radius: 20px;
    min-width: 400px;
}

.loading-spinner {
    width: 80px;
    height: 80px;
    background-image: url('/Assets/UI/spinner.png');
    margin-bottom: 20px;
    /* Rotation animation handled via USS transitions or code */
}

.loading-message {
    font-size: 24px;
    color: #FFFFFF;
    margin-bottom: 20px;
    -unity-text-align: middle-center;
}

.loading-progress-container {
    width: 300px;
    height: 20px;
    background-color: rgba(50, 50, 60, 0.8);
    border-radius: 10px;
    overflow: hidden;
    margin-bottom: 10px;
}

.loading-progress-fill {
    height: 100%;
    background-color: #4A90D9;
    border-radius: 10px;
    width: 0%;
    transition: width 0.2s;
}

.loading-progress-text {
    font-size: 18px;
    color: #AAAAAA;
    -unity-text-align: middle-center;
}
```

---

## Async Scene Loading

### VR-Safe Scene Loading

The key principle: **Never block the main thread during scene loads**.

```csharp
// SceneLoadingManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Core
{
    /// <summary>
    /// Manages async scene loading with VR-safe frame pacing.
    /// </summary>
    public class SceneLoadingManager : MonoBehaviour
    {
        #region Singleton
        
        public static SceneLoadingManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        #endregion
        
        #region Fields
        
        [Header("Settings")]
        [SerializeField] private int maxFrameTimeMs = 8; // Stay under 11ms for 90fps
        [SerializeField] private LoadingScreenController loadingScreen;
        
        public event Action<string> OnSceneLoadStarted;
        public event Action<string, float> OnSceneLoadProgress;
        public event Action<string> OnSceneLoadCompleted;
        
        private bool _isLoading;
        private Queue<string> _loadQueue = new Queue<string>();
        
        #endregion
        
        #region Properties
        
        public bool IsLoading => _isLoading;
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Loads a scene asynchronously with proper VR transitions.
        /// </summary>
        public void LoadScene(string sceneName, bool unloadCurrent = true)
        {
            if (_isLoading)
            {
                // Queue the load if already loading
                _loadQueue.Enqueue(sceneName);
                return;
            }
            
            StartCoroutine(LoadSceneAsync(sceneName, unloadCurrent));
        }
        
        /// <summary>
        /// Loads a scene additively (doesn't unload current).
        /// </summary>
        public void LoadSceneAdditive(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName, false));
        }
        
        /// <summary>
        /// Unloads a specific scene.
        /// </summary>
        public void UnloadScene(string sceneName)
        {
            StartCoroutine(UnloadSceneAsync(sceneName));
        }
        
        #endregion
        
        #region Async Operations
        
        private IEnumerator LoadSceneAsync(string sceneName, bool unloadCurrent)
        {
            _isLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);
            
            // Phase 1: Fade to loading screen
            if (loadingScreen != null)
            {
                yield return loadingScreen.FadeIn(0.3f);
                loadingScreen.UpdateProgress(0, $"Loading {sceneName}...");
            }
            
            // Phase 2: Unload current scene if requested
            if (unloadCurrent)
            {
                var currentScene = SceneManager.GetActiveScene();
                if (currentScene.isLoaded && currentScene.name != "Bootstrapper")
                {
                    loadingScreen?.UpdateProgress(0.1f, "Cleaning up...");
                    
                    var unloadOp = SceneManager.UnloadSceneAsync(currentScene);
                    while (!unloadOp.isDone)
                    {
                        yield return null;
                    }
                    
                    // Memory cleanup
                    yield return CleanupMemoryAsync();
                }
            }
            
            loadingScreen?.UpdateProgress(0.2f, "Loading assets...");
            
            // Phase 3: Load new scene
            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = false;
            
            while (!loadOp.isDone)
            {
                // Scale progress to 20%-90% range
                float progress = Mathf.Lerp(0.2f, 0.9f, loadOp.progress / 0.9f);
                OnSceneLoadProgress?.Invoke(sceneName, progress);
                loadingScreen?.UpdateProgress(progress, $"Loading {sceneName}...");
                
                if (loadOp.progress >= 0.9f)
                {
                    loadOp.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            // Phase 4: Activate and set as active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            
            loadingScreen?.UpdateProgress(0.95f, "Preparing scene...");
            
            // Phase 5: Scene initialization (one frame for scene to set up)
            yield return null;
            yield return null;
            
            loadingScreen?.UpdateProgress(1f, "Ready!");
            
            // Phase 6: Fade out loading screen
            if (loadingScreen != null)
            {
                yield return new WaitForSeconds(0.2f);
                yield return loadingScreen.FadeOut(0.3f);
            }
            
            _isLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
            
            // Check for queued loads
            if (_loadQueue.Count > 0)
            {
                string nextScene = _loadQueue.Dequeue();
                StartCoroutine(LoadSceneAsync(nextScene, true));
            }
        }
        
        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                Debug.LogWarning($"Scene {sceneName} is not loaded.");
                yield break;
            }
            
            var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadOp.isDone)
            {
                yield return null;
            }
            
            yield return CleanupMemoryAsync();
        }
        
        private IEnumerator CleanupMemoryAsync()
        {
            // Spread cleanup across frames
            yield return Resources.UnloadUnusedAssets();
            
            // Give a frame for cleanup
            yield return null;
            
            // GC - this can cause a spike, but we're on loading screen
            GC.Collect();
            
            yield return null;
        }
        
        #endregion
    }
}
```

### Frame-Budgeted Operations

For operations that might cause spikes:

```csharp
// FrameBudgetedLoader.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Systems.Core
{
    /// <summary>
    /// Executes work items within a per-frame time budget.
    /// Prevents frame spikes during loading operations.
    /// </summary>
    public class FrameBudgetedLoader : MonoBehaviour
    {
        #region Fields
        
        [Header("Settings")]
        [SerializeField] private float maxFrameTimeMs = 8f; // Target < 11ms for 90fps
        
        private readonly Queue<Func<IEnumerator>> _workQueue = new Queue<Func<IEnumerator>>();
        private bool _isProcessing;
        private Stopwatch _frameTimer = new Stopwatch();
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Queues a work item to be processed within frame budget.
        /// </summary>
        public void QueueWork(Func<IEnumerator> work)
        {
            _workQueue.Enqueue(work);
            
            if (!_isProcessing)
            {
                StartCoroutine(ProcessWorkQueue());
            }
        }
        
        /// <summary>
        /// Queues multiple items and returns when all complete.
        /// </summary>
        public IEnumerator QueueAndWait(IEnumerable<Func<IEnumerator>> workItems)
        {
            int remaining = 0;
            
            foreach (var work in workItems)
            {
                remaining++;
                QueueWork(() => WrapWork(work(), () => remaining--));
            }
            
            while (remaining > 0)
            {
                yield return null;
            }
        }
        
        #endregion
        
        #region Work Processing
        
        private IEnumerator ProcessWorkQueue()
        {
            _isProcessing = true;
            
            while (_workQueue.Count > 0)
            {
                _frameTimer.Restart();
                
                // Process work items until budget exhausted
                while (_workQueue.Count > 0 && _frameTimer.ElapsedMilliseconds < maxFrameTimeMs)
                {
                    var work = _workQueue.Dequeue();
                    var enumerator = work();
                    
                    // Process steps within budget
                    while (enumerator.MoveNext())
                    {
                        if (_frameTimer.ElapsedMilliseconds >= maxFrameTimeMs)
                        {
                            // Re-queue remaining work
                            _workQueue.Enqueue(() => ContinueEnumerator(enumerator));
                            break;
                        }
                    }
                }
                
                // Yield to maintain frame rate
                yield return null;
            }
            
            _isProcessing = false;
        }
        
        private IEnumerator ContinueEnumerator(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
        
        private IEnumerator WrapWork(IEnumerator work, Action onComplete)
        {
            yield return work;
            onComplete?.Invoke();
        }
        
        #endregion
    }
}
```

---

## Addressables Integration

### Why Addressables?

For larger VR projects, Addressables provide:
- **Async Loading**: Native async support
- **Memory Management**: Load/unload on demand
- **Content Updates**: Update assets without new builds
- **Build Size Reduction**: Not everything in initial build

### Basic Addressables Setup

```csharp
// AddressableLoader.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Systems.Core
{
    /// <summary>
    /// Wrapper for Addressables loading with VR-safe patterns.
    /// </summary>
    public class AddressableLoader : MonoBehaviour
    {
        #region Singleton
        
        public static AddressableLoader Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        #endregion
        
        #region Fields
        
        private Dictionary<string, AsyncOperationHandle> _loadedAssets = new();
        
        #endregion
        
        #region Loading
        
        /// <summary>
        /// Loads an asset by address and caches the handle.
        /// </summary>
        public IEnumerator LoadAsset<T>(string address, System.Action<T> onComplete) where T : Object
        {
            // Check if already loaded
            if (_loadedAssets.TryGetValue(address, out var existingHandle))
            {
                if (existingHandle.IsDone)
                {
                    onComplete?.Invoke(existingHandle.Result as T);
                    yield break;
                }
            }
            
            var handle = Addressables.LoadAssetAsync<T>(address);
            _loadedAssets[address] = handle;
            
            while (!handle.IsDone)
            {
                yield return null;
            }
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to load addressable: {address}");
                onComplete?.Invoke(null);
            }
        }
        
        /// <summary>
        /// Preloads multiple assets in batches.
        /// </summary>
        public IEnumerator PreloadAssets(IList<string> addresses, System.Action<float> onProgress)
        {
            int total = addresses.Count;
            int loaded = 0;
            
            foreach (var address in addresses)
            {
                yield return LoadAsset<Object>(address, _ =>
                {
                    loaded++;
                    onProgress?.Invoke((float)loaded / total);
                });
            }
        }
        
        #endregion
        
        #region Unloading
        
        /// <summary>
        /// Releases a loaded asset.
        /// </summary>
        public void ReleaseAsset(string address)
        {
            if (_loadedAssets.TryGetValue(address, out var handle))
            {
                Addressables.Release(handle);
                _loadedAssets.Remove(address);
            }
        }
        
        /// <summary>
        /// Releases all loaded assets.
        /// </summary>
        public void ReleaseAllAssets()
        {
            foreach (var handle in _loadedAssets.Values)
            {
                Addressables.Release(handle);
            }
            _loadedAssets.Clear();
        }
        
        #endregion
        
        #region Scene Loading
        
        /// <summary>
        /// Loads an Addressable scene.
        /// </summary>
        public IEnumerator LoadAddressableScene(string sceneAddress, System.Action<float> onProgress)
        {
            var handle = Addressables.LoadSceneAsync(sceneAddress, 
                UnityEngine.SceneManagement.LoadSceneMode.Additive);
            
            while (!handle.IsDone)
            {
                onProgress?.Invoke(handle.PercentComplete);
                yield return null;
            }
            
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load scene: {sceneAddress}");
            }
        }
        
        #endregion
    }
}
```

---

## Pool Pre-warming

### Pre-warming During Loading

Object pools should be filled during loading screens, not during gameplay:

```csharp
// PoolPrewarmer.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pooling
{
    /// <summary>
    /// Pre-warms object pools during loading to prevent runtime instantiation spikes.
    /// </summary>
    public class PoolPrewarmer : MonoBehaviour
    {
        #region Fields
        
        [System.Serializable]
        public class PoolConfig
        {
            public string poolName;
            public GameObject prefab;
            public int prewarmCount;
            public int batchSize = 5; // Instantiate this many per frame
        }
        
        [SerializeField] private List<PoolConfig> poolsToPrewarm;
        [SerializeField] private int maxInstantiationsPerFrame = 5;
        
        #endregion
        
        #region Prewarm
        
        /// <summary>
        /// Pre-warms all configured pools, yielding between batches.
        /// </summary>
        public IEnumerator PrewarmAllPools(System.Action<float, string> onProgress)
        {
            int totalToCreate = 0;
            int created = 0;
            
            foreach (var config in poolsToPrewarm)
            {
                totalToCreate += config.prewarmCount;
            }
            
            foreach (var config in poolsToPrewarm)
            {
                onProgress?.Invoke((float)created / totalToCreate, $"Pre-warming {config.poolName}...");
                
                yield return PrewarmPool(config, () =>
                {
                    created += config.batchSize;
                    onProgress?.Invoke((float)created / totalToCreate, $"Pre-warming {config.poolName}...");
                });
            }
            
            onProgress?.Invoke(1f, "Pools ready!");
        }
        
        private IEnumerator PrewarmPool(PoolConfig config, System.Action onBatchComplete)
        {
            int instantiated = 0;
            int thisFrame = 0;
            
            while (instantiated < config.prewarmCount)
            {
                // Create batch
                int batchCount = Mathf.Min(config.batchSize, config.prewarmCount - instantiated);
                
                for (int i = 0; i < batchCount; i++)
                {
                    // Instantiate and add to pool
                    var instance = Instantiate(config.prefab);
                    instance.SetActive(false);
                    
                    // Add to your pool manager here
                    // GamePoolManager.Instance.AddToPool(config.poolName, instance);
                    
                    instantiated++;
                    thisFrame++;
                    
                    // Check frame budget
                    if (thisFrame >= maxInstantiationsPerFrame)
                    {
                        thisFrame = 0;
                        onBatchComplete?.Invoke();
                        yield return null; // Yield frame
                    }
                }
            }
        }
        
        #endregion
    }
}
```

### Arena-Specific Pre-warming

Pre-warm different pools based on which arena is loading:

```csharp
// Example: Called when entering an arena
public IEnumerator PrepareArena(ArenaData arenaData)
{
    // Pre-warm enemies specific to this arena
    foreach (var wave in arenaData.waves)
    {
        foreach (var spawn in wave.spawns)
        {
            yield return PrewarmEnemy(spawn.enemyPrefab, spawn.maxCount);
        }
    }
    
    // Pre-warm boss if applicable
    if (arenaData.bossWave != null)
    {
        yield return PrewarmEnemy(arenaData.bossWave.bossPrefab, 1);
    }
}
```

---

## Performance Optimization

### Loading Performance Tips

```
1. Batch Instantiation
   - Never instantiate more than 5-10 objects per frame
   - Use coroutines with yields between batches

2. Texture Streaming
   - Enable Texture Streaming in Quality Settings
   - Set appropriate streaming budgets
   - Mipmap bias for VR (slightly sharper)

3. Shader Warmup
   - Pre-compile shaders during loading
   - ShaderVariantCollection.WarmUp()

4. Audio Loading
   - Load audio clips asynchronously
   - Decompress on load for frequently used clips
   - Stream music and ambient sounds

5. Mesh Combining
   - Combine static meshes during loading
   - Use StaticBatchingUtility.Combine()
```

### Shader Pre-warming

```csharp
// ShaderPrewarmer.cs
using UnityEngine;
using System.Collections;

namespace Systems.Core
{
    /// <summary>
    /// Pre-compiles shader variants to prevent compile stutter during gameplay.
    /// </summary>
    public class ShaderPrewarmer : MonoBehaviour
    {
        [SerializeField] private ShaderVariantCollection[] variantCollections;
        
        public IEnumerator PrewarmShaders(System.Action<float> onProgress)
        {
            if (variantCollections == null || variantCollections.Length == 0)
            {
                yield break;
            }
            
            for (int i = 0; i < variantCollections.Length; i++)
            {
                var collection = variantCollections[i];
                if (collection == null) continue;
                
                onProgress?.Invoke((float)i / variantCollections.Length);
                
                // WarmUp is synchronous and can cause a spike
                // Call during loading screen only
                collection.WarmUp();
                
                // Yield to prevent total freeze
                yield return null;
            }
            
            onProgress?.Invoke(1f);
        }
    }
}
```

---

## Example Implementation

### Complete Bootstrap Scene Setup

```
Scene Hierarchy: Bootstrapper
│
├── [Bootstrap]
│   ├── BootstrapManager
│   ├── GameBootstrap
│   ├── SceneLoadingManager
│   └── LoadingScreenController
│
├── [Persistent Managers]
│   ├── AudioManager
│   ├── SaveDataManager
│   ├── SettingsManager
│   └── SpaceWarpManager
│
├── [XR]
│   └── XR Origin (Complete Setup)
│       ├── Camera Offset
│       │   ├── Main Camera
│       │   ├── Left Controller
│       │   └── Right Controller
│       └── Locomotion System
│
├── [UI]
│   └── Loading Screen Canvas
│       └── UIDocument (Loading Screen)
│
└── [Lighting]
    └── Minimal ambient lighting
```

### GameBootstrap Integration

Expand your existing `GameBootstrap` to work with the new system:

```csharp
// Enhanced GameBootstrap.cs
using Events;
using Databases;
using UnityEngine;

namespace Systems.Core
{
    /// <summary>
    /// Initializes core game systems (event channels, databases).
    /// Called by BootstrapManager during startup.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private VoidEventChannel onGameSaved;
        [SerializeField] private IntEventChannel onGoldChanged;
        [SerializeField] private IntEventChannel onExperienceChanged;
        [SerializeField] private IntEventChannel onLevelChanged;
        
        [Header("Databases")]
        [SerializeField] private AudioClipDatabase audioDatabase;
        [SerializeField] private EnemyDatabase enemyDatabase;
        [SerializeField] private WeaponDatabase weaponDatabase;
        
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// Initializes all static references.
        /// Called by BootstrapManager.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            
            InitializeEventChannels();
            InitializeDatabases();
            
            _isInitialized = true;
            Debug.Log("[GameBootstrap] Initialization complete.");
        }
        
        private void InitializeEventChannels()
        {
            // Assign to static GameEvents class
            GameEvents.OnGameSaved = onGameSaved;
            GameEvents.OnGoldChanged = onGoldChanged;
            GameEvents.OnExperienceChanged = onExperienceChanged;
            GameEvents.OnLevelChanged = onLevelChanged;
        }
        
        private void InitializeDatabases()
        {
            // Assign to static GameDatabases class
            GameDatabases.AudioClipDatabase = audioDatabase;
            GameDatabases.EnemyDatabase = enemyDatabase;
            GameDatabases.WeaponDatabase = weaponDatabase;
            
            // Trigger dictionary builds
            audioDatabase?.BuildLookup();
            enemyDatabase?.BuildLookup();
            weaponDatabase?.BuildLookup();
        }
        
        private void OnDisable()
        {
            // Clear static references
            GameEvents.Clear();
            GameDatabases.Clear();
            _isInitialized = false;
        }
    }
}
```

---

## Troubleshooting

### Common Issues

#### Issue: Frame spikes during scene load

**Cause**: Synchronous loading or too many instantiations
**Solution**: Use async loading, batch instantiations, show loading screen

#### Issue: Black screen during transition

**Cause**: Loading screen not active or wrong render order
**Solution**: Ensure loading screen shows before scene unload, verify camera stack

#### Issue: Tracking lost during load

**Cause**: XR camera disabled or scene change breaks tracking
**Solution**: Keep XR Origin persistent, never disable XR camera

#### Issue: Memory increasing over time

**Cause**: Scenes not properly unloaded, missing `Resources.UnloadUnusedAssets()`
**Solution**: Call cleanup between scene loads, verify object pools return items

#### Issue: Bootstrap runs multiple times

**Cause**: Multiple bootstrap scenes or missing `DontDestroyOnLoad`
**Solution**: Check scene load modes, verify singleton patterns

### Debug Logging

```csharp
// Add to BootstrapManager for debugging
[Header("Debug")]
[SerializeField] private bool verboseLogging = true;

private void Log(string message)
{
    if (verboseLogging)
        Debug.Log($"[Bootstrap] {message}");
}

// Use throughout bootstrap:
Log("Phase 1: Initializing core systems...");
Log("Phase 2: Loading user data...");
// etc.
```

### Performance Monitoring

```csharp
// Track loading times
private void OnSceneLoadStarted(string sceneName)
{
    _loadStartTime = Time.realtimeSinceStartup;
}

private void OnSceneLoadCompleted(string sceneName)
{
    float loadTime = Time.realtimeSinceStartup - _loadStartTime;
    Debug.Log($"Scene {sceneName} loaded in {loadTime:F2}s");
    
    // Log to analytics
    // AnalyticsManager.LogSceneLoad(sceneName, loadTime);
}
```

---

## Summary

### Key Takeaways

1. **Never block the main thread** during gameplay
2. **Show loading screens** for any operation over 100ms
3. **Pre-warm pools** during loading, not during gameplay
4. **Fade transitions** hide loading artifacts
5. **Test on device** - editor performance is not representative
6. **Budget your frames** - max 8-10ms for loading operations per frame

### Implementation Order

1. Create bootstrap scene and manager
2. Add loading screen controller
3. Implement async scene loading
4. Add pool pre-warming
5. Integrate with existing systems
6. Test on Quest device
7. Profile and optimize

---

> 💡 **Pro Tip**: The most important thing for VR loading is **never showing a black screen** or **losing tracking**. Even a simple "Loading..." text is better than nothing. Players will forgive a 5-second load if they see progress, but a 500ms freeze will break their immersion immediately.
