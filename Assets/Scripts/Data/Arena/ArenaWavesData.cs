using System.Collections.Generic;
using Data.Waves;
using UnityEngine;

namespace Data.Arena
{
    /// <summary>
    /// Represents a collection of waves for an arena or level.
    /// </summary>
    [CreateAssetMenu(fileName = "ArenaWavesData", menuName = "Scriptable Objects/Data/Arena/Arena Waves")]
    public class ArenaWavesData : ScriptableObject
    {
        [SerializeField] private List<WaveData> arenaWaves;
        [SerializeField] private WaveData arenaBoss;
        
        public IReadOnlyList<WaveData> Waves => arenaWaves;
        public WaveData Boss => arenaBoss;
        public int WaveCount => arenaWaves.Count;
    }
}
