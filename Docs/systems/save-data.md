# Save Data System

Player persistence using ESave for JSON-based save files with event-driven architecture.

---

## Architecture

```
SaveFileManagerBase (abstract)
    │
    ├── PlayerSaveFileManager (progression data)
    │
    └── SettingsSaveFileManager (user preferences)
```

---

## SaveFileManagerBase

Abstract base class for save file managers.

```csharp
public abstract class SaveFileManagerBase : MonoBehaviour
{
    protected SaveFile saveFile;
    
    protected virtual void Awake()
    {
        saveFile = GetComponent<SaveFile>();
    }
    
    protected abstract void HandleSaveRequested();
    protected abstract void HandleSaveCompleted();
    protected abstract void HandleLoadRequested();
    protected abstract void HandleLoadCompleted();
}
```

---

## PlayerSaveFileManager

Manages player progression persistence.

```csharp
public class PlayerSaveFileManager : SaveFileManagerBase
{
    [SerializeField] private MetaProgressionData metaProgression;
    [SerializeField] private VoidEventChannel onSaveRequested;
    [SerializeField] private VoidEventChannel onSaveCompleted;
    [SerializeField] private VoidEventChannel onLoadRequested;
    [SerializeField] private VoidEventChannel onLoadCompleted;
    
    private void OnEnable()
    {
        onSaveRequested.Subscribe(HandleSaveRequested);
        onLoadRequested.Subscribe(HandleLoadRequested);
    }
    
    private void OnDisable()
    {
        onSaveRequested.Unsubscribe(HandleSaveRequested);
        onLoadRequested.Unsubscribe(HandleLoadRequested);
    }
    
    protected override void HandleSaveRequested()
    {
        saveFile.AddOrUpdateData(GameConstants.MetaProgressionKey, metaProgression);
        saveFile.Save();
    }
    
    protected override void HandleLoadRequested()
    {
        if (saveFile.HasData(GameConstants.MetaProgressionKey))
        {
            var loaded = saveFile.GetData<MetaProgressionData>(GameConstants.MetaProgressionKey);
            // Apply loaded data to metaProgression
        }
        onLoadCompleted.Raise();
    }
}
```

---

## MetaProgressionData

ScriptableObject storing player progression.

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Progression/Meta Progression")]
public class MetaProgressionData : ScriptableObject
{
    [Header("Currency")]
    public int gold;
    public int gems;
    
    [Header("Experience")]
    public int experience;
    public int level;
    
    [Header("Multipliers")]
    public float expGainMultiplier = 1f;
    public float goldGainMultiplier = 1f;
    public float healthMultiplier = 1f;
    public float armorMultiplier = 1f;
    
    [Header("Unlocks")]
    public List<string> unlockedWeapons;
    public List<string> unlockedArenas;
    public List<string> purchasedUpgrades;
}
```

---

## Save Keys

All save keys are defined in GameConstants.

```csharp
public static class GameConstants
{
    public const string MetaProgressionKey = "MetaProgression";
    public const string PlayerGoldKey = "PlayerGold";
    public const string PlayerExperienceKey = "PlayerExperience";
    public const string PlayerLevelKey = "PlayerLevel";
    public const string SettingsKey = "Settings";
}
```

---

## Save Triggers

### Manual Save

```csharp
GameEvents.OnSaveRequested.Raise();
```

### Automatic Save Points

Save occurs on:

- Arena completion (victory or defeat)
- Returning to hub
- Purchasing upgrades
- Application pause (headset removed)

```csharp
private void OnApplicationPause(bool paused)
{
    if (paused)
    {
        GameEvents.OnSaveRequested.Raise();
    }
}
```

---

## Settings Save

SettingsSaveFileManager handles user preferences separately from progression.

```csharp
public class SettingsSaveFileManager : SaveFileManagerBase
{
    [SerializeField] private FloatAttribute masterVolume;
    [SerializeField] private FloatAttribute musicVolume;
    [SerializeField] private FloatAttribute sfxVolume;
    
    protected override void HandleSaveRequested()
    {
        var settings = new SettingsData
        {
            masterVolume = masterVolume.Value,
            musicVolume = musicVolume.Value,
            sfxVolume = sfxVolume.Value
        };
        
        saveFile.AddOrUpdateData(GameConstants.SettingsKey, settings);
        saveFile.Save();
    }
}
```

---

## ESave API

| Method | Purpose |
|:-------|:--------|
| AddOrUpdateData(key, value) | Store data by key |
| GetData<T\>(key) | Retrieve typed data |
| HasData(key) | Check if key exists |
| Save() | Write to disk |
| Load() | Read from disk |
| Delete(key) | Remove specific key |
| DeleteAll() | Clear all data |

**File Locations:**

- Windows: `%USERPROFILE%/AppData/LocalLow/[Company]/[Product]/`
- Android: `/data/data/[package]/files/`

---

## Data Flow

**Save:**

1. Game event occurs (enemy killed, gold collected)
2. PlayerAttributes updates IntAttribute values
3. Event channels notify subscribers
4. Save trigger fires (manual or automatic)
5. SaveFileManager serializes data
6. ESave writes to disk

**Load:**

1. Game starts or load requested
2. SaveFileManager reads from ESave
3. Data applied to ScriptableObjects/Attributes
4. Event channels notify UI to refresh

---

## Best Practices

| Practice | Reason |
|:---------|:-------|
| Use constants for keys | Prevents typos, enables refactoring |
| Validate loaded data | Handle missing or corrupted saves |
| Save on significant events | Don't lose player progress |
| Separate settings from progression | Settings apply globally, progression per-save |
| Handle save failures gracefully | Disk full, permissions, etc. |
