
namespace Databases
{
    public static class GameDatabases
    {
        #region Databases

        public static AudioClipDatabase AudioClipDatabase { get; internal set; }
        public static WorldAudioDatabase WorldAudioDatabase { get; internal set; }
        public static WeaponDatabase WeaponDatabase { get; internal set; }
        public static EnemyDatabase EnemyDatabase { get; internal set; }
        public static ParticleDatabase ParticleDatabase { get; internal set; }
        public static UpgradeDatabase UpgradeDatabase { get; internal set; }
        public static ArenaDatabase ArenaDatabase { get; internal set; }

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
