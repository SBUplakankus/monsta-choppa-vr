using Constants;
using Data.Settings;
using Events;
using Events.Registries;
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
        
        #endregion
        
        #region Methods

        private void TryLoadSettings()
        {
            if(SaveFile.HasData(GameConstants.AudioSettingsKey))
                audioSettings = SaveFile.GetData<AudioSettingsConfig>(GameConstants.AudioSettingsKey);
            
            if (SaveFile.HasData(GameConstants.VideoSettingsKey))
                videoSettings = SaveFile.GetData<VideoSettingsConfig>(GameConstants.VideoSettingsKey);
            
            if (SaveFile.HasData(GameConstants.LocalizationSettingsKey))
                languageSettings = SaveFile.GetData<LanguageSettingsConfig>(GameConstants.LocalizationSettingsKey);
        }
        
        protected override void HandleSaveRequested()
        {
            SaveFile.AddOrUpdateData(GameConstants.AudioSettingsKey, audioSettings);
            SaveFile.AddOrUpdateData(GameConstants.VideoSettingsKey, videoSettings);
            SaveFile.AddOrUpdateData(GameConstants.LocalizationSettingsKey, languageSettings);
            HandleSaveCompleted();
        }

        protected override void HandleSaveCompleted()
        {
            SaveFile.Save();
        }

        protected override void HandleLoadRequested()
        {
            TryLoadSettings();
            HandleLoadCompleted();
        }

        protected override void HandleLoadCompleted()
        {
        }

        public void InitSettings()
        {
            TryLoadSettings();
        }

        private void Awake()
        {
            GetSaveFile();
        }

        private void OnEnable()
        {
            SystemEvents.SettingsSaveRequested.Subscribe(HandleSaveRequested);
            SystemEvents.SettingsLoadRequested.Subscribe(HandleLoadRequested);
        }

        private void OnDisable()
        {
            SystemEvents.SettingsSaveRequested.Unsubscribe(HandleSaveRequested);
            SystemEvents.SettingsLoadRequested.Unsubscribe(HandleLoadRequested);
        }
        
        #endregion
    }
}