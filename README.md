# 📘 Project Documentation

**A lot of this is AI Generated at the moment for placeholder info while I develop the game**

This folder contains high-level technical documentation for the Unity VR project.  
The goal of these documents is to explain **how systems are structured**, **why architectural decisions were made**, and **how different systems communicate**.

This project is built on top of Unity’s **XR Interaction Toolkit–based VR Template**, with custom systems layered on top to support **modular**, **event-driven**, and **data-driven** gameplay.

---

## 🕶️ Unity VR Template Overview

The project uses Unity’s **VR Template** as a foundation, which provides:

- 🎮 XR Interaction Toolkit setup  
- 🧠 Action-based input using the Input System  
- 🚶 Locomotion (teleport & snap turn)  
- ✋ Basic interactables (grab, poke, UI interaction)  
- 🎯 XR Origin and controller prefabs  

All custom systems documented here are designed to:

- 🔌 Remain **decoupled** from specific XR prefabs  
- 🧩 Extend (not replace) default VR Template behavior  
- 📡 Trigger gameplay logic through **event channels**, not direct references  

---

## 🗂️ Documentation Index

| 📄 Document                                    | 📝 Description                                                        |
|------------------------------------------------|-----------------------------------------------------------------------|
| [Architecture Overview](Docs/Architecture.md)  | High-level architectural philosophy and system relationships          |
| [Event Channel System](Docs/Event_Channels.md) | ScriptableObject-based event system for decoupled communication       |
| [Data & Database System](Docs/Databases.md)    | ScriptableObject databases for items, configuration, and runtime data |
| [VR Systems](Docs/VR_Systems.md)               | How XR interactions hook into gameplay systems                        |
| [UI Factory](Docs/Factories.md)                | Factory class for creating elements through the UI Toolkit in C#      |
| [Weapons](Docs/Weapons.md)                     | How the weapons system is structured                                  |
| [Enemies](Docs/Enemies.md)                     | How the enemy system is structured                                    |
| [Localisation](Docs/Localisation.md)           | How the localisation is handled in game                               |
| [Save Data](Docs/Save_Data.md)                 | How player save data is handled                                       |
| [Constants](Docs/Constants.md)                 | How Global Constant variables are used for type safety                |
| [Tutorials](Docs/Tutorials.md)                 | Tutorials used to learn VR in Unity                                   |
| [References](Docs/References.md)               | Links to content used in project                                      |

---

## 🛠️ How to Use These Docs

- 📌 Each major system should have its own document  
- 🧠 Docs focus on **intent, flow, and design decisions**  
- 🔍 Code references may change, architecture should not  

When adding new systems, create a new document and link it here.

---

## 🚧 Status

These documents are a **living reference** and will evolve as the project grows.  
Sections may be incomplete until systems stabilize and are fully validated.
