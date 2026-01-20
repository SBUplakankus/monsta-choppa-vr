# Weapon System

Data-driven weapon system for VR combat with XR Interaction Toolkit integration, modular modifiers, and object pooling.

---

## Architecture

```
WeaponData (ScriptableObject)
    │
    ├── WeaponDatabase (lookup)
    │
    └── XRWeaponBase (MonoBehaviour)
            │
            ├── MeleeXRWeapon
            ├── BowXRWeapon
            ├── StaffXRWeapon
            ├── ShieldXRWeapon
            └── ThrowableXRWeapon
```

---

## WeaponData

ScriptableObject defining all weapon properties.

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string weaponID;
    public string displayName;
    public WeaponCategory category;
    public WeaponRarity rarity;
    public GameObject weaponPrefab;
    public Sprite icon;
    
    [Header("Stats")]
    public int baseDamage;
    public float attackCooldown;
    public float range;
    public int staminaCost;
    public DamageType damageType;
    
    [Header("VR Settings")]
    public Vector3 gripPositionOffset;
    public Vector3 gripRotationOffset;
    public float hapticStrength;
    public float hapticDuration;
    
    [Header("Effects")]
    public WorldAudioData[] hitSfx;
    public ParticleData hitVFX;
    public GameObject trailEffect;
    public Color trailColor;
    
    [Header("Modifiers")]
    public List<WeaponModifierData> activeModifiers;
    
    [Header("Economy")]
    public int purchasePrice;
    public int sellPrice;
    public bool isPurchasable;
    
    // Calculated properties
    public int TotalDamage
    {
        get
        {
            int total = baseDamage;
            foreach (var mod in activeModifiers)
                total += mod.damageBonus;
            return total;
        }
    }
}
```

---

## WeaponModifierData

Stackable modifiers that enhance weapons.

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Weapons/Weapon Modifier")]
public class WeaponModifierData : ScriptableObject
{
    [Header("Identity")]
    public string modifierId;
    public string modifierName;
    
    [Header("Damage")]
    public DamageType addedDamageType;
    public int damageBonus;
    public float damageMultiplier;
    
    [Header("Speed")]
    public float speedBonus;
    public float cooldownReduction;
    
    [Header("Visual")]
    public GameObject visualEffect;
    public Color trailColor;
    public Color glowColor;
    public float glowIntensity;
    
    [Header("Hit Effects")]
    public ParticleData onHitVFX;
    public WorldAudioData onHitSfx;
}
```

---

## XRWeaponBase

Abstract base class for all VR weapons.

```csharp
public abstract class XRWeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponData data;
    
    protected XRGrabInteractable grabInteractable;
    protected Rigidbody rb;
    
    private float _lastAttackTime;
    private bool _isHeld;
    
    public WeaponData Data => data;
    public bool IsActive => _isHeld;
    public bool CanAttack => Time.time >= _lastAttackTime + data.attackCooldown;
    
    protected virtual void OnGrab(SelectEnterEventArgs args)
    {
        _isHeld = true;
    }
    
    protected virtual void OnRelease(SelectExitEventArgs args)
    {
        _isHeld = false;
    }
    
    public virtual void ProcessHit(IDamageable target, Vector3 hitPoint, Quaternion hitRotation)
    {
        if (!CanAttack) return;
        
        _lastAttackTime = Time.time;
        
        int damage = CalculateDamage();
        target.TakeDamage(damage);
        
        SpawnHitEffects(hitPoint, hitRotation);
        SendHapticFeedback();
    }
    
    protected int CalculateDamage()
    {
        return data.TotalDamage;
    }
}
```

---

## Weapon Types

### MeleeXRWeapon

Close-range weapons using physics-based hit detection.

- Uses WeaponHitbox for collision detection
- Damage based on swing velocity
- Haptic feedback on hit

### BowXRWeapon

Ranged weapon with projectile spawning.

- Pull string to draw
- Release to fire arrow projectile
- Projectiles pooled via GamePoolManager

### StaffXRWeapon

Magic weapon with spell casting.

- Primary: Projectile spell
- Secondary: Area effect

### ShieldXRWeapon

Defensive weapon with blocking and bashing.

```csharp
public class ShieldXRWeapon : XRWeaponBase
{
    [Header("Block Settings")]
    public float blockAngle = 60f;
    public float damageReduction = 0.75f;
    
    [Header("Parry Settings")]
    public float parryWindow = 0.2f;
    
    [Header("Bash Settings")]
    public float bashCooldown = 1f;
    public float bashDamage = 10f;
    public float bashForce = 500f;
    public float bashRadius = 1.5f;
    
    public bool TryBlock(Vector3 attackDirection, out float reduction)
    {
        float angle = Vector3.Angle(transform.forward, -attackDirection);
        if (angle <= blockAngle)
        {
            reduction = damageReduction;
            return true;
        }
        reduction = 0;
        return false;
    }
}
```

### ThrowableXRWeapon

Throwable weapons that return to pool on impact.

- Thrown on release
- Returns to pool after hit or timeout
- Can be recalled (boomerang behavior)

---

## WeaponHitbox

Component for melee damage detection.

```csharp
public class WeaponHitbox : MonoBehaviour
{
    private XRWeaponBase _weapon;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_weapon.IsActive) return;
        
        if (other.TryGetComponent<IDamageable>(out var target))
        {
            _weapon.ProcessHit(target, other.ClosestPoint(transform.position), transform.rotation);
        }
    }
}
```

---

## Enums

```csharp
public enum WeaponCategory
{
    Sword,
    Bow,
    Staff,
    Axe,
    Dagger,
    Shield,
    Throwable
}

public enum WeaponRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum DamageType
{
    Physical,
    Fire,
    Frost,
    Lightning,
    Arcane,
    Poison
}
```

---

## Pooling Integration

Weapons spawn effects through GamePoolManager.

```csharp
// Spawn hit VFX
GamePoolManager.Instance.GetParticlePrefab(data.hitVFX, hitPoint, hitRotation);

// Spawn hit audio
var sfx = data.hitSfx[Random.Range(0, data.hitSfx.Length)];
GamePoolManager.Instance.GetWorldAudioPrefab(sfx, hitPoint);
```

---

## Holster System

WeaponHolsterController manages equipped weapons.

```csharp
public class WeaponHolsterController : MonoBehaviour
{
    [SerializeField] private Transform leftHolster;
    [SerializeField] private Transform rightHolster;
    [SerializeField] private Transform backHolster;
    
    public void EquipWeapon(WeaponData weapon, HolsterSlot slot);
    public void UnequipWeapon(HolsterSlot slot);
    public WeaponData GetEquippedWeapon(HolsterSlot slot);
}
```

---

## Creating New Weapons

1. Create WeaponData asset via Create menu
2. Configure stats, VR settings, effects
3. Create weapon prefab with:
   - XRGrabInteractable
   - Rigidbody
   - Weapon component (MeleeXRWeapon, etc.)
   - WeaponHitbox on striking surface
4. Assign prefab to WeaponData
5. Add to WeaponDatabase
