using Databases;
using UnityEngine;

namespace Data.Registries
{
    [CreateAssetMenu(fileName = "GameDatabaseRegistry", menuName = "Scriptable Objects/Registries/Databases")]
    public class GameDatabaseRegistry : ScriptableObject
    {
        [Header("Audio")]
        public AudioClipDatabase audioClipDatabase;
        public WorldAudioDatabase worldAudioDatabase;

        [Header("Gameplay")]
        public WeaponDatabase weaponDatabase;
        public EnemyDatabase enemyDatabase;
        public UpgradeDatabase upgradeDatabase;
        public ArenaDatabase arenaDatabase;

        [Header("VFX")]
        public ParticleDatabase particleDatabase;

        public void Validate()
        {
            Debug.Assert(audioClipDatabase, "audioClipDatabase missing", this);
            Debug.Assert(worldAudioDatabase, "worldAudioDatabase missing", this);
            Debug.Assert(weaponDatabase, "weaponDatabase missing", this);
            Debug.Assert(enemyDatabase, "enemyDatabase missing", this);
            Debug.Assert(particleDatabase, "particleDatabase missing", this);
            Debug.Assert(arenaDatabase, "arenaDatabase missing", this);
            Debug.Assert(upgradeDatabase, "upgradeDatabase missing", this);
        }

        public void Install()
        {
            GameDatabases.AudioClipDatabase = audioClipDatabase;
            GameDatabases.WorldAudioDatabase = worldAudioDatabase;
            GameDatabases.WeaponDatabase = weaponDatabase;
            GameDatabases.EnemyDatabase = enemyDatabase;
            GameDatabases.ParticleDatabase = particleDatabase;
            GameDatabases.ArenaDatabase = arenaDatabase;
            GameDatabases.UpgradeDatabase = upgradeDatabase;
        }
    }
}