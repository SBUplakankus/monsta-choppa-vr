# Monsta Choppa - AI Context Reference

> **Purpose:** Quick-reference document for AI assistants. Point to this file at the start of any session to provide codebase context.

---

## PROJECT IDENTITY

```yaml
name: Monsta Choppa
engine: Unity
language: C#
architecture: ScriptableObject-driven with Event Channels
ui_framework: UI Toolkit (not UGUI)
original_platform: VR (Quest 2/3, XR Interaction Toolkit)
target_platform: PC (First/Third Person) - Transition in progress
status: Shelved January 2026, PC transition planned
```

---

## DIRECTORY STRUCTURE

```
/home/runner/work/monsta-choppa-vr/monsta-choppa-vr/
├── Assets/
│   ├── Scripts/
│   │   ├── Attributes/          # FloatAttribute, IntAttribute (reactive values)
│   │   ├── Audio/               # Audio controllers
│   │   ├── Characters/
│   │   │   ├── Base/            # HealthComponent, AnimatorComponent
│   │   │   └── Enemies/         # EnemyController, EnemyHealth, EnemyMovement, EnemyAnimator, EnemyAttack
│   │   ├── Constants/           # GameConstants, AudioKeys, LocalizationKeys, UIToolkitStyles
│   │   ├── Data/                # ScriptableObject data classes
│   │   │   ├── Arena/           # ArenaData, WaveData
│   │   │   ├── Core/            # EnemyData, ParticleData, WorldAudioData
│   │   │   ├── Progression/     # MetaProgressionData, UpgradeData
│   │   │   └── Weapons/         # WeaponData, WeaponModifierData
│   │   ├── Databases/           # DatabaseBase<T>, GameDatabases, WeaponDatabase, EnemyDatabase
│   │   ├── Events/
│   │   │   ├── Channels/        # EventChannel<T> generic implementation
│   │   │   └── Registries/      # GameplayEvents, AudioEvents, SystemEvents, UIEvents
│   │   ├── Factories/           # UIToolkitFactory
│   │   ├── Interfaces/          # IDamageable, IUpdateable
│   │   ├── Player/              # PlayerAttributes, PlayerArenaController, XR components
│   │   ├── Pooling/             # GamePoolManager
│   │   ├── Saves/               # SaveFileManagerBase, PlayerSaveFileManager, SettingsSaveFileManager
│   │   ├── Systems/
│   │   │   ├── Arena/           # WaveManager, WaveSpawner, EnemyManager, ArenaStateManager
│   │   │   ├── Core/            # GameFlowManager, GameUpdateManager, BoostrapManager
│   │   │   └── Hub/             # Hub-specific systems
│   │   ├── UI/
│   │   │   ├── Controllers/     # StartMenuController, PauseMenuController, LoadingScreenController
│   │   │   ├── Hosts/           # BasePanelHost, StartMenuPanelHost, SettingsPanelHost
│   │   │   └── Views/           # BasePanelView, StartMenuPanelView, SettingsPanelView
│   │   └── Weapons/             # XRWeaponBase, MeleeXRWeapon, BowXRWeapon, ShieldXRWeapon, WeaponHitbox
│   ├── UI/                      # UXML and USS files
│   ├── Prefabs/                 # Game prefabs
│   ├── Scenes/                  # Bootstrapper, Hub, Arena scenes
│   └── Data/                    # ScriptableObject asset instances
├── Docs/
│   ├── architecture/            # overview.md, database-pattern.md, event-system.md
│   ├── systems/                 # weapons.md, enemies.md, user-interface.md, save-data.md
│   ├── vr/                      # performance.md, comfort.md, spacewarp.md
│   └── future/                  # roadmap.md, project-status.md, vr-to-pc-transition-guide.md, pc-implementation-plan.md, pc-expansion-ideas.md
└── Packages/                    # Unity package manifest
```

---

## CORE PATTERNS

### 1. ScriptableObject Database Pattern
```
Location: Assets/Scripts/Databases/DatabaseBase.cs
Usage: All game data (weapons, enemies, etc.) stored in SO databases with O(1) lookup
Access: GameDatabases.WeaponDatabase.Get("sword_fire")
```

