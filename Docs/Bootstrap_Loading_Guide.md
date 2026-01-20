# Bootstrap and Loading Guide

Game initialization, loading screens, and async scene management for VR.

---

## Why Bootstrap Matters

In VR, any frame drop causes:
- Tracking freeze
- Disorientation
- Motion sickness

Bootstrap ensures all systems initialize before gameplay, with loading screens covering any heavy operations.

---

## Scene Structure

```
Build Index 0: Bootstrapper
  - GameBootstrap (events, databases)
  - Persistent managers
  - Loading screen
  
Build Index 1: Hub
  - Player spawn
  - Shop/armory
  - Arena portals

Build Index 2+: Arenas
  - Loaded additively
  - Unloaded on exit
```

---

## Initialization Sequence

### Phase 1: Core Systems (Synchronous)

1. Create singleton managers
2. Initialize event channels (GameEvents)
3. Load databases (GameDatabases)
4. Initialize audio system

### Phase 2: User Data (May be Async)

5. Load player settings
6. Apply graphics/audio settings
7. Load or create save file
8. Initialize player attributes

### Phase 3: Pre-loading (Async with Loading Screen)

9. Pre-warm object pools
10. Load common assets
11. Initialize XR systems

### Phase 4: Ready

12. Hide loading screen
13. Enable input
14. Start gameplay

---

## GameBootstrap

Initializes all static references on startup.

```csharp
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private VoidEventChannel onGameSaved;
    [SerializeField] private IntEventChannel onGoldChanged;
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private EnemyDatabase enemyDatabase;
    
    public void Initialize()
    {
        // Assign to static classes
        GameEvents.OnGameSaved = onGameSaved;
        GameEvents.OnGoldChanged = onGoldChanged;
        GameDatabases.WeaponDatabase = weaponDatabase;
        GameDatabases.EnemyDatabase = enemyDatabase;
        
        // Build lookup dictionaries
        weaponDatabase.BuildLookup();
        enemyDatabase.BuildLookup();
    }
}
```

---

## Loading Screen Requirements

### Must Have

- Always visible (never black screen)
- Static or minimal movement
- Progress indicator
- Comfortable viewing distance

### Should Have

- Fade transitions
- Loading tips
- Cancel option for long loads

### Avoid

- Complex 3D scenes
- Rapid animations
- Small text

---

## Async Scene Loading

### Basic Pattern

```csharp
public IEnumerator LoadSceneAsync(string sceneName)
{
    // Show loading
    loadingScreen.Show();
    loadingScreen.UpdateProgress(0, "Loading...");
    
    // Unload current
    yield return SceneManager.UnloadSceneAsync(currentScene);
    yield return Resources.UnloadUnusedAssets();
    
    // Load new
    var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    operation.allowSceneActivation = false;
    
    while (operation.progress < 0.9f)
    {
        loadingScreen.UpdateProgress(operation.progress, "Loading...");
        yield return null;
    }
    
    operation.allowSceneActivation = true;
    yield return operation;
    
    // Hide loading
    loadingScreen.UpdateProgress(1, "Ready!");
    yield return loadingScreen.FadeOut(0.3f);
}
```

### Key Points

- Never block main thread
- Use allowSceneActivation for controlled activation
- Yield between heavy operations
- Show progress to player

---

## Pool Pre-warming

Instantiate pooled objects during loading, not gameplay.

```csharp
public IEnumerator PrewarmPools()
{
    int batchSize = 5;
    int instantiated = 0;
    
    foreach (var config in poolConfigs)
    {
        for (int i = 0; i < config.count; i++)
        {
            Instantiate(config.prefab).SetActive(false);
            instantiated++;
            
            if (instantiated >= batchSize)
            {
                instantiated = 0;
                yield return null;  // Yield to prevent freeze
            }
        }
    }
}
```

---

## Persistent Objects

Objects that survive scene loads:

```csharp
void Awake()
{
    DontDestroyOnLoad(gameObject);
}
```

Typical persistent objects:
- GameBootstrap
- AudioManager
- SaveDataManager
- XR Rig
- LoadingScreen

---

## Best Practices

| Practice | Reason |
|----------|--------|
| Show loading for operations over 100ms | Player knows game is responding |
| Pre-warm pools during loading | No instantiation spikes in gameplay |
| Fade transitions | Hide loading artifacts |
| Keep XR Rig persistent | Never lose tracking |
| Test on device | Editor performance differs |
