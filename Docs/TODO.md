# Project TODO

Comprehensive development roadmap for Monsta Choppa. Tasks organized into weekly sprints with priorities and dependencies.

Last Updated: January 2025

---

## Current Implementation Status

| System | Completion | Notes |
|--------|------------|-------|
| Weapon Grab/Select | 90% | XR integration complete, needs holster polish |
| Shield Block/Bash | 80% | Core mechanics work, missing durability |
| Melee Combat | 75% | Needs velocity-based damage, hit feedback |
| Enemy Spawning | 85% | Wave system works, needs variety |
| Enemy Navigation | 70% | Basic NavMesh, needs AI states |
| Enemy Animations | 40% | Structure exists, animations needed |
| Player Health | 85% | System complete, needs UI |
| Player Levelling | 60% | Attributes exist, no progression curves |
| Arena System | 85% | Waves work, needs modular setup |
| Game Flow Manager | 30% | State enum exists, no logic |
| Pause System | 0% | Not implemented |
| Save System | 85% | Works, needs autosave triggers |
| Loading Screens | 50% | Basic exists, needs polish |
| UI - Menus | 80% | Start menu, settings complete |
| UI - HUD | 20% | Enemy health bars only |
| UI - Pause | 0% | Not implemented |
| UI - Results | 0% | Not implemented |
| UI - Wrist/Hand | 0% | Not implemented |
| Shop System | 0% | Not implemented |
| Economy/Upgrades | 0% | Not implemented |
| Level Portals | 30% | Basic portal exists |
| Roguelike Loop | 10% | Meta progression data exists |
| Leaderboards | 0% | Not implemented |

---

## Week 1: Core Systems Foundation

**Goal:** Establish working game loop from hub to arena and back.

### Game Flow Manager

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Implement GameFlowManager state machine | Critical | 4h | None |
| Add state transition methods (ToHub, ToArena, etc.) | Critical | 2h | State machine |
| Create GameStateEventChannel for state broadcasts | Critical | 1h | State machine |
| Integrate with existing scene loading | High | 2h | State machine |
| Add pause state handling | High | 2h | State machine |

### Pause System

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create pause input binding (menu button) | High | 1h | None |
| Implement custom time management (game vs real time) | High | 2h | None |
| Create PausePanelView with buttons | High | 2h | None |
| Create PausePanelHost with show/hide | High | 1h | View |
| Create PauseController with button handlers | High | 2h | Host |
| Test pause/resume cycle | High | 1h | All above |

### Autosave and Loading

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Add autosave on arena complete | High | 1h | Game flow |
| Add autosave on return to hub | High | 1h | Game flow |
| Add autosave on application pause | High | 1h | None |
| Add periodic autosave timer (hub only) | Medium | 1h | None |
| Create save indicator UI element | Medium | 1h | None |
| Implement auto-load on game start | High | 2h | Save system |

### Player Health UI

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create PlayerHealthView (bar, text) | High | 2h | None |
| Create PlayerHealthHost (data binding) | High | 1h | View |
| Position in VR space (floating or wrist) | High | 1h | Host |
| Add damage flash effect | Medium | 1h | Host |
| Add low health warning | Medium | 1h | Host |

---

## Week 2: Arena and Combat Polish

**Goal:** Complete arena experience with proper enemy behavior.

### Modular Arena Setup

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Extend ArenaData with full configuration | High | 2h | None |
| Create ArenaLoader for scene setup | High | 3h | ArenaData |
| Implement spawn point initialization | High | 2h | ArenaLoader |
| Add arena-specific wave configurations | High | 2h | ArenaData |
| Create arena completion detection | High | 2h | Wave system |
| Implement victory/defeat transitions | High | 2h | Game flow |

### Level Select Portals

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create portal prefab with visuals | High | 2h | None |
| Add ArenaData reference to portal | High | 1h | Portal prefab |
| Implement lock/unlock visual states | High | 2h | Portal |
| Add interaction to enter arena | High | 2h | Portal |
| Show arena preview on hover | Medium | 2h | Portal |
| Connect to arena loading flow | High | 1h | ArenaLoader |