### 2. Event Channel Pattern
```
Location: Assets/Scripts/Events/Channels/EventChannel.cs
Registries: Assets/Scripts/Events/Registries/
Usage: Decoupled communication between systems
Example: GameplayEvents.EnemySpawned.Raise(enemyController)
         GameplayEvents.EnemySpawned.Subscribe(HandleEnemySpawn)
```

### 3. UI Factory-View-Host Pattern
```
Factory: Assets/Scripts/Factories/UIToolkitFactory.cs
Views: Assets/Scripts/UI/Views/BasePanelView.cs (define UI structure)
Hosts: Assets/Scripts/UI/Hosts/BasePanelHost.cs (manage lifecycle, animations)
Controllers: Assets/Scripts/UI/Controllers/ (game logic)
```

### 4. Object Pooling
```
Location: Assets/Scripts/Pooling/GamePoolManager.cs
Usage: GamePoolManager.Instance.GetEnemyPrefab(data, position, rotation)
       GamePoolManager.Instance.ReturnEnemyPrefab(enemy)
```

### 5. Priority Update System
```
Location: Assets/Scripts/Systems/Core/GameUpdateManager.cs
Usage: Throttled updates for performance (High=every frame, Medium=0.2s, Low=0.4s)
       GameUpdateManager.Instance.Register(this, UpdatePriority.High)
```

---

## KEY FILES QUICK REFERENCE

### Game Flow
| Purpose | File |
|---------|------|
| Game State Machine | `Assets/Scripts/Systems/Core/GameFlowManager.cs` |
| Arena State Machine | `Assets/Scripts/Systems/Arena/ArenaStateManager.cs` |
| Wave Management | `Assets/Scripts/Systems/Arena/WaveManager.cs` |
| Enemy Spawning | `Assets/Scripts/Systems/Arena/WaveSpawner.cs` |
| Bootstrap/Init | `Assets/Scripts/Systems/Core/BoostrapManager.cs` |

### Data
| Purpose | File |
|---------|------|
| Weapon Definition | `Assets/Scripts/Data/Weapons/WeaponData.cs` |
| Enemy Definition | `Assets/Scripts/Data/Core/EnemyData.cs` |
| Player Progression | `Assets/Scripts/Data/Progression/MetaProgressionData.cs` |
| Upgrades | `Assets/Scripts/Data/Progression/UpgradeData.cs` |

### Events
| Purpose | File |
|---------|------|
| Gameplay Events | `Assets/Scripts/Events/Registries/GameplayEvents.cs` |
| Audio Events | `Assets/Scripts/Events/Registries/AudioEvents.cs` |
| System Events | `Assets/Scripts/Events/Registries/SystemEvents.cs` |
| UI Events | `Assets/Scripts/Events/Registries/UIEvents.cs` |

### Characters
| Purpose | File |
|---------|------|
| Enemy Main Controller | `Assets/Scripts/Characters/Enemies/EnemyController.cs` |
| Enemy Health | `Assets/Scripts/Characters/Enemies/EnemyHealth.cs` |
| Enemy Movement/AI | `Assets/Scripts/Characters/Enemies/EnemyMovement.cs` |
| Enemy Animation | `Assets/Scripts/Characters/Enemies/EnemyAnimator.cs` |
| Enemy Combat | `Assets/Scripts/Characters/Enemies/EnemyAttack.cs` |

### UI
| Purpose | File |
|---------|------|
| UI Element Factory | `Assets/Scripts/Factories/UIToolkitFactory.cs` |
| Base View Class | `Assets/Scripts/UI/Views/BasePanelView.cs` |
| Base Host Class | `Assets/Scripts/UI/Hosts/BasePanelHost.cs` |
| Style Constants | `Assets/Scripts/Constants/UIToolkitStyles.cs` |

### Constants
| Purpose | File |
|---------|------|
| Game Constants | `Assets/Scripts/Constants/GameConstants.cs` |
| Audio Keys | `Assets/Scripts/Constants/AudioKeys.cs` |
| Localization Keys | `Assets/Scripts/Constants/LocalizationKeys.cs` |

