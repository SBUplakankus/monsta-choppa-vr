using Databases;
using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems
{
    // Add execution order to ensure this runs before other scripts
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        #region Fields
        
        [Header("Player Events")] 
        [SerializeField] private IntEventChannel onPlayerDamaged;
        [SerializeField] private IntEventChannel onGoldChanged;
        [SerializeField] private IntEventChannel onExperienceChanged;
        [SerializeField] private IntEventChannel onLevelChanged;

        [Header("Game Events")] 
        [SerializeField] private StringEventChannel onEnemyDeath;
        
        [Header("Audio Events")]
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onSfxRequested;
        
        [Header("Databases")]
        [SerializeField] private AudioClipDatabase audioDatabase;
        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private EnemyDatabase enemyDatabase;
        [SerializeField] private SpriteDatabase spriteDatabase; // Added missing database
        [SerializeField] private TMPFontDatabase tmpFontDatabase; // Added missing database
        
        [Header("UI Toolkit")]
        [SerializeField] private StyleSheet styleSheet;
        
        private static bool _isInitialized = false;

        #endregion
        
        #region Class Methods
        
        private void SetPlayerEvents()
        {
            GameEvents.OnPlayerDamaged =  onPlayerDamaged;
            GameEvents.OnGoldChanged =  onGoldChanged;
            GameEvents.OnExperienceGained =  onExperienceChanged;
            GameEvents.OnLevelChanged =  onLevelChanged;
        }

        private void SetGameEvents()
        {
            GameEvents.OnEnemyDeath = onEnemyDeath;
        }

        private void SetAudioEvents()
        {
            GameEvents.OnMusicRequested = onMusicRequested;
            GameEvents.OnSfxRequested = onSfxRequested;
        }

        private void SetDatabases()
        {
            if (audioDatabase != null)
                GameDatabases.AudioClipDatabase = audioDatabase;
            else
                Debug.LogError($"{nameof(audioDatabase)} not assigned in {name}", this);
                
            if (weaponDatabase != null)
                GameDatabases.WeaponDatabase = weaponDatabase;
            else
                Debug.LogError($"{nameof(weaponDatabase)} not assigned in {name}", this);
                
            if (enemyDatabase != null)
                GameDatabases.EnemyDatabase = enemyDatabase;
            else
                Debug.LogError($"{nameof(enemyDatabase)} not assigned in {name}", this);
            
            if (spriteDatabase != null)
                GameDatabases.SpriteDatabase = spriteDatabase;
                
            if (tmpFontDatabase != null)
                GameDatabases.TMPFontDatabase = tmpFontDatabase;
        }
        
        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            // Prevent duplicate initialization across scenes
            if (_isInitialized)
            {
                Destroy(gameObject);
                return;
            }
            
            // Optional: Make persistent across scenes
            // DontDestroyOnLoad(gameObject);
            
            SetPlayerEvents();
            SetGameEvents();
            SetAudioEvents();
            SetDatabases();
            
            _isInitialized = true;
        }
        
        private void OnDestroy()
        {
            if (!_isInitialized) return;
            
            // Clear references when destroyed
            GameDatabases.Clear();
            _isInitialized = false;
        }
        
        #endregion
    }
}
