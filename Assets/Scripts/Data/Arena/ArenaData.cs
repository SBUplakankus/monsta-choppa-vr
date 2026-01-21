using System;
using Constants;
using Data.Core;
using Data.Waves;
using Databases;
using UnityEngine;

namespace Data.Arena
{
    public enum ArenaLocation
    {
        GoblinCamp,
        CliffPass,
        DwarfHall
    }

    public enum ArenaDifficulty
    {
        Daytime,
        Dusk,
        Midnight
    }

    public enum ArenaBoss
    {
        Goblin,
        Ork,
        Skeleton
    }
    
    [CreateAssetMenu(fileName = "ArenaData", menuName = "Scriptable Objects/Arena/Arena")]
    public class ArenaData : ScriptableObject
    {
        #region Fields

        [Header("Arena Settings")]
        [SerializeField] private ArenaLocation location = ArenaLocation.GoblinCamp;
        [SerializeField] private ArenaDifficulty difficulty = ArenaDifficulty.Daytime;
        [SerializeField] private ArenaBoss boss = ArenaBoss.Goblin;
        [SerializeField] private ArenaWavesData arenaWavesData;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioClipData ambience;
        [SerializeField] private AudioClipData intermissionMusic;
        [SerializeField] private AudioClipData waveMusic;
        [SerializeField] private AudioClipData bossMusic;
        
        [Header("Scene Settings")]
        [SerializeField] private string sceneName;

        #endregion

        #region Properties

        public string ID => name;
        public ArenaLocation Location => location;
        public ArenaDifficulty Difficulty => difficulty;
        public ArenaBoss Boss => boss;
        
        
        public string LocationKey => GetLocationKey();
        public string DifficultyKey => GetDifficultyKey();
        public string BossKey => GetBossKey();
        
        public ArenaWavesData WavesData => arenaWavesData;
        
        public string Ambience => ambience.ID;
        public string IntermissionMusicKey => intermissionMusic.ID;
        public string WaveMusicKey => waveMusic.ID;
        public string BossMusicKey => bossMusic.ID;
        public string Scene => sceneName; 

        #endregion
        
        #region Methods
        
        private string GetLocationKey()
        {
            return location switch
            {
                ArenaLocation.GoblinCamp => LocalizationKeys.GoblinCamp,
                ArenaLocation.CliffPass => LocalizationKeys.CliffPass,
                ArenaLocation.DwarfHall => LocalizationKeys.DwarfHall,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetDifficultyKey()
        {
            return difficulty switch
            {
                ArenaDifficulty.Daytime => LocalizationKeys.Daytime,
                ArenaDifficulty.Dusk => LocalizationKeys.Dusk,
                ArenaDifficulty.Midnight => LocalizationKeys.Midnight,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetBossKey()
        {
            return boss switch
            {
                ArenaBoss.Goblin => LocalizationKeys.GoblinBoss,
                ArenaBoss.Ork => LocalizationKeys.OrkBoss,
                ArenaBoss.Skeleton => LocalizationKeys.SkeletonBoss,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        #endregion
    }
}
