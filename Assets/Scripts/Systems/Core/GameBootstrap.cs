using Constants;
using Databases;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [Header("Audio Events")] 
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onSfxRequested;
        
        [Header("Game Events")] 
        [SerializeField] private EnemyEventChannel onEnemySpawned;
        [SerializeField] private EnemyEventChannel onEnemyDespawned;
        [SerializeField] private GameStateEventChannel onGameStateChanged;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;
        [SerializeField] private VoidEventChannel onPauseRequested;
        [SerializeField] private VoidEventChannel onGameOverSequenceRequested;
        [SerializeField] private VoidEventChannel onGameWonSequenceRequested;

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
            GameEvents.OnGameStateChanged = onGameStateChanged;
            GameEvents.OnGameStateChangeRequested = onGameStateChangeRequested;
            GameEvents.OnPauseRequested = onPauseRequested;
            GameEvents.OnGameOverSequenceRequested = onGameOverSequenceRequested;
            GameEvents.OnGameWonSequenceRequested = onGameWonSequenceRequested;
        }

        private void SetDatabases()
        {
            if (audioDatabase != null)
                GameDatabases.AudioClipDatabase = audioDatabase;
            else
                Debug.LogError($"{nameof(audioDatabase)} not assigned in {name}", this);
            
            if (worldAudioDatabase != null)
                GameDatabases.WorldAudioDatabase = worldAudioDatabase;
            else 
                Debug.LogError($"{nameof(worldAudioDatabase)} not assigned in {name}", this);
                
            if (weaponDatabase != null)
                GameDatabases.WeaponDatabase = weaponDatabase;
            else
                Debug.LogError($"{nameof(weaponDatabase)} not assigned in {name}", this);
                
            if (enemyDatabase != null)
                GameDatabases.EnemyDatabase = enemyDatabase;
            else
                Debug.LogError($"{nameof(enemyDatabase)} not assigned in {name}", this);
            
            if (particleDatabase != null)
                GameDatabases.ParticleDatabase = particleDatabase;
            else
                Debug.LogError($"{nameof(particleDatabase)} not assigned in {name}", this);
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
            
            SetPlayerEvents();
            SetAudioEvents();
            SetGameEvents();
            SetDatabases();
            
            DontDestroyOnLoad(gameObject);
            
            _isInitialized = true;
            _isOwner = true;
        }

        private void Start()
        {
            SceneManager.LoadScene(GameConstants.StartMenu);
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