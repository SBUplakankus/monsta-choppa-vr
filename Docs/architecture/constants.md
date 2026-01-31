# Constants System

Centralized string keys and configuration values to eliminate magic strings and improve maintainability.

> **Source**: [`Assets/Scripts/Constants/`](../../Assets/Scripts/Constants/)

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

> **Source**: [`GameConstants.cs`](../../Assets/Scripts/Constants/GameConstants.cs)

```csharp
public static class GameConstants
{
    // Scene Names
    public const string Bootstrapper = "Bootstrapper";
    public const string StartMenu = "StartMenu";
    public const string Hub = "Hub";
    public const string GoblinCampDay = "GoblinCampDay";
    
    // Save Keys
    public const string MetaProgressionKey = "MetaProgression";
    public const string AudioSettingsKey = "AudioSettings";
    public const string VideoSettingsKey = "VideoSettings";
    
    // Attribute Keys
    public const string PlayerGoldKey = "PlayerGold";
    public const string PlayerExperienceKey = "PlayerExperience";
    public const string PlayerLevelKey = "PlayerLevel";
    
    // Combat Settings
    public const float DefaultMeleeAttackRange = 2.0f;
    public const float MinSwingVelocity = 1.0f;
    public const float MaxSwingVelocity = 5.0f;
    public const float MinVelocityDamageMultiplier = 0.5f;
    public const float MaxVelocityDamageMultiplier = 2.0f;
    public const float InvincibilityDuration = 0.2f;
    
    // Arena Properties
    public const int PreludeDuration = 8;
    public const int WaveIntermissionDuration = 6;
    public const int BossIntermissionDuration = 6;
}
```

### AudioKeys

Audio mixer groups and clip identifiers.

> **Source**: [`AudioKeys.cs`](../../Assets/Scripts/Constants/AudioKeys.cs)

```csharp
public static class AudioKeys
{
    // Mixer Groups
    public const string MixerMaster = "Master";
    public const string MixerMusic = "Music";
    public const string MixerSfx = "SFX";
    public const string MixerUI = "UI";
    public const string MixerAmbience = "Ambience";
    
    // UI Sounds
    public const string ButtonClick = "button_click";
    public const string ButtonEnter = "button_enter";
    public const string ButtonExit = "button_exit";
    public const string Pop = "pop";
    
    // Game Music
    public const string GameIntroKey = "Game_Intro_Music";
    public const string GameWonKey = "Game_Won_Music";
    public const string GameOverKey = "Game_Over_Music";
}
```
```

### LocalizationKeys

UI text localization keys.

> **Source**: [`LocalizationKeys.cs`](../../Assets/Scripts/Constants/LocalizationKeys.cs)

```csharp
public static class LocalizationKeys
{
    public const string MainTable = "MainGame";
    
    // Loading
    public const string Initializing = "initializing";
    public const string LoadingComplete = "loading-complete";
    
    // Main Menu
    public const string Play = "play";
    public const string Settings = "settings";
    public const string Quit = "quit";
    public const string Resume = "resume";
    
    // Settings
    public const string AudioSettings = "audio-settings";
    public const string VideoSettings = "video-settings";
    public const string LanguageSettings = "language-settings";
    
    // Audio
    public const string Master = "master";
    public const string Music = "music";
    public const string SFX = "sfx";
    public const string Ambience = "ambience";
    
    // Stats
    public const string Level = "level";
    public const string Experience = "experience";
    public const string Gold = "gold";
}
```

### UIToolkitStyles

USS class names for UI Toolkit styling.

> **Source**: [`UIToolkitStyles.cs`](../../Assets/Scripts/Constants/UIToolkitStyles.cs)

```csharp
public static class UIToolkitStyles
{
    // Containers
    public const string Container = "container";
    public const string PanelBody = "panel-body";
    public const string PanelHeader = "panel-header";
    public const string PanelContent = "panel-content";
    
    // Buttons
    public const string MenuButton = "menu-button";
    public const string ContinueButton = "continue-button";
    
    // Health Bars
    public const string HealthBarContainer = "health-bar-container";
    public const string HealthBarBackground = "health-bar-background";
    public const string HealthBarFill = "health-bar-fill";
    
    // Loading Screen
    public const string LoadingScreenContainer = "loading-screen-container";
    public const string LoadingBarContainer = "loading-bar-container";
    public const string LoadingBarFill = "loading-bar-fill";
    
    // Settings
    public const string SettingsSlider = "settings-slider";
    public const string SettingsDropdown = "settings-dropdown";
    
    // Tabs
    public const string TabBar = "tab-bar";
    public const string Tab = "tab";
    public const string TabContent = "tab-content";
}
```
```

---

## Usage

### Scene Loading

```csharp
SceneManager.LoadScene(GameConstants.Hub);
```

### Save Data

```csharp
SaveFile.AddOrUpdateData(GameConstants.MetaProgressionKey, metaProgressionData);
var data = SaveFile.GetData<MetaProgressionData>(GameConstants.MetaProgressionKey);
```

### Audio

```csharp
AudioEvents.SfxRequested.Raise(AudioKeys.ButtonClick);
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
