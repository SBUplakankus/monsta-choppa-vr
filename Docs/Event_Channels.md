# 📡 Event Channel System

This document describes the **ScriptableObject-based Event Channel system** used to decouple systems in the Unity VR project.  
Event Channels allow gameplay systems to communicate **without direct references**, supporting modular, reactive design.

---

## 🎯 Purpose

The Event Channel system:

- Decouples sender and receiver logic
- Enables global event broadcasting
- Supports typed and untyped events
- Integrates seamlessly with VR interactions and databases

This system is especially useful in VR where multiple systems may need to react to the same controller input or game state change.

---

## 🧠 Core Concepts

### ✨ Void Events
- Events with no parameters
- Implemented via `VoidEventChannel`
- Example use: Player grabbed an object, button pressed

### 🔢 Typed Events
- Events with a single payload (generic type `T`)
- Base class: `TypeEventChannelBase<T>`
- Examples: `FloatEventChannel`, `IntEventChannel`, `StringEventChannel`
- Type safety ensures systems only subscribe to compatible event types

### 🌍 Static Access
- Event channels are assigned during bootstrap to `GameEvents` static references
- This allows any system to raise or listen without scene dependencies

---

## 🧱 System Components

### 🧩 `VoidEventChannel`
- Holds `event Action Handlers` internally
- Methods:
    - `Raise()` — triggers all subscribed handlers
    - `Subscribe(Action handler)` — add a listener
    - `Unsubscribe(Action handler)` — remove a listener
- Automatically clears handlers on `OnDisable`

### 🧩 `TypeEventChannelBase<T>`
- Generic base for typed events
- Holds `event Action<T> Handlers` internally
- Methods:
    - `Raise(T value)` — triggers all subscribed handlers with a value
    - `Subscribe(Action<T> handler)` — add listener
    - `Unsubscribe(Action<T> handler)` — remove listener
- Automatically clears handlers on `OnDisable`

### 📄 Example Event Channel
- `FloatEventChannel` inherits `TypeEventChannelBase<float>`
- Other typed channels follow the same pattern: `IntEventChannel`, `StringEventChannel`

---

## 🔁 Data Flow

1. A system or user input triggers an event (e.g., player takes damage)
2. The corresponding Event Channel is raised via `Raise()`
3. All subscribed listeners receive the event
4. Each listener reacts independently (UI update, sound, gameplay logic)

**Flow Overview:**

- Sender system raises event
- Event Channel ScriptableObject
- Subscribed listeners
- Reaction to event

**Example:**
- Player presses “Jump”
- `VoidEventChannel` is raised
- Listener systems: play audio, animate character, update UI

---

## 🚀 Initialization & Bootstrap

All Event Channels are assigned during application startup via the `GameBootstrap` MonoBehaviour.

- Player events (e.g., `OnPlayerDamaged`)
- Audio events (e.g., `OnMusicRequested`, `OnSfxRequested`)

**Static Access:**

`GameEvents` class holds static references:

    public static class GameEvents
    {
        #region Player Events
        public static IntEventChannel OnPlayerDamaged;
        #endregion

        #region Audio Events
        public static StringEventChannel OnMusicRequested;
        public static StringEventChannel OnSfxRequested;
        #endregion
    }

During `GameBootstrap.Awake()`, serialized event channel assets are assigned to the static references.

---

## 🧪 Example Usage

### Subscribe to an event:

    private void OnEnable()
        GameEvents.OnPlayerDamaged.Subscribe(HandlePlayerDamage);

    private void OnDisable()
        GameEvents.OnPlayerDamaged.Unsubscribe(HandlePlayerDamage);

    private void HandlePlayerDamage(int damage)
        // React to damage (UI, audio, gameplay)

### Raise an event:

    GameEvents.OnSfxRequested.Raise("Explosion");

---

## 📝 Design Notes

- 📌 Event Channels prevent tight coupling between systems
- 🔄 Supports multiple subscribers without requiring references
- 🧩 Generic base allows creating new typed events easily
- 🧠 Exceptions in subscriber callbacks are logged but do not break the chain

---

## ⚠️ Gotchas & Limitations

- ❗ Always unsubscribe in `OnDisable` to avoid memory leaks
- ❗ Event order is not guaranteed if multiple subscribers exist
- 🧪 Handlers should be lightweight to avoid blocking the main thread

---

## 🔮 Future Improvements

- Event channels with multiple parameters
- Editor tooling to list all subscribers
- Async or delayed event raising
- Integration with pooling systems for high-frequency events

---

