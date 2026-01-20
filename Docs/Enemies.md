# Enemy System

Modular enemy system with component-based architecture, object pooling, and data-driven configuration.

---

## Architecture

```
EnemyData (ScriptableObject)
    │
    ├── EnemyDatabase (lookup)
    │
    └── EnemyController (MonoBehaviour coordinator)
            │
            ├── EnemyHealth (damage, death)
            ├── EnemyMovement (NavMesh navigation)
            ├── EnemyAnimator (animation states)
            ├── EnemyAttack (combat behavior)
            └── EnemyId (unique identifier)
```

---

## EnemyData

ScriptableObject defining enemy configuration.

```csharp
[CreateAssetMenu(menuName = "Scriptable Objects/Characters/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyId;
    public GameObject prefab;
    
    [Header("Stats")]
    public int maxHealth;
    public float moveSpeed;
    
    [Header("Combat")]
    public WeaponData weapon;
    
    [Header("Audio")]
    public WorldAudioData[] ambientSfx;
    public WorldAudioData[] hitSfx;
    public WorldAudioData[] deathSfx;
    
    [Header("Effects")]
    public ParticleData deathVFX;
    
    // Random audio selection
    public WorldAudioData GetHitSfx()
    {
        if (hitSfx == null || hitSfx.Length == 0) return null;
        return hitSfx[Random.Range(0, hitSfx.Length)];
    }
    
    public WorldAudioData GetDeathSfx()
    {
        if (deathSfx == null || deathSfx.Length == 0) return null;
        return deathSfx[Random.Range(0, deathSfx.Length)];
    }
}
```

---

## EnemyController

Main coordinator that manages all enemy components.

```csharp
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyAnimator animator;
    [SerializeField] private EnemyHealth health;
    [SerializeField] private EnemyAttack attack;
    [SerializeField] private EnemyId enemyId;
    
    private EnemyData _data;
    
    public EnemyData Data => _data;
    public EnemyMovement Movement => movement;
    public EnemyAnimator Animator => animator;
    public EnemyHealth Health => health;
    
    public event Action OnDeath;
    
    public void Initialize(EnemyData data)
    {
        _data = data;
        health.Initialize(data.maxHealth);
        movement.SetSpeed(data.moveSpeed);
        attack.SetWeapon(data.weapon);
    }
    
    public void HandleEnemyDeath()
    {
        OnDeath?.Invoke();
        SpawnDeathEffects();
        GameEvents.OnEnemyDespawned.Raise(this);
        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        GamePoolManager.Instance.ReturnEnemyPrefab(_data, gameObject);
    }
}
```

---

## EnemyHealth

Health component implementing IDamageable.

```csharp
public class EnemyHealth : HealthComponent
{
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private EnemyAnimator animator;
    
    private EnemyData _data;
    
    public float HealthBarValue => (float)CurrentHealth / MaxHealth;
    
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        healthBar?.UpdateValue(HealthBarValue);
        animator?.PlayHitReaction();
        PlayHitSound();
        
        if (CurrentHealth <= 0)
        {
            GetComponent<EnemyController>().HandleEnemyDeath();
        }
    }
    
    private void PlayHitSound()
    {
        var sfx = _data?.GetHitSfx();
        if (sfx != null)
        {
            GamePoolManager.Instance.GetWorldAudioPrefab(sfx, transform.position);
        }
    }
}
```

---

## EnemyMovement

NavMesh-based navigation.

```csharp
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float pathUpdateInterval = 0.2f;
    
    private Transform _target;
    private float _lastPathUpdate;
    
    public void SetTarget(Transform target)
    {
        _target = target;
    }
    
    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }
    
    private void Update()
    {
        if (_target == null) return;
        
        if (Time.time >= _lastPathUpdate + pathUpdateInterval)
        {
            _lastPathUpdate = Time.time;
            agent.SetDestination(_target.position);
        }
    }
}
```

---

## EnemyAnimator

Animation state management.

```csharp
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Death = Animator.StringToHash("Death");
    
    public void SetMoving(bool moving)
    {
        animator.SetBool(IsMoving, moving);
    }
    
    public void PlayAttack()
    {
        animator.SetTrigger(Attack);
    }
    
    public void PlayHitReaction()
    {
        animator.SetTrigger(Hit);
    }
    
    public void PlayDeath()
    {
        animator.SetTrigger(Death);
    }
}
```

---

## Pooling Integration

Enemies are spawned and returned via GamePoolManager.

```csharp
// Spawn enemy
var enemyData = GameDatabases.EnemyDatabase.Get(enemyId);
var instance = GamePoolManager.Instance.GetEnemyPrefab(enemyData, spawnPoint.position, spawnPoint.rotation);
var controller = instance.GetComponent<EnemyController>();
controller.Initialize(enemyData);

// Return enemy (called on death)
GamePoolManager.Instance.ReturnEnemyPrefab(enemyData, gameObject);
```

---

## Event Integration

Enemies communicate through event channels.

```csharp
// Raised when enemy spawns
GameEvents.OnEnemySpawned.Raise(controller);

// Raised when enemy dies
GameEvents.OnEnemyDespawned.Raise(controller);

// Subscribers
waveManager.Subscribe(GameEvents.OnEnemyDespawned, HandleEnemyDeath);
scoreManager.Subscribe(GameEvents.OnEnemyDespawned, AwardPoints);
```

---

## Wave Spawning

WaveSpawner creates enemies during arena combat.

```csharp
public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxActiveEnemies = 8;
    
    public void SpawnWave(WaveData wave)
    {
        StartCoroutine(SpawnWaveCoroutine(wave));
    }
    
    private IEnumerator SpawnWaveCoroutine(WaveData wave)
    {
        foreach (var spawn in wave.spawns)
        {
            while (activeEnemies >= maxActiveEnemies)
                yield return null;
            
            SpawnEnemy(spawn.enemyData, GetRandomSpawnPoint());
            yield return new WaitForSeconds(spawn.delay);
        }
    }
}
```

---

## Creating New Enemies

1. Create EnemyData asset via Create menu
2. Configure stats, audio, effects
3. Create enemy prefab with:
   - EnemyController
   - EnemyHealth
   - EnemyMovement (with NavMeshAgent)
   - EnemyAnimator (with Animator)
   - EnemyAttack
   - Collider for damage detection
4. Assign prefab to EnemyData
5. Add to EnemyDatabase
6. Add to wave configurations
