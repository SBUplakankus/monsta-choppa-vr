# Meta Application SpaceWarp Guide

Implementation guide for Meta Application SpaceWarp (ASW) for Quest VR performance optimization.

---

## Overview

Application SpaceWarp allows rendering at half the native frame rate (36 FPS instead of 72 FPS) while maintaining smooth visuals through frame interpolation.

```
Normal Rendering (72 FPS):
Frame 1 -> Frame 2 -> Frame 3 -> Frame 4
  13.9ms    13.9ms    13.9ms    13.9ms

With SpaceWarp (36 FPS rendered):
Frame 1 -> [Interpolated] -> Frame 2 -> [Interpolated]
  27.8ms       Auto           27.8ms       Auto

Result: Player sees 72 FPS, GPU renders 36 FPS
```

---

## When to Use

### Good Use Cases

- Graphically intensive scenes
- Consistent frame budget scenarios
- Hub areas, menus, exploration
- Slower-paced gameplay

### Avoid When

- Fast-moving objects or particles
- Rapid UI animations
- Competitive/precision gameplay
- Inconsistent performance (can't maintain 36 FPS)

### Hybrid Approach

Toggle dynamically:
- Enable during hub, menus, exploration
- Disable during intense combat

---

## Prerequisites

### Hardware

- Meta Quest 2 or newer (Quest 1 not supported)
- Quest Runtime v39+

### Software

| Requirement | Version |
|-------------|---------|
| Unity | 2021.3+ LTS |
| URP | 12.0+ |
| Meta XR Core SDK | 60.0+ |
| Graphics API | Vulkan (required) |

### Project Settings

```
Player Settings:
- Graphics API: Vulkan (not OpenGL ES)
- Color Space: Linear
- Scripting Backend: IL2CPP
- Target Architecture: ARM64

Quality Settings:
- Anti-Aliasing: 4x MSAA (required)
```

---

## Setup Steps

### 1. XR Settings

```
Edit -> Project Settings -> XR Plug-in Management -> Oculus:
- Low Overhead Mode: Enabled
- Phase Sync: Enabled
- Application SpaceWarp: Enabled
```

### 2. URP Motion Vectors

```
URP Asset Settings:
1. Select URP asset
2. Enable "Motion Vectors" in Rendering section
```

### 3. Vulkan Graphics API

```
Edit -> Project Settings -> Player -> Other Settings:
- Set Vulkan as first/only Graphics API
- Remove OpenGL ES if present
```

---

## Implementation

### SpaceWarp Manager

```csharp
public class SpaceWarpManager : MonoBehaviour
{
    public static SpaceWarpManager Instance { get; private set; }
    
    [SerializeField] private bool enableByDefault = false;
    
    private bool _isEnabled;
    private bool _isSupported;
    
    public bool IsEnabled => _isEnabled;
    public bool IsSupported => _isSupported;
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        CheckSupport();
        if (_isSupported && enableByDefault)
            EnableSpaceWarp();
    }
    
    private void CheckSupport()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        _isSupported = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan;
        #else
        _isSupported = false;
        #endif
    }
    
    public void EnableSpaceWarp()
    {
        if (!_isSupported || _isEnabled) return;
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 36;
        _isEnabled = true;
        #endif
    }
    
    public void DisableSpaceWarp()
    {
        if (!_isEnabled) return;
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 72;
        _isEnabled = false;
        #endif
    }
    
    public void Toggle()
    {
        if (_isEnabled) DisableSpaceWarp();
        else EnableSpaceWarp();
    }
}
```

### Game Integration

```csharp
public class SpaceWarpGameIntegration : MonoBehaviour
{
    [SerializeField] private VoidEventChannel onArenaStarted;
    [SerializeField] private VoidEventChannel onArenaEnded;
    [SerializeField] private VoidEventChannel onHubEntered;
    
    private void OnEnable()
    {
        onHubEntered?.Subscribe(OnHubEntered);
        onArenaStarted?.Subscribe(OnArenaStarted);
        onArenaEnded?.Subscribe(OnArenaEnded);
    }
    
    private void OnDisable()
    {
        onHubEntered?.Unsubscribe(OnHubEntered);
        onArenaStarted?.Unsubscribe(OnArenaStarted);
        onArenaEnded?.Unsubscribe(OnArenaEnded);
    }
    
    private void OnHubEntered()
    {
        // Hub is low-intensity, enable SpaceWarp
        SpaceWarpManager.Instance?.EnableSpaceWarp();
    }
    
    private void OnArenaStarted()
    {
        // Combat requires full frame rate
        SpaceWarpManager.Instance?.DisableSpaceWarp();
    }
    
    private void OnArenaEnded()
    {
        // After combat, can re-enable
        SpaceWarpManager.Instance?.EnableSpaceWarp();
    }
}
```

---

## Motion Vectors

Motion vectors are critical for SpaceWarp quality. Without them, moving objects have visible artifacts.

### Enable in URP

1. Select Universal Render Pipeline Asset
2. Find "Rendering" section
3. Enable "Motion Vectors"

### Per-Object Motion Vectors

For skinned meshes and fast-moving objects:

```csharp
[RequireComponent(typeof(Renderer))]
public class MotionVectorForcer : MonoBehaviour
{
    private Renderer _renderer;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    
    private void OnEnable()
    {
        _renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
    }
    
    private void OnDisable()
    {
        _renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
    }
}
```

---

## Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| Ghosting on moving objects | Missing motion vectors | Add MotionVectorForcer component |
| UI appears blurry | UI interpolated by SpaceWarp | Use screen-space overlay or disable motion vectors on UI |
| Frame rate unstable | Scene too heavy | Profile and optimize, or disable SpaceWarp |
| SpaceWarp not activating | Missing prerequisites | Check Vulkan, MSAA, XR settings |
| Edge artifacts | Known limitation | Add vignette during movement |

---

## Best Practices

| Do | Don't |
|----|-------|
| Use in controlled scenarios | Leave always on |
| Test on device | Trust editor previews |
| Add motion vectors to movers | Forget skinned mesh motion vectors |
| Provide user toggle | Force without option |
| Use transitions when toggling | Instantly switch state |

---

## Performance Tips

1. Profile first - SpaceWarp is not a fix for poor optimization
2. Target consistent 36 FPS - Budget 27.8ms per frame
3. Combine with foveated rendering for maximum performance
4. Consider as enhancement, not crutch

---

## User Settings

Provide player control:

```csharp
public enum PerformanceMode
{
    Quality,      // 72 FPS, no SpaceWarp
    Balanced,     // Dynamic based on load
    Performance   // Always SpaceWarp (36 FPS interpolated)
}
```

---

## Resources

- [Meta Quest Developer Documentation - Application SpaceWarp](https://developer.oculus.com/documentation/unity/unity-asw/)
- [Unity XR Documentation - Motion Vectors](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