### Enemy Navigation Improvements

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Implement EnemyAI state machine | High | 3h | None |
| Add Idle/Patrol state | Medium | 1h | State machine |
| Add Chase state with target tracking | High | 2h | State machine |
| Add Attack state with range detection | High | 2h | State machine |
| Add Stagger state for hit reactions | Medium | 1h | State machine |
| Tune NavMesh agent parameters | Medium | 1h | None |
| Add obstacle avoidance for groups | Medium | 2h | NavMesh |

### Enemy Animations

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Set up animator controller template | High | 2h | None |
| Create idle/walk/run blend tree | High | 2h | Controller |
| Add attack animation trigger | High | 1h | Controller |
| Add hit reaction animation | High | 1h | Controller |
| Add death animation | High | 1h | Controller |
| Connect animations to AI states | High | 2h | AI, Controller |
| Add animation events for damage timing | Medium | 1h | Attack anim |

### Improved Melee Combat

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Track weapon velocity during swing | High | 2h | None |
| Calculate damage based on velocity | High | 1h | Velocity tracking |
| Set minimum velocity threshold | High | 1h | Damage calc |
| Add hit stop effect (brief pause) | Medium | 2h | None |
| Improve haptic feedback patterns | Medium | 1h | None |
| Add directional knockback to enemies | Medium | 2h | Enemy system |

---

## Week 3: User Interface

**Goal:** Complete all in-game UI panels.

### Arena HUD

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create ArenaHUDView | High | 2h | None |
| Add wave counter display | High | 1h | View |
| Add enemies remaining counter | High | 1h | View |
| Add elapsed time display | Medium | 1h | View |
| Add score display | Medium | 1h | View |
| Create ArenaHUDHost with bindings | High | 2h | View |
| Position HUD in VR space | High | 1h | Host |

### Arena Results Panel

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create ArenaResultsView | High | 3h | None |
| Display final score | High | 1h | View |
| Display completion time | High | 1h | View |
| Display enemies killed | High | 1h | View |
| Display gold earned | High | 1h | View |
| Display XP earned | High | 1h | View |
| Add star rating (1-3) | Medium | 2h | View |
| Create ArenaResultsHost | High | 1h | View |
| Add continue button handler | High | 1h | Host |

### Hand/Wrist UI

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create wrist anchor on controller | High | 1h | None |
| Create WristHUDView (compact stats) | High | 2h | None |
| Implement palm-up visibility detection | High | 2h | Anchor |
| Add fade in/out animation | Medium | 1h | Detection |
| Display health, gold, wave | High | 1h | View |
| Create WristHUDHost | High | 1h | View |

### Level Stats UI

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create PlayerStatsView | Medium | 2h | None |
| Display current level | High | 1h | View |
| Display XP progress bar | High | 1h | View |
| Display gold total | High | 1h | View |
| Create PlayerStatsHost | Medium | 1h | View |
| Add level-up notification | Medium | 2h | Host |

### Loading Screen Polish

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Improve LoadingScreenView design | Medium | 2h | None |
| Add animated progress bar | Medium | 1h | View |
| Add loading tips rotation | Low | 1h | View |
| Add spinner animation | Low | 1h | View |
| Ensure smooth fade transitions | Medium | 1h | Host |

---

## Week 4: Economy and Progression

**Goal:** Implement shop, upgrades, and roguelike meta-progression.

### Shop System

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create ShopItemData ScriptableObject | High | 1h | None |
| Create ShopDatabase | High | 1h | ShopItemData |
| Create ShopPanelView | High | 3h | None |
| Add item grid/list display | High | 2h | View |
| Add item detail section | High | 1h | View |
| Add purchase button | High | 1h | View |
| Add gold display | High | 1h | View |
| Create ShopPanelHost | High | 2h | View |
| Create ShopController | High | 2h | Host |
| Implement purchase validation | High | 1h | Controller |
| Implement gold deduction | High | 1h | Controller |
| Add purchase to unlocks | High | 1h | Controller |
| Trigger save after purchase | High | 1h | Controller |

