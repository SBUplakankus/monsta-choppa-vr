# Constants System

Centralized string keys and configuration values to eliminate magic strings and improve maintainability.

---

## Overview

All string literals used for lookups, save keys, and style classes are defined as constants. This provides compile-time checking and a single source of truth for values used across the codebase.

| Benefit | Explanation |
|:--------|:------------|
| Compile-time checking | Typos caught at build time, not runtime |
| Refactoring safety | Change value in one place |
| Autocomplete support | IDE suggests available constants |
| Self-documenting | Clear naming indicates purpose |

---

## Constant Categories

### GameConstants

Core game configuration and save data keys.

```csharp
public static class GameConstants
{
    // Scene Names
    public const string Bootstrapper = "Bootstrapper";
    public const string StartMenu = "StartMenu";
    public const string Hub = "Hub";
    
    // Save Keys
    public const string PlayerGoldKey = "PlayerGold";
    public const string PlayerExperienceKey = "PlayerExperience";
    public const string PlayerLevelKey = "PlayerLevel";
    public const string MetaProgressionKey = "MetaProgression";
    
    // Attribute Keys
    public const string HealthAttribute = "Health";
    public const string StaminaAttribute = "Stamina";
}
```

### AudioKeys

Audio mixer groups and clip identifiers.

```csharp
public static class AudioKeys
{
    // Mixer Groups
    public const string MixerMaster = "Master";
    public const string MixerMusic = "Music";
    public const string MixerSfx = "SFX";
    public const string MixerAmbience = "Ambience";
    
    // Common Clips
    public const string ButtonClick = "button_click";
    public const string ButtonHover = "button_hover";
    public const string MenuOpen = "menu_open";
    public const string MenuClose = "menu_close";
}
```

### LocalizationKeys

UI text localization keys.

```csharp
public static class LocalizationKeys
{
    // Main Menu
    public const string Play = "play";
    public const string Settings = "settings";
    public const string Quit = "quit";
    public const string Resume = "resume";
    
    // Settings
    public const string Audio = "audio";
    public const string Video = "video";
    public const string Controls = "controls";
    public const string Language = "language";
    
    // Stats
    public const string Level = "level";
    public const string Experience = "experience";
    public const string Gold = "gold";
    public const string Health = "health";
}
```

### UIToolkitStyles

USS class names for UI Toolkit styling.

```csharp
public static class UIToolkitStyles
{
    // Containers
    public const string Container = "container";
    public const string PanelBody = "panel-body";
    public const string PanelHeader = "panel-header";
    
    // Components
    public const string MenuButton = "menu-button";
    public const string SettingsSlider = "settings-slider";
    public const string Tab = "tab";
    public const string TabActive = "tab-active";
    
    // Game UI
    public const string HealthBar = "health-bar";
    public const string HealthBarFill = "health-bar-fill";
    public const string StatRow = "stat-row";
    public const string StatValue = "stat-value";
}
```

---

## Usage

### Scene Loading

```csharp
SceneManager.LoadScene(GameConstants.Hub);
```

### Save Data

```csharp
saveFile.AddOrUpdateData(GameConstants.PlayerGoldKey, goldValue);
var gold = saveFile.GetData<int>(GameConstants.PlayerGoldKey);
```

### Audio

```csharp
GameEvents.OnSfxRequested.Raise(AudioKeys.ButtonClick);
audioMixer.SetFloat(AudioKeys.MixerMaster, volume);
```

### UI Styling

```csharp
var button = UIToolkitFactory.CreateButton(
    LocalizationKeys.Play,
    HandlePlay,
    UIToolkitStyles.MenuButton
);
```

---

## Adding New Constants

1. Identify the category (Game, Audio, Localization, UI)
2. Add to appropriate static class
3. Use descriptive naming: `CategoryAction` or `CategoryItem`
4. Replace all magic string usages

```csharp
public static class GameConstants
{
    // Existing keys...
    public const string PlayerManaKey = "PlayerMana";  // New key
}

// Usage
saveFile.AddOrUpdateData(GameConstants.PlayerManaKey, manaValue);
```
