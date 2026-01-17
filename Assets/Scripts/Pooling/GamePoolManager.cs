using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Audio;
using Characters.Enemies;
using Databases;
using Systems;
using Visual_Effects;
using Weapons;

namespace Pooling
{
    [RequireComponent(typeof(VFXPriorityRouter), typeof(AudioPriorityRouter))]
    public class GamePoolManager : MonoBehaviour
    {
        #region Singleton

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

        #region Pool Roots (Persistent)

        private Transform _poolRoot;
        private Transform _enemyRoot;
        private Transform _weaponRoot;
        private Transform _particleRoot;
        private Transform _audioRoot;

        #endregion

        #region Pools

        private readonly Dictionary<EnemyData, ObjectPool<GameObject>> _enemyPoolDictionary = new();
        private readonly Dictionary<WeaponData, ObjectPool<GameObject>> _weaponPoolDictionary = new();
        private readonly Dictionary<ParticleData, ObjectPool<GameObject>> _particlePoolDictionary = new();
        private readonly Dictionary<WorldAudioData, ObjectPool<GameObject>> _worldAudioPoolDictionary = new();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreatePoolRoots();

            _vfxRouter = GetComponent<VFXPriorityRouter>() ?? gameObject.AddComponent<VFXPriorityRouter>();
            _audioRouter = GetComponent<AudioPriorityRouter>() ?? gameObject.AddComponent<AudioPriorityRouter>();

            PrewarmPools();
        }

        #endregion

        #region Pool Root Creation

        private void CreatePoolRoots()
        {
            _poolRoot = new GameObject("PoolRoot").transform;
            DontDestroyOnLoad(_poolRoot.gameObject);

            _enemyRoot = CreateChildRoot("Enemies");
            _weaponRoot = CreateChildRoot("Weapons");
            _particleRoot = CreateChildRoot("Particles");
            _audioRoot = CreateChildRoot("WorldAudio");
        }

        private Transform CreateChildRoot(string rootName)
        {
            var go = new GameObject(rootName);
            go.transform.SetParent(_poolRoot);
            return go.transform;
        }

        #endregion

        #region Pool Creation

