# Monsta Choppa

A VR arena combat game built with Unity's XR Interaction Toolkit. This documentation explains the project architecture, systems, and development guides.

---

## Documentation Index

### Core Architecture

| Document | Description |
|----------|-------------|
| [Architecture](Docs/Architecture.md) | System overview and layer responsibilities |
| [Event Channels](Docs/Event_Channels.md) | ScriptableObject-based event system |
| [Databases](Docs/Databases.md) | Generic database pattern for game data |
| [Constants](Docs/Constants.md) | Centralized string keys and configuration |

### Systems

| Document | Description |
|----------|-------------|
| [User Interface](Docs/User_Interface.md) | Factory-View-Host-Controller pattern |
| [Weapons](Docs/Weapons.md) | Data-driven weapon system |
| [Enemies](Docs/Enemies.md) | Enemy spawning and management |
| [Save Data](Docs/Save_Data.md) | Player persistence with ESave |

### Development Guides

| Document | Description |
|----------|-------------|
| [VR Development](Docs/VR_Development_Guide.md) | Performance targets, comfort settings, Quest tips |
| [Bootstrap and Loading](Docs/Bootstrap_Loading_Guide.md) | Initialization and async scene loading |
| [UI Toolkit Practices](Docs/UI_Toolkit_Best_Practices.md) | Memory safety and panel creation patterns |
| [Combat TODO](Docs/Combat_TODO.md) | Current optimization checklist |

---

## Tutorials and References

### VR Development Tutorials

| Title | Creator | Topic |
|-------|---------|-------|
| [World Space UI Toolkit for VR](https://www.youtube.com/watch?v=XJRxGHENrjc) | Valem Tutorials | UI Systems, Unity 6.2 |
| [Starter Assets and Hand Tracking](https://www.youtube.com/watch?v=6DcwHPxCE54) | Dilmer Valecillos | XR Starter Assets, Hand Tracking |
| [XR Rig Setup - Quest 3 and Unity 6](https://www.youtube.com/watch?v=I1JcytXwXM4) | Justin P. Barnett | XR Origin configuration |

### UI Toolkit Tutorials

| Title | Creator | Topic |
|-------|---------|-------|
| [Building Runtime UI with UI Toolkit](https://www.youtube.com/watch?v=-z3wNeYlJV4) | Game Dev Guide | UIDocument and runtime control |
| [UI Toolkit Primer](https://www.youtube.com/watch?v=acQd7yr6eWg) | Tarodev | Visual Tree, UXML, USS fundamentals |

### Project Dependencies

| Asset | Source | Purpose |
|-------|--------|---------|
| PrimeTween | Unity Asset Store | Allocation-free animation |
| Auto VR Optimizer | Unity Asset Store | Runtime performance adjustment |
| Mesh Combiner | Unity Asset Store | Draw call reduction |
| Synty Dungeon Realms | Unity Asset Store | Environment art |
| Volumetric Light Beam | Unity Asset Store | Atmospheric lighting |
| Ovanisound | Unity Asset Store | Sound effects and UI audio |

---
