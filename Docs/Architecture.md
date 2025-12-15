# 🏗️ Project Architecture Overview

This document describes the **high-level architecture** of the Unity VR project.  
It explains how systems are structured, how data flows, and the interaction patterns between components.

The project is built on **Unity’s XR Interaction Toolkit VR Template** with **custom modular systems** for gameplay, audio, UI, and database management.

---

## 🎯 Goals

- 🔌 **Decoupled Systems** – Systems communicate via Event Channels, not direct references
- 📦 **Data-Driven Design** – Databases store all configurable content (audio, sprites, fonts)
- 🕹️ **VR Ready** – VR interactions trigger gameplay events without hard dependencies
- 🧩 **Extensible** – Adding new systems should be simple and low-friction

---

## 🧠 Core Layers

| Layer | Purpose | Examples |
|-------|---------|---------|
| **Gameplay Layer** | Implements core game mechanics and interactions | Player, Enemies, Gameplay rules |
| **Data Layer** | Stores game data in ScriptableObject databases | AudioClipDatabase, SpriteDatabase, TMPFontDatabase |
| **Event Layer** | Provides decoupled communication | VoidEventChannel, TypeEventChannelBase<T>, GameEvents |
| **VR Template Layer** | Handles XR input, locomotion, and interactables | XR Origin, XR Controllers, Teleportation, Interactable Prefabs |
| **Presentation Layer** | UI, Audio, Visual Feedback | Canvas UI, Sound playback, Particle effects |

---

## 🔁 System Interactions

**Data Flow:**

- VR Controller triggers input (grab, press, teleport)
- Input is converted into events via Event Channels
- Gameplay systems subscribe to events and react accordingly
- Systems may query Databases for assets or configuration

**Flow Overview (ASCII Diagram):**

Flow Overview:

<pre>
        [VR Input / XR Controllers]
                   |
                   v
            [Event Channels]
           /       |       \
  [Gameplay]  [Audio]  [UI / Feedback]
           \       |       /
           [Databases / Assets]
</pre>

Notes:
- Event Channels decouple senders from receivers
- Databases provide centralized content access
- All layers remain scene-agnostic and modular

---

## 🧱 Key Components

### 1️⃣ Databases
- Generic `DatabaseBase<T>` holds entries and builds lookup dictionaries
- Types: AudioClipDatabase, TMPFontDatabase, SpriteDatabase
- Accessible via static `GameDatabases` references

### 2️⃣ Event System
- ScriptableObject-based channels
- Void events (`VoidEventChannel`) for triggers without payload
- Typed events (`TypeEventChannelBase<T>`) for events with data
- Static `GameEvents` provide global access

### 3️⃣ VR Interaction
- Uses Unity XR Template prefabs: XR Origin, Controllers, Locomotion
- Interactions raise events instead of directly calling systems
- Supports grab, poke, teleport, snap-turn, and UI input

### 4️⃣ Gameplay Systems
- Subscribe to Event Channels for player actions, AI events, and environment triggers
- Query Databases for configuration and asset references

### 5️⃣ Presentation / Feedback
- Listens to events to play sounds, update UI, or spawn visual effects
- Remains decoupled from core gameplay logic

---

## 🔄 Initialization & Bootstrap

- `GameBootstrap` MonoBehaviour initializes:
    - Event Channels → static `GameEvents`
    - Databases → static `GameDatabases`
- Ensures systems are ready before any gameplay logic runs

**Example Startup Flow:**

Example Startup Flow:

<pre>
      [Scene Loads]
             |
             v
    GameBootstrap.Awake()
        /           \
 Assign Databases    Assign Event Channels
     |                    |
     v                    v
 Gameplay & VR Systems Ready
</pre>

---

## 📝 Design Notes

- 🎯 **Event-Driven Architecture** reduces coupling
- 📦 **ScriptableObjects** centralize configuration and content
- 🧩 **Layered Approach** allows swapping systems (VR template, database, audio) without rewriting core logic
- 🔄 **Scene-Agnostic Systems** simplify prototyping and testing

---

## ⚠️ Best Practices

- Always unsubscribe from events in `OnDisable` to prevent leaks
- Keep databases read-only during runtime
- Avoid long-running logic in event handlers to maintain VR performance
- Keep UI and audio feedback in the presentation layer

---

## 🔮 Future Improvements

- Addressables integration for large content sets
- More generic multi-parameter Event Channels
- Enhanced VR interaction modularity (new locomotion or input schemes)
- Editor validation for database and event consistency

---

## 📚 References

- XR Interaction Toolkit VR Template
- ScriptableObject-based event and database patterns
- Unity’s recommended data-driven architecture best practices

