using Constants;
using Data.Registries;
using Databases;
using Events;
using Events.Registries;
using Pooling;
using UnityEngine;

namespace Systems.Core
{
    /// <summary>
    /// Lightweight bootstrap for testing scenes directly in-editor.
    /// Instantly initializes core systems without loading screens or scene transitions.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class InstantBootstrapManager : MonoBehaviour
    {
        [Header("Core Dependencies")]
        [SerializeField] private GameDatabaseRegistry gameDatabaseRegistry;
        [SerializeField] private GamePoolManager gamePoolManager;

        private static bool _initialized;

        private void Awake()
        {
#if UNITY_EDITOR
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }

            _initialized = true;
            DontDestroyOnLoad(gameObject);

            Initialize();
#else
            Destroy(gameObject);
#endif
        }

        private void Initialize()
        {
            Debug.Log("InstantBootstrapManager: Initializing test bootstrap");

            // Databases
            if (gameDatabaseRegistry)
            {
                gameDatabaseRegistry.Validate();
                gameDatabaseRegistry.Install();
            }
            else
            {
                Debug.LogError("InstantBootstrapManager: GameDatabaseRegistry missing!");
            }

            // Pooling
            if (gamePoolManager)
            {
                gamePoolManager.Initialise();
            }
        }

        private void OnDestroy()
        {
            if (!_initialized) return;

            GameDatabases.Clear();
            AudioEvents.Clear();
            GameplayEvents.Clear();
            SystemEvents.Clear();
            UIEvents.Clear();
            _initialized = false;
        }
    }
}
