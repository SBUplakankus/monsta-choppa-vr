# VR Development Guide

Performance targets, optimization techniques, and Quest-specific considerations.

---

## Performance Targets

### Frame Rate

| Platform | Minimum | Target |
|----------|---------|--------|
| Quest 2 | 72 FPS | 90 FPS |
| Quest 3 | 72 FPS | 120 FPS |

Frame budget at 72 FPS: 13.9ms per frame

### Resource Budgets

| Resource | Target | Maximum |
|----------|--------|---------|
| Draw calls | 100 | 150 |
| Triangles | 300k | 500k |
| Active enemies | 6 | 8 |
| Active VFX | 15 | 20 |

---

## VR-Specific Rules

### Never Do

| Action | Problem |
|--------|---------|
| Move camera transform directly | Causes motion sickness |
| Change camera FOV | VR FOV is fixed by hardware |
| Disable head tracking | Breaks spatial awareness |
| Use Instantiate in gameplay | Causes frame spikes |
| Block main thread | Tracking freezes |

### Always Do

| Action | Reason |
|--------|--------|
| Move XR Rig, not camera | Maintains tracking |
| Use Time.deltaTime | Frame-rate independent |
| Pool all spawned objects | Avoids GC spikes |
| Cache GetComponent results | Reduces per-frame cost |
| Unsubscribe events in OnDisable | Prevents memory leaks |

---

## Optimization Techniques

### Rendering

- Enable Single Pass Instanced stereo rendering
- Use GPU instancing for repeated meshes
- Enable foveated rendering on Quest
- Aggressive LOD groups
- Bake lighting where possible
- Limit real-time shadows

### Physics

- Use layer matrix to limit collision checks
- Prefer trigger colliders over continuous collision
- Update NavMesh paths at intervals (0.2s)
- Use OverlapSphereNonAlloc

### Scripts

- Minimize Update() logic
- Use priority update system for enemies
- Cache all component references
- Avoid LINQ in hot paths
- Use StringBuilder for string operations

---

## Comfort Settings

Always provide player options:

### Locomotion

| Type | Description | Comfort Level |
|------|-------------|---------------|
| Teleport | Point and blink | High |
| Dash | Quick move | Medium |
| Smooth | Continuous | Low |

### Turning

| Type | Options |
|------|---------|
| Snap | 15, 30, 45, 90 degrees |
| Smooth | Variable speed |

### Comfort Aids

- Movement vignette
- Seated mode
- Height calibration
- Dominant hand selection

---

## Quest Build Settings

### Player Settings

```
Graphics API: Vulkan
Color Space: Linear
Scripting Backend: IL2CPP
Architecture: ARM64
Managed Stripping: Medium
```

### Quality Settings

```
Pixel Light Count: 1-2
Texture Quality: Half Res
Anti-Aliasing: 4x MSAA
Shadows: Disabled or blob only
```

### XR Settings

```
Stereo Rendering: Single Pass Instanced
Low Overhead Mode: Enabled
Phase Sync: Enabled
```

---

## Profiling

### Tools

| Tool | Purpose |
|------|---------|
| Unity Profiler | CPU/GPU timing |
| Frame Debugger | Draw call analysis |
| Memory Profiler | Allocation tracking |
| OVR Metrics Tool | Quest-specific metrics |

### Key Metrics

- GPU time per frame
- Draw call count
- SetPass calls
- GC allocations
- Memory usage

---

## Common Issues

| Issue | Solution |
|-------|----------|
| Frame drops | Profile GPU, reduce draw calls |
| Tracking loss | Never disable XR camera |
| Motion sickness | Add comfort options |
| Memory growth | Pool objects, unload unused |
| Thermal throttling | Reduce quality settings |
