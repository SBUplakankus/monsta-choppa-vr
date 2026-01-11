using System;
using System.Collections;
using Pooling;
using UnityEngine;
using Waves;
using Random = UnityEngine.Random;

namespace Systems.Arena
{
    public enum WaveType
    {
        Inactive,
        Main,
        Boss
    }
    
    [RequireComponent(typeof(EnemyManager))]
    public class WaveSpawner : MonoBehaviour
    {
        #region Fields
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform bossSpawnPoint;

        [Header("Spawn Parameters")]
        [SerializeField] private int maxEnemies = 6;

        private EnemyManager _enemyManager;
        private GamePoolManager _gamePoolManager;

        private WaveType _waveType;
        private int _enemiesRemaining;
        public event Action OnWaveEnemiesDefeated;
        public event Action OnBossDefeated;

        #endregion
        
        #region Spawner Logic

        /// <summary>
        /// Spawns a new wave of enemies.
        /// </summary>
        public void SpawnWave(WaveData waveData)
        {
            _waveType = WaveType.Main;
            _enemiesRemaining = waveData.EnemyCount;
            StartCoroutine(SpawnEnemies(waveData));
        }

        /// <summary>
        /// Spawns the boss.
        /// </summary>
        public void SpawnBoss(WaveData bossData)
        {
            _waveType = WaveType.Boss;
            _enemiesRemaining = 1;
            StartCoroutine(SpawnBossRoutine(bossData));
        }

        /// <summary>
        /// Tracks and responds to enemy deaths, monitoring wave progress.
        /// </summary>
        private void HandleEnemyDeath()
        {
            _enemiesRemaining--;
            if (_enemiesRemaining > 0) return;

            switch (_waveType)
            {
                case WaveType.Main:
                    OnWaveEnemiesDefeated?.Invoke();
                    break;
                case WaveType.Boss:
                    OnBossDefeated?.Invoke();
                    break;
            }

            CleanUp();
        }

        /// <summary>
        /// Cleans up all spawning activity and remaining enemies.
        /// </summary>
        private void CleanUp()
        {
            _enemyManager.CleanupEnemies();

            _waveType = WaveType.Inactive;
            _enemiesRemaining = 0;
        }

        #endregion

        #region Coroutines

        private IEnumerator SpawnEnemies(WaveData waveData)
        {
            foreach (var enemyData in waveData.Wave)
            {
                for (var i = 0; i < enemyData.spawnAmount; i++)
                {
                    while (_enemyManager.ActiveEnemiesCount >= maxEnemies)
                        yield return null;

                    var spawnPoint = GetRandomSpawnPoint();
                    _gamePoolManager.GetEnemyPrefab(enemyData.enemy, spawnPoint.position, spawnPoint.rotation);

                    yield return new WaitForSeconds(enemyData.spawnInterval);
                }
            }
        }

        private IEnumerator SpawnBossRoutine(WaveData bossData)
        {
            if (bossData.Wave.Count <= 0) yield break;

            var bossEnemy = bossData.Wave[0]; // Assume boss data is the first entry
            yield return new WaitForSeconds(1f); // Optional boss intro delay
            _gamePoolManager.GetEnemyPrefab(bossEnemy.enemy, bossSpawnPoint.position, bossSpawnPoint.rotation);
        }

        #endregion

        #region Helpers

        private Transform GetRandomSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _gamePoolManager = GamePoolManager.Instance;
            _enemyManager = GetComponent<EnemyManager>();
        }

        private void OnEnable()
        {
            _enemyManager.OnEnemyDeath += HandleEnemyDeath;
            if (!_gamePoolManager)
                _gamePoolManager = GamePoolManager.Instance;
        }

        private void OnDisable()
        {
            _enemyManager.OnEnemyDeath -= HandleEnemyDeath;
            CleanUp();
        }

        #endregion
    }
}