# 🎮 Combat System TODO & Improvement Suggestions

## VR Performance Optimization Checklist

### ✅ Implemented
- [x] Object pooling for projectiles (arrows, spells)
- [x] Object pooling for enemies via GamePoolManager
- [x] Object pooling for VFX and audio
- [x] Cached Transform references to avoid GetComponent calls
- [x] Event-driven architecture to reduce Update() overhead
- [x] Priority-based update system via EnemyManager
- [x] Minimal allocations in hot paths
- [x] NavMesh path update throttling (0.2s intervals)
- [x] Non-allocating physics queries (OverlapSphereNonAlloc)
- [x] Static player reference caching for enemy AI

### 🔄 Recommended Improvements

#### High Priority (VR Performance Critical)

1. **Physics Optimization**
   - [x] Use Physics.OverlapSphereNonAlloc instead of OverlapSphere
   - [ ] Implement collision layer matrix optimization
   - [ ] Consider using trigger colliders over continuous collision for weapons
   - [ ] Add physics LOD - reduce physics checks for distant enemies

2. **Animation Optimization**
   - [ ] Implement animation LOD - simpler animations for distant enemies
   - [x] Use Animator.StringToHash for all animation parameters
   - [ ] Consider disabling Animator on very distant enemies
   - [ ] Optimize animator controllers for mobile GPU

3. **Rendering Optimization**
   - [ ] Implement GPU instancing for enemy meshes
   - [ ] Add LOD groups to enemy prefabs
   - [ ] Use occlusion culling aggressively
   - [ ] Reduce overdraw on VFX

4. **Memory Optimization**
   - [ ] Pre-warm all pools during loading screen
   - [ ] Implement soft caps on active enemies based on device performance
   - [ ] Use ArrayPool<T> for temporary arrays

---

## 🏟️ Arena System Improvements

### Current State
The arena system uses `WaveSpawner`, `WaveManager`, and `EnemyManager` for wave-based combat.

### Recommended Factory Pattern: ArenaFactory

```csharp
// ArenaFactory.cs - Proposed Implementation
public class ArenaFactory : MonoBehaviour
{
    [SerializeField] private ArenaDatabase arenaDatabase;
    
    public ArenaInstance CreateArena(string arenaId)
    {
        var data = arenaDatabase.GetArena(arenaId);
        // Instantiate arena prefab
        // Configure spawn points
        // Initialize wave system
        return instance;
    }
}
```

### ArenaData Improvements (ScriptableObject)

| Field | Type | Purpose |
|-------|------|---------|
| `arenaId` | `string` | Unique identifier |
| `displayName` | `string` | Localized name |
| `difficulty` | `ArenaDifficulty` | Easy/Medium/Hard/Nightmare |
| `waves` | `WaveData[]` | Wave configurations |
| `bossWave` | `WaveData` | Final boss wave |
| `spawnPointPrefabs` | `SpawnPointData[]` | Spawn location configs |
| `environmentHazards` | `HazardData[]` | Arena hazards (fire, spikes) |
| `rewards` | `RewardData` | Completion rewards |
| `timeLimit` | `float` | Optional time limit |
| `musicTrack` | `AudioClip` | Arena background music |

### Arena State Machine

```
[Inactive] → [Loading] → [Prelude] → [WaveActive] → [Intermission] → [BossWave] → [Victory/Defeat]
```

Implement as ScriptableObject-based state machine for designer iteration.

---

## 🔄 Pooling System Improvements

### Current GamePoolManager Analysis
- ✅ Pools enemies, weapons, particles, audio
- ✅ Singleton pattern for easy access
- ⚠️ No dynamic pool sizing
- ⚠️ No pool statistics/monitoring
- ⚠️ No async prewarming

### Recommended Improvements

#### 1. PoolConfig ScriptableObject
```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Pooling/Pool Config")]
public class PoolConfig : ScriptableObject
{
    public int initialSize = 10;
    public int maxSize = 50;
    public bool prewarmOnLoad = true;
    public bool allowGrowth = true;
    public float shrinkThreshold = 0.25f; // Shrink if < 25% used
    public float shrinkDelay = 30f; // Seconds before shrinking
}
```

#### 2. Pool Statistics (Debug/Profiling)
```csharp
public struct PoolStats
{
    public int TotalCreated;
    public int ActiveCount;
    public int PooledCount;
    public int PeakActive;
    public float AverageActiveTime;
}
```

#### 3. Async Prewarming
```csharp
public async Task PrewarmPoolsAsync(IProgress<float> progress)
{
    // Spread instantiation across frames
    // Show loading progress
    // Prevent frame drops during loading
}
```

