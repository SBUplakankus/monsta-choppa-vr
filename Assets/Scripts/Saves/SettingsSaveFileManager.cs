using Constants;
using Data.Settings;
using Events;
using UnityEngine;

namespace Saves
{
    public class SettingsSaveFileManager : SaveFileManagerBase
    {
        #region Fields
        
        [Header("Settings Configs")]
        [SerializeField] private AudioSettingsConfig audioSettings;
        [SerializeField] private VideoSettingsConfig videoSettings;
        [SerializeField] private LanguageSettingsConfig languageSettings;
        
        [Header("Save Events")] 
        [SerializeField] private VoidEventChannel onSettingsSaveRequested;
        [SerializeField] private VoidEventChannel onSettingsSaveCompleted;
        [SerializeField] private VoidEventChannel onSettingsLoadRequested;
        [SerializeField] private VoidEventChannel onSettingsLoadCompleted;
        
        #endregion
        
        #region Methods
        
        protected override void HandleSaveRequested()
        {
            SaveFile.AddOrUpdateData(GameConstants.AudioSettingsKey, audioSettings);
            SaveFile.AddOrUpdateData(GameConstants.VideoSettingsKey, videoSettings);
            SaveFile.AddOrUpdateData(GameConstants.LocalizationSettingsKey, languageSettings);
            HandleSaveCompleted();
        }

        protected override void HandleSaveCompleted()
        {
            onSettingsSaveCompleted.Raise();
            SaveFile.Save();
        }

        protected override void HandleLoadRequested()
        {
            if(SaveFile.HasData(GameConstants.AudioSettingsKey))
                audioSettings = SaveFile.GetData<AudioSettingsConfig>(GameConstants.AudioSettingsKey);
            
            if (SaveFile.HasData(GameConstants.VideoSettingsKey))
                videoSettings = SaveFile.GetData<VideoSettingsConfig>(GameConstants.VideoSettingsKey);
            
            if (SaveFile.HasData(GameConstants.LocalizationSettingsKey))
                languageSettings = SaveFile.GetData<LanguageSettingsConfig>(GameConstants.LocalizationSettingsKey);
            
            HandleLoadCompleted();
        }

        protected override void HandleLoadCompleted()
        {
            onSettingsLoadCompleted.Raise();
        }

        private void OnEnable()
        {
            onSettingsSaveRequested.Subscribe(HandleSaveRequested);
            onSettingsLoadRequested.Subscribe(HandleLoadRequested);
        }

        private void OnDisable()
        {
            onSettingsSaveRequested.Unsubscribe(HandleSaveRequested);
            onSettingsSaveCompleted.Unsubscribe(HandleSaveCompleted);
        }
        
        #endregion
    }
}