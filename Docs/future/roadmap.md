# Development Roadmap

Tasks that were planned for completion before the project was shelved. Organized by priority and system.

---

## Implementation Status

| System | Completion | Notes |
|:-------|:-----------|:------|
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

---

## Week 1: Core Systems Foundation

**Goal:** Establish working game loop from hub to arena and back.

### Game Flow Manager

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Implement GameFlowManager state machine | Critical | 4h |
| Add state transition methods | Critical | 2h |
| Create GameStateEventChannel | Critical | 1h |
| Integrate with scene loading | High | 2h |
| Add pause state handling | High | 2h |

### Pause System

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Create pause input binding | High | 1h |
| Implement custom time management | High | 2h |
| Create PausePanelView | High | 2h |
| Create PausePanelHost | High | 1h |
| Create PauseController | High | 2h |

### Player Health UI

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Create PlayerHealthView | High | 2h |
| Create PlayerHealthHost | High | 1h |
| Position in VR space | High | 1h |
| Add damage flash effect | Medium | 1h |

---

## Week 2: Arena and Combat Polish

**Goal:** Complete arena experience with proper enemy behavior.

### Modular Arena Setup

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Extend ArenaData configuration | High | 2h |
| Create ArenaLoader | High | 3h |
| Implement spawn point initialization | High | 2h |
| Add arena-specific wave configs | High | 2h |
| Create arena completion detection | High | 2h |
| Implement victory/defeat transitions | High | 2h |

### Enemy Navigation Improvements

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Implement EnemyAI state machine | High | 3h |
| Add Idle/Patrol state | Medium | 1h |
| Add Chase state | High | 2h |
| Add Attack state | High | 2h |
| Add Stagger state | Medium | 1h |

### Improved Melee Combat

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Track weapon velocity | High | 2h |
| Calculate velocity-based damage | High | 1h |
| Set minimum velocity threshold | High | 1h |
| Add hit stop effect | Medium | 2h |
| Improve haptic feedback | Medium | 1h |

---

## Week 3: User Interface

**Goal:** Complete all in-game UI panels.

### Arena HUD

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Create ArenaHUDView | High | 2h |
| Add wave counter | High | 1h |
| Add enemies remaining | High | 1h |
| Add elapsed time | Medium | 1h |
| Position in VR space | High | 1h |

### Arena Results Panel

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Create ArenaResultsView | High | 3h |
| Display final score | High | 1h |
| Display gold/XP earned | High | 1h |
| Add star rating | Medium | 2h |
| Add continue button | High | 1h |

---

## Week 4: Economy and Progression

**Goal:** Implement shop, upgrades, and meta-progression.

### Shop System

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Create ShopItemData | High | 1h |
| Create ShopDatabase | High | 1h |
| Create ShopPanelView | High | 3h |
| Create ShopController | High | 2h |
| Implement purchase validation | High | 1h |

### Player Levelling

| Task | Priority | Estimate |
|:-----|:---------|:---------|
| Define XP threshold formula | High | 1h |
| Create LevelProgressionData | High | 1h |
| Implement level-up check | High | 2h |
| Apply stat bonuses | High | 2h |

---

## Backlog

Lower priority items planned for post-MVP:

- Leaderboard system
- Save slots UI
- Boss unique attack patterns
- Environmental hazards
- Combo system
- Achievement system
- Tutorial arena

---

## Definition of Done

A feature was considered complete when:

1. Works correctly on Quest 2 device
2. Maintains 72+ FPS
3. Events properly subscribed/unsubscribed
4. Pooled objects returned correctly
5. Data saved/loaded correctly
6. UI readable at VR distances
7. Haptic feedback where appropriate
8. No console errors or warnings
