using System;
using System.Collections;
using Data.Waves;
using Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Arena
{
    public enum WaveType
    {
        Inactive,
        Main,
        Boss
    }
    
    /// <summary>
    /// Manages the spawning of waves and boss entities in the arena.
    /// Handles enemy spawning, boss spawning, and cleanup of active entities.
    /// </summary>
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

        /// <summary>
        /// Raised when all enemies in the current wave are defeated.
        /// </summary>
        public event Action OnWaveEnemiesDefeated;

        /// <summary>
        /// Raised when the boss is defeated.
        /// </summary>
        public event Action OnBossDefeated;

        #endregion
        
        #region Spawner Logic

        /// <summary>
        /// Spawns a new wave of enemies.
        /// </summary>
        /// <param name="waveData">The wave data defining the type and number of enemies.</param>
        public void SpawnWave(WaveData waveData)
        {
            if(!_gamePoolManager)
                _gamePoolManager = GamePoolManager.Instance;
            
            _waveType = WaveType.Main;
            _enemiesRemaining = waveData.EnemyCount;
            StartCoroutine(SpawnEnemies(waveData));
        }

        /// <summary>
        /// Spawns the boss enemy.
        /// </summary>
        /// <param name="bossData">The wave data defining the boss enemy.</param>
        public void SpawnBoss(WaveData bossData)
        {
            _waveType = WaveType.Boss;
            _enemiesRemaining = 1;
            StartCoroutine(SpawnBossRoutine(bossData));
        }

        /// <summary>
        /// Tracks and handles the defeat of an enemy, updating the count and raising events if the wave ends.
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
        /// Cleans up all spawning activity and remaining enemies at the end of a wave.
        /// </summary>
        private void CleanUp()
        {
            _enemyManager.CleanupEnemies();
            _waveType = WaveType.Inactive;
            _enemiesRemaining = 0;
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Coroutine to spawn enemies at random spawn points based on the provided <see cref="WaveData"/>.
        /// </summary>
        /// <param name="waveData">The wave configuration data.</param>
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

        /// <summary>
        /// Coroutine to spawn the boss entity at the dedicated boss spawn point.
        /// </summary>
        /// <param name="bossData">The boss configuration data.</param>
        private IEnumerator SpawnBossRoutine(WaveData bossData)
        {
            if (bossData.Wave.Count <= 0) yield break;

            var bossEnemy = bossData.Wave[0]; // Assume boss data is the first entry
            yield return new WaitForSeconds(1f); // Optional boss intro delay
            _gamePoolManager.GetEnemyPrefab(bossEnemy.enemy, bossSpawnPoint.position, bossSpawnPoint.rotation);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets a random spawn point from the provided spawn points array.
        /// </summary>
        /// <returns>A random spawn point.</returns>
        private Transform GetRandomSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Initializes references for enemy management and game pooling.
        /// </summary>
        private void Awake()
        {
            _gamePoolManager = GamePoolManager.Instance;
            _enemyManager = GetComponent<EnemyManager>();
        }

        /// <summary>
        /// Subscribes to enemy death events on enable.
        /// </summary>
        private void OnEnable()
        {
            _enemyManager.OnEnemyDeath += HandleEnemyDeath;
            if (!_gamePoolManager)
                _gamePoolManager = GamePoolManager.Instance;
        }

        /// <summary>
        /// Unsubscribes from enemy death events on disable and cleans up enemies.
        /// </summary>
        private void OnDisable()
        {
            _enemyManager.OnEnemyDeath -= HandleEnemyDeath;
            CleanUp();
        }

        #endregion
    }
}