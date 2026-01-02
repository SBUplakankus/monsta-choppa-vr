using Attributes;
using Constants;
using Esper.ESave;
using Events;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Manages player attributes (gold, experience, level) with save/load functionality.
    /// Handles attribute changes through event subscriptions.
    /// </summary>
    public class PlayerAttributes  : MonoBehaviour
    {
        #region Fields
        
        [Header("Attributes")]
        [SerializeField] private IntAttribute playerGold;
        [SerializeField] private IntAttribute playerExperience;
        [SerializeField] private IntAttribute playerLevel;

        [Header("Events")] 
        [SerializeField] private VoidEventChannel onGameSaved;
        [SerializeField] private IntEventChannel onGoldChanged;
        [SerializeField] private IntEventChannel onExperienceChanged;
        [SerializeField] private IntEventChannel onLevelChanged;
        
        private SaveFile _saveFile;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets the current amount of player gold.
        /// </summary>
        /// <value>The gold amount from <see cref="playerGold"/>.</value>
        public int Gold => playerGold.Value;
        
        /// <summary>
        /// Gets the current amount of player experience.
        /// </summary>
        /// <value>The experience amount from <see cref="playerExperience"/>.</value>
        public int Experience => playerExperience.Value;
        
        /// <summary>
        /// Gets the current player level.
        /// </summary>
        /// <value>The level from <see cref="playerLevel"/>.</value>
        public int Level => playerLevel.Value;
        
        #endregion
        
        #region Class Functions

        private void SetDefaultValues()
        {
            playerGold.Value = 50;
            playerExperience.Value = 0;
            playerLevel.Value = 1;
        }
        
        private void SaveAttributes()
        {
            _saveFile.AddOrUpdateData(GameConstants.PlayerGoldKey, playerGold.Value);
            Debug.Log($"Saved {GameConstants.PlayerGoldKey}: {_saveFile.GetData(GameConstants.PlayerGoldKey, playerGold.Value)}");

            
            _saveFile.AddOrUpdateData(GameConstants.PlayerExperienceKey, playerExperience.Value);
            Debug.Log($"Saved {GameConstants.PlayerExperienceKey}: {_saveFile.GetData(GameConstants.PlayerExperienceKey, playerExperience.Value)}");

            
            _saveFile.AddOrUpdateData(GameConstants.PlayerLevelKey, playerLevel.Value);
            Debug.Log($"Saved {GameConstants.PlayerLevelKey}: {_saveFile.GetData(GameConstants.PlayerLevelKey,  playerLevel.Value)}");
        }

        private void LoadAttributes()
        {
            playerGold.Value = _saveFile.GetData(GameConstants.PlayerGoldKey, playerGold.Value);
            playerExperience.Value = _saveFile.GetData(GameConstants.PlayerExperienceKey, playerExperience.Value);
            playerLevel.Value = _saveFile.GetData(GameConstants.PlayerLevelKey,  playerLevel.Value);  
        }
        
        #endregion

        #region Event Handlers
        
        private void HandleGameSave() => SaveAttributes();
        private void HandleGoldChange(int amount) => playerGold.Add(amount);
        private void HandleExperienceChange(int amount) => playerExperience.Add(amount);
        private void HandleLevelChange(int amount) => playerLevel.Add(amount);
        
        private void SubscribeToEvents()
        {
            onGameSaved.Subscribe(HandleGameSave);
            onGoldChanged.Subscribe(HandleGoldChange);
            onExperienceChanged.Subscribe(HandleExperienceChange);
            onLevelChanged.Subscribe(HandleLevelChange);
        }

        private void UnsubscribeToEvents()
        {
            onGameSaved.Unsubscribe(HandleGameSave);
            onGoldChanged.Unsubscribe(HandleGoldChange);
            onExperienceChanged.Unsubscribe(HandleExperienceChange);
            onLevelChanged.Unsubscribe(HandleLevelChange);
        }

        #endregion
        
        #region Unity Functions

        private void Awake()
        {
            var setup = GetComponent<SaveFileSetup>();
            if (setup == null)
            {
                Debug.LogError($"No {nameof(SaveFileSetup)} found on {gameObject.name}");
                return;
            }
    
            _saveFile = setup.GetSaveFile();
            SetDefaultValues();
            LoadAttributes();
        }

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();
        
        #endregion
        
    }
}