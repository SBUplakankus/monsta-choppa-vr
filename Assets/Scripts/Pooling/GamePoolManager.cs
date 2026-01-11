using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Audio;
using Characters.Enemies;
using Databases;
using NUnit.Framework;
using Systems;
using Visual_Effects;
using Weapons;

namespace Pooling
{
    /// <summary>
    /// Central manager for object pooling of game entities (enemies, weapons, particles, audio).
    /// Implements priority-based spawning through <see cref="VFXPriorityRouter"/> and <see cref="AudioPriorityRouter"/>.
    /// </summary>
    [RequireComponent(typeof(VFXPriorityRouter),  typeof(AudioPriorityRouter))]
    public class GamePoolManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// Gets the singleton instance of the GamePoolManager.
        /// </summary>
        /// <value>The global <see cref="GamePoolManager"/> instance.</value>
        public static GamePoolManager Instance { get; private set; }

        #endregion
        
        #region Performance Settings
        
        [Header("Pool Load Settings")]
        [SerializeField] private int prewarmCount = 5;
        private bool _isPrewarming;
        
        [Header("VR Performance Settings")]
        [SerializeField] private bool enableCollectionCheck;
        [SerializeField] private int poolSize = 50;
        [SerializeField] private int maxPoolSize = 100;
        
        private VFXPriorityRouter _vfxRouter;
        private AudioPriorityRouter _audioRouter;
        
        #endregion
        
        #region Pools
        
        private readonly Dictionary<EnemyData, ObjectPool<GameObject>> _enemyPoolDictionary = new();
        private readonly Dictionary<WeaponData, ObjectPool<GameObject>> _weaponPoolDictionary = new();
        private readonly Dictionary<ParticleData, ObjectPool<GameObject>> _particlePoolDictionary = new();
        private readonly Dictionary<WorldAudioData, ObjectPool<GameObject>> _worldAudioPoolDictionary = new();
        
        #endregion
        
        #region Pool Creation Methods
        
