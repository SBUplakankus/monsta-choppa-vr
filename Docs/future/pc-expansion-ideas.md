# PC Expansion Ideas

Now that Monsta Choppa is no longer constrained by VR hardware limitations, this document explores exciting possibilities for expanding and enhancing the game on PC.

---

## Table of Contents

1. [Performance Unlocked](#performance-unlocked)
2. [Combat System Expansions](#combat-system-expansions)
3. [Enemy System Expansions](#enemy-system-expansions)
4. [Progression System Expansions](#progression-system-expansions)
5. [New Game Modes](#new-game-modes)
6. [Visual & Audio Enhancements](#visual--audio-enhancements)
7. [Quality of Life Features](#quality-of-life-features)
8. [Multiplayer Considerations](#multiplayer-considerations)
9. [Modding Support](#modding-support)
10. [Implementation Priority Matrix](#implementation-priority-matrix)

---

## Performance Unlocked

### What VR Constrained

| Constraint | VR Budget | PC Budget | Multiplier |
|:-----------|:----------|:----------|:-----------|
| Frame Rate | 72-120 FPS | 60 FPS | Easier target |
| Frame Time | 8.3-13.9ms | 16.7ms | **2x headroom** |
| Draw Calls | 100-150 | 2000+ | **15-20x increase** |
| Triangles | 300-500k | 5M+ | **10x increase** |
| Active Enemies | 6-8 | 50+ | **6-8x increase** |
| Real-time Lights | 1-2 | 8+ | **4x increase** |
| Shadows | Blob/None | Full cascade | **Full quality** |
| Post-Processing | Minimal | Full stack | **All effects** |
| Physics Objects | 20-30 | 200+ | **10x increase** |
| Particle Systems | 15-20 | 100+ | **5x increase** |

### What This Enables

The relaxed performance constraints open up entirely new gameplay possibilities that weren't feasible in VR.

---

## Combat System Expansions

### 1. Combo System

**Concept:** Chain attacks together for increased damage and style points.

**Why it wasn't in VR:** VR combat is physics-based and velocity-tracked. Combos didn't make sense when the player physically swings weapons.

**PC Implementation:**

```
Light → Light → Light = Basic Combo (1.0x → 1.1x → 1.3x damage)
Light → Light → Heavy = Launcher Combo (ends with enemy juggle)
Heavy → Light → Light = Crusher Combo (high stagger)
Block → Light = Parry Counter (2x damage window)
Dodge → Heavy = Backstab (3x damage from behind)
```

**File to extend:** Create `Assets/Scripts/Player/ComboSystem.cs`

```csharp
public class ComboSystem : MonoBehaviour
{
    [SerializeField] private ComboData[] combos;
    [SerializeField] private float comboWindowDuration = 0.5f;
    
    private List<AttackType> _currentSequence = new();
    private float _comboTimer;
    
    public event Action<ComboData> OnComboExecuted;
    
    public void RegisterAttack(AttackType type)
    {
        if (Time.time - _comboTimer > comboWindowDuration)
            _currentSequence.Clear();
        
        _currentSequence.Add(type);
        _comboTimer = Time.time;
        
        CheckForCombo();
    }
    
    private void CheckForCombo()
    {
        foreach (var combo in combos)
        {
            if (MatchesSequence(combo.Sequence))
            {
                OnComboExecuted?.Invoke(combo);
                _currentSequence.Clear();
                break;
            }
        }
    }
}
```

**ScriptableObject:** `Assets/Scripts/Data/Combat/ComboData.cs`

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Data/Combat/Combo")]
public class ComboData : ScriptableObject
{
    public string comboName;
    public AttackType[] sequence;
    public float damageMultiplier = 1.5f;
    public AnimationClip finisherAnimation;
    public ParticleData finisherVFX;
    public WorldAudioData finisherSFX;
}
```

### 2. Skill System

**Concept:** Unlockable active abilities with cooldowns.

**Why it wasn't in VR:** Limited input options on VR controllers. Each button already mapped to essential functions.

**PC Implementation:**

| Skill Type | Example Skills |
|:-----------|:---------------|
| **Offensive** | Whirlwind Slash, Fire Wave, Lightning Strike |
| **Defensive** | Shield Barrier, Dodge Roll, Time Slow |
| **Utility** | Grapple Hook, Blink Dash, Health Drain |
| **Ultimate** | Berserker Rage, Meteor Storm, Summon Allies |

**Input Mapping:**
- Q, E, R, F = Skill slots 1-4
- Gamepad: Bumpers + Face buttons

**File to create:** `Assets/Scripts/Player/SkillSystem.cs`

**File to create:** `Assets/Scripts/Data/Skills/SkillData.cs`

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Data/Skills/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Identity")]
    public string skillId;
    public string displayNameKey;
    public string descriptionKey;
    public Sprite icon;
    
    [Header("Stats")]
    public float cooldown = 10f;
    public float manaCost = 25f;
    public float damage = 50f;
    public float range = 5f;
    public float duration = 0f;  // For buffs
    
    [Header("Execution")]
    public SkillTargetType targetType;
    public AnimationClip castAnimation;
    public ParticleData castVFX;
    public WorldAudioData castSFX;
    
    [Header("Unlock")]
    public int levelRequired = 1;
    public int goldCost = 100;
    public SkillData[] prerequisites;
}

public enum SkillTargetType
{
    Self,
    SingleTarget,
    AoEGround,
    AoESelf,
    Projectile,
    Cone
}
```

### 3. Weapon Mastery System

**Concept:** Gain experience with specific weapon types to unlock bonuses.

**Extends existing:** [`Assets/Scripts/Data/Weapons/WeaponData.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Data/Weapons/WeaponData.cs)

**New fields:**

```csharp
public enum WeaponMasteryTier { Novice, Apprentice, Journeyman, Expert, Master }

[Header("Mastery")]
[SerializeField] private int[] masteryThresholds = { 0, 100, 500, 2000, 10000 };
[SerializeField] private float[] masteryBonuses = { 1.0f, 1.1f, 1.25f, 1.4f, 1.6f };
[SerializeField] private SkillData[] masteryUnlocks;  // Skills unlocked at each tier
```

**Mastery Perks by Weapon Type:**

| Weapon | Mastery 1 | Mastery 2 | Mastery 3 | Mastery 4 | Mastery 5 |
|:-------|:----------|:----------|:----------|:----------|:----------|
| Sword | +10% speed | Parry window +50% | Combo extender | Critical hits | Blade Dance skill |
| Axe | +15% damage | Cleave 2 enemies | Armor break | Execute <20% | Whirlwind skill |
| Bow | +10% range | Faster draw | Multishot | Piercing | Rain of Arrows skill |
| Staff | +10% mana regen | Faster cast | Spell echo | AoE increase | Meteor skill |
| Shield | +20% block angle | Block staggers | Shield bash | Reflect projectiles | Phalanx skill |

### 4. Dual Wielding

**Concept:** Equip two one-handed weapons for faster attacks.

**Why it wasn't in VR:** Already possible naturally - players have two hands.

**PC Implementation:**

```csharp
public class DualWieldController : MonoBehaviour
{
    [SerializeField] private PCWeaponBase mainHand;
    [SerializeField] private PCWeaponBase offHand;
    
    private bool _alternateSwing;
    
    public void Attack()
    {
        var weapon = _alternateSwing ? offHand : mainHand;
        weapon.PrimaryAttack();
        _alternateSwing = !_alternateSwing;
    }
}
```

**Dual Wield bonuses:**
- +30% attack speed
- -20% damage per hit
- Unique dual-wield combos
- Cannot block (no shield)

### 5. Finisher System

**Concept:** Execute weakened enemies with cinematic finishers.

**Why it wasn't in VR:** VR players expect full agency. Taking camera control causes disorientation.

**PC Implementation:**

```csharp
public class FinisherSystem : MonoBehaviour
{
    [SerializeField] private float finisherHealthThreshold = 0.2f;  // 20% health
    [SerializeField] private float finisherRange = 2f;
    [SerializeField] private KeyCode finisherKey = KeyCode.E;
    
    private void Update()
    {
        if (Input.GetKeyDown(finisherKey))
        {
            var target = FindFinisherTarget();
            if (target != null)
                ExecuteFinisher(target);
        }
    }
    
    private void ExecuteFinisher(EnemyController enemy)
    {
        // Start cinematic
        CinematicController.Instance.PlayFinisher(enemy.Data.FinisherAnimation);
        
        // Apply damage
        enemy.TakeDamage(enemy.MaxHealth);
        
        // XP/Gold bonus
        GameplayEvents.FinisherExecuted.Raise(enemy);
    }
}
```

**Finisher rewards:**
- +50% XP
- +25% Gold
- Health restore on kill
- Style meter boost

---

## Enemy System Expansions

### 1. Massive Enemy Counts

**Current:** 6-8 enemies max (VR performance)
**Target:** 30-50 enemies simultaneously

**Extends existing:** [`Assets/Scripts/Systems/Arena/WaveSpawner.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Systems/Arena/WaveSpawner.cs)

**New wave types:**

| Wave Type | Enemy Count | Notes |
|:----------|:------------|:------|
| Standard | 8-12 | Normal difficulty |
| Swarm | 25-40 | Many weak enemies |
| Elite | 3-5 | Fewer but stronger |
| Boss Rush | 2-3 bosses | Multiple bosses |
| Siege | 50+ | Tower defense style |

**Performance optimization for many enemies:**

```csharp
// In GameUpdateManager - tiered update rates based on distance
public void RegisterEnemy(EnemyController enemy, float distanceToPlayer)
{
    UpdatePriority priority = distanceToPlayer switch
    {
        < 10f => UpdatePriority.High,    // Every frame
        < 25f => UpdatePriority.Medium,  // Every 0.1s
        _ => UpdatePriority.Low          // Every 0.2s
    };
    
    Register(enemy, priority);
}
```

### 2. Enemy AI Enhancements

**Current:** Basic chase and attack (NavMesh pathfinding)
**Target:** Tactical group behavior, flanking, retreating

**Extends existing:** [`Assets/Scripts/Characters/Enemies/EnemyMovement.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Characters/Enemies/EnemyMovement.cs)

**New AI States:**

```csharp
public enum EnemyAIState 
{ 
    Idle, 
    Patrol,           // NEW: Wander when no target
    Chasing, 
    Attacking, 
    Flanking,         // NEW: Circle around player
    Retreating,       // NEW: Back off when hurt
    Regrouping,       // NEW: Join allies
    SupportingAlly,   // NEW: Heal/buff nearby ally
    Dead 
}
```

**Group Tactics System:**

```csharp
public class EnemySquadController : MonoBehaviour
{
    private List<EnemyController> _squadMembers = new();
    
    public void AssignFormation(FormationType formation)
    {
        switch (formation)
        {
            case FormationType.Surround:
                PositionMembersAroundTarget();
                break;
            case FormationType.Line:
                PositionMembersInLine();
                break;
            case FormationType.Protect:
                PositionMembersAroundLeader();
                break;
        }
    }
    
    public void CoordinateAttack()
    {
        // Only 2-3 attack at once, others wait
        var attackers = _squadMembers.Take(3);
        foreach (var enemy in attackers)
            enemy.BeginAttack();
    }
}
```

### 3. Enemy Variety

**Current:** Basic enemy types with shared animations
**Target:** Diverse enemy roster with unique behaviors

**Extends existing:** [`Assets/Scripts/Data/Core/EnemyData.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Data/Core/EnemyData.cs)

**New Enemy Categories:**

| Category | Examples | Special Mechanics |
|:---------|:---------|:------------------|
| **Swarmers** | Goblins, Rats, Bats | Low HP, attack in groups |
| **Brutes** | Ogres, Trolls, Golems | High HP, slow, AoE attacks |
| **Rangers** | Archers, Mages, Spear Throwers | Keep distance, projectiles |
| **Assassins** | Rogues, Shadows, Stalkers | Invisibility, backstab |
| **Supports** | Shamans, Healers, Buffers | Heal/buff other enemies |
| **Summoners** | Necromancers, Beastmasters | Spawn additional enemies |
| **Mini-Bosses** | Champions, Veterans | Enhanced versions with abilities |
| **Bosses** | Dragon, Lich King, Giant | Multi-phase, unique mechanics |

**New EnemyData fields:**

```csharp
[Header("AI Behavior")]
[SerializeField] private EnemyBehaviorType behaviorType;
[SerializeField] private float preferredCombatRange;
[SerializeField] private bool canFlee;
[SerializeField] private float fleeHealthThreshold;

[Header("Special Abilities")]
[SerializeField] private SkillData[] abilities;
[SerializeField] private float[] abilityCooldowns;
[SerializeField] private float abilityUseChance;

public enum EnemyBehaviorType
{
    Melee,
    Ranged,
    Support,
    Summoner,
    Assassin,
    Tank
}
```

### 4. Boss Enhancements

**Current:** Boss phase exists but minimal implementation
**Target:** Multi-phase bosses with unique mechanics

**Boss Phase System:**

```csharp
public class BossController : EnemyController
{
    [SerializeField] private BossPhaseData[] phases;
    
    private int _currentPhase;
    
    protected override void OnDamageTaken()
    {
        base.OnDamageTaken();
        CheckPhaseTransition();
    }
    
    private void CheckPhaseTransition()
    {
        var nextPhaseThreshold = phases[_currentPhase + 1].healthThreshold;
        
        if (HealthPercentage <= nextPhaseThreshold)
        {
            _currentPhase++;
            ExecutePhaseTransition(phases[_currentPhase]);
        }
    }
    
    private void ExecutePhaseTransition(BossPhaseData phase)
    {
        // Cinematic
        CinematicController.Instance.PlayBossPhase(phase.TransitionCinematic);
        
        // Apply new abilities
        EnableAbilities(phase.Abilities);
        
        // Visual change
        ApplyVisualChanges(phase.MaterialOverrides);
        
        // Summon adds if applicable
        if (phase.SummonWave != null)
            SpawnAdds(phase.SummonWave);
    }
}
```

**Example Boss: The Goblin King**

| Phase | Health | Mechanics |
|:------|:-------|:----------|
| Phase 1 | 100-70% | Basic attacks, occasional charge |
| Phase 2 | 70-40% | Summons goblin guards, ground slam |
| Phase 3 | 40-10% | Enraged (+damage, +speed), throws throne |
| Phase 4 | 10-0% | Desperation mode, constant attacks |

### 5. Dynamic Spawning

**Concept:** Enemies spawn based on player performance, not fixed waves.

**Director System:**

```csharp
public class CombatDirector : MonoBehaviour
{
    [SerializeField] private float intensityBuildRate = 0.1f;
    [SerializeField] private float intensityDecayRate = 0.2f;
    [SerializeField] private AnimationCurve spawnCurve;
    
    private float _currentIntensity;
    private float _targetIntensity;
    
    private void Update()
    {
        AdjustIntensityBasedOnPerformance();
        SpawnEnemiesBasedOnIntensity();
    }
    
    private void AdjustIntensityBasedOnPerformance()
    {
        var playerHealth = PlayerHealth.Percentage;
        var killsPerMinute = _recentKills / _timeSinceStart * 60f;
        
        // Player doing well -> increase intensity
        if (playerHealth > 0.7f && killsPerMinute > 10)
            _targetIntensity += intensityBuildRate * Time.deltaTime;
        
        // Player struggling -> decrease intensity
        else if (playerHealth < 0.3f)
            _targetIntensity -= intensityDecayRate * Time.deltaTime;
        
        _currentIntensity = Mathf.Lerp(_currentIntensity, _targetIntensity, Time.deltaTime);
    }
}
```

---

## Progression System Expansions

### 1. Full Skill Tree

**Concept:** Branching skill trees for different playstyles.

**Extends existing:** [`Assets/Scripts/Data/Progression/UpgradeData.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Data/Progression/UpgradeData.cs)

**Skill Trees:**

```
Warrior Tree          Mage Tree           Rogue Tree
     │                    │                   │
  ┌──┴──┐             ┌──┴──┐            ┌──┴──┐
  │     │             │     │            │     │
Offense Defense    Destruction Utility  Agility Precision
  │     │             │     │            │     │
  ▼     ▼             ▼     ▼            ▼     ▼
 ...   ...           ...   ...          ...   ...
```

**Skill Tree Data:**

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Data/Progression/Skill Tree")]
public class SkillTreeData : ScriptableObject
{
    public string treeId;
    public string treeNameKey;
    public Sprite treeIcon;
    public SkillNodeData rootNode;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Data/Progression/Skill Node")]
public class SkillNodeData : ScriptableObject
{
    public string nodeId;
    public string nameKey;
    public string descriptionKey;
    public Sprite icon;
    public int pointCost = 1;
    public int levelRequired;
    public SkillNodeData[] prerequisites;
    public SkillNodeData[] children;
    
    [Header("Effect")]
    public SkillNodeEffect effectType;
    public float effectValue;
    public SkillData unlockedSkill;  // If this node unlocks an active skill
}

public enum SkillNodeEffect
{
    DamageIncrease,
    HealthIncrease,
    SpeedIncrease,
    CooldownReduction,
    CriticalChance,
    LifeSteal,
    UnlockSkill
}
```

### 2. Equipment System

**Concept:** Equippable armor and accessories with stats.

**New ScriptableObject:** `Assets/Scripts/Data/Equipment/EquipmentData.cs`

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Data/Equipment/Equipment")]
public class EquipmentData : ScriptableObject
{
    [Header("Identity")]
    public string equipmentId;
    public string nameKey;
    public string descriptionKey;
    public Sprite icon;
    public EquipmentSlot slot;
    public EquipmentRarity rarity;
    
    [Header("Stats")]
    public int armor;
    public int healthBonus;
    public float damageBonus;
    public float speedBonus;
    public float criticalChance;
    
    [Header("Special")]
    public EquipmentSetData setBonus;
    public SkillData grantedAbility;
    
    [Header("Visual")]
    public GameObject prefab;  // For visible equipment
    public Material[] materialOverrides;
}

public enum EquipmentSlot { Helm, Chest, Gloves, Boots, Amulet, Ring1, Ring2 }
public enum EquipmentRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }
```

**Equipment Set Bonuses:**

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Data/Equipment/Set")]
public class EquipmentSetData : ScriptableObject
{
    public string setName;
    public EquipmentData[] setPieces;
    
    [System.Serializable]
    public class SetBonus
    {
        public int piecesRequired;
        public StatModifier[] bonuses;
    }
    
    public SetBonus[] bonuses;  // e.g., 2-piece bonus, 4-piece bonus
}
```

### 3. Loot System

**Concept:** Random loot drops with affixes.

**New component:** `Assets/Scripts/Systems/Loot/LootGenerator.cs`

```csharp
public class LootGenerator : MonoBehaviour
{
    [SerializeField] private LootTableData[] lootTables;
    
    public List<ItemDrop> GenerateLoot(EnemyData enemy, float luckMultiplier)
    {
        var drops = new List<ItemDrop>();
        var table = GetLootTable(enemy.EnemyType);
        
        foreach (var entry in table.Entries)
        {
            var roll = Random.value * luckMultiplier;
            if (roll <= entry.dropChance)
            {
                var item = GenerateItem(entry.itemPool);
                drops.Add(item);
            }
        }
        
        return drops;
    }
    
    private ItemDrop GenerateItem(ItemPool pool)
    {
        var baseItem = pool.GetRandomItem();
        var rarity = RollRarity();
        var affixes = RollAffixes(rarity);
        
        return new ItemDrop
        {
            BaseItem = baseItem,
            Rarity = rarity,
            Affixes = affixes
        };
    }
}
```

### 4. New Game Plus

**Concept:** Restart with bonuses, increased difficulty.

```csharp
public class NewGamePlusManager : MonoBehaviour
{
    public int NGPlusLevel { get; private set; }
    
    public float EnemyHealthMultiplier => 1f + (NGPlusLevel * 0.5f);
    public float EnemyDamageMultiplier => 1f + (NGPlusLevel * 0.3f);
    public float ExperienceMultiplier => 1f + (NGPlusLevel * 0.25f);
    public float LootRarityBonus => NGPlusLevel * 0.1f;
    
    public void StartNewGamePlus()
    {
        NGPlusLevel++;
        
        // Keep: Level, skills, equipment
        // Reset: Story progress, arenas completed
        // Increase: Difficulty, rewards
    }
}
```

### 5. Daily/Weekly Challenges

**Concept:** Rotating challenges with bonus rewards.

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Data/Challenges/Daily Challenge")]
public class DailyChallengeData : ScriptableObject
{
    public string challengeId;
    public string descriptionKey;
    public ChallengeType type;
    public int targetAmount;
    public RewardData[] rewards;
    
    public enum ChallengeType
    {
        KillEnemies,
        KillEnemyType,
        CompleteArena,
        EarnGold,
        UseSkill,
        ComboChain,
        NoDamageArena,
        SpeedrunArena
    }
}
```

---

## New Game Modes

### 1. Endless Mode

**Concept:** Infinite waves with increasing difficulty.

**Leaderboard integration:**

```csharp
public class EndlessModeManager : MonoBehaviour
{
    private int _currentWave;
    private int _totalKills;
    private float _survivalTime;
    
    public float DifficultyMultiplier => 1f + (_currentWave * 0.1f);
    
    private void OnWaveComplete()
    {
        _currentWave++;
        
        // Every 10 waves, spawn a boss
        if (_currentWave % 10 == 0)
            SpawnBoss();
        
        // Scale enemy stats
        ApplyDifficultyScaling();
        
        // Spawn next wave
        SpawnWave();
    }
    
    private void OnPlayerDeath()
    {
        SubmitScore(new EndlessScore
        {
            Wave = _currentWave,
            Kills = _totalKills,
            Time = _survivalTime
        });
    }
}
```

### 2. Challenge Arenas

**Concept:** Pre-designed encounters with specific rules.

**Challenge Types:**

| Challenge | Rules |
|:----------|:------|
| **Time Attack** | Complete in X seconds |
| **No Damage** | Take 0 damage |
| **Weapon Restriction** | Only use specific weapon |
| **One-Shot** | Everything dies in one hit (including player) |
| **Boss Rush** | All bosses, no breaks |
| **Swarm** | 100 enemies, endless respawn |
| **Handicap** | No skills, no blocking |

### 3. Roguelike Mode

**Concept:** Procedural runs with permanent death.

**Run Structure:**

```
Start → Room 1 → Room 2 → Shop → Room 3 → Elite → Room 4 → Shop → Boss
                                                                    ↓
                                                              Next Floor
```

**Roguelike Systems:**

```csharp
public class RoguelikeRunManager : MonoBehaviour
{
    [SerializeField] private RoomData[] normalRooms;
    [SerializeField] private RoomData[] eliteRooms;
    [SerializeField] private RoomData[] bossRooms;
    [SerializeField] private RoomData shopRoom;
    
    private int _currentFloor;
    private int _roomsCleared;
    private List<BuffData> _runBuffs = new();
    
    public void GenerateFloor()
    {
        var rooms = new List<RoomData>();
        
        // 4 normal rooms
        for (int i = 0; i < 4; i++)
            rooms.Add(GetRandomRoom(normalRooms));
        
        // 1 elite room
        rooms.Add(GetRandomRoom(eliteRooms));
        
        // Shop after elite
        rooms.Add(shopRoom);
        
        // Boss at end
        rooms.Add(GetFloorBoss());
        
        LoadFirstRoom(rooms);
    }
    
    public void AddRunBuff(BuffData buff)
    {
        _runBuffs.Add(buff);
        ApplyBuff(buff);
    }
}
```

### 4. Arena Builder (Advanced)

**Concept:** Let players create and share custom arenas.

**Editor Tools:**

```csharp
public class ArenaEditor : MonoBehaviour
{
    [SerializeField] private PlaceableObjectData[] availableObjects;
    
    private List<PlacedObject> _placedObjects = new();
    
    public void PlaceObject(PlaceableObjectData data, Vector3 position, Quaternion rotation)
    {
        var obj = Instantiate(data.Prefab, position, rotation);
        _placedObjects.Add(new PlacedObject { Data = data, Instance = obj });
    }
    
    public ArenaData ExportArena()
    {
        return new ArenaData
        {
            Objects = _placedObjects.Select(o => new SerializedPlacement
            {
                DataId = o.Data.Id,
                Position = o.Instance.transform.position,
                Rotation = o.Instance.transform.rotation.eulerAngles
            }).ToList()
        };
    }
}
```

---

## Visual & Audio Enhancements

### 1. Post-Processing Effects

**Effects to enable:**

| Effect | VR Status | PC Implementation |
|:-------|:----------|:------------------|
| Bloom | Limited | Full quality |
| Motion Blur | Never | Per-preference |
| Depth of Field | Never | Aim/finisher focus |
| Ambient Occlusion | Never | SSAO or HBAO |
| Color Grading | Basic | Full LUT |
| Chromatic Aberration | Never | Damage effect |
| Film Grain | Never | Optional |
| Lens Distortion | Never | Hit effect |
| Vignette | VR comfort only | Damage/low health |

### 2. Enhanced Particle Effects

**PC-quality effects:**

```csharp
public class PCParticleSettings : MonoBehaviour
{
    public void ApplyQualitySetting(QualityLevel level)
    {
        var settings = GetSettingsForLevel(level);
        
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.maxParticles = settings.MaxParticles;
            
            var emission = ps.emission;
            emission.rateOverTimeMultiplier = settings.EmissionMultiplier;
        }
    }
}
```

### 3. Dynamic Music System

**Concept:** Music adapts to combat intensity.

```csharp
public class DynamicMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip explorationTrack;
    [SerializeField] private AudioClip combatLayerLight;
    [SerializeField] private AudioClip combatLayerHeavy;
    [SerializeField] private AudioClip bossTrack;
    
    private float _combatIntensity;
    
    private void Update()
    {
        _combatIntensity = CalculateCombatIntensity();
        
        // Crossfade between layers based on intensity
        _explorationSource.volume = Mathf.Lerp(1, 0, _combatIntensity);
        _combatLightSource.volume = Mathf.Lerp(0, 1, _combatIntensity);
        _combatHeavySource.volume = Mathf.Lerp(0, 1, (_combatIntensity - 0.5f) * 2);
    }
}
```

### 4. Ragdoll Physics

**Why it wasn't in VR:** Physics simulation expensive, potential performance issues.

```csharp
public class EnemyRagdollController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody[] ragdollBodies;
    
    public void EnableRagdoll(Vector3 force)
    {
        animator.enabled = false;
        
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
        }
        
        // Return to pool after delay
        StartCoroutine(ReturnToPoolAfterDelay(5f));
    }
}
```

---

## Quality of Life Features

### 1. Keybinding System

```csharp
public class KeybindingManager : MonoBehaviour
{
    private Dictionary<string, KeyCode> _bindings = new();
    
    public void SetBinding(string action, KeyCode key)
    {
        _bindings[action] = key;
        SaveBindings();
    }
    
    public KeyCode GetBinding(string action)
    {
        return _bindings.TryGetValue(action, out var key) ? key : GetDefaultBinding(action);
    }
}
```

### 2. Accessibility Options

| Option | Type | Purpose |
|:-------|:-----|:--------|
| Color Blind Mode | Dropdown | Protanopia, Deuteranopia, Tritanopia |
| High Contrast UI | Toggle | Improved visibility |
| Screen Shake | Slider | Reduce or disable |
| Flash Effects | Toggle | Epilepsy safety |
| Subtitle Size | Slider | Readability |
| Enemy Outlines | Toggle | Target visibility |

### 3. Photo Mode

**Why it wasn't in VR:** No stationary camera position.

```csharp
public class PhotoMode : MonoBehaviour
{
    private bool _isActive;
    
    public void Enter()
    {
        _isActive = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _freeCam.enabled = true;
    }
    
    public void TakePhoto()
    {
        StartCoroutine(CaptureScreenshot());
    }
    
    public void ApplyFilter(PhotoFilter filter)
    {
        _postProcessVolume.profile = filter.Profile;
    }
}
```

### 4. Training Mode

**Concept:** Practice combat without consequences.

- Infinite health option
- Spawn specific enemies
- Freeze enemies
- Show hitboxes
- Frame advance
- Damage numbers always visible

---

## Multiplayer Considerations

> **Note:** Multiplayer is a significant undertaking. Only consider if scope/timeline allows.

### 1. Co-op Mode (2-4 players)

**Architecture considerations:**

- Use existing event system for synchronization
- Pool manager needs network-aware spawning
- Enemy aggro needs multi-target support
- Existing [`GameplayEvents.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Events/Registries/GameplayEvents.cs) can be extended for network events

**Difficulty scaling:**

```csharp
public float GetCoopDifficultyMultiplier(int playerCount)
{
    return playerCount switch
    {
        1 => 1.0f,
        2 => 1.6f,
        3 => 2.2f,
        4 => 3.0f,
        _ => 1.0f
    };
}
```

### 2. PvP Arena

**1v1 or 2v2 combat modes:**

- Separate balance from PvE
- Matchmaking system
- Ranked leagues

### 3. Shared Leaderboards

**Even without real-time multiplayer:**

- Global leaderboards for endless mode
- Weekly challenge rankings
- Speedrun times

---

## Modding Support

### 1. Data Modding

**Expose ScriptableObjects for easy modding:**

```
ModsFolder/
├── Weapons/
│   └── CustomSword.json
├── Enemies/
│   └── CustomGoblin.json
├── Arenas/
│   └── CustomArena.json
└── manifest.json
```

**Mod loader:**

```csharp
public class ModLoader : MonoBehaviour
{
    private void LoadMods()
    {
        var modFolders = Directory.GetDirectories(ModsPath);
        
        foreach (var folder in modFolders)
        {
            var manifest = LoadManifest(folder);
            
            foreach (var weaponFile in manifest.Weapons)
                LoadCustomWeapon(Path.Combine(folder, weaponFile));
            
            foreach (var enemyFile in manifest.Enemies)
                LoadCustomEnemy(Path.Combine(folder, enemyFile));
        }
    }
}
```

### 2. Steam Workshop Integration

**If releasing on Steam:**

- Workshop item upload/download
- Mod rating and popularity
- Automatic updates

---

## Implementation Priority Matrix

### Priority 1: Low Effort, High Impact

| Feature | Effort | Impact | Sprint |
|:--------|:-------|:-------|:-------|
| Increased enemy counts | Low | High | 5 |
| Post-processing | Low | High | 5 |
| Quality settings | Low | Medium | 5 |
| Dodge roll | Medium | High | 5 |

### Priority 2: Medium Effort, High Impact

| Feature | Effort | Impact | Sprint |
|:--------|:-------|:-------|:-------|
| Combo system | Medium | High | 7+ |
| Boss phases | Medium | High | 7+ |
| Skill system (basic) | Medium | High | 7+ |
| Finisher system | Medium | Medium | 7+ |

### Priority 3: High Effort, High Impact

| Feature | Effort | Impact | Sprint |
|:--------|:-------|:-------|:-------|
| Full skill trees | High | High | 9+ |
| Equipment system | High | High | 9+ |
| Roguelike mode | High | High | 9+ |
| Endless mode | Medium-High | High | 9+ |

### Priority 4: Post-Launch

| Feature | Effort | Notes |
|:--------|:-------|:------|
| Multiplayer | Very High | Major undertaking |
| Modding support | High | Community building |
| Arena builder | High | User content |
| Photo mode | Low | Nice to have |

---

## Summary

The transition from VR to PC opens up a massive range of possibilities:

1. **Combat** can become deeper with combos, skills, and finishers
2. **Enemies** can be more numerous and smarter
3. **Progression** can include skill trees, equipment, and loot
4. **Modes** can expand to endless, roguelike, and challenges
5. **Visuals** can leverage full post-processing and effects
6. **Quality of Life** features become possible (keybinding, accessibility, photo mode)

The existing architecture (ScriptableObjects, event channels, pooling) provides an excellent foundation for these expansions. Most new features can be added as new ScriptableObject types and new MonoBehaviour systems that integrate through the existing event channels.

**Recommended Focus Order:**
1. Get the PC port stable and polished (Sprints 1-6)
2. Add combat depth (combo system, skill system)
3. Expand enemy variety and counts
4. Add new game modes (endless, roguelike)
5. Consider multiplayer only if previous features are complete

Good luck with the expansion!
