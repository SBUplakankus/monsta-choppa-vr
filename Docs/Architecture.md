# Project Architecture

This document describes the high-level architecture of the Monsta Choppa VR project. The system is built on Unity's XR Interaction Toolkit with custom layers for gameplay, data management, and UI.

---

## Core Design Principles

| Principle | Implementation |
|-----------|----------------|
| Decoupled Systems | Event channels for communication, no direct references |
| Data-Driven Design | ScriptableObjects define all game content |
| VR Performance | Object pooling, priority updates, minimal allocations |
| Extensibility | Generic base classes, factory patterns |

---

## System Layers

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│         UI Hosts, Views, Audio, Visual Effects          │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                    Gameplay Layer                        │
│      Weapons, Enemies, Combat, Arena Management          │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                    Event Layer                           │
│         Event Channels, GameEvents Registry              │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                    Data Layer                            │
│     ScriptableObject Databases, Pooling, Constants       │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                    VR Template Layer                     │
│       XR Origin, Controllers, Locomotion, Input          │
└─────────────────────────────────────────────────────────┘
```

---

## Data-Driven ScriptableObject Architecture

The project uses a consistent pattern where ScriptableObjects define configuration and MonoBehaviours handle runtime logic.

### Pattern Overview

```
ScriptableObject (Data)  →  MonoBehaviour (Controller)  →  Components (Behavior)
       ↓                            ↓                            ↓
   WeaponData              XRWeaponBase               Rigidbody, Collider
   EnemyData               EnemyController            EnemyHealth, EnemyMovement
   ParticleData            ParticleController         ParticleSystem
```

### Why This Pattern

| Benefit | Explanation |
|---------|-------------|
| Designer-friendly | Configure in Inspector without code changes |
| Runtime immutable | Data assets are read-only, no accidental modifications |
| Easy balancing | Tweak stats by editing assets, not code |
| Pooling compatible | Same data drives multiple pooled instances |
| Serialization ready | ScriptableObjects serialize naturally |

### Data Class Structure

All data ScriptableObjects follow this structure:

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Category/TypeData")]
public class TypeData : ScriptableObject
{
    [Header("Identity")]
    public string id;           // Unique lookup key
    public string displayName;  // Localized display name
    
    [Header("Configuration")]
    // Type-specific fields...
    
    [Header("References")]
    public GameObject prefab;   // Associated prefab
    
    // Calculated properties
    public int CalculatedValue => baseValue + modifier;
}
```

### Database Pattern

All databases inherit from a generic base:

```csharp
public abstract class DatabaseBase<T> : ScriptableObject where T : ScriptableObject
{
    [SerializeField] private T[] entries;
    private Dictionary<string, T> _lookup;
    
    public bool TryGet(string id, out T entry)
    {
        BuildLookup();
        return _lookup.TryGetValue(id.ToLower().Trim(), out entry);
    }
    
    protected abstract string GetKey(T entry);
    
    private void BuildLookup()
    {
        if (_lookup != null) return;
        _lookup = new Dictionary<string, T>();
        foreach (var entry in entries)
            _lookup[GetKey(entry).ToLower().Trim()] = entry;
    }
}
```

### Static Access Layer

Global access to databases and events through static classes:

```csharp
// GameDatabases - centralized database access
GameDatabases.WeaponDatabase.TryGet("sword_fire", out var weapon);
GameDatabases.EnemyDatabase.TryGet("goblin_melee", out var enemy);

// GameEvents - centralized event access
GameEvents.OnPlayerDamaged.Raise(damage);
GameEvents.OnEnemySpawned.Subscribe(HandleEnemySpawn);
```

---

## Key System Relationships

### Weapon System Flow

```
WeaponData (ScriptableObject)
    │
    ├── WeaponDatabase (lookup)
    │
    └── XRWeaponBase (MonoBehaviour)
            │
            ├── MeleeXRWeapon / BowXRWeapon / StaffXRWeapon
            │
            ├── WeaponHitbox (damage detection)
            │
            └── WeaponModifierData[] (stackable modifiers)
```

### Enemy System Flow

```
EnemyData (ScriptableObject)
    │
    ├── EnemyDatabase (lookup)
    │
    └── EnemyController (MonoBehaviour coordinator)
            │
            ├── EnemyHealth (IDamageable, health bar)
            ├── EnemyMovement (NavMesh navigation)
            ├── EnemyAnimator (state machine)
            └── EnemyAttack (WeaponData reference)
```

### Pooling Flow

```
GamePoolManager (Singleton)
    │
    ├── Enemy Pools (keyed by EnemyData)
    ├── Weapon Pools (keyed by WeaponData)
    ├── Particle Pools (keyed by ParticleData)
    └── Audio Pools (keyed by WorldAudioData)
    
Spawn: GamePoolManager.Instance.GetEnemyPrefab(enemyData, position, rotation)
Return: GamePoolManager.Instance.ReturnEnemyPrefab(enemyData, gameObject)
```

---

## Initialization

The GameBootstrap MonoBehaviour initializes all static references on startup:

1. **Event Channels** - Assigned to GameEvents static fields
2. **Databases** - Assigned to GameDatabases static fields
3. **Pool Manager** - Pre-warms object pools
4. **Save System** - Loads player data via ESave

All systems are ready before any gameplay scene loads.

---

## Best Practices

| Rule | Reason |
|------|--------|
| Always unsubscribe in OnDisable | Prevents memory leaks and null references |
| Never modify ScriptableObjects at runtime | Breaks data consistency |
| Use TryGet for database lookups | Handles missing entries gracefully |
| Pool all instantiated objects | Avoids GC spikes in VR |
| Keep Update logic minimal | VR requires consistent 72+ FPS |

---

## File Organization

```
Assets/Scripts/
├── Data/                   # ScriptableObject data classes
│   ├── Core/              # EnemyData, ParticleData
│   └── Weapons/           # WeaponData, WeaponModifierData
├── Databases/             # DatabaseBase<T>, GameDatabases
├── Events/                # Event channels, GameEvents
├── Characters/            # Enemy components
├── Weapons/               # Weapon components
├── Pooling/               # GamePoolManager
├── Saves/                 # ESave integration
├── UI/                    # Views, Hosts, Controllers
└── Factories/             # UIToolkitFactory
```
