# Database System

Generic ScriptableObject-based database pattern for storing and retrieving game data with O(1) lookups.

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
    └── ParticleDatabase → ParticleData[]
```

---

## DatabaseBase<T>

Generic base class for all databases.

```csharp
public abstract class DatabaseBase<T> : ScriptableObject where T : ScriptableObject
{
    [SerializeField] private T[] entries;
    private Dictionary<string, T> _lookup;
    
    public bool TryGet(string id, out T entry)
    {
        BuildLookup();
        var key = id?.ToLower().Trim() ?? string.Empty;
        return _lookup.TryGetValue(key, out entry);
    }
    
    public T Get(string id)
    {
        return TryGet(id, out var entry) ? entry : null;
    }
    
    protected abstract string GetKey(T entry);
    
    private void BuildLookup()
    {
        if (_lookup != null) return;
        
        _lookup = new Dictionary<string, T>();
        foreach (var entry in entries)
        {
            var key = GetKey(entry)?.ToLower().Trim();
            if (!string.IsNullOrEmpty(key))
                _lookup[key] = entry;
        }
    }
}
```

---

## Database Implementations

### WeaponDatabase

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Weapon Database")]
public class WeaponDatabase : DatabaseBase<WeaponData>
{
    protected override string GetKey(WeaponData entry) => entry.WeaponID;
}
```

### EnemyDatabase

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Enemy Database")]
public class EnemyDatabase : DatabaseBase<EnemyData>
{
    protected override string GetKey(EnemyData entry) => entry.EnemyId;
}
```

### ParticleDatabase

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Particle Database")]
public class ParticleDatabase : DatabaseBase<ParticleData>
{
    protected override string GetKey(ParticleData entry) => entry.ID;
}
```

---

## GameDatabases Static Access

Global access point for all databases. Assigned during bootstrap.

```csharp
public static class GameDatabases
{
    public static WeaponDatabase WeaponDatabase { get; set; }
    public static EnemyDatabase EnemyDatabase { get; set; }
    public static AudioClipDatabase AudioClipDatabase { get; set; }
    public static WorldAudioDatabase WorldAudioDatabase { get; set; }
    public static ParticleDatabase ParticleDatabase { get; set; }
    
    public static void Clear()
    {
        WeaponDatabase = null;
        EnemyDatabase = null;
        // ... clear all references
    }
}
```

---

## Usage

### Retrieving Data

```csharp
// Safe retrieval with null check
if (GameDatabases.WeaponDatabase.TryGet("sword_fire", out var weapon))
{
    SpawnWeapon(weapon);
}

// Direct retrieval (returns null if not found)
var enemy = GameDatabases.EnemyDatabase.Get("goblin_melee");
if (enemy != null)
{
    SpawnEnemy(enemy);
}
```

### With Pooling

```csharp
// Get data from database, spawn from pool
var enemyData = GameDatabases.EnemyDatabase.Get(enemyId);
var instance = GamePoolManager.Instance.GetEnemyPrefab(enemyData, position, rotation);
```

---

## Data Flow

```
1. Designer creates ScriptableObject asset (e.g., WeaponData)
2. Asset added to database entries array
3. Database builds lookup dictionary on first access
4. Runtime systems query via GameDatabases static class
5. Retrieved data used to configure pooled instances
```

---

## Adding New Database Types

1. Create data ScriptableObject with unique ID field
2. Create database class extending DatabaseBase<T>
3. Override GetKey to return the ID field
4. Add property to GameDatabases
5. Assign in bootstrap scene

```csharp
// Step 1: Data class
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityId;
    public string displayName;
    public float cooldown;
    public GameObject effectPrefab;
}

// Step 2: Database class
[CreateAssetMenu(menuName = "Scriptable Objects/Databases/Ability Database")]
public class AbilityDatabase : DatabaseBase<AbilityData>
{
    protected override string GetKey(AbilityData entry) => entry.abilityId;
}

// Step 3: Add to GameDatabases
public static AbilityDatabase AbilityDatabase { get; set; }
```

---

## Best Practices

| Practice | Reason |
|----------|--------|
| Use TryGet over Get | Handles missing entries gracefully |
| Normalize keys (lowercase, trimmed) | Prevents lookup failures from formatting |
| Keep databases read-only at runtime | Maintains data consistency |
| Assign databases during bootstrap | Ensures availability before gameplay |
