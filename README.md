# Monsta Choppa VR

A VR roguelike arena combat game built with Unity's XR Interaction Toolkit. Fight waves of enemies in arenas, earn gold and experience, upgrade equipment, and progress through increasingly difficult challenges.

## Project Status

This portfolio project was shelved in January 2026. See the [full documentation](Docs/index.md) for details.

## Documentation

Full documentation is available in the `Docs/` folder, structured for MkDocs Material.

| Section | Description |
|:--------|:------------|
| [Architecture](Docs/architecture/overview.md) | System design, event channels, database patterns |
| [Game Systems](Docs/systems/weapons.md) | Weapons, enemies, UI, save data |
| [VR Development](Docs/vr/performance.md) | Performance optimization, comfort, SpaceWarp |
| [Future Plans](Docs/future/roadmap.md) | Roadmap and project status |

## Technical Highlights

- **ScriptableObject-driven architecture** with generic database patterns
- **Event channel system** for decoupled communication
- **Object pooling** with priority routing for VR performance
- **UI Toolkit** with Factory-View-Host pattern
- **XR Interaction Toolkit** integration for VR input

## Dependencies

| Asset | Purpose |
|:------|:--------|
| PrimeTween | Allocation-free animation |
| ESave | Save file serialization |
| XR Interaction Toolkit | VR input and interaction |
| Synty Dungeon Realms | Environment art |

## Building the Documentation

```bash
pip install mkdocs-material
mkdocs serve
```