#### 4. ProjectilePoolManager (New)
```csharp
public class ProjectilePoolManager : MonoBehaviour
{
    // Dedicated pool for projectiles
    // Separate from GamePoolManager for performance
    // Projectiles have their own lifecycle
}
```

#### 5. Pool Priority System
```csharp
public enum PoolPriority
{
    Critical,  // Always spawn (player effects)
    High,      // Spawn if < 80% capacity
    Medium,    // Spawn if < 60% capacity  
    Low        // Spawn if < 40% capacity
}
```

---

## 👹 Enemy Spawning Improvements

### Current WaveSpawner Analysis
- ✅ Coroutine-based spawning
- ✅ Max enemy limit
- ✅ Spawn point randomization
- ⚠️ No spawn point weighting
- ⚠️ No formation spawning
- ⚠️ No difficulty scaling

### Recommended Improvements

#### 1. SpawnPointData ScriptableObject
```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Spawning/Spawn Point")]
public class SpawnPointData : ScriptableObject
{
    public string pointId;
    public SpawnPointType type; // Ground, Elevated, Ambush, Boss
    public float weight = 1f; // Spawn probability weight
    public int maxSimultaneous = 3; // Max enemies from this point
    public float cooldown = 2f; // Time between spawns
    public EnemyData[] allowedEnemies; // Filter by enemy type
}
```

#### 2. SpawnFormation System
```csharp
public enum SpawnFormation
{
    Random,
    Circle,      // Surround player
    Line,        // Front assault
    Flanking,    // Left and right
    Ambush,      // Behind player
    Wave         // Staggered approach
}

[CreateAssetMenu(menuName = "Scriptable Objects/Spawning/Formation")]
public class FormationData : ScriptableObject
{
    public SpawnFormation formation;
    public Vector3[] relativePositions;
    public float spawnDelay = 0.2f;
}
```

#### 3. Difficulty Scaling
```csharp
public class DifficultyScaler : MonoBehaviour
{
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float spawnRateMultiplier = 1f;
    public int additionalEnemiesPerWave = 0;
    
    public void ApplyDifficulty(DifficultyLevel level)
    {
        // Scale based on player performance
        // Adaptive difficulty for VR comfort
    }
}
```

#### 4. SpawnCondition System
```csharp
public abstract class SpawnCondition : ScriptableObject
{
    public abstract bool CanSpawn(SpawnContext context);
}

// Examples:
// - PlayerHealthCondition (spawn healers if player low)
// - TimeCondition (spawn at specific wave time)
// - EnemyCountCondition (spawn reinforcements)
// - PlayerPositionCondition (spawn flankers)
```

---

## 🎯 Enemy Management Improvements

### Current EnemyManager Analysis
- ✅ HashSet for O(1) add/remove
- ✅ Priority update system
- ✅ Event-based spawn/despawn
- ⚠️ No enemy type tracking
- ⚠️ No spatial partitioning
- ⚠️ No threat assessment

### Recommended Improvements

#### 1. Enemy Registry Pattern
```csharp
public class EnemyRegistry : MonoBehaviour
{
    private readonly Dictionary<string, HashSet<EnemyController>> _enemiesByType = new();
    private readonly Dictionary<EnemyArchetype, HashSet<EnemyController>> _enemiesByArchetype = new();
    
    public int GetCountByType(string enemyId) => _enemiesByType[enemyId].Count;
    public IEnumerable<EnemyController> GetEnemiesOfType(string enemyId);
    public EnemyController GetNearestEnemy(Vector3 position);
    public EnemyController GetNearestEnemyOfType(Vector3 position, string enemyId);
}
```

#### 2. Spatial Partitioning (VR Critical)
```csharp
public class EnemySpatialHash : MonoBehaviour
{
    private readonly Dictionary<Vector3Int, List<EnemyController>> _cells = new();
    private const float CellSize = 5f;
    
    public List<EnemyController> GetNearbyEnemies(Vector3 position, float radius)
    {
        // O(1) spatial lookup instead of O(n) distance checks
    }
}
```

#### 3. Threat Assessment System
```csharp
public class ThreatAssessor : MonoBehaviour
{
    public EnemyController GetHighestThreat(Vector3 playerPosition)
    {
        // Consider: distance, damage potential, current target
    }
    
    public float CalculateThreatLevel(EnemyController enemy)
    {
        var distance = Vector3.Distance(enemy.transform.position, playerPosition);
        var healthPercent = enemy.Health.HealthBarValue;
        var isAttacking = enemy.Movement.CurrentState == EnemyAIState.Attacking;
        
        return (1f / distance) * healthPercent * (isAttacking ? 2f : 1f);
    }
}
```

