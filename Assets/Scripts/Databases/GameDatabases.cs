
namespace Databases
{
    public static class GameDatabases
    {
        #region Databases

        public static AudioClipDatabase AudioClipDatabase { get; set; }
        public static WorldAudioDatabase WorldAudioDatabase { get; set; }
        public static WeaponDatabase WeaponDatabase { get; set; }
        public static EnemyDatabase EnemyDatabase { get; set; }
        public static ParticleDatabase ParticleDatabase { get; set; }
        public static UpgradeDatabase UpgradeDatabase { get; set; }
        public static ArenaDatabase ArenaDatabase { get; set; }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Clear all database references (useful for scene transitions or tests)
        /// </summary>
        public static void Clear()
        {
            AudioClipDatabase = null;
            WorldAudioDatabase = null;
            WeaponDatabase = null;
            EnemyDatabase = null;
            ParticleDatabase = null;
            ArenaDatabase = null;
            UpgradeDatabase = null;
        }
        
        #endregion
    }
}
