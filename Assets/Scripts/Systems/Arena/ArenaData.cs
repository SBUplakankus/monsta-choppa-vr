using System;
using Constants;
using UnityEngine;

namespace Systems.Arena
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
    
    [CreateAssetMenu(fileName = "ArenaData", menuName = "Scriptable Objects/ArenaData")]
    public class ArenaData : ScriptableObject
    {
        [Header("Arena Settings")]
        [SerializeField] private ArenaLocation location = ArenaLocation.GoblinCamp;
        [SerializeField] private ArenaDifficulty difficulty = ArenaDifficulty.Daytime;
        [SerializeField] private ArenaBoss boss = ArenaBoss.Goblin;
        
        public string Location => GetLocationKey();
        public string Difficulty => GetDifficultyKey();
        public string Boss => GetBossKey();

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
    }
}
