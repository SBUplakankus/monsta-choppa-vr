using System;
using System.Collections.Generic;
using Characters.Enemies;
using UnityEngine;

namespace Waves
{
    /// <summary>
    /// Defines the data for enemy types in a wave.
    /// </summary>
    [Serializable]
    public class WaveEnemyData
    {
        public EnemyData enemy;
        public int spawnAmount;
        public int spawnInterval;
    }
    
    /// <summary>
    /// Represents all enemy data for a single wave.
    /// </summary>
    [CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/Waves/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [SerializeField] private List<WaveEnemyData> waveData;
        
        public IReadOnlyList<WaveEnemyData> Wave =>  waveData;
        
        public bool IsValidWave()
        {
            foreach (var data in waveData)
            {
                if (data == null || data.spawnAmount <= 0 || data.spawnInterval <= 0)
                    return false;
            }
            return waveData.Count > 0;
        }

        public int EnemyCount
        {
            get
            {
                var count = 0;
                foreach (var data in waveData)
                    count+= data.spawnAmount;
                return count;
            }
        }
    }
}
