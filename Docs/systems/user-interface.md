# User Interface System

The UI system uses Unity's UI Toolkit with a Factory-View-Host-Controller architecture for clean separation of concerns and memory safety.

---

## Architecture

```
Controller (game logic)
    │
    ▼
Host (MonoBehaviour lifecycle, animations)
    │
    ▼
View (UI element creation, layout)
    │
    ▼
Factory (element builders, styling)
```

---

## Components

### UIToolkitFactory

Static factory class that creates pre-configured UI Toolkit elements with consistent styling and localization.

| Method | Returns | Purpose |
|:-------|:--------|:--------|
| CreateElement<T\>() | T | Generic element with class names |
| CreateContainer() | VisualElement | Styled container |
| CreateButton() | Button | Localized button with click handler |
| CreateLabel() | Label | Localized label |
| CreateBoundLabel() | Label | Data-bound label (auto-updates) |
| CreateSlider() | Slider | Range input with callback |
| CreateToggle() | Toggle | Boolean input |
| CreateDropdown() | DropdownField | Selection input |
| CreateHealthBar() | HealthBarElements | Container, background, fill |

**Fluent Extensions:**

```csharp
var panel = UIToolkitFactory.CreateContainer()
    .WithClasses(UIToolkitStyles.PanelBody)
    .WithPadding(20)
    .WithMargin(10)
    .WithFlexGrow(1);
```

---

### BasePanelView

Abstract base class for all UI views. Views define visual structure only.

```csharp
public abstract class BasePanelView : IDisposable
{
    public VisualElement Container { get; protected set; }
    
    protected abstract void GenerateUI(VisualElement root);
    
    public virtual void Dispose()
    {
        Container?.Clear();
        Container?.RemoveFromHierarchy();
        Container = null;
    }
}
```

**View Responsibilities:**

- Create UI element hierarchy using Factory
- Expose elements as properties for Host binding
- Expose events for user interactions
- Implement Dispose for cleanup

**View Restrictions:**

- No game logic
- No external event subscriptions
- No references to game systems

---

### BasePanelHost

MonoBehaviour that manages View lifecycle and animations.

```csharp
public abstract class BasePanelHost : MonoBehaviour
{
    [SerializeField] protected UIDocument uiDocument;
    [SerializeField] protected StyleSheet styleSheet;
    
    protected VisualElement ContentRoot;
    private ITweenable[] _tweenables;
    
    public abstract void Generate();
    
    public virtual void Show()
    {
        foreach (var tween in _tweenables)
            tween.Show();
    }
    
    public virtual void Hide()
    {
        foreach (var tween in _tweenables)
            tween.Hide();
    }
    
    protected virtual void Dispose()
    {
        // Override to cleanup View
    }
    
    private void OnDisable() => Dispose();
}
```

**Host Responsibilities:**

- Create and destroy Views
- Subscribe to View events
- Forward events to Controllers
- Manage show/hide animations via ITweenable

---

### Controllers

MonoBehaviours that handle game logic in response to UI events.

```csharp
public class StartMenuController : MonoBehaviour
{
    [SerializeField] private StartMenuPanelHost menuHost;
    [SerializeField] private SettingsPanelHost settingsHost;
    
    private void OnEnable()
    {
        menuHost.Generate();
        menuHost.OnPlayClicked += HandlePlay;
        menuHost.OnSettingsClicked += HandleSettings;
    }
    
    private void OnDisable()
    {
        menuHost.OnPlayClicked -= HandlePlay;
        menuHost.OnSettingsClicked -= HandleSettings;
    }
    
    private void HandlePlay()
    {
        SceneManager.LoadScene(GameConstants.Hub);
    }
    
    private void HandleSettings()
    {
        settingsHost.Generate();
        settingsHost.Show();
    }
}
```

---

## Data Binding

### Automatic Binding

Use CreateBoundLabel for labels that auto-update when data changes.

```csharp
var goldLabel = UIToolkitFactory.CreateBoundLabel(
    playerGold,
    nameof(playerGold.Value),
    UIToolkitStyles.StatValue
);
```

### Manual Binding

For complex formatting or conditional display.

```csharp
private void BindData()
{
    playerGold.OnValueChanged += UpdateGoldDisplay;
    UpdateGoldDisplay(playerGold.Value);
}

private void UnbindData()
{
    playerGold.OnValueChanged -= UpdateGoldDisplay;
}

private void UpdateGoldDisplay(int value)
{
    goldLabel.text = value >= 1000 ? $"{value/1000f:F1}K" : value.ToString();
}
```

---

## Event Handling Pattern

Store callback references for proper unsubscription.

```csharp
public class AudioSettingsPanelHost : BasePanelHost
{
    private Action _unbindAll;
    
    public void BindViewSliders(AudioSettingsPanelView view)
    {
        _unbindAll = () => { };
        _unbindAll += BindSlider(view.MasterVolume, masterVolume);
        _unbindAll += BindSlider(view.MusicVolume, musicVolume);
    }
    
    private static Action BindSlider(Slider slider, FloatAttribute attribute)
    {
        EventCallback<ChangeEvent<float>> callback = e => attribute.Value = e.newValue;
        slider.RegisterValueChangedCallback(callback);
        
        return () => slider.UnregisterValueChangedCallback(callback);
    }
    
    protected override void Dispose()
    {
        _unbindAll?.Invoke();
        _unbindAll = null;
        base.Dispose();
    }
}
```

---

## Animation System

Hosts use ITweenable components for show/hide animations.

```csharp
public class TweenTransform : MonoBehaviour, ITweenable
{
    [SerializeField] private float displayScale = 1f;
    [SerializeField] private float displayStartScale = 0.75f;
    [SerializeField] private float duration = 0.25f;
    
    public void Show()
    {
        transform.localScale = Vector3.one * displayStartScale;
        // Animate to displayScale with OutCubic easing
    }
    
    public void Hide()
    {
        // Animate to zero with InCubic easing
    }
}
```

---

## Existing Panels

| Panel | View | Host | Status |
|:------|:-----|:-----|:-------|
| Start Menu | StartMenuPanelView | StartMenuPanelHost | Complete |
| Settings | SettingsPanelView | SettingsPanelHost | Complete |
| Audio Settings | AudioSettingsPanelView | AudioSettingsPanelHost | Complete |
| Video Settings | VideoSettingsPanelView | VideoSettingsPanelHost | Complete |
| Loading Screen | LoadingScreenView | LoadingScreenHost | Complete |
| Arena Intro | ArenaIntroView | ArenaIntroHost | Complete |
| Boss Intro | BossIntroView | BossIntroHost | Complete |
| Enemy Health Bar | - | EnemyHealthBar | Complete |

---

## Memory Safety Rules

| Rule | Implementation |
|:-----|:---------------|
| Always dispose Views | Call Dispose in Host.OnDisable |
| Unsubscribe all events | Use _unbindAll pattern or explicit unsubscribe |
| No lambda event handlers | Store callback references for unsubscription |
| Null check in callbacks | View may be disposed when callback fires |
| Dispose before regenerate | Call Dispose before creating new View |
