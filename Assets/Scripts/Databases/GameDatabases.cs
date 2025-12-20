using UnityEngine;

namespace Databases
{
    public static class GameDatabases
    {
         // Use readonly fields for thread safety and JIT optimization
        private static AudioClipDatabase _audioClipDatabase;
        private static WeaponDatabase _weaponDatabase;
        private static TMPFontDatabase _tmpFontDatabase;
        private static SpriteDatabase _spriteDatabase;
        private static EnemyDatabase _enemyDatabase;
        
        // Properties with null checks
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
        
        /// <summary>
        /// Clear all database references (useful for scene transitions or tests)
        /// </summary>
        public static void Clear()
        {
            _audioClipDatabase = null;
            _weaponDatabase = null;
            _tmpFontDatabase = null;
            _spriteDatabase = null;
            _enemyDatabase = null;
        }
    }
}