### Upgrade System

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create UpgradeData ScriptableObject | High | 2h | None |
| Define upgrade categories | High | 1h | UpgradeData |
| Create UpgradeDatabase | High | 1h | UpgradeData |
| Create UpgradesPanelView | High | 3h | None |
| Add category tabs | Medium | 1h | View |
| Add upgrade cards with levels | High | 2h | View |
| Create UpgradesPanelHost | High | 2h | View |
| Implement upgrade purchase logic | High | 2h | Host |
| Apply upgrade effects to player | High | 2h | Player system |

### Equipment Selection

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create EquipmentPanelView | Medium | 2h | None |
| Display weapon slots | High | 1h | View |
| Display owned weapons | High | 2h | View |
| Implement equip/unequip | High | 2h | View |
| Create EquipmentPanelHost | Medium | 1h | View |
| Save equipped loadout | High | 1h | Save system |
| Load loadout on arena entry | High | 1h | Arena loader |

### Weapon/Shield Selection in Hub

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Create weapon rack prefab | Medium | 2h | None |
| Display owned weapons on rack | Medium | 2h | Rack |
| Implement grab to equip | Medium | 2h | XR system |
| Create shield rack prefab | Medium | 1h | Weapon rack |
| Add weapon stats preview | Low | 2h | Rack |

### Player Levelling

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Define XP threshold formula | High | 1h | None |
| Create LevelProgressionData | High | 1h | Formula |
| Implement level-up check on XP gain | High | 2h | Progression |
| Define rewards per level | Medium | 1h | None |
| Apply stat bonuses on level up | High | 2h | Rewards |
| Save level to progression data | High | 1h | Save system |

### Game Economy Balance

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Define gold rewards per enemy type | High | 1h | None |
| Define gold rewards per wave/arena | High | 1h | None |
| Set upgrade costs progression | High | 1h | Upgrades |
| Set weapon prices | High | 1h | Shop |
| Balance economy spreadsheet | Medium | 2h | All above |
| Test economy flow | Medium | 2h | All above |

### Roguelike Progression

| Task | Priority | Estimate | Dependencies |
|------|----------|----------|--------------|
| Define permanent unlocks | Medium | 1h | None |
| Implement unlock tracking | Medium | 2h | Save system |
| Add partial rewards on defeat | Medium | 1h | Economy |
| Add full rewards on victory | Medium | 1h | Economy |
| Track arena completion records | Medium | 2h | Save system |

---

## Backlog (Post-Month 1)

### Leaderboard System

| Task | Priority |
|------|----------|
| Design leaderboard data structure | Low |
| Create LeaderboardPanelView | Low |
| Implement local score storage | Low |
| Display personal bests | Low |
| Add rank display | Low |

### Save Slots UI

| Task | Priority |
|------|----------|
| Create SaveSlotPanelView | Low |
| Display multiple save slots | Low |
| Show slot preview (level, time) | Low |
| Implement slot selection | Low |
| Add delete save option | Low |

### XR Time Manager

| Task | Priority |
|------|----------|
| Separate game time from real time | Medium |
| Pause only affects game time | Medium |
| VR tracking uses real time | Medium |

### Additional Features

| Task | Priority |
|------|----------|
| Boss unique attack patterns | Medium |
| Environmental hazards | Low |
| Combo system | Low |
| Achievement system | Low |
| Tutorial arena | Medium |
| Audio settings per-zone | Low |

---

## Performance Checklist

Run weekly on Quest 2 device:

- [ ] Maintain 72+ FPS in combat
- [ ] Max 8 active enemies at once
- [ ] No GC spikes during gameplay
- [ ] All spawned objects use pooling
- [ ] UI updates use data binding (no polling)
- [ ] NavMesh paths update at intervals (not every frame)
- [ ] Profile and address any hotspots

---

## Definition of Done

A feature is complete when:

1. Works correctly on Quest 2 device
2. Maintains 72+ FPS
3. Events properly subscribed/unsubscribed
4. Pooled objects returned correctly
5. Data saved/loaded correctly
6. UI readable at VR distances
7. Haptic feedback where appropriate
8. No console errors or warnings
