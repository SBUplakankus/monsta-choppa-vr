# 📘 The Monsta Choppa Repository

**A lot of this is AI Generated at the moment for placeholder info while I develop the game**

This folder contains high-level technical documentation for the Unity VR project **Monsta Choppa*.  
The goal of these documents is to explain **how systems are structured**, **why architectural decisions were made**, and **how different systems communicate**.

This project is built on top of Unity’s **XR Interaction Toolkit–based VR Template**, with custom systems layered on top to support **modular**, **event-driven**, and **data-driven** gameplay.

---

## 🗂️ Documentation Index

### 📚 Core Architecture
| 📄 Document                                    | 📝 Description                                                        |
|------------------------------------------------|-----------------------------------------------------------------------|
| [Architecture Overview](Docs/Architecture.md)  | High-level architectural philosophy and system relationships          |
| [Event Channel System](Docs/Event_Channels.md) | ScriptableObject-based event system for decoupled communication       |
| [Data & Database System](Docs/Databases.md)    | ScriptableObject databases for items, configuration, and runtime data |
| [Constants](Docs/Constants.md)                 | How Global Constant variables are used for type safety                |

### 🎮 Systems Documentation
| 📄 Document                                    | 📝 Description                                                        |
|------------------------------------------------|-----------------------------------------------------------------------|
| [User Interface](Docs/User_Interface.md)       | How the UI Works using UI Toolkit and C#                              |
| [Factories](Docs/Factories.md)                 | Factory classes for creating elements                                 |
| [Weapons](Docs/Weapons.md)                     | How the weapons system is structured                                  |
| [Enemies](Docs/Enemies.md)                     | How the enemy system is structured                                    |
| [Save Data](Docs/Save_Data.md)                 | How player save data is handled                                       |

### 🎓 Development Guides
| 📄 Document                                                    | 📝 Description                                                        |
|----------------------------------------------------------------|-----------------------------------------------------------------------|
| [Game Development Guide](Docs/Game_Development_Guide.md)       | Complete guide to game flow, levelling, economy, UI and more          |
| [UI Toolkit Best Practices](Docs/UI_Toolkit_Best_Practices.md) | Memory leak prevention and efficient UITK panel creation              |
| [Meta SpaceWarp Guide](Docs/Meta_SpaceWarp_Guide.md)           | Implementing Application SpaceWarp for Quest performance              |
| [Bootstrap & Loading Guide](Docs/Bootstrap_Loading_Guide.md)   | Proper initialization, loading screens, and async scene loading       |
| [VR Development Guide](Docs/VR_Development_Guide.md)           | VR fundamentals, custom hands, arenas, and Quest 2 tips               |
| [Combat TODO](Docs/Combat_TODO.md)                             | Combat system improvements and optimization checklist                 |

### 📖 Resources
| 📄 Document                                    | 📝 Description                                                        |
|------------------------------------------------|-----------------------------------------------------------------------|
| [Tutorials](Docs/Tutorials.md)                 | Tutorials used to learn VR in Unity                                   |
| [References](Docs/References.md)               | Links to content used in project                                      |

---