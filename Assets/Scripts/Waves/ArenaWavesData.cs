using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    /// <summary>
    /// Represents a collection of waves for an arena or level.
    /// </summary>
    [CreateAssetMenu(fileName = "ArenaWavesData", menuName = "Scriptable Objects/Waves/Arena Waves")]
    public class ArenaWavesData : ScriptableObject
    {
        [SerializeField] private List<WaveData> arenaWaves;
        [SerializeField] private WaveData arenaBoss;
        
        public IReadOnlyList<WaveData> Waves => arenaWaves;
        public WaveData Boss => arenaBoss;
        public int WaveCount => arenaWaves.Count;
    }
}
