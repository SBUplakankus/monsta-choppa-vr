# Database Pattern

Generic ScriptableObject-based database pattern for storing and retrieving game data with O(1) lookups.

> **Source**: [`Assets/Scripts/Databases/`](../../Assets/Scripts/Databases/)

---

## Overview

Databases provide centralized access to game content (weapons, enemies, audio, particles) without hard-coded references or scene dependencies.

```
GameDatabases (static access)
    │
    ├── WeaponDatabase  →  WeaponData[]
    ├── EnemyDatabase   →  EnemyData[]
    ├── AudioClipDatabase → AudioClipData[]
    ├── WorldAudioDatabase → WorldAudioData[]
    ├── ParticleDatabase → ParticleData[]
    └── ArenaDatabase → ArenaData[]
```

---

## DatabaseBase<T\>

Generic base class for all databases. Uses ordinal string comparison for better performance.

> **Source**: [`DatabaseBase.cs`](../../Assets/Scripts/Databases/DatabaseBase.cs)

```csharp
public abstract class DatabaseBase<T> : ScriptableObject
{
    [SerializeField] private T[] entries;
    
    private Dictionary<string, T> _lookup;
    private bool _isLookupBuilt;
    
    public T[] Entries => entries;
    
    protected abstract string GetKey(T entry);
    
    private static string NormalizeKey(string id) => id.Trim().ToLowerInvariant();
    
    private void BuildLookup()
    {
        if (_isLookupBuilt) return;
        
        _lookup = new Dictionary<string, T>(entries.Length, StringComparer.Ordinal);
        
        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            var key = NormalizeKey(GetKey(entry));
            _lookup[key] = entry;
        }
        
        _isLookupBuilt = true;
    }
    
    public bool TryGet(string id, out T entry)
    {
        if (!_isLookupBuilt) BuildLookup();
        return _lookup.TryGetValue(NormalizeKey(id), out entry);
    }
    
    public T Get(string id)
    {
        if (!_isLookupBuilt) BuildLookup();
        return _lookup[NormalizeKey(id)];
    }
}
```

---

## Implementations

> **Source**: [`Assets/Scripts/Databases/`](../../Assets/Scripts/Databases/)

### WeaponDatabase

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Weapon")]
public class WeaponDatabase : DatabaseBase<WeaponData>
{
    protected override string GetKey(WeaponData entry) => entry.WeaponID;
}
```

### EnemyDatabase

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Enemy")]
public class EnemyDatabase : DatabaseBase<EnemyData>
{
    protected override string GetKey(EnemyData entry) => entry.EnemyId;
}
```

---

## GameDatabases Static Access

Global access point for all databases. Assigned during bootstrap.

> **Source**: [`GameDatabases.cs`](../../Assets/Scripts/Databases/GameDatabases.cs)

```csharp
public static class GameDatabases
{
    public static AudioClipDatabase AudioClipDatabase { get; internal set; }
    public static WorldAudioDatabase WorldAudioDatabase { get; internal set; }
    public static WeaponDatabase WeaponDatabase { get; internal set; }
    public static EnemyDatabase EnemyDatabase { get; internal set; }
    public static ParticleDatabase ParticleDatabase { get; internal set; }
    public static UpgradeDatabase UpgradeDatabase { get; internal set; }
    public static ArenaDatabase ArenaDatabase { get; internal set; }
    
    public static void Clear()
    {
        AudioClipDatabase = null;
        WorldAudioDatabase = null;
        WeaponDatabase = null;
        EnemyDatabase = null;
        ParticleDatabase = null;
        ArenaDatabase = null;
        UpgradeDatabase = null;
    }
}
```

---

## Usage

### Safe Retrieval

```csharp
if (GameDatabases.WeaponDatabase.TryGet("sword_fire", out var weapon))
{
    SpawnWeapon(weapon);
}
```

### Direct Retrieval

```csharp
// Note: Throws KeyNotFoundException if entry doesn't exist
var enemy = GameDatabases.EnemyDatabase.Get("goblin_melee");
SpawnEnemy(enemy);
```

### With Pooling

```csharp
var enemyData = GameDatabases.EnemyDatabase.Get(enemyId);
var instance = GamePoolManager.Instance.GetEnemyPrefab(enemyData, position, rotation);
```

---

## Data Flow

1. Designer creates ScriptableObject asset (e.g., WeaponData)
2. Asset added to database entries array
3. Database builds lookup dictionary on first access
4. Runtime systems query via GameDatabases static class
5. Retrieved data used to configure pooled instances

---

## Adding New Database Types

### Step 1: Create Data ScriptableObject

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityId;
    public string displayName;
    public float cooldown;
    public GameObject effectPrefab;
}
```

### Step 2: Create Database Class

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Ability Database")]
public class AbilityDatabase : DatabaseBase<AbilityData>
{
    protected override string GetKey(AbilityData entry) => entry.abilityId;
}
```

### Step 3: Add to GameDatabases

```csharp
public static AbilityDatabase AbilityDatabase { get; set; }
```

---

## Best Practices

| Practice | Reason |
|:---------|:-------|
| Use TryGet over Get | Handles missing entries gracefully |
| Normalize keys (lowercase, trimmed) | Prevents lookup failures from formatting |
| Keep databases read-only at runtime | Maintains data consistency |
| Assign databases during bootstrap | Ensures availability before gameplay |