#### 4. Enemy Archetype System
```csharp
public enum EnemyArchetype
{
    Melee,      // Close combat
    Ranged,     // Bow/Magic
    Tank,       // High health, slow
    Assassin,   // Low health, high damage
    Support,    // Buffs other enemies
    Boss        // Special behaviors
}

[CreateAssetMenu(menuName = "Scriptable Objects/Enemies/Archetype")]
public class EnemyArchetypeData : ScriptableObject
{
    public EnemyArchetype archetype;
    public float preferredRange;
    public float aggressionLevel;
    public bool canFlank;
    public bool targetsPriority; // Focuses weakened player
}
```

#### 5. Group Behavior System
```csharp
public class EnemySquad : MonoBehaviour
{
    public List<EnemyController> Members;
    public EnemyController Leader;
    
    public void CoordinateAttack()
    {
        // Prevent all enemies attacking at once
        // Stagger attacks for fair gameplay
        // Assign flanking positions
    }
}
```

---

## 📊 Data-Driven Architecture Summary

### Current Pattern (Maintain Consistency)
```
ScriptableObject (Data) → MonoBehaviour (Controller) → Components (Behavior)
```

### Recommended New ScriptableObjects

| ScriptableObject | Purpose |
|------------------|---------|
| `PoolConfig` | Pool sizing and behavior |
| `SpawnPointData` | Spawn location configuration |
| `FormationData` | Enemy spawn formations |
| `EnemyArchetypeData` | AI behavior patterns |
| `DifficultyData` | Difficulty scaling settings |
| `ArenaRewardData` | Completion rewards |
| `StatusEffectData` | Burn, freeze, shock effects |

### Recommended Factories

| Factory | Creates |
|---------|---------|
| `ArenaFactory` | Arena instances from ArenaData |
| `EnemyFactory` | Enemies with archetype behaviors |
| `WeaponFactory` | Weapons with modifiers applied |
| `ProjectileFactory` | Spell/arrow projectiles |
| `StatusEffectFactory` | Status effect instances |

### Event Channels to Add

| Event | Purpose |
|-------|---------|
| `OnWaveStarted` | Wave begins |
| `OnWaveCompleted` | All enemies in wave defeated |
| `OnArenaCompleted` | All waves completed |
| `OnEnemyHit` | Enemy takes damage (for UI/audio) |
| `OnPlayerHit` | Player takes damage |
| `OnComboAchieved` | Player combo milestone |
| `OnStatusEffectApplied` | Burn/freeze started |

---

## 🎮 Quest 2 Specific Considerations

### Performance Targets
- 72 FPS minimum (90 FPS preferred with ASW)
- Max 6-8 active enemies at once
- Max 10-15 active projectiles
- Max 20-30 active VFX
- Update budget: 3-4ms for gameplay logic

### Memory Budget
- Keep total scene memory under 1GB
- Pool sizes should be tuned based on testing
- Consider async loading for weapon variants

### VR Comfort
- Avoid rapid enemy spawns behind player
- Audio cues before attacks from behind
- Consistent enemy movement speeds
- No teleporting enemies (disorienting)

---

## Example Weapon Configurations

### Fire Sword (Rare)
- Base: Sword, 15 damage
- Modifier: Flaming (+5 fire damage, orange trail)
- VFX: Fire particles on hit
- Status: Apply burning (3 DPS for 3s)

### Frost Bow (Epic)
- Base: Bow, 20 damage
- Modifier: Frozen (+8 frost damage, blue trail)
- Effect: Slow enemies by 30% for 2s
- Status: Chance to freeze solid

### Lightning Staff (Legendary)
- Base: Staff, 25 damage
- Modifier: Shocking (+12 lightning, chain effect)
- Special: Chain to 2 nearby enemies at 50% damage
- Status: Stun for 0.5s

### Throwing Knife Set (Uncommon)
- Base: ThrowingKnife, 12 damage
- Feature: Boomerang recall
- Skill: Return path damages enemies at 50%
- Upgrade: Increase return damage to 75%

### Poison Dagger (Rare)
- Base: Dagger, 8 damage (fast attack)
- Modifier: Venomous (+3 poison damage)
- Status: Poison (2 DPS for 5s, stacks 3x)
- Special: Backstab bonus (+50% from behind)

