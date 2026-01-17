# 🥽 VR Development Guide for Unity

A comprehensive guide for VR development targeting Quest 2, written for self-taught developers.

---

## Table of Contents
1. [VR Performance Fundamentals](#vr-performance-fundamentals)
2. [Common VR Pitfalls](#common-vr-pitfalls)
3. [VR Comfort & Safety](#vr-comfort--safety)
4. [Custom Hand Setup](#custom-hand-setup)
5. [Custom Interaction Rays](#custom-interaction-rays)
6. [Modular Arena Architecture](#modular-arena-architecture)
7. [Testing & Debugging](#testing--debugging)
8. [Quest 2 Specific Tips](#quest-2-specific-tips)

---

## VR Performance Fundamentals

### Frame Rate Targets

| Platform | Minimum FPS | Target FPS | Notes |
|----------|-------------|------------|-------|
| Quest 2 | 72 | 90 | 120Hz experimental |
| Quest 3 | 72 | 90-120 | Better GPU headroom |
| PCVR | 80 | 90+ | Depends on headset |

**Why it matters:** Dropping below minimum FPS causes:
- Motion sickness
- Visual judder
- Tracking issues
- Broken immersion

### Frame Budget

At 72 FPS, you have **13.9ms per frame**. Budget allocation:
```
Rendering:     8-9ms   (60-65%)
Physics:       1-2ms   (10-15%)
Scripts:       2-3ms   (15-20%)
Audio:         0.5ms   (3-5%)
Overhead:      1-2ms   (buffer)
```

### VR-Specific Optimizations

#### 1. Single Pass Instanced Rendering
```
Edit → Project Settings → XR Plug-in Management → Oculus → Stereo Rendering Mode → Single Pass Instanced
```
This renders both eyes in one draw call. **Always use this.**

#### 2. Foveated Rendering (Quest)
```csharp
using UnityEngine.XR;

// Enable fixed foveated rendering
OculusLoader.SetFoveatedRenderingLevel(3); // 0-4, higher = more aggressive
```

#### 3. Application SpaceWarp (ASW)
Renders at half frame rate and interpolates. Good for maintaining smoothness during frame drops.

### Draw Call Budget

| Quality Level | Max Draw Calls | Max Triangles |
|---------------|----------------|---------------|
| Quest 2 Low | 50-100 | 100k-200k |
| Quest 2 Medium | 100-150 | 200k-400k |
| Quest 2 High | 150-200 | 400k-750k |

**Tips to reduce draw calls:**
- Use GPU instancing for repeated objects
- Combine meshes with same material
- Use texture atlases
- Aggressive LOD groups

---

## Common VR Pitfalls

### ❌ Things That Break VR

#### 1. Moving the Camera Programmatically
```csharp
// ❌ NEVER DO THIS - causes instant motion sickness
Camera.main.transform.position = newPosition;

// ✅ Move the XR Rig instead
xrRig.transform.position = newPosition;
```

#### 2. Changing FOV
```csharp
// ❌ NEVER DO THIS
Camera.main.fieldOfView = 90f;
// VR cameras have fixed FOV based on headset optics
```

#### 3. Disabling Head Tracking
```csharp
// ❌ NEVER DO THIS
xrCamera.enabled = false;
// Player loses all spatial awareness
```

#### 4. Frame Rate Dependent Code
```csharp
// ❌ BAD - speed varies with frame rate
transform.position += Vector3.forward * 0.1f;

// ✅ GOOD - consistent speed regardless of FPS
transform.position += Vector3.forward * speed * Time.deltaTime;
```

#### 5. Instantiate() in Gameplay
```csharp
// ❌ BAD - causes frame spikes
var enemy = Instantiate(enemyPrefab);

// ✅ GOOD - use object pooling
var enemy = enemyPool.Get();
```

### ⚠️ Common Performance Killers

| Issue | Impact | Solution |
|-------|--------|----------|
| Real-time shadows | High | Baked lighting, blob shadows |
| Reflection probes | High | Bake, reduce resolution |
| Post-processing | High | Minimal, VR-safe effects only |
| Physics.Raycast every frame | Medium | Cache results, use intervals |
| GetComponent() in Update | Medium | Cache references in Awake |
| String operations | Medium | Use StringBuilder, avoid in Update |
| LINQ in hot paths | Medium | Use for loops instead |
| Garbage allocation | High | Pool everything, avoid new in Update |

### 🔍 Profiling Checklist

1. **Unity Profiler** - CPU/GPU timing
2. **Frame Debugger** - Draw call analysis  
3. **Memory Profiler** - Allocation tracking
4. **OVR Metrics Tool** - Quest-specific metrics
5. **RenderDoc** - GPU debugging

---

## VR Comfort & Safety

### Locomotion Guidelines

#### Teleportation (Most Comfortable)
```csharp
// Fade to black during teleport
public async void Teleport(Vector3 destination)
{
    await FadeToBlack(0.15f);
    xrRig.transform.position = destination;
    await FadeFromBlack(0.15f);
}
```

#### Smooth Locomotion (Moderate)
- Add vignette during movement
- Keep speed consistent (2-4 m/s)
- Allow snap turning option

```csharp
[Header("Comfort Options")]
[SerializeField] private bool useVignette = true;
[SerializeField] private bool snapTurning = true;
[SerializeField] private float snapAngle = 45f;
```

#### Things to Avoid
- Acceleration/deceleration
- Head-bob effects
- Rotating the view programmatically
- Elevators/platforms moving player
- Forced camera animations

### Comfort Settings (Always Provide)

```csharp
[System.Serializable]
public class ComfortSettings
{
    public LocomotionType locomotion = LocomotionType.Teleport;
    public TurningType turning = TurningType.Snap;
    public float snapAngle = 45f;
    public bool movementVignette = true;
    public float vignetteIntensity = 0.5f;
    public bool reducedEffects = false;
    public bool seatedMode = false;
}
```

### Height Calibration

```csharp
public class HeightCalibration : MonoBehaviour
{
    [SerializeField] private Transform cameraOffset;
    [SerializeField] private float defaultHeight = 1.7f;
    
    public void CalibrateHeight()
    {
        var currentHeight = Camera.main.transform.localPosition.y;
        var offset = defaultHeight - currentHeight;
        cameraOffset.localPosition = new Vector3(0, offset, 0);
    }
    
    public void SetSeatedMode(bool seated)
    {
        cameraOffset.localPosition = seated 
            ? new Vector3(0, 0.5f, 0) 
            : Vector3.zero;
    }
}
```

---

## Custom Hand Setup

### Overview

Replace Unity's default XR hands with custom hand models and behaviors.

### Hand Data Structure

```csharp
using UnityEngine;

namespace Player.Hands
{
    /// <summary>
    /// Data container for hand configuration.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Player/Hand Data")]
    public class HandData : ScriptableObject
    {
        [Header("Identity")]
        public HandType handType;
        public GameObject handModelPrefab;
        
        [Header("Interaction")]
        public float grabRadius = 0.1f;
        public LayerMask grabLayers;
        public LayerMask uiLayers;
        
        [Header("Haptics")]
        public float hoverHapticStrength = 0.1f;
        public float grabHapticStrength = 0.3f;
        public float hitHapticStrength = 0.5f;
        
        [Header("Visuals")]
        public Material defaultMaterial;
        public Material hoverMaterial;
        public Material grabMaterial;
    }
    
    public enum HandType
    {
        Left,
        Right
    }
}
```

### Custom Hand Controller

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Player.Hands
{
    /// <summary>
    /// Custom hand controller with grip detection, pointing, and haptics.
    /// Replaces default XR Direct Interactor for more control.
    /// </summary>
    public class CustomHandController : MonoBehaviour
    {
        #region Fields

        [Header("Configuration")]
        [SerializeField] private HandData handData;
        [SerializeField] private Transform handModelRoot;
        
        [Header("Input Actions")]
        [SerializeField] private InputActionProperty gripAction;
        [SerializeField] private InputActionProperty triggerAction;
        [SerializeField] private InputActionProperty primaryButtonAction;
        [SerializeField] private InputActionProperty secondaryButtonAction;
        [SerializeField] private InputActionProperty thumbstickAction;
        
        [Header("Components")]
        [SerializeField] private XRBaseInteractor directInteractor;
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private Animator handAnimator;
        
        // State
        private bool _isGripping;
        private bool _isPointing;
        private float _gripValue;
        private float _triggerValue;
        
        // Cached
        private XRBaseController _xrController;
        private GameObject _currentHandModel;
        
        // Animation hashes
        private static readonly int GripHash = Animator.StringToHash("Grip");
        private static readonly int TriggerHash = Animator.StringToHash("Trigger");
        private static readonly int PointHash = Animator.StringToHash("Point");
        private static readonly int ThumbsUpHash = Animator.StringToHash("ThumbsUp");

        #endregion

        #region Properties

        public bool IsGripping => _isGripping;
        public bool IsPointing => _isPointing;
        public float GripValue => _gripValue;
        public float TriggerValue => _triggerValue;
        public HandType HandType => handData.handType;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _xrController = GetComponent<XRBaseController>();
            SetupHandModel();
        }

        private void OnEnable()
        {
            gripAction.action.Enable();
            triggerAction.action.Enable();
            primaryButtonAction.action?.Enable();
            secondaryButtonAction.action?.Enable();
            thumbstickAction.action?.Enable();
        }

        private void OnDisable()
        {
            gripAction.action.Disable();
            triggerAction.action.Disable();
            primaryButtonAction.action?.Disable();
            secondaryButtonAction.action?.Disable();
            thumbstickAction.action?.Disable();
        }

        private void Update()
        {
            UpdateInputValues();
            UpdateHandAnimation();
            UpdateInteractionMode();
        }

        #endregion

        #region Setup

        private void SetupHandModel()
        {
            if (handData.handModelPrefab == null) return;
            
            // Destroy existing model
            if (_currentHandModel != null)
                Destroy(_currentHandModel);
            
            // Instantiate custom hand model
            _currentHandModel = Instantiate(handData.handModelPrefab, handModelRoot);
            _currentHandModel.transform.localPosition = Vector3.zero;
            _currentHandModel.transform.localRotation = Quaternion.identity;
            
            // Get animator from new model
            handAnimator = _currentHandModel.GetComponent<Animator>();
        }

        #endregion

        #region Input

        private void UpdateInputValues()
        {
            _gripValue = gripAction.action.ReadValue<float>();
            _triggerValue = triggerAction.action.ReadValue<float>();
            _isGripping = _gripValue > 0.5f;
        }

        private void UpdateHandAnimation()
        {
            if (handAnimator == null) return;
            
            handAnimator.SetFloat(GripHash, _gripValue);
            handAnimator.SetFloat(TriggerHash, _triggerValue);
            
            // Point gesture: trigger pressed, grip released
            _isPointing = _triggerValue > 0.5f && _gripValue < 0.3f;
            handAnimator.SetBool(PointHash, _isPointing);
            
            // Thumbs up: check if thumb is up (via capacitive touch or button state)
            var thumbUp = !primaryButtonAction.action.IsPressed() && 
                          !secondaryButtonAction.action.IsPressed();
            handAnimator.SetBool(ThumbsUpHash, thumbUp && _isGripping);
        }

        private void UpdateInteractionMode()
        {
            // Switch between direct grab and ray based on pointing
            if (directInteractor != null)
                directInteractor.enabled = !_isPointing;
            
            if (rayInteractor != null)
                rayInteractor.enabled = _isPointing;
        }

        #endregion

        #region Haptics

        /// <summary>
        /// Sends haptic feedback to this controller.
        /// </summary>
        public void SendHapticImpulse(float amplitude, float duration)
        {
            if (_xrController != null)
            {
                _xrController.SendHapticImpulse(amplitude, duration);
            }
        }

        /// <summary>
        /// Sends haptic feedback for hover.
        /// </summary>
        public void OnHover()
        {
            SendHapticImpulse(handData.hoverHapticStrength, 0.05f);
        }

        /// <summary>
        /// Sends haptic feedback for grab.
        /// </summary>
        public void OnGrab()
        {
            SendHapticImpulse(handData.grabHapticStrength, 0.1f);
        }

        /// <summary>
        /// Sends haptic feedback for weapon hit.
        /// </summary>
        public void OnWeaponHit()
        {
            SendHapticImpulse(handData.hitHapticStrength, 0.15f);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sets the hand model at runtime.
        /// </summary>
        public void SetHandModel(GameObject modelPrefab)
        {
            if (_currentHandModel != null)
                Destroy(_currentHandModel);
            
            _currentHandModel = Instantiate(modelPrefab, handModelRoot);
            handAnimator = _currentHandModel.GetComponent<Animator>();
        }

        /// <summary>
        /// Gets the thumbstick input value.
        /// </summary>
        public Vector2 GetThumbstickValue()
        {
            return thumbstickAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        /// <summary>
        /// Checks if primary button is pressed.
        /// </summary>
        public bool IsPrimaryButtonPressed()
        {
            return primaryButtonAction.action?.IsPressed() ?? false;
        }

        /// <summary>
        /// Checks if secondary button is pressed.
        /// </summary>
        public bool IsSecondaryButtonPressed()
        {
            return secondaryButtonAction.action?.IsPressed() ?? false;
        }

        #endregion
    }
}
```

### Hand Pose System

```csharp
using UnityEngine;

namespace Player.Hands
{
    /// <summary>
    /// Defines a hand pose for gripping specific objects.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Player/Hand Pose")]
    public class HandPoseData : ScriptableObject
    {
        public string poseName;
        
        [Header("Finger Curls (0-1)")]
        [Range(0, 1)] public float thumb = 0.5f;
        [Range(0, 1)] public float index = 0.8f;
        [Range(0, 1)] public float middle = 0.9f;
        [Range(0, 1)] public float ring = 0.9f;
        [Range(0, 1)] public float pinky = 0.9f;
        
        [Header("Transition")]
        public float blendSpeed = 10f;
    }

    /// <summary>
    /// Applies hand poses for object gripping.
    /// </summary>
    public class HandPoseController : MonoBehaviour
    {
        [SerializeField] private Animator handAnimator;
        [SerializeField] private HandPoseData defaultPose;
        
        private HandPoseData _currentPose;
        private HandPoseData _targetPose;
        
        private static readonly int ThumbHash = Animator.StringToHash("Thumb");
        private static readonly int IndexHash = Animator.StringToHash("Index");
        private static readonly int MiddleHash = Animator.StringToHash("Middle");
        private static readonly int RingHash = Animator.StringToHash("Ring");
        private static readonly int PinkyHash = Animator.StringToHash("Pinky");

        public void SetPose(HandPoseData pose)
        {
            _targetPose = pose ?? defaultPose;
        }

        private void Update()
        {
            if (_targetPose == null || handAnimator == null) return;
            
            var speed = _targetPose.blendSpeed * Time.deltaTime;
            
            BlendFingerValue(ThumbHash, _targetPose.thumb, speed);
            BlendFingerValue(IndexHash, _targetPose.index, speed);
            BlendFingerValue(MiddleHash, _targetPose.middle, speed);
            BlendFingerValue(RingHash, _targetPose.ring, speed);
            BlendFingerValue(PinkyHash, _targetPose.pinky, speed);
        }

        private void BlendFingerValue(int hash, float target, float speed)
        {
            var current = handAnimator.GetFloat(hash);
            handAnimator.SetFloat(hash, Mathf.MoveTowards(current, target, speed));
        }
    }
}
```

---

## Custom Interaction Rays

### Ray Interactor Data

```csharp
using UnityEngine;

namespace Player.Interaction
{
    /// <summary>
    /// Configuration for custom interaction rays.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Player/Ray Interactor Data")]
    public class RayInteractorData : ScriptableObject
    {
        [Header("Ray Settings")]
        public float maxRayDistance = 10f;
        public float rayWidth = 0.01f;
        public LayerMask interactionLayers;
        
        [Header("Curve Settings")]
        public bool useCurve = true;
        public float curveHeight = 0.5f;
        public int curveSegments = 20;
        
        [Header("Visual")]
        public Material rayMaterial;
        public Material validHitMaterial;
        public Material invalidHitMaterial;
        public Gradient rayGradient;
        
        [Header("Reticle")]
        public GameObject reticlePrefab;
        public float reticleScale = 0.05f;
        public bool scaleReticleWithDistance = true;
        
        [Header("Haptics")]
        public float hoverHapticStrength = 0.05f;
        public float selectHapticStrength = 0.2f;
    }
}
```

### Custom Ray Interactor

```csharp
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Player.Interaction
{
    /// <summary>
    /// Custom ray interactor with curved ray support and visual customization.
    /// </summary>
    public class CustomRayInteractor : MonoBehaviour
    {
        #region Fields

        [Header("Configuration")]
        [SerializeField] private RayInteractorData rayData;
        [SerializeField] private CustomHandController handController;
        
        [Header("Components")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private XRRayInteractor xrRayInteractor;
        
        private GameObject _reticle;
        private Transform _reticleTransform;
        private bool _isHovering;
        private bool _hasValidTarget;
        
        // Curved ray points
        private Vector3[] _curvePoints;
        
        // Cached
        private Transform _transform;
        private Camera _mainCamera;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _transform = transform;
            _mainCamera = Camera.main;
            
            SetupLineRenderer();
            SetupReticle();
            SetupCurvePoints();
        }

        private void OnEnable()
        {
            if (xrRayInteractor != null)
            {
                xrRayInteractor.hoverEntered.AddListener(OnHoverEnter);
                xrRayInteractor.hoverExited.AddListener(OnHoverExit);
                xrRayInteractor.selectEntered.AddListener(OnSelectEnter);
            }
        }

        private void OnDisable()
        {
            if (xrRayInteractor != null)
            {
                xrRayInteractor.hoverEntered.RemoveListener(OnHoverEnter);
                xrRayInteractor.hoverExited.RemoveListener(OnHoverExit);
                xrRayInteractor.selectEntered.RemoveListener(OnSelectEnter);
            }
        }

        private void Update()
        {
            if (!enabled || handController == null) return;
            
            // Only show ray when pointing
            var showRay = handController.IsPointing;
            lineRenderer.enabled = showRay;
            
            if (showRay)
            {
                UpdateRay();
                UpdateReticle();
            }
            else
            {
                HideReticle();
            }
        }

        #endregion

        #region Setup

        private void SetupLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            
            lineRenderer.material = rayData.rayMaterial;
            lineRenderer.startWidth = rayData.rayWidth;
            lineRenderer.endWidth = rayData.rayWidth * 0.5f;
            lineRenderer.colorGradient = rayData.rayGradient;
            lineRenderer.useWorldSpace = true;
            
            if (rayData.useCurve)
            {
                lineRenderer.positionCount = rayData.curveSegments;
            }
            else
            {
                lineRenderer.positionCount = 2;
            }
        }

        private void SetupReticle()
        {
            if (rayData.reticlePrefab == null) return;
            
            _reticle = Instantiate(rayData.reticlePrefab);
            _reticleTransform = _reticle.transform;
            _reticle.SetActive(false);
        }

        private void SetupCurvePoints()
        {
            _curvePoints = new Vector3[rayData.curveSegments];
        }

        #endregion

        #region Ray Update

        private void UpdateRay()
        {
            var origin = _transform.position;
            var direction = _transform.forward;
            
            _hasValidTarget = Physics.Raycast(
                origin, 
                direction, 
                out var hit, 
                rayData.maxRayDistance, 
                rayData.interactionLayers
            );

            if (rayData.useCurve)
            {
                UpdateCurvedRay(origin, direction, _hasValidTarget ? hit.point : origin + direction * rayData.maxRayDistance);
            }
            else
            {
                UpdateStraightRay(origin, _hasValidTarget ? hit.point : origin + direction * rayData.maxRayDistance);
            }

            // Update material based on valid target
            lineRenderer.material = _hasValidTarget ? rayData.validHitMaterial : rayData.rayMaterial;
        }

        private void UpdateStraightRay(Vector3 start, Vector3 end)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        private void UpdateCurvedRay(Vector3 start, Vector3 direction, Vector3 end)
        {
            var distance = Vector3.Distance(start, end);
            
            for (int i = 0; i < rayData.curveSegments; i++)
            {
                var t = (float)i / (rayData.curveSegments - 1);
                var point = Vector3.Lerp(start, end, t);
                
                // Add curve arc
                var curveOffset = Mathf.Sin(t * Mathf.PI) * rayData.curveHeight;
                point.y += curveOffset;
                
                _curvePoints[i] = point;
            }
            
            lineRenderer.SetPositions(_curvePoints);
        }

        #endregion

        #region Reticle

        private void UpdateReticle()
        {
            if (_reticle == null || !_hasValidTarget) 
            {
                HideReticle();
                return;
            }

            // Get hit point from XR Ray Interactor
            if (xrRayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                _reticle.SetActive(true);
                _reticleTransform.position = hit.point + hit.normal * 0.001f;
                _reticleTransform.rotation = Quaternion.LookRotation(-hit.normal);
                
                // Scale with distance if enabled
                if (rayData.scaleReticleWithDistance)
                {
                    var distance = Vector3.Distance(_transform.position, hit.point);
                    var scale = rayData.reticleScale * (1 + distance * 0.1f);
                    _reticleTransform.localScale = Vector3.one * scale;
                }
                else
                {
                    _reticleTransform.localScale = Vector3.one * rayData.reticleScale;
                }
            }
        }

        private void HideReticle()
        {
            if (_reticle != null)
                _reticle.SetActive(false);
        }

        #endregion

        #region Events

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            _isHovering = true;
            handController?.SendHapticImpulse(rayData.hoverHapticStrength, 0.05f);
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            _isHovering = false;
        }

        private void OnSelectEnter(SelectEnterEventArgs args)
        {
            handController?.SendHapticImpulse(rayData.selectHapticStrength, 0.1f);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sets the ray visual to valid/invalid state.
        /// </summary>
        public void SetValid(bool valid)
        {
            lineRenderer.material = valid ? rayData.validHitMaterial : rayData.invalidHitMaterial;
        }

        /// <summary>
        /// Gets the current hit point if valid.
        /// </summary>
        public bool TryGetHitPoint(out Vector3 point)
        {
            if (_hasValidTarget && xrRayInteractor.TryGetCurrent3DRaycastHit(out var hit))
            {
                point = hit.point;
                return true;
            }
            
            point = Vector3.zero;
            return false;
        }

        #endregion
    }
}
```

---

## Modular Arena Architecture

### Core Concept

The arena system should be completely swappable through ScriptableObjects:

```
ArenaData (What to spawn) 
    ↓
ArenaController (When/how to spawn)
    ↓
ArenaModules (Swappable behaviors)
```

### Arena Module System

```csharp
using UnityEngine;

namespace Systems.Arena.Modules
{
    /// <summary>
    /// Base class for swappable arena behavior modules.
    /// Modules can be mixed and matched to create different arena types.
    /// </summary>
    public abstract class ArenaModule : ScriptableObject
    {
        public string moduleName;
        public string description;
        
        /// <summary>
        /// Called when the arena starts.
        /// </summary>
        public abstract void OnArenaStart(ArenaContext context);
        
        /// <summary>
        /// Called each frame during arena gameplay.
        /// </summary>
        public abstract void OnArenaUpdate(ArenaContext context, float deltaTime);
        
        /// <summary>
        /// Called when the arena ends.
        /// </summary>
        public abstract void OnArenaEnd(ArenaContext context);
    }

    /// <summary>
    /// Context passed to all arena modules.
    /// </summary>
    public class ArenaContext
    {
        public ArenaController Controller;
        public EnemyManager EnemyManager;
        public WaveSpawner Spawner;
        public Transform PlayerTransform;
        public ArenaData ArenaData;
        
        // Runtime state
        public int CurrentWave;
        public int EnemiesKilled;
        public float ElapsedTime;
        public bool IsBossWave;
    }
}
```

### Example Modules

```csharp
using UnityEngine;

namespace Systems.Arena.Modules
{
    /// <summary>
    /// Standard wave-based survival module.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Arena/Modules/Wave Survival")]
    public class WaveSurvivalModule : ArenaModule
    {
        public int totalWaves = 10;
        public float intermissionTime = 15f;
        
        private int _currentWave;
        private float _intermissionTimer;
        private bool _inIntermission;

        public override void OnArenaStart(ArenaContext context)
        {
            _currentWave = 0;
            _inIntermission = false;
            StartNextWave(context);
        }

        public override void OnArenaUpdate(ArenaContext context, float deltaTime)
        {
            if (_inIntermission)
            {
                _intermissionTimer -= deltaTime;
                if (_intermissionTimer <= 0)
                {
                    StartNextWave(context);
                }
            }
            else if (context.EnemyManager.ActiveEnemiesCount == 0)
            {
                OnWaveComplete(context);
            }
        }

        public override void OnArenaEnd(ArenaContext context)
        {
            // Cleanup
        }

        private void StartNextWave(ArenaContext context)
        {
            _currentWave++;
            _inIntermission = false;
            context.CurrentWave = _currentWave;
            
            if (_currentWave <= context.ArenaData.Waves.Length)
            {
                context.Spawner.SpawnWave(context.ArenaData.Waves[_currentWave - 1]);
            }
        }

        private void OnWaveComplete(ArenaContext context)
        {
            if (_currentWave >= totalWaves)
            {
                context.Controller.OnArenaVictory();
            }
            else
            {
                _inIntermission = true;
                _intermissionTimer = intermissionTime;
            }
        }
    }

    /// <summary>
    /// Endless mode - waves increase in difficulty until player dies.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Arena/Modules/Endless")]
    public class EndlessModule : ArenaModule
    {
        public float difficultyScalePerWave = 0.1f;
        public int baseEnemiesPerWave = 5;
        public int additionalEnemiesPerWave = 2;
        
        public override void OnArenaStart(ArenaContext context)
        {
            context.CurrentWave = 0;
            SpawnNextWave(context);
        }

        public override void OnArenaUpdate(ArenaContext context, float deltaTime)
        {
            if (context.EnemyManager.ActiveEnemiesCount == 0)
            {
                SpawnNextWave(context);
            }
        }

        public override void OnArenaEnd(ArenaContext context) { }

        private void SpawnNextWave(ArenaContext context)
        {
            context.CurrentWave++;
            var enemyCount = baseEnemiesPerWave + (context.CurrentWave * additionalEnemiesPerWave);
            // Generate wave dynamically
        }
    }

    /// <summary>
    /// Time attack - survive for a set duration.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Arena/Modules/Time Attack")]
    public class TimeAttackModule : ArenaModule
    {
        public float targetTime = 180f; // 3 minutes
        public float spawnInterval = 5f;
        
        private float _spawnTimer;

        public override void OnArenaStart(ArenaContext context)
        {
            context.ElapsedTime = 0f;
            _spawnTimer = 0f;
        }

        public override void OnArenaUpdate(ArenaContext context, float deltaTime)
        {
            context.ElapsedTime += deltaTime;
            
            if (context.ElapsedTime >= targetTime)
            {
                context.Controller.OnArenaVictory();
                return;
            }
            
            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0 && context.EnemyManager.ActiveEnemiesCount < 8)
            {
                // Spawn enemies
                _spawnTimer = spawnInterval;
            }
        }

        public override void OnArenaEnd(ArenaContext context) { }
    }

    /// <summary>
    /// Boss rush - sequence of boss fights.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Arena/Modules/Boss Rush")]
    public class BossRushModule : ArenaModule
    {
        public WaveData[] bossWaves;
        public float bossIntermissionTime = 10f;

        public override void OnArenaStart(ArenaContext context)
        {
            context.CurrentWave = 0;
            context.IsBossWave = true;
            SpawnNextBoss(context);
        }

        public override void OnArenaUpdate(ArenaContext context, float deltaTime)
        {
            if (context.EnemyManager.ActiveEnemiesCount == 0)
            {
                if (context.CurrentWave >= bossWaves.Length)
                {
                    context.Controller.OnArenaVictory();
                }
                else
                {
                    SpawnNextBoss(context);
                }
            }
        }

        public override void OnArenaEnd(ArenaContext context) { }

        private void SpawnNextBoss(ArenaContext context)
        {
            context.CurrentWave++;
            if (context.CurrentWave <= bossWaves.Length)
            {
                context.Spawner.SpawnBoss(bossWaves[context.CurrentWave - 1]);
            }
        }
    }
}
```

### Modular Arena Controller

```csharp
using System.Collections.Generic;
using UnityEngine;
using Systems.Arena.Modules;

namespace Systems.Arena
{
    /// <summary>
    /// Main arena controller that orchestrates modules.
    /// </summary>
    public class ModularArenaController : MonoBehaviour
    {
        #region Fields

        [Header("Configuration")]
        [SerializeField] private ArenaData arenaData;
        
        [Header("Modules")]
        [SerializeField] private ArenaModule gameplayModule;
        [SerializeField] private List<ArenaModule> additionalModules = new();
        
        [Header("References")]
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private WaveSpawner waveSpawner;
        
        private ArenaContext _context;
        private ArenaState _currentState = ArenaState.Inactive;

        #endregion

        #region Properties

        public ArenaState CurrentState => _currentState;
        public ArenaContext Context => _context;

        #endregion

        #region Public API

        /// <summary>
        /// Starts the arena with current configuration.
        /// </summary>
        public void StartArena()
        {
            if (_currentState != ArenaState.Inactive) return;
            
            SetupContext();
            _currentState = ArenaState.Active;
            
            gameplayModule?.OnArenaStart(_context);
            foreach (var module in additionalModules)
            {
                module?.OnArenaStart(_context);
            }
        }

        /// <summary>
        /// Ends the arena.
        /// </summary>
        public void EndArena()
        {
            gameplayModule?.OnArenaEnd(_context);
            foreach (var module in additionalModules)
            {
                module?.OnArenaEnd(_context);
            }
            
            _currentState = ArenaState.Inactive;
            enemyManager.CleanupEnemies();
        }

        /// <summary>
        /// Swaps the gameplay module at runtime.
        /// </summary>
        public void SetGameplayModule(ArenaModule module)
        {
            if (_currentState == ArenaState.Active)
            {
                gameplayModule?.OnArenaEnd(_context);
            }
            
            gameplayModule = module;
            
            if (_currentState == ArenaState.Active)
            {
                gameplayModule?.OnArenaStart(_context);
            }
        }

        /// <summary>
        /// Adds an additional module.
        /// </summary>
        public void AddModule(ArenaModule module)
        {
            additionalModules.Add(module);
            if (_currentState == ArenaState.Active)
            {
                module.OnArenaStart(_context);
            }
        }

        /// <summary>
        /// Called by modules when player wins.
        /// </summary>
        public void OnArenaVictory()
        {
            _currentState = ArenaState.Victory;
            // Trigger victory events, UI, etc.
        }

        /// <summary>
        /// Called when player dies.
        /// </summary>
        public void OnPlayerDeath()
        {
            _currentState = ArenaState.Defeat;
            EndArena();
        }

        #endregion

        #region Private Methods

        private void SetupContext()
        {
            _context = new ArenaContext
            {
                Controller = this,
                EnemyManager = enemyManager,
                Spawner = waveSpawner,
                PlayerTransform = Camera.main?.transform,
                ArenaData = arenaData,
                CurrentWave = 0,
                EnemiesKilled = 0,
                ElapsedTime = 0f,
                IsBossWave = false
            };
        }

        private void Update()
        {
            if (_currentState != ArenaState.Active) return;
            
            var deltaTime = Time.deltaTime;
            _context.ElapsedTime += deltaTime;
            
            gameplayModule?.OnArenaUpdate(_context, deltaTime);
            foreach (var module in additionalModules)
            {
                module?.OnArenaUpdate(_context, deltaTime);
            }
        }

        #endregion
    }

    public enum ArenaState
    {
        Inactive,
        Loading,
        Active,
        Paused,
        Victory,
        Defeat
    }
}
```

### Arena Data (Enhanced)

```csharp
using UnityEngine;
using Waves;
using Systems.Arena.Modules;

namespace Systems.Arena
{
    /// <summary>
    /// Complete arena configuration. All aspects of an arena are defined here.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Arena/Arena Data")]
    public class ArenaData : ScriptableObject
    {
        [Header("Identity")]
        public string arenaId;
        public string displayName;
        public string description;
        public Sprite previewImage;
        
        [Header("Difficulty")]
        public ArenaDifficulty difficulty;
        public int recommendedLevel;
        
        [Header("Gameplay Module")]
        public ArenaModule defaultGameplayModule;
        
        [Header("Waves")]
        public WaveData[] waves;
        public WaveData bossWave;
        
        [Header("Environment")]
        public GameObject arenaPrefab;
        public Transform[] spawnPointOverrides;
        
        [Header("Audio")]
        public AudioClip ambientMusic;
        public AudioClip combatMusic;
        public AudioClip bossMusic;
        
        [Header("Rewards")]
        public int goldReward;
        public int experienceReward;
        public LootTableData lootTable;
        
        [Header("Unlocks")]
        public bool isUnlockedByDefault;
        public string[] requiredCompletedArenas;

        public WaveData[] Waves => waves;
    }

    public enum ArenaDifficulty
    {
        Tutorial,
        Easy,
        Normal,
        Hard,
        Nightmare,
        Endless
    }
}
```

---

## Testing & Debugging

### VR Testing Without Headset

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simulates VR input for testing in editor without headset.
/// </summary>
public class VRSimulator : MonoBehaviour
{
    [Header("Simulated Controllers")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform head;
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lookSpeed = 2f;
    
    private bool _isSimulating;

    private void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            _isSimulating = !_isSimulating;
            Cursor.lockState = _isSimulating ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        if (!_isSimulating) return;
        
        // WASD movement
        var move = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) move += head.forward;
        if (Keyboard.current.sKey.isPressed) move -= head.forward;
        if (Keyboard.current.aKey.isPressed) move -= head.right;
        if (Keyboard.current.dKey.isPressed) move += head.right;
        
        head.parent.position += move.normalized * moveSpeed * Time.deltaTime;
        
        // Mouse look
        var look = Mouse.current.delta.ReadValue() * lookSpeed * Time.deltaTime;
        head.parent.Rotate(Vector3.up, look.x);
        head.Rotate(Vector3.right, -look.y);
        
        // Hand simulation
        if (Mouse.current.leftButton.isPressed)
        {
            leftHand.position = head.position + head.forward * 0.5f + head.right * -0.2f;
        }
        if (Mouse.current.rightButton.isPressed)
        {
            rightHand.position = head.position + head.forward * 0.5f + head.right * 0.2f;
        }
    }
}
#endif
```

### Debug Visualization

```csharp
using UnityEngine;

/// <summary>
/// Debug visualization for VR development.
/// </summary>
public class VRDebugVisualizer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool showHandBounds = true;
    [SerializeField] private bool showRaycasts = true;
    [SerializeField] private bool showEnemyTargets = true;
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showEnemyTargets)
        {
            // Draw enemy attack ranges, paths, etc.
        }
    }
    #endif
}
```

---

## Quest 2 Specific Tips

### Build Settings

```
Player Settings:
- Color Space: Linear
- Graphics API: OpenGL ES 3.0 (Vulkan experimental)
- Scripting Backend: IL2CPP
- Target Architecture: ARM64
- Managed Stripping Level: Medium/High

Quality Settings:
- Pixel Light Count: 1-2
- Texture Quality: Half/Quarter Res
- Anti-Aliasing: 4x MSAA
- Shadows: Disabled or Blob only
```

### Memory Management

```csharp
// Call during loading screens to clean up memory
public void OptimizeMemory()
{
    Resources.UnloadUnusedAssets();
    System.GC.Collect();
}
```

### Thermal Management

```csharp
// Monitor device temperature
public class ThermalMonitor : MonoBehaviour
{
    private void Update()
    {
        // Reduce quality if overheating
        if (OVRManager.gpuLevel > 3)
        {
            QualitySettings.SetQualityLevel(0);
        }
    }
}
```

### Common Quest Issues

| Issue | Solution |
|-------|----------|
| Black screen on start | Check Oculus permissions, try restarting |
| Tracking lost | Ensure good lighting, clear guardian |
| Frame drops | Profile, reduce draw calls, check GPU time |
| Crashes on load | Memory issue - reduce pool sizes, assets |
| Controller drift | Battery issue or needs re-pairing |

---

## Resources

### Official Documentation
- [Unity XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/index.html)
- [Oculus Developer Documentation](https://developer.oculus.com/documentation/)
- [Meta Quest Performance Guidelines](https://developer.oculus.com/resources/os-performance/)

### Recommended Assets
- Synty Studios (low-poly, mobile-optimized)
- Oculus Integration SDK
- XR Hands (hand tracking)

### Community
- r/oculusdev
- Unity XR Forums
- Meta Developer Forums
