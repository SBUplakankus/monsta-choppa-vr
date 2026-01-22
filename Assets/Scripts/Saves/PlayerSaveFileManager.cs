using Constants;
using Data.Progression;
using Events;
using UnityEngine;

namespace Saves
{
    public class PlayerSaveFileManager : SaveFileManagerBase
    {
        #region Fields

        [Header("Player Data Objects")]
        [SerializeField] private MetaProgressionData metaProgressionData;
        
        [Header("Save Events")] 
        private readonly VoidEventChannel _onPlayerSaveRequested = GameEvents.OnPlayerSaveRequested;
        private readonly VoidEventChannel _onPlayerSaveCompleted = GameEvents.OnPlayerSaveCompleted;
        private readonly VoidEventChannel _onPlayerLoadRequested = GameEvents.OnPlayerLoadRequested;
        private readonly VoidEventChannel _onPlayerLoadCompleted = GameEvents.OnPlayerLoadCompleted;

        #endregion
        
        #region Methods

        protected override void HandleSaveRequested()
        {
            SaveFile.AddOrUpdateData(GameConstants.MetaProgressionKey, metaProgressionData);
            HandleSaveCompleted();
        }

        protected override void HandleSaveCompleted()
        {
            Debug.Log("Player Data Save Completed");
            _onPlayerSaveCompleted.Raise();
            SaveFile.Save();
        }

        protected override void HandleLoadRequested()
        {
            if(SaveFile.HasData(GameConstants.MetaProgressionKey))
                metaProgressionData = SaveFile.GetData<MetaProgressionData>(GameConstants.MetaProgressionKey);
            
            HandleLoadCompleted();
        }

        protected override void HandleLoadCompleted()
        {
            Debug.Log("Player Data Load Completed");
            _onPlayerLoadCompleted.Raise();
        }

        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            
        }
        
        private void OnEnable()
        {
            _onPlayerSaveRequested.Subscribe(HandleSaveRequested);
            _onPlayerLoadRequested.Subscribe(HandleLoadRequested);
        }

        private void OnDisable()
        {
            _onPlayerSaveRequested.Unsubscribe(HandleSaveRequested);
            _onPlayerLoadRequested.Unsubscribe(HandleLoadRequested);
        }
        
        #endregion
    }
}