        private ObjectPool<GameObject> CreateEnemyPrefabPool(EnemyData data)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(data.Prefab);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnGet: OnGet,
                actionOnRelease: OnEnemyRelease,
                actionOnDestroy: Destroy,
                collectionCheck: enableCollectionCheck && Application.isEditor,
                defaultCapacity: poolSize,
                maxSize: maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateWeaponPrefabPool(WeaponData data)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(data.WeaponPrefab);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnGet: OnGet,
                actionOnRelease: OnWeaponRelease,
                actionOnDestroy: Destroy,
                collectionCheck: enableCollectionCheck && Application.isEditor,
                defaultCapacity: poolSize,
                maxSize: maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateParticlePool(ParticleData data)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(data.Prefab);
                    var particle = obj.GetComponent<ParticleController>();
                    particle.Initialise(data);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: Destroy,
                collectionCheck: enableCollectionCheck && Application.isEditor,
                defaultCapacity: poolSize,
                maxSize: maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateWorldAudioPool(WorldAudioData data)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(data.Prefab);
                    var controller = obj.GetComponent<WorldAudioController>();
                    controller.Initialise(data);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: Destroy,
                collectionCheck: enableCollectionCheck && Application.isEditor,
                defaultCapacity: poolSize,
                maxSize: maxPoolSize
            );
        }

        private void CreateEnemyPools()
        {
            foreach (var enemyData in GameDatabases.EnemyDatabase.Entries)
            {
                if (_enemyPoolDictionary.ContainsKey(enemyData)) continue;

                var pool = CreateEnemyPrefabPool(enemyData);
                _enemyPoolDictionary[enemyData] = pool;

                var temp = new List<GameObject>();
                for (var i = 0; i < prewarmCount; i++)
                    temp.Add(pool.Get());

                foreach (var obj in temp)
                    pool.Release(obj);
            }
        }

        private void CreateWeaponPools()
        {
            foreach (var weaponData in GameDatabases.WeaponDatabase.Entries)
            {
                if(_weaponPoolDictionary.ContainsKey(weaponData)) continue;

                var pool = CreateWeaponPrefabPool(weaponData);
                _weaponPoolDictionary[weaponData] = pool;
                
                var temp = new List<GameObject>();
                for (var i = 0; i < prewarmCount; i++)
                    temp.Add(pool.Get());

                foreach (var obj in temp)
                    pool.Release(obj);
            }
        }

        private void CreateParticlePools()
        {
            foreach (var particleData in GameDatabases.ParticleDatabase.Entries)
            {
                if(_particlePoolDictionary.ContainsKey(particleData)) continue;
                
                var pool = CreateParticlePool(particleData);
                _particlePoolDictionary[particleData] = pool;
                
                var temp = new List<GameObject>();
                for(var i = 0; i < prewarmCount; i++)
                    temp.Add(pool.Get());
                
                foreach (var obj in temp)
                    pool.Release(obj);
            }
        }

        private void CreateWorldAudioPools()
        {
            foreach (var worldAudioData in GameDatabases.WorldAudioDatabase.Entries)
            {
                if(_worldAudioPoolDictionary.ContainsKey(worldAudioData)) continue;
                
                var pool = CreateWorldAudioPool(worldAudioData);
                _worldAudioPoolDictionary[worldAudioData] = pool;

                var temp = new List<GameObject>();
                for(var i = 0; i < prewarmCount; i++)
                    temp.Add(pool.Get());
                
                foreach (var obj in temp)
                    pool.Release(obj);
            }
        }
        
        private void PrewarmPools()
        {
            _isPrewarming = true;
            CreateEnemyPools();
            CreateWeaponPools();
            CreateParticlePools();
            CreateWorldAudioPools();
            _isPrewarming = false;
        }
        
        #endregion
        
        #region Pool Access Methods

        private static void OnGet(GameObject obj) { }
        private static void OnRelease(GameObject obj) => obj.SetActive(false);

        private void OnEnemyRelease(GameObject obj)
        {
            if (!_isPrewarming)
                obj.GetComponent<EnemyController>()?.OnDespawn();
            
            obj.SetActive(false);
        }

        private void OnWeaponRelease(GameObject obj)
        {
            // if (!_isPrewarming)
                // TODO: Weapon Controller
                
            obj.SetActive(false);
        }

        private void OnParticleRelease(GameObject obj)
        {
            // TODO: Particle Controller Reset
            obj.SetActive(false);
        }
        
        /// <summary>
        /// Retrieves an enemy instance from the pool or creates a new one.
        /// </summary>
        /// <param name="data">The <see cref="EnemyData"/> defining the enemy type.</param>
        /// <param name="position">World position to spawn the enemy.</param>
        /// <param name="rotation">World rotation to spawn the enemy.</param>
        /// <returns>The spawned enemy GameObject, or null if pool not found.</returns>
        public GameObject GetEnemyPrefab(EnemyData data, Vector3 position, Quaternion rotation)
        {
            if (!_enemyPoolDictionary.TryGetValue(data, out var pool))
            {
                Debug.LogError($"No enemy pool found for {data.name}");
                return null;
            }

            var obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);

            var controller = obj.GetComponent<EnemyController>();
            controller.OnSpawn(data);
            
            obj.SetActive(true);
            return obj;
        }
        
        /// <summary>
        /// Returns an enemy instance to the pool for reuse.
        /// </summary>
        /// <param name="enemy">The <see cref="EnemyController"/> to return to the pool.</param>
        public void ReturnEnemyPrefab(EnemyController enemy)
        {
            var data = enemy.Data;

            if (_enemyPoolDictionary.TryGetValue(data, out var pool))
                pool.Release(enemy.gameObject);
            else
                Destroy(enemy.gameObject);
        }

        /// <summary>
        /// Retrieves a particle effect instance from the pool, checking priority limits.
        /// </summary>
        /// <param name="data">The <see cref="ParticleData"/> defining the effect.</param>
        /// <param name="position">World position to spawn the effect.</param>
        /// <param name="rotation">World rotation to spawn the effect.</param>
        /// <returns>The spawned particle GameObject, or null if priority limit reached.</returns>
        public GameObject GetParticlePrefab(ParticleData data, Vector3 position, Quaternion rotation)
        {
            if (!_vfxRouter.CanSpawn(data.Priority))
                return null;

            if (!_particlePoolDictionary.TryGetValue(data, out var pool))
                return null;

            _vfxRouter.RegisterSpawn();
            
            var obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);

            var controller = obj.GetComponent<ParticleController>();
            
            obj.SetActive(true);
            controller.Play();
            return obj;
        }

        /// <summary>
        /// Returns a particle effect instance to the pool for reuse.
        /// </summary>
        /// <param name="particle">The <see cref="ParticleController"/> to return to the pool.</param>
        public void ReturnParticlePrefab(ParticleController particle)
        {
            var data = particle.Data;
            _vfxRouter.RegisterDespawn();
            
            if (_particlePoolDictionary.TryGetValue(data, out var pool))
                pool.Release(particle.gameObject);
            else
                Destroy(particle.gameObject);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Retrieves a world audio instance from the pool, checking priority limits.
        /// </summary>
        /// <param name="data">The <see cref="WorldAudioData"/> defining the audio.</param>
        /// <param name="position">World position to play the audio.</param>
        /// <returns>The spawned audio GameObject, or null if priority limit reached.</returns>
        public GameObject GetWorldAudioPrefab(WorldAudioData data, Vector3 position)
        {
            if (!_audioRouter.CanSpawn(data.Priority))
                return null;

            if (!_worldAudioPoolDictionary.TryGetValue(data, out var pool))
                return null;

            _audioRouter.RegisterSpawn();
            
            var obj = pool.Get();
            var controller = obj.GetComponent<WorldAudioController>();
            
            obj.SetActive(true);
            controller.PlayAtPosition(position);
            return obj;
        }

        /// <summary>
        /// Returns a world audio instance to the pool for reuse.
        /// </summary>
        /// <param name="worldAudio">The <see cref="WorldAudioController"/> to return to the pool.</param>
        public void ReturnWorldAudioPrefab(WorldAudioController worldAudio)
        {
            var data = worldAudio.Data;
            _audioRouter.RegisterDespawn();
            
            if (_worldAudioPoolDictionary.TryGetValue(data, out var pool))
                pool.Release(worldAudio.gameObject);
            else
                Destroy(worldAudio.gameObject);
        }

        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                PrewarmPools();
                _vfxRouter = GetComponent<VFXPriorityRouter>();
                _audioRouter = GetComponent<AudioPriorityRouter>();
                
            }
            else
            {
                Destroy(gameObject);
            }
            
            if(!_vfxRouter)
                _vfxRouter = gameObject.AddComponent<VFXPriorityRouter>();
            
            if (!_audioRouter)
                _audioRouter = gameObject.AddComponent<AudioPriorityRouter>();
        }

        #endregion
    }
}