---

## GAME STATES

### GameState (GameFlowManager)
```csharp
enum GameState { StartMenu, Loading, Hub, Arena, ArenaVictory, ArenaDefeat, ArenaPaused }
```

### ArenaState (ArenaStateManager)
```csharp
enum ArenaState { ArenaPrelude, WaveIntermission, WaveActive, WaveComplete, BossIntermission, BossActive, BossComplete, ArenaVictory, ArenaDefeat, ArenaPaused }
```

---

## EVENT CHANNELS AVAILABLE

```csharp
// GameplayEvents
GameplayEvents.PlayerDamaged          // EventChannel<int>
GameplayEvents.PlayerHealed           // EventChannel<int>
GameplayEvents.EnemySpawned           // EventChannel<EnemyController>
GameplayEvents.EnemyDespawned         // EventChannel<EnemyController>
GameplayEvents.GoldChanged            // EventChannel<int>
GameplayEvents.ExperienceChanged      // EventChannel<int>
GameplayEvents.LevelChanged           // EventChannel<int>
GameplayEvents.GamePaused             // EventChannel (void)
GameplayEvents.GameResumed            // EventChannel (void)
GameplayEvents.GameStateChanged       // EventChannel<GameState>
GameplayEvents.GameStateChangeRequested  // EventChannel<GameState>
GameplayEvents.ArenaStateChanged      // EventChannel<ArenaState>
GameplayEvents.ArenaStateChangeRequested // EventChannel<ArenaState>

// SystemEvents
SystemEvents.SettingsLoadRequested
SystemEvents.SettingsSaveRequested
SystemEvents.PlayerLoadRequested
SystemEvents.PlayerSaveRequested

// AudioEvents
AudioEvents.SfxRequested
AudioEvents.MusicRequested
```

---

## NAMING CONVENTIONS

```yaml
ScriptableObjects: PascalCase with Data suffix (WeaponData, EnemyData)
MonoBehaviours: PascalCase descriptive (EnemyController, WaveSpawner)
Events: Past tense for occurred, Requested for requests (EnemySpawned, GameStateChangeRequested)
Interfaces: I prefix (IDamageable, IUpdateable)
Constants: SCREAMING_SNAKE or PascalCase (MaxActiveEnemies)
Private fields: _camelCase with underscore prefix
Namespaces: Match folder structure (Characters.Enemies, Systems.Arena)
```

---

## VR-TO-PC TRANSITION STATUS

### Files to Create (PC)
- `Assets/Scripts/Player/PCInputHandler.cs`
- `Assets/Scripts/Player/FPSPlayerController.cs`
- `Assets/Scripts/Player/TargetingSystem.cs`
- `Assets/Scripts/Player/PCCombatManager.cs`
- `Assets/Scripts/Weapons/PCWeaponBase.cs`
- `Assets/Scripts/Weapons/PCMeleeWeapon.cs`
- `Assets/Scripts/Weapons/PCRangedWeapon.cs`

### Files to Delete (VR-specific)
- `Assets/Scripts/Player/PlayerHapticFeedback.cs`
- `Assets/Scripts/Player/WristProximityDetector.cs`
- `Assets/Scripts/Systems/Core/SpaceWarpCameraExtension.cs`
- `Assets/Scripts/Systems/Core/RefreshRateController.cs`
- `Assets/Scripts/UI/Views/VignetteView.cs`
- `Assets/Scripts/UI/Hosts/VignetteHost.cs`
- `Assets/XR/` (entire folder)
- `Assets/XRI/` (entire folder)

### Files to Modify
- `GameConstants.cs` - Update limits for PC performance
- `GameFlowManager.cs` - Remove VR-specific handling
- `WaveSpawner.cs` - Increase enemy limits
- `WeaponData.cs` - Remove VR grip offsets
- `EnemyMovement.cs` - Update target finding

