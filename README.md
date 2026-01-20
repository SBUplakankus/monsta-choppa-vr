# Monsta Choppa

A VR roguelike arena combat game built with Unity's XR Interaction Toolkit. Fight waves of enemies in arenas, earn gold and experience, upgrade your equipment, and progress through increasingly difficult challenges.

---

## Documentation

### Architecture

| Document | Description |
|----------|-------------|
| [Architecture](Docs/Architecture.md) | System overview, data-driven ScriptableObject patterns |
| [Event Channels](Docs/Event_Channels.md) | ScriptableObject-based publish/subscribe system |
| [Databases](Docs/Databases.md) | Generic database pattern for game data lookup |
| [Constants](Docs/Constants.md) | Centralized string keys and configuration |

### Game Systems

| Document | Description |
|----------|-------------|
| [User Interface](Docs/User_Interface.md) | Factory-View-Host-Controller UI pattern |
| [Weapons](Docs/Weapons.md) | Weapon data, modifiers, XR integration |
| [Enemies](Docs/Enemies.md) | Enemy components, spawning, pooling |
| [Save Data](Docs/Save_Data.md) | Player persistence with ESave |

### Development

| Document | Description |
|----------|-------------|
| [Game Development](Docs/Game_Development_Guide.md) | Game loop, progression, economy design |
| [VR Development](Docs/VR_Development_Guide.md) | Performance targets, optimization, Quest settings |
| [Bootstrap and Loading](Docs/Bootstrap_Loading_Guide.md) | Initialization, async scene loading |
| [UI Toolkit Practices](Docs/UI_Toolkit_Best_Practices.md) | Memory safety, cleanup patterns |
| [TODO](Docs/TODO.md) | Development roadmap and task list |

---

## Tutorials

### VR Development

| Title | Creator |
|-------|---------|
| [World Space UI Toolkit for VR](https://www.youtube.com/watch?v=XJRxGHENrjc) | Valem Tutorials |
| [Starter Assets and Hand Tracking](https://www.youtube.com/watch?v=6DcwHPxCE54) | Dilmer Valecillos |
| [XR Rig Setup - Quest 3 and Unity 6](https://www.youtube.com/watch?v=I1JcytXwXM4) | Justin P. Barnett |

### UI Toolkit

| Title | Creator |
|-------|---------|
| [Building Runtime UI with UI Toolkit](https://www.youtube.com/watch?v=-z3wNeYlJV4) | Game Dev Guide |
| [UI Toolkit Primer](https://www.youtube.com/watch?v=acQd7yr6eWg) | Tarodev |

---

## Dependencies

| Asset | Purpose |
|-------|---------|
| PrimeTween | Allocation-free animation |
| Auto VR Optimizer | Runtime performance adjustment |
| Mesh Combiner | Draw call reduction |
| Synty Dungeon Realms | Environment art |
| Volumetric Light Beam | Atmospheric lighting |
| Ovanisound | Sound effects |
| ESave | Save file serialization |

---

## Quick Start

1. Open the Bootstrapper scene
2. Enter Play mode or build to Quest
3. Start menu loads automatically
4. Press Play to enter hub world
5. Approach arena portals to start combat