        private ObjectPool<GameObject> CreateEnemyPrefabPool(EnemyData data)
        {
            return new ObjectPool<GameObject>(
                () =>
                {
                    var obj = Instantiate(data.Prefab, _enemyRoot);
                    obj.SetActive(false);
                    return obj;
                },
                OnGet,
                OnEnemyRelease,
                Destroy,
                enableCollectionCheck && Application.isEditor,
                poolSize,
                maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateWeaponPrefabPool(WeaponData data)
        {
            return new ObjectPool<GameObject>(
                () =>
                {
                    var obj = Instantiate(data.WeaponPrefab, _weaponRoot);
                    obj.SetActive(false);
                    return obj;
                },
                OnGet,
                OnWeaponRelease,
                Destroy,
                enableCollectionCheck && Application.isEditor,
                poolSize,
                maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateParticlePool(ParticleData data)
        {
            return new ObjectPool<GameObject>(
                () =>
                {
                    var obj = Instantiate(data.Prefab, _particleRoot);
                    obj.GetComponent<ParticleController>()?.Initialise(data);
                    obj.SetActive(false);
                    return obj;
                },
                OnGet,
                OnParticleRelease,
                Destroy,
                enableCollectionCheck && Application.isEditor,
                poolSize,
                maxPoolSize
            );
        }

        private ObjectPool<GameObject> CreateWorldAudioPool(WorldAudioData data)
        {
            return new ObjectPool<GameObject>(
                () =>
                {
                    var obj = Instantiate(data.Prefab, _audioRoot);
                    obj.GetComponent<WorldAudioController>()?.Initialise(data);
                    obj.SetActive(false);
                    return obj;
                },
                OnGet,
                OnRelease,
                Destroy,
                enableCollectionCheck && Application.isEditor,
                poolSize,
                maxPoolSize
            );
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

        #region Pool Initialisation

        private void CreateEnemyPools()
        {
            foreach (var data in GameDatabases.EnemyDatabase.Entries)
                Prewarm(_enemyPoolDictionary, data, CreateEnemyPrefabPool);
        }

        private void CreateWeaponPools()
        {
            foreach (var data in GameDatabases.WeaponDatabase.Entries)
                Prewarm(_weaponPoolDictionary, data, CreateWeaponPrefabPool);
        }

        private void CreateParticlePools()
        {
            foreach (var data in GameDatabases.ParticleDatabase.Entries)
                Prewarm(_particlePoolDictionary, data, CreateParticlePool);
        }

        private void CreateWorldAudioPools()
        {
            foreach (var data in GameDatabases.WorldAudioDatabase.Entries)
                Prewarm(_worldAudioPoolDictionary, data, CreateWorldAudioPool);
        }

        private void Prewarm<T>(
            Dictionary<T, ObjectPool<GameObject>> dict,
            T data,
            System.Func<T, ObjectPool<GameObject>> factory)
        {
            if (dict.ContainsKey(data))
                return;

            var pool = factory(data);
            dict[data] = pool;

            var temp = new List<GameObject>();
            for (int i = 0; i < prewarmCount; i++)
                temp.Add(pool.Get());

            foreach (var obj in temp)
                pool.Release(obj);
        }

        #endregion

        #region Pool Access

        private static void OnGet(GameObject obj) { }

        private static void OnRelease(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void OnEnemyRelease(GameObject obj)
        {
            if (!_isPrewarming)
                obj.GetComponent<EnemyController>()?.OnDespawn();

            obj.transform.SetParent(_enemyRoot);
            obj.SetActive(false);
        }

        private void OnWeaponRelease(GameObject obj)
        {
            obj.transform.SetParent(_weaponRoot);
            obj.SetActive(false);
        }

        private void OnParticleRelease(GameObject obj)
        {
            obj.transform.SetParent(_particleRoot);
            obj.SetActive(false);
        }

        #endregion

        #region Public API

        public GameObject GetEnemyPrefab(EnemyData data, Vector3 position, Quaternion rotation)
        {
            if (!_enemyPoolDictionary.TryGetValue(data, out var pool))
                return null;

            var obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.GetComponent<EnemyController>()?.OnSpawn(data);
            obj.SetActive(true);
            return obj;
        }

        public void ReturnEnemyPrefab(EnemyController enemy)
        {
            if (!enemy) return;

            if (_enemyPoolDictionary.TryGetValue(enemy.Data, out var pool))
                pool.Release(enemy.gameObject);
            else
                Destroy(enemy.gameObject);
        }

        public GameObject GetParticlePrefab(ParticleData data, Vector3 position, Quaternion rotation)
        {
            if (!_vfxRouter.CanSpawn(data.Priority))
                return null;

            if (!_particlePoolDictionary.TryGetValue(data, out var pool))
                return null;

            _vfxRouter.RegisterSpawn();

            var obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            obj.GetComponent<ParticleController>()?.Play();
            return obj;
        }

        public void ReturnParticlePrefab(ParticleController particle)
        {
            _vfxRouter.RegisterDespawn();

            if (_particlePoolDictionary.TryGetValue(particle.Data, out var pool))
                pool.Release(particle.gameObject);
            else
                Destroy(particle.gameObject);
        }

        public GameObject GetWorldAudioPrefab(WorldAudioData data, Vector3 position)
        {
            if (!_audioRouter.CanSpawn(data.Priority))
                return null;

            if (!_worldAudioPoolDictionary.TryGetValue(data, out var pool))
                return null;

            _audioRouter.RegisterSpawn();

            var obj = pool.Get();
            obj.SetActive(true);
            obj.GetComponent<WorldAudioController>()?.PlayAtPosition(position);
            return obj;
        }

        public void ReturnWorldAudioPrefab(WorldAudioController worldAudio)
        {
            _audioRouter.RegisterDespawn();

            if (_worldAudioPoolDictionary.TryGetValue(worldAudio.Data, out var pool))
                pool.Release(worldAudio.gameObject);
            else
                Destroy(worldAudio.gameObject);
        }

        #endregion
    }
}
