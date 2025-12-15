using System;
using Events.Data;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using PrimeTween;

namespace Pooling
{
    public class GamePoolManager : MonoBehaviour
    {
        #region Singleton

        public static GamePoolManager Instance { get; private set; }

        #endregion
        
        #region Performance Settings
        [Header("VR Performance Settings")]
        [SerializeField] private bool enableCollectionCheck = false; // Keep false for builds!
        [SerializeField] private int damagePoolSize = 50;
        [SerializeField] private int controllerPoolSize = 100; // Higher for frequent events
        #endregion
        
        #region Pools
        private ObjectPool<DamageEventData> damagePool;
        
        private readonly Dictionary<Type, object> poolDictionary = new();
        #endregion
        
        #region Methods
        private void InitPools()
        {
            // Damage Event Pool - medium frequency
            damagePool = new ObjectPool<DamageEventData>(
                createFunc: () => new DamageEventData(),
                actionOnGet: null, // No extra work on get
                actionOnRelease: data => data.Reset(),
                actionOnDestroy: null, // No destruction logging
                collectionCheck: enableCollectionCheck && Application.isEditor, // Editor only!
                defaultCapacity: damagePoolSize,
                maxSize: damagePoolSize * 10 // Allow growth but with limit
            );
            
            // Register pools for easy access
            poolDictionary[typeof(DamageEventData)] = damagePool;
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitPools();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Public access methods
        public DamageEventData GetDamageEvent() => damagePool.Get();
        public void ReleaseDamageEvent(DamageEventData data) => damagePool.Release(data);
        
        // Generic method for any pool
        public T GetFromPool<T>() where T : class, new()
        {
            if (poolDictionary.TryGetValue(typeof(T), out var poolObj))
            {
                return ((ObjectPool<T>)poolObj).Get();
            }
            
            // Auto-create pool if it doesn't exist (lazy initialization)
            Debug.LogWarning($"Pool for {typeof(T).Name} not found, creating default");
            return CreateDefaultPool<T>().Get();
        }
        
        private ObjectPool<T> CreateDefaultPool<T>() where T : class, new()
        {
            var pool = new ObjectPool<T>(
                createFunc: () => new T(),
                actionOnGet: null,
                actionOnRelease: null,
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 20,
                maxSize: 200
            );
            
            poolDictionary[typeof(T)] = pool;
            return pool;
        }
        
        // Monitor pool usage (call occasionally, not every frame)
        public void LogPoolStats()
        {
            Debug.Log($"Damage Pool: {damagePool.CountActive} active, {damagePool.CountInactive} available");
            
            // VR Performance warning
            if (damagePool.CountAll > damagePoolSize * 8)
                Debug.LogWarning("Damage pool growing large - check for missing Release() calls");
        }
        #endregion
    }
}