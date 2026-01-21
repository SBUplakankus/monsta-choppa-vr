using Constants;
using Databases;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Systems.Core
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

        [Header("Audio Events")] 
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onSfxRequested;
        
        [Header("Game Events")] 
        [SerializeField] private VoidEventChannel onPauseRequested;
        [SerializeField] private VoidEventChannel onGameOverSequenceRequested;
        [SerializeField] private VoidEventChannel onGameWonSequenceRequested;
        
        [Header("Arena Events")]
        [SerializeField] private EnemyEventChannel onEnemySpawned;
        [SerializeField] private EnemyEventChannel onEnemyDespawned;
        [SerializeField] private ArenaStateEventChannel onArenaStateChanged;
        [SerializeField] private ArenaStateEventChannel onArenaStateChangeRequested;
        
        [Header("Databases")]
        [SerializeField] private AudioClipDatabase audioDatabase;
        [SerializeField] private WorldAudioDatabase worldAudioDatabase;
        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private EnemyDatabase enemyDatabase;
        [SerializeField] private ParticleDatabase particleDatabase;
        
        [Header("UI Toolkit")]
        [SerializeField] private StyleSheet styleSheet;
        
        private static bool _isInitialized;
        private bool _isOwner;

        #endregion
        
        #region Class Methods
        
        private void SetPlayerEvents()
        {
            GameEvents.OnPlayerDamaged = onPlayerDamaged;
            GameEvents.OnGoldChanged = onGoldChanged;
            GameEvents.OnExperienceGained = onExperienceChanged;
            GameEvents.OnLevelChanged = onLevelChanged;
        }

        private void SetAudioEvents()
        {
            GameEvents.OnMusicRequested = onMusicRequested;
            GameEvents.OnSfxRequested = onSfxRequested;
        }

        private void SetGameEvents()
        {
            GameEvents.OnEnemySpawned = onEnemySpawned;
            GameEvents.OnEnemyDespawned = onEnemyDespawned;
            GameEvents.OnArenaStateChanged = onArenaStateChanged;
            GameEvents.OnArenaStateChangeRequested = onArenaStateChangeRequested;
            GameEvents.OnPauseRequested = onPauseRequested;
            GameEvents.OnGameOverSequenceRequested = onGameOverSequenceRequested;
            GameEvents.OnGameWonSequenceRequested = onGameWonSequenceRequested;
        }

        private void SetDatabases()
        {
            if (audioDatabase)
                GameDatabases.AudioClipDatabase = audioDatabase;
            else
                Debug.LogError($"{nameof(audioDatabase)} not assigned in {name}", this);
            
            if (worldAudioDatabase)
                GameDatabases.WorldAudioDatabase = worldAudioDatabase;
            else 
                Debug.LogError($"{nameof(worldAudioDatabase)} not assigned in {name}", this);
                
            if (weaponDatabase)
                GameDatabases.WeaponDatabase = weaponDatabase;
            else
                Debug.LogError($"{nameof(weaponDatabase)} not assigned in {name}", this);
                
            if (enemyDatabase)
                GameDatabases.EnemyDatabase = enemyDatabase;
            else
                Debug.LogError($"{nameof(enemyDatabase)} not assigned in {name}", this);
            
            if (particleDatabase)
                GameDatabases.ParticleDatabase = particleDatabase;
            else
                Debug.LogError($"{nameof(particleDatabase)} not assigned in {name}", this);
        }

        public void Initialize()
        {
            SetPlayerEvents();
            SetAudioEvents();
            SetGameEvents();
            SetDatabases();
            
            _isInitialized = true;
            _isOwner = true;
        }
        
        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            if (_isInitialized)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }

        
        private void Start()
        {
            if (!Application.isEditor)
            {
                SceneManager.LoadScene(GameConstants.StartMenu);
            }
        }
        
        private void OnDestroy()
        {
            if (!_isOwner)
                return;
            
            GameDatabases.Clear();
            _isInitialized = false;
        }
        
        #endregion
    }
}