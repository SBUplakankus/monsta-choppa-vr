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
        [SerializeField] private VoidEventChannel onPlayerSaveRequested;
        [SerializeField] private VoidEventChannel onPlayerSaveCompleted;
        [SerializeField] private VoidEventChannel onPlayerLoadRequested;
        [SerializeField] private VoidEventChannel onPlayerLoadCompleted;

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
            onPlayerSaveCompleted.Raise();
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
            onPlayerLoadCompleted.Raise();
        }

        #endregion
        
        #region Unity Methods
        
        private void OnEnable()
        {
            onPlayerSaveRequested.Subscribe(HandleSaveRequested);
            onPlayerLoadRequested.Subscribe(HandleLoadRequested);
        }

        private void OnDisable()
        {
            onPlayerSaveRequested.Unsubscribe(HandleSaveRequested);
            onPlayerLoadRequested.Unsubscribe(HandleLoadRequested);
        }
        
        #endregion
    }
}