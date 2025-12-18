using Attributes;
using Constants;
using Esper.ESave;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerAttributes  : MonoBehaviour
    {
        #region Fields
        
        [Header("Attributes")]
        [SerializeField] private IntAttribute playerGold;
        [SerializeField] private IntAttribute playerExperience;
        [SerializeField] private IntAttribute playerLevel;

        [Header("Events")] 
        [SerializeField] private VoidEventChannel saveGameEvent;
        
        private SaveFile _saveFile;
        
        #endregion
        
        #region Properties
        
        public int Gold => playerGold.Value;
        public int Experience => playerExperience.Value;
        public int Level => playerLevel.Value;
        
        #endregion
        
        #region Class Functions
        
        private void SaveAttributes()
        {
            _saveFile.AddOrUpdateData(GameConstants.PlayerGoldKey, playerGold.Value);
            _saveFile.AddOrUpdateData(GameConstants.PlayerExperienceKey, playerExperience.Value);
            _saveFile.AddOrUpdateData(GameConstants.PlayerLevelKey, playerLevel.Value);
        }

        private void LoadAttributes()
        {
            playerGold.Value = _saveFile.GetData(GameConstants.PlayerGoldKey, 0);
            playerExperience.Value = _saveFile.GetData(GameConstants.PlayerExperienceKey, 0);
            playerLevel.Value = _saveFile.GetData(GameConstants.PlayerLevelKey, 1);
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
            LoadAttributes();
        }

        private void OnEnable()
        {
            saveGameEvent.Subscribe(SaveAttributes);
        }

        private void OnDisable()
        {
            saveGameEvent.Unsubscribe(SaveAttributes);
        }
        
        #endregion
        
    }
}
