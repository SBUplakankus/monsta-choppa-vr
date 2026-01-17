# 🎮 Combat System TODO & Improvement Suggestions

## VR Performance Optimization Checklist

### ✅ Implemented
- [x] Object pooling for projectiles (arrows, spells)
- [x] Object pooling for enemies via GamePoolManager
- [x] Object pooling for VFX and audio
- [x] Cached Transform references to avoid GetComponent calls
- [x] Event-driven architecture to reduce Update() overhead
- [x] Priority-based update system via EnemyManager
- [x] Minimal allocations in hot paths
- [x] NavMesh path update throttling (0.2s intervals)

### 🔄 Recommended Improvements

#### High Priority (VR Performance Critical)

1. **Physics Optimization**
   - [ ] Use Physics.OverlapSphereNonAlloc instead of OverlapSphere
   - [ ] Implement collision layer matrix optimization
   - [ ] Consider using trigger colliders over continuous collision for weapons
   - [ ] Add physics LOD - reduce physics checks for distant enemies

2. **Animation Optimization**
   - [ ] Implement animation LOD - simpler animations for distant enemies
   - [ ] Use Animator.StringToHash for all animation parameters (done)
   - [ ] Consider disabling Animator on very distant enemies
   - [ ] Optimize animator controllers for mobile GPU

3. **Rendering Optimization**
   - [ ] Implement GPU instancing for enemy meshes
   - [ ] Add LOD groups to enemy prefabs
   - [ ] Use occlusion culling aggressively
   - [ ] Reduce overdraw on VFX

4. **Memory Optimization**
   - [ ] Pre-warm all pools during loading screen
   - [ ] Implement soft caps on active enemies based on device performance
   - [ ] Use ArrayPool<T> for temporary arrays

#### Medium Priority (Gameplay Enhancement)

5. **Combat Feel**
   - [ ] Add haptic patterns for different damage types (fire = pulse, ice = sustained)
   - [ ] Implement weapon trails using Line Renderer with pooling
   - [ ] Add slow-motion on critical hits (VR-safe implementation)
   - [ ] Screen shake alternatives for VR (controller shake, vignette pulse)

6. **Enemy AI Improvements**
   - [ ] Implement enemy archetypes (melee rusher, ranged, tank, support)
   - [ ] Add group tactics - flanking, surrounding
   - [ ] Stagger system to prevent attack spam
   - [ ] Retreating behavior when low health

7. **Weapon System**
   - [ ] Implement weapon upgrading system
   - [ ] Add socket system for gems/runes on weapons
   - [ ] Create weapon skill trees
   - [ ] Dual wielding support

8. **Projectile Improvements**
   - [ ] Add projectile prediction for better VR aiming
   - [ ] Implement arrow sticking with proper parent handling
   - [ ] Add ricochet for special arrows
   - [ ] Homing projectiles for certain spells

#### Lower Priority (Future Features)

9. **Status Effects System**
   - [ ] Create StatusEffect base class
   - [ ] Implement burning (DoT + spread)
   - [ ] Implement frozen (slow + shatter)
   - [ ] Implement shocked (chain lightning + stun)
   - [ ] Implement poisoned (DoT + weakness)

10. **Combo System**
    - [ ] Track weapon swing patterns
    - [ ] Reward varied attack combinations
    - [ ] Special finisher moves

11. **Enemy Variety**
    - [ ] Create EnemyArchetype ScriptableObjects
    - [ ] Boss-specific behaviors
    - [ ] Environmental hazard enemies

---

## Data-Driven Architecture Notes

### Current Pattern (Keep Consistent)
```
ScriptableObject (Data) → MonoBehaviour (Controller) → Components (Behavior)
```

### Recommended Patterns

1. **Factory Pattern for Weapons**
   - WeaponFactory creates weapon instances from WeaponData
   - Handles modifier application consistently

2. **Strategy Pattern for Enemy AI**
   - EnemyBehavior ScriptableObjects define behavior trees
   - Easy to create new enemy types without code changes

3. **Observer Pattern (Current)**
   - Event channels for decoupled communication
   - Expand to include combat events (OnHit, OnBlock, OnDodge)

4. **State Machine for Combat**
   - Consider ScriptableObject-based state machines
   - Each state as a ScriptableObject for designer iteration

---

## Quest 2 Specific Considerations

### Performance Targets
- 72 FPS minimum (90 FPS preferred)
- Max 6-8 active enemies at once
- Max 10-15 active projectiles
- Max 20-30 active VFX

### Memory Budget
- Keep total scene memory under 1GB
- Pool sizes should be tuned based on testing
- Consider async loading for weapon variants

### Input Considerations
- Support both Index trigger and Grip for different actions
- Allow input remapping
- Consider hand tracking for future updates

---

## Integration Points

### Player Health System
The player needs a health component. Recommend:
- PlayerHealth : HealthComponent
- Integrate with EnemyAttack damage dealing
- Add visual feedback (vignette, controller haptics)

### Shop/Economy
WeaponData now includes:
- purchasePrice
- sellPrice
- isPurchasable

Ready for shop UI integration.

### Save System
Weapons with modifiers need serialization:
- Save applied modifier IDs
- Save weapon upgrade levels
- Consider using weapon instance IDs

---

## Example Weapon Configurations

### Fire Sword (Rare)
- Base: Sword, 15 damage
- Modifier: Flaming (+5 fire damage, orange trail)
- VFX: Fire particles on hit

### Frost Bow (Epic)
- Base: Bow, 20 damage
- Modifier: Frozen (+8 frost damage, blue trail)
- Effect: Slow enemies on hit

### Lightning Staff (Legendary)
- Base: Staff, 25 damage
- Modifier: Shocking (+12 lightning, chain effect)
- Special: Chain to nearby enemies

### Throwing Knife Set (Uncommon)
- Base: ThrowingKnife, 12 damage
- Feature: Boomerang recall
- Skill: Return damages enemies in path