### Systems Unchanged
- All databases (DatabaseBase, GameDatabases, WeaponDatabase, EnemyDatabase)
- All event channels (EventChannel, GameplayEvents, etc.)
- All enemy systems (EnemyController, EnemyHealth, etc.)
- Object pooling (GamePoolManager)
- Save system (SaveFileManagerBase, PlayerSaveFileManager)
- UI Factory-View-Host pattern (unchanged architecture, sizing updates only)

---

## DOCUMENTATION LOCATIONS

| Topic | Location |
|-------|----------|
| Architecture Overview | `Docs/architecture/overview.md` |
| Database Pattern | `Docs/architecture/database-pattern.md` |
| Event System | `Docs/architecture/event-system.md` |
| Weapon System | `Docs/systems/weapons.md` |
| Enemy System | `Docs/systems/enemies.md` |
| UI System | `Docs/systems/user-interface.md` |
| VR Performance | `Docs/vr/performance.md` |
| Project Status | `Docs/future/project-status.md` |
| Original Roadmap | `Docs/future/roadmap.md` |
| PC Transition Guide | `Docs/future/vr-to-pc-transition-guide.md` |
| PC Implementation Plan | `Docs/future/pc-implementation-plan.md` |
| PC Expansion Ideas | `Docs/future/pc-expansion-ideas.md` |

---

## COMMON TASKS QUICK REFERENCE

### Add New Weapon Type
1. Create `WeaponData` ScriptableObject
2. Create weapon prefab with `PCWeaponBase` subclass (or `XRWeaponBase` for VR)
3. Add to `WeaponDatabase`
4. Configure in `WeaponData` asset

### Add New Enemy Type
1. Create `EnemyData` ScriptableObject
2. Create prefab with `EnemyController` + components
3. Add to `EnemyDatabase`
4. Add to wave configurations

### Add New UI Panel
1. Create `[Name]View.cs` extending `BasePanelView`
2. Create `[Name]Host.cs` extending `BasePanelHost`
3. Create `[Name]Controller.cs` for logic
4. Use `UIToolkitFactory` for elements

### Add New Event
1. Add to appropriate registry (`GameplayEvents.cs`, etc.)
2. Pattern: `public static readonly EventChannel<T> EventName = new();`
3. Subscribe in `OnEnable`, Unsubscribe in `OnDisable`

### Add New Database
1. Create data class extending `ScriptableObject`
2. Create database class extending `DatabaseBase<T>`
3. Add static reference in `GameDatabases.cs`
4. Initialize in `BoostrapManager`

---

## THIRD-PARTY DEPENDENCIES

| Package | Purpose |
|---------|---------|
| PrimeTween | Allocation-free animation |
| ESave | Save file serialization |
| XR Interaction Toolkit | VR input (removing for PC) |
| Synty Dungeon Realms | Environment art |
| UI Toolkit | Built-in UI framework |
| Cinemachine | Camera (adding for PC) |
| Input System | Input handling |

---

## USEFUL COMMANDS

```bash
# View project structure
ls -la /home/runner/work/monsta-choppa-vr/monsta-choppa-vr/Assets/Scripts/

# Search for specific pattern
grep -r "pattern" /home/runner/work/monsta-choppa-vr/monsta-choppa-vr/Assets/Scripts/

# Find all ScriptableObjects
find /home/runner/work/monsta-choppa-vr/monsta-choppa-vr/Assets/Scripts/Data -name "*.cs"

# Find all interfaces
grep -r "interface I" /home/runner/work/monsta-choppa-vr/monsta-choppa-vr/Assets/Scripts/
```

---

## SESSION STARTER PROMPT

When starting a new session, paste this context:

```
I'm working on Monsta Choppa, a Unity C# project transitioning from VR to PC.

Key info:
- ScriptableObject-driven architecture with generic databases
- Event channel system for decoupled communication
- UI Toolkit with Factory-View-Host pattern
- Object pooling via GamePoolManager
- Currently transitioning from XR Interaction Toolkit to PC FPS/TPS

Reference file: Docs/future/ai-context-reference.md

[Your specific question/task here]
```

---

*Last updated: January 2026*
*Generated for AI assistant context*
