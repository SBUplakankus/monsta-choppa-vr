# PC Implementation Plan

A detailed sprint-by-sprint implementation plan for transitioning Monsta Choppa from VR to PC, with direct references to existing files and systems.

---

## Table of Contents

1. [Pre-Development Phase](#pre-development-phase)
2. [Sprint 1: Core Input & Movement](#sprint-1-core-input--movement-week-1-2)
3. [Sprint 2: Combat Foundation](#sprint-2-combat-foundation-week-3-4)
4. [Sprint 3: UI Transition](#sprint-3-ui-transition-week-5-6)
5. [Sprint 4: Game Flow & Polish](#sprint-4-game-flow--polish-week-7-8)
6. [Sprint 5: Content & Expansion](#sprint-5-content--expansion-week-9-10)
7. [Sprint 6: Testing & Release](#sprint-6-testing--release-week-11-12)
8. [File Reference Matrix](#file-reference-matrix)
9. [Definition of Done](#definition-of-done)

---

## Pre-Development Phase

### Week 0: Setup (2-3 days)

| Task | Priority | Estimate | Notes |
|:-----|:---------|:---------|:------|
| Create PC development branch | Critical | 1h | Branch from main |
| Remove XR packages from manifest | Critical | 2h | See [Packages to Remove](#packages-to-remove) |
| Update Project Settings | Critical | 2h | Platform switch, input, rendering |
| Create PC-specific Quality Settings | High | 2h | New quality tiers |
| Update `.gitignore` for PC builds | Low | 30m | Exclude PC build folders |

### Packages to Remove

Update `Packages/manifest.json`:

```json
// Remove these packages:
"com.unity.xr.interaction.toolkit": "x.x.x",
"com.unity.xr.management": "x.x.x", 
"com.unity.xr.openxr": "x.x.x",
"com.unity.xr.hands": "x.x.x"
```

### Packages to Add

```json
// Add these packages:
"com.unity.cinemachine": "2.9.7",
"com.unity.inputsystem": "1.7.0"  // Likely already present
```

### Files to Delete

| Path | Reason |
|:-----|:-------|
| `Assets/XR/` | XR loader settings |
| `Assets/XRI/` | XR Interaction settings |
| `Assets/Scripts/Player/PlayerHapticFeedback.cs` | VR haptics |
| `Assets/Scripts/Player/WristProximityDetector.cs` | VR wrist UI |
| `Assets/Scripts/Systems/Core/SpaceWarpCameraExtension.cs` | Quest-specific |
| `Assets/Scripts/Systems/Core/RefreshRateController.cs` | Quest-specific |
| `Assets/Scripts/UI/Views/VignetteView.cs` | VR comfort |
| `Assets/Scripts/UI/Hosts/VignetteHost.cs` | VR comfort |
| `Assets/Scripts/UI/Controllers/CombatVignetteController.cs` | VR comfort |

---

## Sprint 1: Core Input & Movement (Week 1-2)

**Goal:** Player can move around Hub and Arena scenes with keyboard/mouse and gamepad.

### 1.1 Input System Setup

| Task | Priority | Estimate | Output Files |
|:-----|:---------|:---------|:-------------|
| Create PC Input Action Asset | Critical | 4h | `Assets/Settings/Input/PCInputActions.inputasset` |
| Create input action maps (Player, UI, Weapons) | Critical | 4h | Same as above |
| Create PCInputHandler component | Critical | 6h | `Assets/Scripts/Player/PCInputHandler.cs` |
| Add gamepad bindings | High | 2h | In Input Action Asset |
| Test input across devices | High | 2h | Manual testing |

**Input Action Map Structure:**

```
PCInputActions/
├── Player/
│   ├── Move (Vector2) - WASD, Left Stick
│   ├── Look (Vector2) - Mouse Delta, Right Stick
│   ├── Jump (Button) - Space, A
│   ├── Sprint (Button) - Shift, Left Stick Click
│   ├── Dodge (Button) - Ctrl, B
│   └── Interact (Button) - E, X
├── Combat/
│   ├── PrimaryAttack (Button) - LMB, Right Trigger
│   ├── SecondaryAttack (Button) - RMB, Left Trigger  
│   ├── Block (Button) - RMB Hold, Left Bumper
│   ├── WeaponSlot1-4 (Button) - 1-4, D-Pad
│   └── WeaponWheel (Button) - Tab, Left Bumper Hold
└── UI/
    ├── Navigate (Vector2) - Arrow Keys, D-Pad
    ├── Submit (Button) - Enter, A
    ├── Cancel (Button) - Escape, B
    └── Pause (Button) - Escape, Start
```

**New File:** `Assets/Scripts/Player/PCInputHandler.cs`

```csharp
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PCInputHandler : MonoBehaviour
    {
        public static PCInputHandler Instance { get; private set; }
        
        private PlayerInput _playerInput;
        
        // Movement
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool SprintHeld { get; private set; }
        
        // Combat
        public event Action OnPrimaryAttack;
        public event Action OnSecondaryAttack;
        public event Action OnBlockStart;
        public event Action OnBlockEnd;
        public event Action<int> OnWeaponSlotSelected;
        
        // UI
        public event Action OnPausePressed;
        public event Action OnInteractPressed;
        
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            
            _playerInput = GetComponent<PlayerInput>();
            SetupCallbacks();
        }
        
        private void SetupCallbacks()
        {
            var playerMap = _playerInput.actions.FindActionMap("Player");
            playerMap.FindAction("Move").performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            playerMap.FindAction("Move").canceled += _ => MoveInput = Vector2.zero;
            playerMap.FindAction("Look").performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            playerMap.FindAction("Look").canceled += _ => LookInput = Vector2.zero;
            playerMap.FindAction("Sprint").performed += _ => SprintHeld = true;
            playerMap.FindAction("Sprint").canceled += _ => SprintHeld = false;
            
            var combatMap = _playerInput.actions.FindActionMap("Combat");
            combatMap.FindAction("PrimaryAttack").performed += _ => OnPrimaryAttack?.Invoke();
            combatMap.FindAction("Block").performed += _ => OnBlockStart?.Invoke();
            combatMap.FindAction("Block").canceled += _ => OnBlockEnd?.Invoke();
            
            var uiMap = _playerInput.actions.FindActionMap("UI");
            uiMap.FindAction("Pause").performed += _ => OnPausePressed?.Invoke();
        }
        
        public void EnablePlayerInput() => _playerInput.SwitchCurrentActionMap("Player");
        public void EnableUIInput() => _playerInput.SwitchCurrentActionMap("UI");
    }
}
```

### 1.2 Player Controller

| Task | Priority | Estimate | Related Files |
|:-----|:---------|:---------|:--------------|
| Create FPSPlayerController | Critical | 12h | `Assets/Scripts/Player/FPSPlayerController.cs` |
| Add CharacterController setup | Critical | 2h | Same file |
| Implement movement (walk, run, jump) | Critical | 4h | Same file |
| Implement mouse look | Critical | 3h | Same file |
| Add ground check and gravity | High | 2h | Same file |
| Create player prefab | Critical | 2h | `Assets/Prefabs/Player/PCPlayer.prefab` |

**Modify:** `Assets/Scripts/Player/PlayerAttributes.cs`
- Remove VR-specific attributes
- Keep health, stamina, gold, experience

**Reference Existing:** 
- [`Assets/Scripts/Player/PlayerArenaController.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Player/PlayerArenaController.cs) - Keep arena integration
- [`Assets/Scripts/Player/CombatFeedbackController.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Player/CombatFeedbackController.cs) - Adapt for screen effects

**New File:** `Assets/Scripts/Player/FPSPlayerController.cs`

```csharp
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSPlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -15f;
        
        [Header("Look")]
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxVerticalAngle = 85f;
        
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        
        private CharacterController _controller;
        private PCInputHandler _input;
        private Vector3 _velocity;
        private float _verticalRotation;
        private bool _isGrounded;
        
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void Start()
        {
            _input = PCInputHandler.Instance;
        }
        
        private void Update()
        {
            GroundCheck();
            HandleMovement();
            HandleLook();
            ApplyGravity();
        }
        
        private void GroundCheck()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f;
        }
        
        private void HandleMovement()
        {
            var input = _input.MoveInput;
            var speed = _input.SprintHeld ? runSpeed : walkSpeed;
            
            var move = transform.right * input.x + transform.forward * input.y;
            _controller.Move(move * speed * Time.deltaTime);
        }
        
        private void HandleLook()
        {
            var lookInput = _input.LookInput;
            
            transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
            
            _verticalRotation -= lookInput.y * mouseSensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -maxVerticalAngle, maxVerticalAngle);
            cameraHolder.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
        }
        
        private void ApplyGravity()
        {
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
        
        public void Jump()
        {
            if (_isGrounded)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
```

### 1.3 Camera System

| Task | Priority | Estimate | Output Files |
|:-----|:---------|:---------|:-------------|
| Setup Cinemachine | High | 2h | Scene setup |
| Create FPS virtual camera | High | 2h | `Assets/Prefabs/Cameras/FPSCamera.prefab` |
| Configure camera collision | Medium | 2h | Cinemachine settings |
| Add camera shake system | Medium | 3h | `Assets/Scripts/Player/CameraShakeController.cs` |
| Create FOV adjustment | Low | 1h | Settings integration |

**New File:** `Assets/Scripts/Player/CameraShakeController.cs`

```csharp
using Cinemachine;
using UnityEngine;

namespace Player
{
    public class CameraShakeController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        
        private CinemachineBasicMultiChannelPerlin _noise;
        private float _shakeTimer;
        
        private void Awake()
        {
            _noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        
        public void Shake(float intensity, float duration)
        {
            _noise.m_AmplitudeGain = intensity;
            _shakeTimer = duration;
        }
        
        private void Update()
        {
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0)
                    _noise.m_AmplitudeGain = 0;
            }
        }
    }
}
```

### 1.4 Update Existing Systems

| Task | Priority | Estimate | Files to Modify |
|:-----|:---------|:---------|:----------------|
| Update XRComponentController → PCComponentController | High | 2h | Rename and refactor |
| Remove XR references from PlayerArenaController | High | 2h | `Assets/Scripts/Player/PlayerArenaController.cs` |
| Update BoostrapManager for PC | High | 2h | `Assets/Scripts/Systems/Core/BoostrapManager.cs` |

### Sprint 1 Deliverables

- [ ] Player moves with WASD/gamepad
- [ ] Mouse/stick look works smoothly
- [ ] Jump and sprint functional
- [ ] Hub scene navigable
- [ ] Arena scene navigable

---

## Sprint 2: Combat Foundation (Week 3-4)

**Goal:** Player can attack enemies and enemies die. Core combat loop functional.

### 2.1 Weapon System Refactor

| Task | Priority | Estimate | Related Files |
|:-----|:---------|:---------|:--------------|
| Create PCWeaponBase | Critical | 8h | `Assets/Scripts/Weapons/PCWeaponBase.cs` |
| Create PCMeleeWeapon | Critical | 8h | `Assets/Scripts/Weapons/PCMeleeWeapon.cs` |
| Create PCRangedWeapon | High | 6h | `Assets/Scripts/Weapons/PCRangedWeapon.cs` |
| Create PCShieldWeapon | High | 4h | `Assets/Scripts/Weapons/PCShieldWeapon.cs` |
| Migrate WeaponData compatibility | High | 4h | Modify `Assets/Scripts/Data/Weapons/WeaponData.cs` |

**Keep:** The existing weapon data architecture works perfectly:
- [`Assets/Scripts/Data/Weapons/WeaponData.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Data/Weapons/WeaponData.cs)
- [`Assets/Scripts/Databases/WeaponDatabase.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Databases/WeaponDatabase.cs)

**Replace:**
- `XRWeaponBase.cs` → `PCWeaponBase.cs`
- `MeleeXRWeapon.cs` → `PCMeleeWeapon.cs`
- `BowXRWeapon.cs` → `PCRangedWeapon.cs`
- `ShieldXRWeapon.cs` → `PCShieldWeapon.cs`

**Modify:** `Assets/Scripts/Data/Weapons/WeaponData.cs`

```csharp
// Remove VR-specific fields:
// [SerializeField] private Vector3 gripPositionOffset;
// [SerializeField] private Vector3 gripRotationOffset;

// Add PC-specific fields:
[Header("PC Combat Settings")]
[SerializeField] private AnimationClip[] attackAnimations;
[SerializeField] private float attackRange = 2.5f;
[SerializeField] private float attackAngle = 90f;
```

### 2.2 Targeting System

| Task | Priority | Estimate | Output Files |
|:-----|:---------|:---------|:-------------|
| Create TargetingSystem | Critical | 8h | `Assets/Scripts/Player/TargetingSystem.cs` |
| Create Crosshair UI | Critical | 4h | `Assets/UI/Components/Crosshair.uxml` |
| Add target highlighting | Medium | 4h | `Assets/Scripts/Visual Effects/TargetHighlight.cs` |
| Optional: Lock-on system | Low | 6h | `Assets/Scripts/Player/LockOnSystem.cs` |

**New File:** `Assets/Scripts/Player/TargetingSystem.cs`

```csharp
using System.Collections.Generic;
using Characters.Enemies;
using UnityEngine;

namespace Player
{
    public class TargetingSystem : MonoBehaviour
    {
        [SerializeField] private float maxRange = 20f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float softLockAngle = 15f;
        
        private Camera _mainCamera;
        private Transform _currentTarget;
        
        public Transform CurrentTarget => _currentTarget;
        public bool HasTarget => _currentTarget != null;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
        }
        
        private void Update()
        {
            UpdateTarget();
        }
        
        private void UpdateTarget()
        {
            // Primary: Raycast from crosshair
            var ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out var hit, maxRange, targetLayers))
            {
                _currentTarget = hit.transform;
                return;
            }
            
            // Fallback: Soft-lock to nearest enemy in cone
            _currentTarget = FindNearestInCone();
        }
        
        private Transform FindNearestInCone()
        {
            var enemies = FindObjectsOfType<EnemyController>();
            Transform nearest = null;
            float nearestAngle = softLockAngle;
            
            foreach (var enemy in enemies)
            {
                var dir = (enemy.transform.position - _mainCamera.transform.position).normalized;
                var angle = Vector3.Angle(_mainCamera.transform.forward, dir);
                
                if (angle < nearestAngle)
                {
                    nearestAngle = angle;
                    nearest = enemy.transform;
                }
            }
            
            return nearest;
        }
        
        public Vector3 GetAimPoint()
        {
            if (_currentTarget != null)
                return _currentTarget.position + Vector3.up;
            
            var ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            return ray.GetPoint(maxRange);
        }
    }
}
```

### 2.3 Combat Manager

| Task | Priority | Estimate | Output Files |
|:-----|:---------|:---------|:-------------|
| Create PCCombatManager | Critical | 6h | `Assets/Scripts/Player/PCCombatManager.cs` |
| Integrate with input system | Critical | 2h | Links to PCInputHandler |
| Add combo system foundation | Medium | 4h | In PCCombatManager |
| Add attack cooldowns | High | 2h | In PCCombatManager |

**New File:** `Assets/Scripts/Player/PCCombatManager.cs`

```csharp
using Events.Registries;
using UnityEngine;
using Weapons;

namespace Player
{
    public class PCCombatManager : MonoBehaviour
    {
        [SerializeField] private PCWeaponBase[] equippedWeapons;
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private Animator playerAnimator;
        
        private PCInputHandler _input;
        private PCWeaponBase _currentWeapon;
        private int _currentWeaponIndex;
        private bool _isBlocking;
        
        public PCWeaponBase CurrentWeapon => _currentWeapon;
        public bool IsBlocking => _isBlocking;
        
        private void Start()
        {
            _input = PCInputHandler.Instance;
            
            _input.OnPrimaryAttack += HandlePrimaryAttack;
            _input.OnSecondaryAttack += HandleSecondaryAttack;
            _input.OnBlockStart += () => _isBlocking = true;
            _input.OnBlockEnd += () => _isBlocking = false;
            _input.OnWeaponSlotSelected += SwitchWeapon;
            
            if (equippedWeapons.Length > 0)
                EquipWeapon(0);
        }
        
        private void HandlePrimaryAttack()
        {
            if (_currentWeapon == null || !_currentWeapon.CanAttack) return;
            
            _currentWeapon.PrimaryAttack();
            playerAnimator.SetTrigger("Attack");
        }
        
        private void HandleSecondaryAttack()
        {
            _currentWeapon?.SecondaryAttack();
        }
        
        private void SwitchWeapon(int index)
        {
            if (index < 0 || index >= equippedWeapons.Length) return;
            EquipWeapon(index);
        }
        
        private void EquipWeapon(int index)
        {
            if (_currentWeapon != null)
                _currentWeapon.gameObject.SetActive(false);
            
            _currentWeaponIndex = index;
            _currentWeapon = equippedWeapons[index];
            _currentWeapon.gameObject.SetActive(true);
        }
    }
}
```

### 2.4 Enemy System Verification

**Keep Unchanged** - These systems work perfectly for PC:
- [`Assets/Scripts/Characters/Enemies/EnemyController.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Characters/Enemies/EnemyController.cs)
- [`Assets/Scripts/Characters/Enemies/EnemyHealth.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Characters/Enemies/EnemyHealth.cs)
- [`Assets/Scripts/Characters/Enemies/EnemyMovement.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Characters/Enemies/EnemyMovement.cs)
- [`Assets/Scripts/Characters/Enemies/EnemyAnimator.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Characters/Enemies/EnemyAnimator.cs)
- [`Assets/Scripts/Characters/Enemies/EnemyAttack.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Characters/Enemies/EnemyAttack.cs)

| Task | Priority | Estimate | Files |
|:-----|:---------|:---------|:------|
| Update enemy target finding (remove XR rig reference) | High | 2h | `EnemyMovement.cs` |
| Increase max enemy count | Medium | 1h | `Assets/Scripts/Systems/Arena/WaveSpawner.cs` |
| Test enemy combat loop | Critical | 4h | Manual testing |

### Sprint 2 Deliverables

- [ ] Player can attack with primary weapon
- [ ] Enemies take damage and die
- [ ] Weapon switching works
- [ ] Blocking functional
- [ ] Combat effects (VFX, SFX) trigger correctly
- [ ] Pooling still functions for combat entities

---

## Sprint 3: UI Transition (Week 5-6)

**Goal:** All UI converted to screen-space and fully functional with mouse/keyboard/gamepad.

### 3.1 Update UI Toolkit Styles

| Task | Priority | Estimate | Files to Modify |
|:-----|:---------|:---------|:----------------|
| Update base styles for screen-space | Critical | 4h | `Assets/UI/Styles/*.uss` |
| Reduce font sizes from VR | Critical | 2h | All USS files |
| Update button sizes | High | 2h | `Assets/Scripts/Constants/UIToolkitStyles.cs` |
| Add hover/focus states | High | 4h | USS files |

**Modify:** `Assets/Scripts/Constants/UIToolkitStyles.cs`

```csharp
// Update default values for PC
public const int DefaultFontSize = 14;      // Was 18-24 for VR
public const int HeaderFontSize = 24;       // Was 32-48 for VR
public const int ButtonHeight = 40;         // Was 64+ for VR
public const int ButtonWidth = 200;         // Was 300+ for VR
```

### 3.2 Convert Existing Panels

| Panel | Priority | Estimate | View File | Host File |
|:------|:---------|:---------|:----------|:----------|
| Start Menu | Critical | 4h | `StartMenuPanelView.cs` | `StartMenuPanelHost.cs` |
| Settings | Critical | 4h | `SettingsPanelView.cs` | `SettingsPanelHost.cs` |
| Audio Settings | High | 2h | `AudioSettingsPanelView.cs` | `AudioSettingsPanelHost.cs` |
| Video Settings | High | 4h | `VideoSettingsPanelView.cs` | `VideoSettingsPanelHost.cs` |
| Pause Menu | Critical | 4h | `PauseMenuView.cs` | `PauseMenuHost.cs` |
| Loading Screen | High | 2h | `LoadingScreenView.cs` | `LoadingScreenHost.cs` |
| Arena Intro | Medium | 2h | `ArenaIntroView.cs` | `ArenaIntroHost.cs` |

**Keep:** The Factory-View-Host architecture remains unchanged:
- [`Assets/Scripts/UI/Views/BasePanelView.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/UI/Views/BasePanelView.cs)
- [`Assets/Scripts/UI/Hosts/BasePanelHost.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/UI/Hosts/BasePanelHost.cs)
- [`Assets/Scripts/Factories/UIToolkitFactory.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Factories/UIToolkitFactory.cs)

### 3.3 Create New PC UI Elements

| Element | Priority | Estimate | Output Files |
|:--------|:---------|:---------|:-------------|
| Crosshair | Critical | 4h | `Assets/UI/Components/Crosshair.uxml` |
| HUD Layout | Critical | 8h | `Assets/UI/Screens/HUD.uxml` |
| Health Bar (Screen) | Critical | 4h | `Assets/Scripts/UI/Game/PlayerHealthHUD.cs` |
| Hotbar | High | 6h | `Assets/Scripts/UI/Game/WeaponHotbar.cs` |
| Damage Indicators | Medium | 4h | `Assets/Scripts/UI/Game/DamageIndicator.cs` |
| Results Screen | High | 6h | `Assets/Scripts/UI/Views/ArenaResultsView.cs` |

### 3.4 Update Settings Panel

| Setting | Priority | Type | Notes |
|:--------|:---------|:-----|:------|
| Mouse Sensitivity | Critical | Slider | New for PC |
| Invert Y-Axis | High | Toggle | New for PC |
| Field of View | High | Slider | New for PC (60-120) |
| Graphics Quality | High | Dropdown | Expanded options |
| Resolution | High | Dropdown | New for PC |
| Fullscreen Mode | High | Dropdown | New for PC |
| VSync | Medium | Toggle | New for PC |
| Frame Rate Limit | Medium | Dropdown | New for PC |

**Modify:** `Assets/Scripts/UI/Views/VideoSettingsPanelView.cs`

Add new PC-specific settings and remove VR comfort options.

### 3.5 Pause System Integration

| Task | Priority | Estimate | Related Files |
|:-----|:---------|:---------|:--------------|
| Connect pause input to GameFlowManager | Critical | 2h | `GameFlowManager.cs` |
| Update PauseMenuController | Critical | 4h | `PauseMenuController.cs` |
| Add cursor lock/unlock | High | 2h | In controller |
| Test pause/resume cycle | Critical | 2h | Manual testing |

**Modify:** [`Assets/Scripts/UI/Controllers/PauseMenuController.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/UI/Controllers/PauseMenuController.cs)

```csharp
// Add cursor management
private void ShowPause()
{
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    _pauseHost.Show();
}

private void HidePause()
{
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    _pauseHost.Hide();
}
```

### Sprint 3 Deliverables

- [ ] All menus work with mouse
- [ ] All menus work with gamepad
- [ ] Settings save and load correctly
- [ ] HUD displays player stats
- [ ] Crosshair visible during gameplay
- [ ] Pause menu fully functional

---

## Sprint 4: Game Flow & Polish (Week 7-8)

**Goal:** Complete game loop from start menu to arena victory/defeat and back.

### 4.1 Game Flow Manager Updates

| Task | Priority | Estimate | Files to Modify |
|:-----|:---------|:---------|:----------------|
| Remove VR-specific state handling | High | 2h | `GameFlowManager.cs` |
| Add PC-specific transitions | High | 4h | Same file |
| Test full state machine | Critical | 4h | Manual testing |

**Existing file:** [`Assets/Scripts/Systems/Core/GameFlowManager.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Systems/Core/GameFlowManager.cs)

The state machine is already well-designed:
- `StartMenu` → `Loading` → `Hub` → `Arena` → `Victory/Defeat` → `Hub`

### 4.2 Arena System Polish

| Task | Priority | Estimate | Files |
|:-----|:---------|:---------|:------|
| Update WaveManager for more enemies | High | 2h | `WaveManager.cs` |
| Update WaveSpawner limits | High | 2h | `WaveSpawner.cs` |
| Create ArenaResultsView | High | 6h | New file |
| Test full arena loop | Critical | 4h | Manual testing |

**Modify:** [`Assets/Scripts/Systems/Arena/WaveSpawner.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Systems/Arena/WaveSpawner.cs)

```csharp
// Update for PC:
[SerializeField] private int maxEnemies = 15;  // Was 6-8 for VR
```

### 4.3 Combat Polish

| Task | Priority | Estimate | Output Files |
|:-----|:---------|:---------|:-------------|
| Add screen damage effects | High | 4h | Modify `CombatFeedbackController.cs` |
| Add hit marker UI | Medium | 4h | `Assets/Scripts/UI/Game/HitMarker.cs` |
| Add kill feed/notifications | Low | 4h | `Assets/Scripts/UI/Game/KillNotification.cs` |
| Camera shake on hit | High | 2h | Use `CameraShakeController.cs` |
| Screen flash on damage | High | 2h | Post-processing |

### 4.4 Audio Adjustments

| Task | Priority | Estimate | Notes |
|:-----|:---------|:---------|:------|
| Update spatial audio for single camera | Medium | 2h | AudioSource settings |
| Add music system | Medium | 4h | If not present |
| Adjust volume defaults | Low | 1h | Settings |

**Keep:** Audio system works unchanged:
- [`Assets/Scripts/Events/Registries/AudioEvents.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Events/Registries/AudioEvents.cs)
- [`Assets/Scripts/Databases/WorldAudioDatabase.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Databases/WorldAudioDatabase.cs)

### Sprint 4 Deliverables

- [ ] Full game loop works (menu → hub → arena → results → hub)
- [ ] Victory and defeat screens functional
- [ ] Combat feels responsive with feedback
- [ ] Audio plays correctly
- [ ] Save/load works through full loop

---

## Sprint 5: Content & Expansion (Week 9-10)

**Goal:** Take advantage of PC performance for expanded content.

### 5.1 Increased Enemy Counts

| Task | Priority | Estimate | Files |
|:-----|:---------|:---------|:------|
| Update GameConstants for PC | High | 1h | `GameConstants.cs` |
| Create larger wave configurations | High | 4h | Wave data assets |
| Test performance with 15-20 enemies | Critical | 4h | Profiling |

**Modify:** [`Assets/Scripts/Constants/GameConstants.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Constants/GameConstants.cs)

```csharp
#region PC Performance Budgets

public const int MaxActiveEnemies = 20;          // Was 6-8
public const int MaxActiveVFX = 50;              // Was 15-20
public const int MaxAudioSources = 32;           // Was 16
public const float EnemyUpdateInterval = 0.05f;  // Was 0.2-0.4s

#endregion
```

### 5.2 Enhanced Graphics

| Task | Priority | Estimate | Notes |
|:-----|:---------|:---------|:------|
| Add post-processing volume | High | 4h | URP post-processing |
| Enable real-time shadows | High | 2h | Quality settings |
| Add bloom and color grading | Medium | 2h | Post-processing |
| Add ambient occlusion | Medium | 2h | Post-processing |
| Quality presets (Low/Med/High/Ultra) | High | 4h | Quality settings |

### 5.3 Additional Combat Features

| Task | Priority | Estimate | Output Files |
|:-----|:---------|:---------|:-------------|
| Add dodge/roll mechanic | High | 8h | In `FPSPlayerController.cs` |
| Add combo system | Medium | 8h | In `PCCombatManager.cs` |
| Add finisher moves | Low | 6h | New animation events |
| Add stamina system | Medium | 4h | `Assets/Scripts/Player/StaminaSystem.cs` |

### 5.4 Update Manager Optimization

| Task | Priority | Estimate | Files |
|:-----|:---------|:---------|:------|
| Relax update intervals | Medium | 2h | `GameUpdateManager.cs` |
| Profile and optimize | High | 4h | Various |

**Modify:** [`Assets/Scripts/Systems/Core/GameUpdateManager.cs`](https://github.com/SBUplakankus/monsta-choppa-vr/blob/main/Assets/Scripts/Systems/Core/GameUpdateManager.cs)

```csharp
// PC can handle faster updates:
private const float MediumPriorityInterval = 0.1f;  // Was 0.2f
private const float LowPriorityInterval = 0.2f;     // Was 0.4f
```

### Sprint 5 Deliverables

- [ ] 15-20 enemies run at 60 FPS
- [ ] Post-processing enabled
- [ ] Quality settings functional
- [ ] Dodge/roll works
- [ ] Stamina system works

---

## Sprint 6: Testing & Release (Week 11-12)

**Goal:** Bug-free, polished PC release.

### 6.1 Full Testing Pass

| Task | Priority | Estimate | Notes |
|:-----|:---------|:---------|:------|
| Complete playthrough (3x) | Critical | 12h | Start to finish |
| Input testing (KB/M + Gamepad) | Critical | 4h | All input methods |
| Edge case testing | High | 8h | Edge cases |
| Performance profiling | Critical | 4h | 60 FPS validation |
| Memory profiling | High | 4h | No leaks |

### 6.2 Bug Fixing

| Task | Priority | Estimate | Notes |
|:-----|:---------|:---------|:------|
| Fix critical bugs | Critical | 16h | Must fix |
| Fix high priority bugs | High | 8h | Should fix |
| Fix medium bugs | Medium | 4h | Nice to fix |

### 6.3 Build & Package

| Task | Priority | Estimate | Output |
|:-----|:---------|:---------|:-------|
| Create Windows build | Critical | 4h | Standalone .exe |
| Create installer (optional) | Low | 4h | Setup wizard |
| Test on multiple machines | High | 4h | Compatibility |
| Create release notes | Medium | 2h | Documentation |

### Sprint 6 Deliverables

- [ ] No critical bugs
- [ ] Stable 60 FPS on target hardware
- [ ] Windows build works
- [ ] Release ready

---

## File Reference Matrix

### Files to Create

| New File | Purpose | Sprint |
|:---------|:--------|:-------|
| `Assets/Scripts/Player/PCInputHandler.cs` | Input management | 1 |
| `Assets/Scripts/Player/FPSPlayerController.cs` | Character movement | 1 |
| `Assets/Scripts/Player/CameraShakeController.cs` | Camera effects | 1 |
| `Assets/Scripts/Player/TargetingSystem.cs` | Target acquisition | 2 |
| `Assets/Scripts/Player/PCCombatManager.cs` | Combat coordination | 2 |
| `Assets/Scripts/Weapons/PCWeaponBase.cs` | Weapon base class | 2 |
| `Assets/Scripts/Weapons/PCMeleeWeapon.cs` | Melee weapons | 2 |
| `Assets/Scripts/Weapons/PCRangedWeapon.cs` | Ranged weapons | 2 |
| `Assets/Scripts/UI/Game/PlayerHealthHUD.cs` | Health display | 3 |
| `Assets/Scripts/UI/Game/WeaponHotbar.cs` | Weapon slots | 3 |
| `Assets/Settings/Input/PCInputActions.inputasset` | Input bindings | 1 |

### Files to Modify

| Existing File | Changes | Sprint |
|:--------------|:--------|:-------|
| `GameConstants.cs` | PC performance values | 1, 5 |
| `GameFlowManager.cs` | Remove VR handling | 4 |
| `WaveSpawner.cs` | Increase limits | 4 |
| `GameUpdateManager.cs` | Faster intervals | 5 |
| `WeaponData.cs` | Remove VR fields | 2 |
| `EnemyMovement.cs` | Update target finding | 2 |
| `UIToolkitStyles.cs` | PC sizing | 3 |
| `VideoSettingsPanelView.cs` | PC settings | 3 |
| `PauseMenuController.cs` | Cursor handling | 3 |

### Files to Delete

| File | Reason | Sprint |
|:-----|:-------|:-------|
| `PlayerHapticFeedback.cs` | VR-specific | 0 |
| `WristProximityDetector.cs` | VR-specific | 0 |
| `SpaceWarpCameraExtension.cs` | Quest-specific | 0 |
| `RefreshRateController.cs` | Quest-specific | 0 |
| `VignetteView.cs` | VR comfort | 0 |
| `VignetteHost.cs` | VR comfort | 0 |
| `CombatVignetteController.cs` | VR comfort | 0 |
| `XRWeaponBase.cs` | Replaced | 2 |
| `MeleeXRWeapon.cs` | Replaced | 2 |
| `BowXRWeapon.cs` | Replaced | 2 |
| `ShieldXRWeapon.cs` | Replaced | 2 |
| `ThrowableXRWeapon.cs` | Replaced | 2 |

### Files to Keep Unchanged

| File | Reason |
|:-----|:-------|
| `DatabaseBase.cs` | Generic pattern works |
| `EventChannel.cs` | Event system works |
| `GameDatabases.cs` | Static access works |
| `GameplayEvents.cs` | Events work |
| `GamePoolManager.cs` | Pooling still useful |
| `EnemyController.cs` | Enemy system works |
| `EnemyHealth.cs` | Damage system works |
| `EnemyMovement.cs` | NavMesh works |
| `EnemyAnimator.cs` | Animation works |
| `WaveManager.cs` | Wave system works |
| `ArenaStateManager.cs` | State machine works |
| `SaveFileManagerBase.cs` | Save system works |
| `PlayerSaveFileManager.cs` | Player saves work |
| `UIToolkitFactory.cs` | UI creation works |
| `BasePanelView.cs` | View pattern works |
| `BasePanelHost.cs` | Host pattern works |

---

## Definition of Done

### Per-Task Criteria

- [ ] Code compiles without errors
- [ ] Feature works as specified
- [ ] No regression in existing features
- [ ] Performance within budget (60 FPS)
- [ ] Memory stable (no leaks)
- [ ] Input works with keyboard/mouse
- [ ] Input works with gamepad (where applicable)

### Per-Sprint Criteria

- [ ] All Critical tasks complete
- [ ] All High tasks complete or deferred with justification
- [ ] Sprint deliverables achieved
- [ ] Build runs stable
- [ ] Documentation updated

### Release Criteria

- [ ] Full game loop playable
- [ ] No critical bugs
- [ ] Performance stable at 60 FPS
- [ ] All input methods work
- [ ] Settings save/load correctly
- [ ] Build runs on clean Windows install
