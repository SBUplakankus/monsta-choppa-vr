# Project Status

This document explains the current state of the project, why development was paused, and what lessons were learned.

---

## Why the Project Was Shelved

In January 2026, I started a new position working with **Meta's native SDK** rather than OpenXR. The two SDKs have fundamentally different approaches:

| Aspect | OpenXR (This Project) | Meta Native SDK (Work) |
|:-------|:----------------------|:-----------------------|
| Input System | XR Interaction Toolkit | OVRInput |
| Controller Handling | XRController + Action Maps | OVRCameraRig |
| Hand Tracking | XR Hands | OVRHand |
| Passthrough | OpenXR Extensions | OVRPassthroughLayer |
| Performance APIs | Standard Unity | OVRManager + OVRMetrics |

Context-switching between these two paradigms became impractical. Rather than produce subpar work in both contexts, I chose to pause this portfolio project.

---

## What Was Completed

### Architecture (Complete)

- ScriptableObject-driven data layer
- Generic database pattern with O(1) lookups
- Type-safe event channel system
- Centralized static access (GameEvents, GameDatabases)
- Object pooling with priority routing

### VR Foundation (Complete)

- XR Rig setup with locomotion options
- Teleport, smooth locomotion, snap/smooth turn
- Controller input mappings
- Haptic feedback system

### Combat System (75%)

- Weapon base classes and inheritance hierarchy
- Melee, bow, staff, shield, throwable weapon types
- WeaponData ScriptableObjects
- WeaponHitbox collision detection
- XR grab integration

**Missing:** Velocity-based damage calculation, hit feedback polish

### Enemy System (70%)

- Component-based architecture (Controller, Health, Movement, Animator, Attack)
- EnemyData ScriptableObjects
- NavMesh navigation
- Wave spawning system
- Object pooling integration

**Missing:** AI state machine, animation polish, variety

### UI System (60%)

- UI Toolkit integration
- Factory-View-Host architecture
- Start menu, settings panels
- Loading screen
- Enemy health bars

**Missing:** Pause menu, arena HUD, results screen, wrist UI

### Save System (85%)

- ESave integration
- MetaProgressionData structure
- Event-driven save/load
- Settings persistence

**Missing:** Autosave triggers, save indicator UI

---

## What Was Not Started

- Shop system
- Equipment selection UI
- Player levelling with progression curves
- Roguelike meta-progression loop
- Leaderboards
- Tutorial

---

## Technical Highlights

### Strong Points

The architecture demonstrates professional Unity development practices:

1. **Decoupled Systems** - Event channels allow systems to communicate without direct references, making the codebase modular and testable.

2. **Data-Driven Design** - ScriptableObjects define all game content, enabling rapid iteration without code changes.

3. **VR Performance Awareness** - Object pooling, priority-based updates, and non-allocating physics queries keep frame rates stable.

4. **Memory Safety** - UI Toolkit patterns with proper subscription management prevent memory leaks.

5. **Clean Code Organization** - Consistent folder structure, namespaces, and naming conventions.

### Lessons Learned

1. **Scope Management** - The original scope was too ambitious for a solo portfolio project. Should have focused on a complete vertical slice rather than breadth.

2. **SDK Lock-in** - Building on OpenXR was the right choice for portability, but made transitioning to Meta's ecosystem more difficult.

3. **Documentation First** - Writing documentation while building forced clearer thinking about architecture decisions.

4. **VR Playtesting** - More frequent on-device testing would have caught comfort and usability issues earlier.

---

## Code Quality Assessment

Based on self-review:

| Category | Grade | Notes |
|:---------|:------|:------|
| Architecture | A | Excellent layering, proper patterns |
| Code Organization | A- | Great structure, minor namespace inconsistencies |
| Documentation | A | XML docs, markdown documentation |
| VR Best Practices | A | Non-allocating physics, pooling, update throttling |
| Completeness | C+ | Many systems incomplete |
| Ship Readiness | C | Core loop exists but incomplete |

---

## If Development Resumed

Priority order for completing the project:

1. **Game Flow Manager** - Complete state machine to enable full gameplay loop
2. **Pause System** - Critical for VR usability
3. **Results Screen** - Provide player feedback after combat
4. **Player Health UI** - Essential gameplay information
5. **Simplified Shop** - Core progression mechanic
6. **Polish Pass** - Combat feedback, animations, audio

Estimated time to MVP: 4-6 weeks of focused development.

---

## Repository Contents

```
Assets/
├── Scripts/           # C# source code (documented)
├── Data/              # ScriptableObject assets
├── Prefabs/           # Weapon, enemy, UI prefabs
├── Scenes/            # Bootstrapper, Hub, Arena scenes
└── UI/                # USS stylesheets

Docs/                  # This documentation
```

The codebase is clean and well-documented, suitable for review or continuation by another developer.
