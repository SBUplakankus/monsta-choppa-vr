# VR Performance Guide

Performance targets, optimization techniques, and Quest-specific considerations.

---

## Performance Targets

### Frame Rate

| Platform | Minimum | Target |
|:---------|:--------|:-------|
| Quest 2 | 72 FPS | 90 FPS |
| Quest 3 | 72 FPS | 120 FPS |

Frame budget at 72 FPS: **13.9ms per frame**

### Resource Budgets

| Resource | Target | Maximum |
|:---------|:-------|:--------|
| Draw calls | 100 | 150 |
| Triangles | 300k | 500k |
| Active enemies | 6 | 8 |
| Active VFX | 15 | 20 |

---

## VR-Specific Rules

### Never Do

| Action | Problem |
|:-------|:--------|
| Move camera transform directly | Causes motion sickness |
| Change camera FOV | VR FOV is fixed by hardware |
| Disable head tracking | Breaks spatial awareness |
| Use Instantiate in gameplay | Causes frame spikes |
| Block main thread | Tracking freezes |

### Always Do

| Action | Reason |
|:-------|:-------|
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

## Update Manager

Centralized update system with priority-based throttling.

```csharp
public class GameUpdateManager : MonoBehaviour
{
    private const float MediumPriorityInterval = 0.2f;
    private const float LowPriorityInterval = 0.4f;
    
    private List<IUpdateable> _highPriority;
    private List<IUpdateable> _mediumPriority;
    private List<IUpdateable> _lowPriority;
    
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        
        // High priority: every frame
        for (int i = 0; i < _highPriority.Count; i++)
            _highPriority[i].OnUpdate(deltaTime);
        
        // Medium priority: every 0.2s
        if (Time.time >= _lastMediumUpdate + MediumPriorityInterval)
        {
            for (int i = 0; i < _mediumPriority.Count; i++)
                _mediumPriority[i].OnUpdate(MediumPriorityInterval);
            _lastMediumUpdate = Time.time;
        }
        
        // Low priority: every 0.4s
        if (Time.time >= _lastLowUpdate + LowPriorityInterval)
        {
            for (int i = 0; i < _lowPriority.Count; i++)
                _lowPriority[i].OnUpdate(LowPriorityInterval);
            _lastLowUpdate = Time.time;
        }
    }
}
```

---

## Object Pooling

All spawned objects use GamePoolManager to avoid runtime allocations.

```csharp
public class GamePoolManager : MonoBehaviour
{
    private Dictionary<EnemyData, ObjectPool<GameObject>> _enemyPools;
    private Dictionary<ParticleData, ObjectPool<GameObject>> _particlePools;
    
    public GameObject GetEnemyPrefab(EnemyData data, Vector3 position, Quaternion rotation)
    {
        if (!_enemyPools.TryGetValue(data, out var pool))
        {
            pool = CreatePool(data.prefab);
            _enemyPools[data] = pool;
        }
        
        var instance = pool.Get();
        instance.transform.SetPositionAndRotation(position, rotation);
        return instance;
    }
    
    public void ReturnEnemyPrefab(EnemyData data, GameObject instance)
    {
        instance.SetActive(false);
        _enemyPools[data].Release(instance);
    }
}
```

### Priority Routing

VFX and audio have priority systems to prevent overload.

```csharp
if (!_vfxRouter.CanSpawn(data.Priority))
    return null;
```

---

## Quest Build Settings

### Player Settings

| Setting | Value |
|:--------|:------|
| Graphics API | Vulkan |
| Color Space | Linear |
| Scripting Backend | IL2CPP |
| Architecture | ARM64 |
| Managed Stripping | Medium |

### Quality Settings

| Setting | Value |
|:--------|:------|
| Pixel Light Count | 1-2 |
| Texture Quality | Half Res |
| Anti-Aliasing | 4x MSAA |
| Shadows | Disabled or blob only |

### XR Settings

| Setting | Value |
|:--------|:------|
| Stereo Rendering | Single Pass Instanced |
| Low Overhead Mode | Enabled |
| Phase Sync | Enabled |

---

## Profiling

### Tools

| Tool | Purpose |
|:-----|:--------|
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
|:------|:---------|
| Frame drops | Profile GPU, reduce draw calls |
| Tracking loss | Never disable XR camera |
| Motion sickness | Add comfort options |
| Memory growth | Pool objects, unload unused |
| Thermal throttling | Reduce quality settings |
