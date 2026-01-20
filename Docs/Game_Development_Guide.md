# Game Development Guide

Overview of game flow, progression systems, and VR-specific design considerations for Monsta Choppa.

---

## Game Loop

```
HUB WORLD
  - Shop: Buy/sell items
  - Armory: Equip weapons
  - Portals: Select arena
        |
        v
ARENA
  - Wave 1 -> Wave 2 -> ... -> Boss
  - Collect: Gold, Experience
        |
        v
RESULTS
  - Score, Rewards
  - Save Progress
  - Return to Hub
```

---

## Game States

| State | Description | Transitions To |
|-------|-------------|----------------|
| StartMenu | Initial menu | Loading, Hub |
| Hub | Player base, shops, portals | Arena, Settings |
| Arena | Active combat | Victory, Defeat, Paused |
| ArenaPaused | Combat suspended | Arena, Hub |
| ArenaVictory | Completed all waves | Results, Hub |
| ArenaDefeat | Player died | Results, Hub |

---

## Progression Systems

### Player Level

Experience earned from combat increases player level.

Level thresholds use polynomial scaling:
- Level 1 to 2: 100 XP
- Level 2 to 3: 400 XP
- Level N to N+1: 100 * N^2 XP

Level rewards:
- Stat bonuses (health, damage)
- Unlock new arenas
- Unlock shop items

### Gold Economy

Sources:
- Basic enemy kill: 5-10 gold
- Elite enemy kill: 20-30 gold
- Boss kill: 100+ gold
- Wave completion: 25 gold
- Arena completion: 100-500 gold

Expenses:
- Basic weapon: 100-300 gold
- Rare weapon: 500-1000 gold
- Stat upgrade: 50-200 gold
- Arena unlock: 200-500 gold

### Meta Progression

Permanent upgrades that persist across runs:
- Stat multipliers (health, damage, gold gain)
- Weapon unlocks
- Arena unlocks
- Starting loadout options

---

## Arena Design

### Wave Structure

Each arena contains multiple waves of enemies, culminating in a boss wave.

Wave configuration:
- Enemy types and counts
- Spawn delays
- Intermission time between waves

### Difficulty Scaling

| Arena | Enemies | Health | Recommended Level |
|-------|---------|--------|-------------------|
| Tutorial | 3-5 | 50% | 1 |
| Easy | 5-8 | 100% | 1-3 |
| Normal | 8-12 | 100% | 4-6 |
| Hard | 12-15 | 125% | 7-10 |
| Nightmare | 15-20 | 150% | 11+ |

---

## VR Comfort Settings

### Locomotion Options

| Type | Description | Comfort |
|------|-------------|---------|
| Teleport | Point and blink | High |
| Dash | Quick move | Medium |
| Smooth | Continuous | Low |

### Turning Options

- Snap: Instant rotation (45, 30, or 15 degrees)
- Smooth: Continuous rotation

### Comfort Aids

- Vignette during movement
- Seated mode support
- Height calibration
- Dominant hand selection

---

## Save System

### Auto-save Triggers

- Arena completion
- Returning to hub
- Purchasing items
- Application pause
- Timer (every 5 minutes in hub)

### Saved Data

- Player level and experience
- Gold and currency
- Purchased upgrades
- Unlocked content
- Arena completion records
- Equipment loadout
- Settings

---

## Performance Targets

| Metric | Target | Maximum |
|--------|--------|---------|
| Frame Rate | 72 FPS | - |
| Active Enemies | 6 | 8 |
| Draw Calls | 100 | 150 |
| Frame Budget | 11ms | 13.9ms |
