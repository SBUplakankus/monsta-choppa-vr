using Constants;
using Data.Progression;
using Events;
using Events.Registries;
using UnityEngine;

namespace Saves
{
    public class PlayerSaveFileManager : SaveFileManagerBase
    {
        #region Fields

        [Header("Player Data Objects")]
        [SerializeField] private MetaProgressionData metaProgressionData;

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
        }

        #endregion
        
        #region Unity Methods
        
        private void OnEnable()
        {
            SystemEvents.PlayerSaveRequested.Subscribe(HandleSaveRequested);
            SystemEvents.PlayerLoadRequested.Subscribe(HandleLoadRequested);
        }

        private void OnDisable()
        {
            SystemEvents.PlayerSaveRequested.Unsubscribe(HandleSaveRequested);
            SystemEvents.PlayerLoadRequested.Unsubscribe(HandleLoadRequested);
        }
        
        #endregion
    }
}