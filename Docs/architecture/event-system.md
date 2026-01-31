# Event System

ScriptableObject-based publish/subscribe system for decoupled communication between game systems.

---

## Overview

Event channels allow systems to communicate without direct references. A sender raises an event, and any number of subscribers receive it.

```
Sender  →  Event Channel (ScriptableObject)  →  Subscribers
```

---

## Base Classes

### TypeEventChannelBase<T\>

Generic base class for typed events.

```csharp
public abstract class TypeEventChannelBase<T> : ScriptableObject
{
    private event Action<T> _handlers;
    
    public void Raise(T value)
    {
        _handlers?.Invoke(value);
    }
    
    public void Subscribe(Action<T> handler)
    {
        _handlers += handler;
    }
    
    public void Unsubscribe(Action<T> handler)
    {
        _handlers -= handler;
    }
    
    private void OnDisable()
    {
        _handlers = null;
    }
}
```

### VoidEventChannel

For events with no payload.

```csharp
public class VoidEventChannel : ScriptableObject
{
    private event Action _handlers;
    
    public void Raise() => _handlers?.Invoke();
    public void Subscribe(Action handler) => _handlers += handler;
    public void Unsubscribe(Action handler) => _handlers -= handler;
}
```

---

## Event Channel Types

| Type | Payload | Example Usage |
|:-----|:--------|:--------------|
| VoidEventChannel | None | Pause requested, game over trigger |
| IntEventChannel | int | Damage dealt, gold changed, experience gained |
| FloatEventChannel | float | Volume changed, timer tick |
| StringEventChannel | string | Audio clip request, message display |
| EnemyEventChannel | EnemyController | Enemy spawned, enemy despawned |
| ArenaStateEventChannel | ArenaState | Arena state transitions |
| LocaleEventChannel | Locale | Language changed |

---

## GameEvents Registry

Static class providing global access to all event channels. Assigned during bootstrap.

```csharp
public static class GameEvents
{
    // Player Events
    public static IntEventChannel OnPlayerDamaged;
    public static IntEventChannel OnGoldChanged;
    public static IntEventChannel OnExperienceGained;
    public static IntEventChannel OnLevelChanged;
    
    // Audio Events
    public static StringEventChannel OnMusicRequested;
    public static StringEventChannel OnSfxRequested;
    
    // Game Events
    public static EnemyEventChannel OnEnemySpawned;
    public static EnemyEventChannel OnEnemyDespawned;
    public static ArenaStateEventChannel OnArenaStateChanged;
    public static VoidEventChannel OnPauseRequested;
    public static VoidEventChannel OnGameOverSequenceRequested;
    public static VoidEventChannel OnGameWonSequenceRequested;
}
```

---

## Usage Examples

### Subscribing to Events

```csharp
public class GoldDisplay : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnGoldChanged.Subscribe(UpdateDisplay);
    }
    
    private void OnDisable()
    {
        GameEvents.OnGoldChanged.Unsubscribe(UpdateDisplay);
    }
    
    private void UpdateDisplay(int newGold)
    {
        goldLabel.text = newGold.ToString();
    }
}
```

### Raising Events

```csharp
public class EnemyHealth : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            GameEvents.OnEnemyDespawned.Raise(controller);
        }
    }
}
```

### Requesting Audio

```csharp
GameEvents.OnSfxRequested.Raise("sword_hit");
GameEvents.OnMusicRequested.Raise("combat_theme");
```

---

## Rules

| Rule | Reason |
|:-----|:-------|
| Always unsubscribe in OnDisable | Prevents memory leaks and null reference errors |
| Keep handlers lightweight | Long handlers block other subscribers |
| Use null-conditional invoke | Handlers may be null if no subscribers |
| Match subscribe/unsubscribe methods | Using different method references causes leaks |

---

## Correct Pattern: Method Reference

```csharp
// Correct - same method reference for subscribe and unsubscribe
private void OnEnable() => GameEvents.OnGoldChanged.Subscribe(HandleGoldChanged);
private void OnDisable() => GameEvents.OnGoldChanged.Unsubscribe(HandleGoldChanged);
private void HandleGoldChanged(int gold) { /* ... */ }
```

### Incorrect Pattern (Memory Leak)

```csharp
// Lambda creates new delegate each time - cannot unsubscribe
private void OnEnable() => GameEvents.OnGoldChanged.Subscribe(g => UpdateUI(g));
// This will NOT work - different delegate instance
```
