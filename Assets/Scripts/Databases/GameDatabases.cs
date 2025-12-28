using UnityEngine;

namespace Databases
{
    public static class GameDatabases
    {
        #region Databases

        private static AudioClipDatabase _audioClipDatabase;
        private static WorldAudioDatabase _worldAudioDatabase; 
        private static WeaponDatabase _weaponDatabase;
        private static TMPFontDatabase _tmpFontDatabase;
        private static SpriteDatabase _spriteDatabase;
        private static EnemyDatabase _enemyDatabase;
        private static ParticleDatabase _particleDatabase;

        #endregion
        
        #region Properties
        
        public static AudioClipDatabase AudioClipDatabase 
        {
            get
            {
                if (_audioClipDatabase == null)
                    Debug.LogError($"{nameof(AudioClipDatabase)} not initialized!");
                return _audioClipDatabase;
            }
            internal set => _audioClipDatabase = value;
        }
        
        public static WorldAudioDatabase WorldAudioDatabase 
        {
            get
            {
                if (_worldAudioDatabase == null)
                    Debug.LogError($"{nameof(WorldAudioDatabase)} not initialized!");
                return _worldAudioDatabase;
            }
            internal set => _worldAudioDatabase = value;
        }
        
        public static WeaponDatabase WeaponDatabase 
        {
            get
            {
                if (_weaponDatabase == null)
                    Debug.LogError($"{nameof(WeaponDatabase)} not initialized!");
                return _weaponDatabase;
            }
            internal set => _weaponDatabase = value;
        }
        
        public static TMPFontDatabase TMPFontDatabase 
        {
            get
            {
                if (_tmpFontDatabase == null)
                    Debug.LogError($"{nameof(TMPFontDatabase)} not initialized!");
                return _tmpFontDatabase;
            }
            internal set => _tmpFontDatabase = value;
        }
        
        public static SpriteDatabase SpriteDatabase 
        {
            get
            {
                if (_spriteDatabase == null)
                    Debug.LogError($"{nameof(SpriteDatabase)} not initialized!");
                return _spriteDatabase;
            }
            internal set => _spriteDatabase = value;
        }
        
        public static EnemyDatabase EnemyDatabase 
        {
            get
            {
                if (_enemyDatabase == null)
                    Debug.LogError($"{nameof(EnemyDatabase)} not initialized!");
                return _enemyDatabase;
            }
            internal set => _enemyDatabase = value;
        }

        public static ParticleDatabase ParticleDatabase
        {
            get
            {
                if (_particleDatabase == null)
                    Debug.LogError($"{nameof(ParticleDatabase)} not initialized!");
                return _particleDatabase;
            }
            internal set => _particleDatabase = value;
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Clear all database references (useful for scene transitions or tests)
        /// </summary>
        public static void Clear()
        {
            _audioClipDatabase = null;
            _worldAudioDatabase = null;
            _weaponDatabase = null;
            _tmpFontDatabase = null;
            _spriteDatabase = null;
            _enemyDatabase = null;
            _particleDatabase = null;
        }
        
        #endregion
    }
}
