# 🚀 Meta Application SpaceWarp (ASW) Implementation Guide

A comprehensive guide to implementing Meta Application SpaceWarp for Quest VR performance optimization.

---

## Table of Contents

1. [What is Application SpaceWarp?](#what-is-application-spacewarp)
2. [When to Use SpaceWarp](#when-to-use-spacewarp)
3. [Prerequisites](#prerequisites)
4. [Project Setup](#project-setup)
5. [Implementation Steps](#implementation-steps)
6. [Motion Vectors Setup](#motion-vectors-setup)
7. [Testing and Validation](#testing-and-validation)
8. [Common Issues and Solutions](#common-issues-and-solutions)
9. [Best Practices](#best-practices)
10. [Example Scripts](#example-scripts)

---

## What is Application SpaceWarp?

### Overview

Application SpaceWarp (ASW) is Meta's frame interpolation technology for Quest headsets. It allows your application to render at **half the native frame rate** (36 FPS instead of 72 FPS, or 45 FPS instead of 90 FPS) while maintaining smooth visuals by generating intermediate frames.

```
Normal Rendering (72 FPS):
Frame 1 → Frame 2 → Frame 3 → Frame 4 → ...
   |         |         |         |
  13.9ms   13.9ms   13.9ms   13.9ms

With SpaceWarp (36 FPS rendered):
Frame 1 → [Interpolated] → Frame 2 → [Interpolated] → Frame 3 → ...
   |           |              |            |              |
  27.8ms     Auto          27.8ms        Auto          27.8ms

Result: Player still sees 72 FPS, but GPU only renders 36 FPS
```

### How It Works

SpaceWarp uses:
1. **Motion Vectors**: Each rendered frame includes per-pixel motion information
2. **Depth Buffer**: Understanding scene geometry
3. **Frame Reprojection**: Generating intermediate frames based on motion data

### SpaceWarp vs. Asynchronous SpaceWarp (Legacy)

| Feature | Application SpaceWarp (ASW) | Asynchronous Time Warp (ATW) |
|---------|----------------------------|------------------------------|
| **Motion Data** | Application-provided motion vectors | Head tracking only |
| **Quality** | High - handles moving objects | Low - only corrects head motion |
| **Artifacts** | Minimal with proper setup | Visible judder on moving objects |
| **Control** | Application-controlled | Automatic fallback |
| **Performance** | Controlled 50% reduction | Variable based on missed frames |

---

## When to Use SpaceWarp

### ✅ Good Use Cases

1. **Graphically Intensive Scenes**
   - Dense environments with high polygon counts
   - Complex shader effects
   - Many light sources

2. **Consistent Frame Budget**
   - Games with predictable GPU load
   - Scenes where you can maintain 36 FPS reliably

3. **Smooth Motion Content**
   - Games without rapid, unpredictable movement
   - Turn-based or slower-paced gameplay

### ❌ Avoid SpaceWarp When

1. **Very Fast Movement**
   - Fast-moving particle effects
   - Rapid UI animations
   - Instant teleportation

2. **Inconsistent Performance**
   - If you can't maintain stable 36 FPS
   - Scenes with dramatic load spikes

3. **High Precision Required**
   - Competitive/precision gameplay
   - Where added latency is problematic

### Hybrid Approach

Consider toggling SpaceWarp dynamically:
- **On**: During hub, menus, exploration
- **Off**: During intense combat, precision aiming

---

## Prerequisites

### Hardware Requirements

- **Meta Quest 2** or newer (Quest 1 not supported)
- **Quest Runtime v39+** for best compatibility

### Software Requirements

| Package | Version | Required |
|---------|---------|----------|
| Unity | 2021.3+ (LTS recommended) | ✅ |
| Universal Render Pipeline (URP) | 12.0+ | ✅ |
| Meta XR Core SDK | 60.0+ | ✅ |
| Oculus Integration | Latest | Recommended |
| Vulkan Graphics API | - | ✅ Required |

### Project Settings

Before implementing SpaceWarp:

```
Player Settings:
- Graphics API: Vulkan (OpenGL ES not supported)
- Color Space: Linear
- Scripting Backend: IL2CPP
- Target Architecture: ARM64

Quality Settings:
- Anti-Aliasing: 4x MSAA (required)
- Shadows: As needed
```

---

## Project Setup

### Step 1: Install Required Packages

```
Via Package Manager (com.meta.xr.sdk.core):
1. Window → Package Manager
2. Add package by name: com.meta.xr.sdk.core
3. Select latest version
4. Import

Or via OpenUPM:
openupm add com.meta.xr.sdk.core
```

### Step 2: Configure XR Settings

```
Edit → Project Settings → XR Plug-in Management:
1. Enable Oculus loader
2. Set Stereo Rendering Mode: Multiview
3. Target Devices: Quest 2, Quest Pro, Quest 3

Edit → Project Settings → XR Plug-in Management → Oculus:
1. Low Overhead Mode: Enabled
2. Phase Sync: Enabled
3. Application SpaceWarp: Enabled ← Important!
```

### Step 3: Configure URP for Motion Vectors

```
URP Asset Settings:
1. Select your URP-HighQuality (or active) asset
2. Find "Motion Vectors" section
3. Enable "Motion Vectors" globally

Alternatively, enable per-camera:
1. Select Main Camera
2. Add "Universal Additional Camera Data" if not present
3. Enable "Render Motion Vectors"
```

### Step 4: Verify Vulkan Setup

```
Edit → Project Settings → Player → Other Settings:
1. Graphics APIs: Set Vulkan as first/only
2. Remove Auto Graphics API checkbox
3. Remove OpenGL ES 3.0 if present
```

---

## Implementation Steps

### Step 1: Create SpaceWarp Manager

Create a central manager to control SpaceWarp state:

```csharp
// SpaceWarpManager.cs
using UnityEngine;
using UnityEngine.XR;
using System.Collections;

#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.XR.Oculus;
#endif

namespace Systems.Performance
{
    /// <summary>
    /// Manages Application SpaceWarp state.
    /// Attach to a persistent GameObject in your bootstrap scene.
    /// </summary>
    public class SpaceWarpManager : MonoBehaviour
    {
        #region Singleton
        
        public static SpaceWarpManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Initialize();
        }
        
        #endregion
        
        #region Fields
        
        [Header("Settings")]
        [SerializeField] private bool enableByDefault = false;
        [SerializeField] private bool allowDynamicToggle = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        private bool _isSpaceWarpSupported;
        private bool _isSpaceWarpEnabled;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Whether SpaceWarp is currently active.
        /// </summary>
        public bool IsEnabled => _isSpaceWarpEnabled;
        
        /// <summary>
        /// Whether the current device supports SpaceWarp.
        /// </summary>
        public bool IsSupported => _isSpaceWarpSupported;
        
        /// <summary>
        /// Current effective frame rate target.
        /// </summary>
        public int EffectiveFrameRate => _isSpaceWarpEnabled ? 36 : 72;
        
        #endregion
        
        #region Initialization
        
        private void Initialize()
        {
            CheckSpaceWarpSupport();
            
            if (_isSpaceWarpSupported && enableByDefault)
            {
                EnableSpaceWarp();
            }
            else
            {
                DisableSpaceWarp();
            }
        }
        
        private void CheckSpaceWarpSupport()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            // Check if device supports SpaceWarp
            // Note: The actual API depends on your Oculus SDK version:
            // - Meta XR Core SDK: Use OVRManager.fixedFoveatedRenderingSupported as a proxy
            //   or check OVRPlugin system properties
            // - Oculus Integration: Check OVRManager settings
            // 
            // Example check (adjust based on your SDK):
            // _isSpaceWarpSupported = OVRPlugin.GetSystemHeadsetType() >= OVRPlugin.SystemHeadset.Oculus_Quest_2;
            _isSpaceWarpSupported = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Vulkan;
            
            if (!_isSpaceWarpSupported)
            {
                Debug.LogWarning("[SpaceWarp] Not supported on this device or Vulkan not enabled.");
            }
            #else
            _isSpaceWarpSupported = false;
            Debug.Log("[SpaceWarp] Only available on Quest devices.");
            #endif
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Enables Application SpaceWarp.
        /// Frame rate drops to 36 FPS with interpolated frames.
        /// </summary>
        public void EnableSpaceWarp()
        {
            if (!_isSpaceWarpSupported)
            {
                Debug.LogWarning("[SpaceWarp] Cannot enable - not supported.");
                return;
            }
            
            if (_isSpaceWarpEnabled) return;
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            // Enable SpaceWarp
            // Note: The actual API depends on your Oculus SDK version.
            // 
            // For Meta XR Core SDK (recommended):
            // OVRManager.SetSpaceWarp(true);
            // 
            // For Oculus Integration SDK:
            // OVRManager.instance.enableDynamicResolution = false; // Disable dynamic res first
            // OVRPlugin.SetAppSpacePosition(...); // If using app-provided motion
            // 
            // The key settings are:
            // 1. Enable SpaceWarp in OVRManager component in Inspector
            // 2. Set target frame rate to half (36 for 72Hz, 45 for 90Hz)
            
            // Set application to render at half frame rate
            // This tells the runtime to expect half-rate rendering
            Application.targetFrameRate = 36; // For 72Hz displays
            
            _isSpaceWarpEnabled = true;
            Debug.Log("[SpaceWarp] Enabled - rendering at 36 FPS");
            #endif
        }
        
        /// <summary>
        /// Disables Application SpaceWarp.
        /// Returns to full frame rate rendering.
        /// </summary>
        public void DisableSpaceWarp()
        {
            if (!_isSpaceWarpEnabled) return;
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            // Disable SpaceWarp
            Utils.EnableSpaceWarp(false);
            
            // Return to full frame rate
            OVRPlugin.SetHalfRate(false);
            
            _isSpaceWarpEnabled = false;
            Debug.Log("[SpaceWarp] Disabled - rendering at 72 FPS");
            #endif
        }
        
        /// <summary>
        /// Toggles SpaceWarp state.
        /// </summary>
        public void ToggleSpaceWarp()
        {
            if (!allowDynamicToggle)
            {
                Debug.LogWarning("[SpaceWarp] Dynamic toggle disabled.");
                return;
            }
            
            if (_isSpaceWarpEnabled)
                DisableSpaceWarp();
            else
                EnableSpaceWarp();
        }
        
        /// <summary>
        /// Sets SpaceWarp state with a smooth transition.
        /// </summary>
        public void SetSpaceWarp(bool enabled, float transitionTime = 0.5f)
        {
            if (transitionTime <= 0)
            {
                if (enabled) EnableSpaceWarp();
                else DisableSpaceWarp();
                return;
            }
            
            StartCoroutine(TransitionSpaceWarp(enabled, transitionTime));
        }
        
        private IEnumerator TransitionSpaceWarp(bool targetState, float duration)
        {
            // Fade to black or apply vignette during transition
            // This hides any visual artifacts during the switch
            
            // Apply visual transition effect here
            // yield return FadeToBlack(duration / 2);
            
            yield return new WaitForSeconds(duration / 2);
            
            if (targetState)
                EnableSpaceWarp();
            else
                DisableSpaceWarp();
            
            yield return new WaitForSeconds(duration / 2);
            
            // Remove visual transition effect
            // yield return FadeFromBlack(duration / 2);
        }
        
        #endregion
        
        #region Debug
        
        private void OnGUI()
        {
            if (!showDebugInfo) return;
            
            var style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter
            };
            
            string status = _isSpaceWarpEnabled ? "ON (36 FPS)" : "OFF (72 FPS)";
            string supported = _isSpaceWarpSupported ? "Supported" : "Not Supported";
            
            GUI.Box(new Rect(10, 10, 300, 100), 
                $"SpaceWarp: {status}\n{supported}\nFPS: {1f / Time.deltaTime:F1}",
                style);
        }
        
        #endregion
    }
}
```

### Step 2: Integrate with Game Flow

Tie SpaceWarp to your game state:

```csharp
// Example: SpaceWarpGameIntegration.cs
using UnityEngine;
using Events;

namespace Systems.Performance
{
    /// <summary>
    /// Integrates SpaceWarp with game state.
    /// Enables during low-intensity moments, disables during combat.
    /// </summary>
    public class SpaceWarpGameIntegration : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private VoidEventChannel onArenaStarted;
        [SerializeField] private VoidEventChannel onArenaEnded;
        [SerializeField] private VoidEventChannel onHubEntered;
        [SerializeField] private VoidEventChannel onMenuOpened;
        
        private void OnEnable()
        {
            // Enable SpaceWarp during low-intensity moments
            onHubEntered?.Subscribe(OnHubEntered);
            onMenuOpened?.Subscribe(OnMenuOpened);
            
            // Disable SpaceWarp during intense gameplay
            onArenaStarted?.Subscribe(OnArenaStarted);
            onArenaEnded?.Subscribe(OnArenaEnded);
        }
        
        private void OnDisable()
        {
            onHubEntered?.Unsubscribe(OnHubEntered);
            onMenuOpened?.Unsubscribe(OnMenuOpened);
            onArenaStarted?.Unsubscribe(OnArenaStarted);
            onArenaEnded?.Unsubscribe(OnArenaEnded);
        }
        
        private void OnHubEntered()
        {
            // Hub is low-intensity, enable SpaceWarp
            SpaceWarpManager.Instance?.SetSpaceWarp(true, 0.3f);
        }
        
        private void OnMenuOpened()
        {
            // Menus are static, great for SpaceWarp
            SpaceWarpManager.Instance?.SetSpaceWarp(true, 0.2f);
        }
        
        private void OnArenaStarted()
        {
            // Combat requires full frame rate
            SpaceWarpManager.Instance?.SetSpaceWarp(false, 0.2f);
        }
        
        private void OnArenaEnded()
        {
            // After combat, can re-enable SpaceWarp
            SpaceWarpManager.Instance?.SetSpaceWarp(true, 0.5f);
        }
    }
}
```

---

## Motion Vectors Setup

Motion vectors are **critical** for SpaceWarp quality. Without proper motion vectors, moving objects will have visible artifacts.

### Understanding Motion Vectors

```
What motion vectors contain:
- Per-pixel 2D velocity information
- How each pixel moved from last frame to current frame
- Used by SpaceWarp to predict intermediate frames

Without motion vectors:
- SpaceWarp guesses movement based on depth
- Fast-moving objects look smeared or ghosted
- UI elements appear blurry during transitions
```

### Setting Up Motion Vectors in URP

#### Step 1: Enable in URP Asset

```
1. Select your Universal Render Pipeline Asset
2. Find "Rendering" section
3. Enable "Motion Vectors"
4. Save asset
```

#### Step 2: Per-Object Motion Vectors

For skinned meshes (characters, enemies) that don't have motion blur but need SpaceWarp:

```csharp
// Add to moving objects that need accurate motion vectors
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Ensures motion vectors are generated for this object.
/// Add to characters, enemies, and fast-moving objects.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class MotionVectorForcer : MonoBehaviour
{
    private Renderer _renderer;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        
        // Force motion vector generation
        _renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
    }
    
    private void OnEnable()
    {
        // When object becomes visible, use object motion
        _renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
    }
    
    private void OnDisable()
    {
        // When hidden, no need to generate
        _renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
    }
}
```

#### Step 3: Shader Motion Vectors

For custom shaders, ensure they output motion vectors:

```hlsl
// In custom URP shader, add motion vector pass:
Pass
{
    Name "MotionVectors"
    Tags { "LightMode" = "MotionVectors" }
    
    ZWrite On
    ColorMask RGB
    
    HLSLPROGRAM
    #pragma vertex MotionVectorVertex
    #pragma fragment MotionVectorFragment
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectors.hlsl"
    ENDHLSL
}
```

### UI and Motion Vectors

UI rendered in world space needs special consideration:

```csharp
// For world-space UI that should NOT have motion blur/SpaceWarp artifacts
// You may need to render UI to a separate layer with motion vectors disabled

// Option 1: Separate UI camera with SpaceWarp hint
// Option 2: Render UI after SpaceWarp compositing
// Option 3: Use screen-space UI for critical elements
```

---

## Testing and Validation

### In-Editor Testing

SpaceWarp only works on Quest devices, but you can validate motion vectors in editor:

```
1. Open Frame Debugger (Window → Analysis → Frame Debugger)
2. Play scene
3. Look for "Motion Vector" pass
4. Verify it's rendering all moving objects
```

### On-Device Testing

Create a debug overlay:

```csharp
// SpaceWarpDebugOverlay.cs
using UnityEngine;
using TMPro;

namespace Systems.Performance
{
    /// <summary>
    /// Debug overlay for SpaceWarp validation.
    /// Attach to a world-space canvas visible in VR.
    /// </summary>
    public class SpaceWarpDebugOverlay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private bool showInBuild = false;
        
        private float _frameTime;
        private int _frameCount;
        private float _timer;
        
        private void Update()
        {
            if (!showInBuild && !Debug.isDebugBuild) return;
            
            // Calculate FPS
            _frameCount++;
            _timer += Time.unscaledDeltaTime;
            
            if (_timer >= 0.5f)
            {
                _frameTime = _timer / _frameCount;
                _frameCount = 0;
                _timer = 0;
                
                UpdateDisplay();
            }
        }
        
        private void UpdateDisplay()
        {
            if (statusText == null) return;
            
            var manager = SpaceWarpManager.Instance;
            if (manager == null)
            {
                statusText.text = "SpaceWarp Manager not found";
                return;
            }
            
            float fps = 1f / _frameTime;
            string status = manager.IsEnabled ? "ENABLED" : "DISABLED";
            string color = manager.IsEnabled ? "#00FF00" : "#FF0000";
            
            // If SpaceWarp is working correctly:
            // - With SpaceWarp ON: You render 36 FPS but see smooth motion
            // - FPS should be ~36 when enabled, ~72 when disabled
            
            string expectedFps = manager.IsEnabled ? "~36" : "~72";
            string warning = "";
            
            if (manager.IsEnabled && fps > 40)
                warning = "\n<color=yellow>WARNING: Rendering faster than expected</color>";
            else if (manager.IsEnabled && fps < 30)
                warning = "\n<color=red>WARNING: Below target - reduce load</color>";
            
            statusText.text = $"<color={color}>SpaceWarp: {status}</color>\n" +
                             $"FPS: {fps:F1} (target: {expectedFps})\n" +
                             $"Frame Time: {_frameTime * 1000:F2}ms" +
                             warning;
        }
    }
}
```

### Validation Checklist

```
Before Release:
□ Motion vectors visible in Frame Debugger
□ No ghosting on fast-moving objects
□ UI text remains sharp
□ Hand tracking/controllers responsive
□ No visible judder during camera movement
□ Stable 36 FPS when SpaceWarp enabled
□ Clean transitions when toggling SpaceWarp
```

---

## Common Issues and Solutions

### Issue 1: Ghosting on Moving Objects

**Symptom**: Moving enemies/projectiles have a "ghost" trail

**Cause**: Missing motion vectors on those objects

**Solution**:
```csharp
// Add to affected objects:
GetComponent<Renderer>().motionVectorGenerationMode = 
    MotionVectorGenerationMode.Object;

// For skinned meshes, also check:
// - Animator is updating
// - SkinnedMeshRenderer has proper bounds
```

### Issue 2: UI Appears Blurry

**Symptom**: World-space UI text/icons look smeared

**Cause**: UI is being interpolated by SpaceWarp

**Solution**:
```
Options:
1. Use screen-space overlay for critical UI (no SpaceWarp)
2. Render UI to separate camera layer
3. Disable motion vectors on UI elements
4. Consider using SpaceWarp hints (per-layer)
```

### Issue 3: Frame Rate Unstable

**Symptom**: Dropping below 36 FPS even with SpaceWarp

**Cause**: Scene is too heavy even for half frame rate

**Solution**:
```
1. Profile with OVR Metrics Tool
2. Reduce draw calls (batching, LOD)
3. Simplify shaders
4. Reduce active enemies
5. Consider disabling SpaceWarp for this scene
```

### Issue 4: SpaceWarp Not Activating

**Symptom**: `IsEnabled` is true but rendering at 72 FPS

**Cause**: Missing prerequisites

**Solution**:
```
Check:
□ Using Vulkan graphics API
□ MSAA 4x enabled
□ SpaceWarp enabled in XR settings
□ Motion vectors enabled in URP
□ Not running in Link/Quest Link
```

### Issue 5: Visible Seams at Screen Edge

**Symptom**: Edge of view shows artifacts during head movement

**Cause**: This is a known limitation of SpaceWarp

**Solution**:
```
1. Increase FOV padding if available
2. Add vignette effect during movement
3. Accept as minor trade-off
```

---

## Best Practices

### Do's and Don'ts

| ✅ Do | ❌ Don't |
|-------|---------|
| Use SpaceWarp in controlled scenarios | Leave SpaceWarp always on |
| Test extensively on device | Trust editor previews |
| Add motion vector components to movers | Forget skinned mesh motion vectors |
| Provide user toggle option | Force SpaceWarp without option |
| Monitor frame rate stability | Assume stable performance |
| Use transitions when toggling | Instantly switch SpaceWarp state |

### Performance Tips

```
1. Profile First
   - Use OVR Metrics Tool to find bottlenecks
   - SpaceWarp is not a fix for poor optimization

2. Target Consistent 36 FPS
   - Spikes will still cause stutters
   - Budget: 27.8ms per frame with SpaceWarp

3. Consider Alternatives
   - Dynamic resolution scaling
   - Foveated rendering (fixed and dynamic)
   - LOD optimization

4. Combine Techniques
   - SpaceWarp + Foveated Rendering = maximum perf
   - Lower quality settings + SpaceWarp for heavy scenes
```

### User Settings

Always give users control:

```csharp
// Add to settings panel
public enum PerformanceMode
{
    Quality,      // 72 FPS, no SpaceWarp
    Balanced,     // Dynamic - SpaceWarp when needed
    Performance   // Always SpaceWarp (36 FPS + interpolation)
}

// Let users choose what works for them
// Some users are sensitive to interpolation artifacts
// Some prefer smooth motion over visual fidelity
```

---

## Example Scripts

### Complete SpaceWarp Toggle Button

```csharp
// SpaceWarpToggleUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Settings
{
    /// <summary>
    /// UI toggle for SpaceWarp setting.
    /// </summary>
    public class SpaceWarpToggleUI : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI statusLabel;
        [SerializeField] private GameObject notSupportedMessage;
        
        private void Start()
        {
            var manager = SpaceWarpManager.Instance;
            
            if (manager == null || !manager.IsSupported)
            {
                // SpaceWarp not available
                toggle.gameObject.SetActive(false);
                notSupportedMessage?.SetActive(true);
                return;
            }
            
            // Initialize toggle state
            toggle.isOn = manager.IsEnabled;
            UpdateStatusLabel();
            
            // Subscribe to changes
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
        
        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
        
        private void OnToggleChanged(bool enabled)
        {
            SpaceWarpManager.Instance?.SetSpaceWarp(enabled, 0.3f);
            UpdateStatusLabel();
        }
        
        private void UpdateStatusLabel()
        {
            if (statusLabel == null) return;
            
            var manager = SpaceWarpManager.Instance;
            if (manager == null) return;
            
            statusLabel.text = manager.IsEnabled 
                ? "Performance Mode (36 FPS interpolated)" 
                : "Quality Mode (72 FPS native)";
        }
    }
}
```

### Automatic SpaceWarp Based on Performance

```csharp
// AdaptiveSpaceWarp.cs
using UnityEngine;
using System.Collections;

namespace Systems.Performance
{
    /// <summary>
    /// Automatically enables/disables SpaceWarp based on performance.
    /// </summary>
    public class AdaptiveSpaceWarp : MonoBehaviour
    {
        [Header("Thresholds")]
        [SerializeField] private float enableBelowFps = 65f;
        [SerializeField] private float disableAboveFps = 70f;
        [SerializeField] private float evaluationInterval = 2f;
        
        [Header("Settings")]
        [SerializeField] private bool autoManageEnabled = true;
        [SerializeField] private int sampleCount = 60;
        
        private float[] _frameTimes;
        private int _sampleIndex;
        private bool _sampling = true;
        
        private void Start()
        {
            _frameTimes = new float[sampleCount];
            StartCoroutine(EvaluatePerformance());
        }
        
        private void Update()
        {
            if (!_sampling || !autoManageEnabled) return;
            
            _frameTimes[_sampleIndex] = Time.unscaledDeltaTime;
            _sampleIndex = (_sampleIndex + 1) % sampleCount;
        }
        
        private IEnumerator EvaluatePerformance()
        {
            while (true)
            {
                yield return new WaitForSeconds(evaluationInterval);
                
                if (!autoManageEnabled) continue;
                
                float avgFps = CalculateAverageFps();
                var manager = SpaceWarpManager.Instance;
                
                if (manager == null) continue;
                
                if (!manager.IsEnabled && avgFps < enableBelowFps)
                {
                    Debug.Log($"[AdaptiveSpaceWarp] FPS {avgFps:F1} < {enableBelowFps}, enabling SpaceWarp");
                    manager.EnableSpaceWarp();
                }
                else if (manager.IsEnabled)
                {
                    // When SpaceWarp is on, we render at half rate
                    // So we need to check if we have headroom
                    // If we're hitting 36 FPS easily, we could try going back
                    
                    // This is a simplified heuristic - in practice you'd want
                    // to track performance history and be more conservative
                    
                    if (avgFps > 35 && HasPerformanceHeadroom())
                    {
                        Debug.Log("[AdaptiveSpaceWarp] Good performance, trying full rate");
                        manager.DisableSpaceWarp();
                    }
                }
            }
        }
        
        private float CalculateAverageFps()
        {
            float total = 0;
            for (int i = 0; i < _frameTimes.Length; i++)
            {
                if (_frameTimes[i] > 0)
                    total += 1f / _frameTimes[i];
            }
            return total / sampleCount;
        }
        
        private bool HasPerformanceHeadroom()
        {
            // Check if we have consistent good performance
            // Returns true if 90% of samples are under budget
            int goodFrames = 0;
            float targetFrameTime = 1f / 36f; // Target for SpaceWarp
            
            for (int i = 0; i < _frameTimes.Length; i++)
            {
                if (_frameTimes[i] < targetFrameTime * 0.9f)
                    goodFrames++;
            }
            
            return goodFrames > sampleCount * 0.9f;
        }
        
        public void SetAutoManage(bool enabled)
        {
            autoManageEnabled = enabled;
        }
    }
}
```

---

## Resources

### Official Documentation

- [Meta Quest Developer Documentation - Application SpaceWarp](https://developer.oculus.com/documentation/unity/unity-asw/)
- [Unity XR Documentation - Motion Vectors](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)

### Related Tools

- **OVR Metrics Tool**: Performance monitoring
- **RenderDoc**: GPU debugging
- **Unity Profiler**: CPU/GPU timing

### Community Resources

- Meta Developer Forums
- r/oculusdev
- Unity XR Discord

---

> 💡 **Pro Tip**: Start with SpaceWarp disabled and ensure your game runs well at 72 FPS first. Only then add SpaceWarp as an *enhancement* for lower-end scenarios or intentionally heavier visuals. Never use SpaceWarp to "fix" a poorly optimized game - it works best when you have performance headroom and want to push visual quality higher.
