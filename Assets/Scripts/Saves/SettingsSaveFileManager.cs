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
        private VoidEventChannel _onSettingsSaveRequested;
        private VoidEventChannel _onSettingsSaveCompleted;
        private VoidEventChannel _onSettingsLoadRequested;
        private VoidEventChannel _onSettingsLoadCompleted;
        
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
            _onSettingsSaveCompleted.Raise();
            SaveFile.Save();
        }

        protected override void HandleLoadRequested()
        {
            TryLoadSettings();
            HandleLoadCompleted();
        }

        protected override void HandleLoadCompleted()
        {
            _onSettingsLoadCompleted.Raise();
        }

        public void InitSettings()
        {
            TryLoadSettings();
        }

        private void Awake()
        {
            GetSaveFile();
            _onSettingsSaveRequested = GameEvents.OnSettingsSaveRequested;
            _onSettingsSaveCompleted = GameEvents.OnSettingsSaveCompleted;
            _onSettingsLoadRequested = GameEvents.OnSettingsLoadRequested;
            _onSettingsLoadCompleted = GameEvents.OnSettingsLoadCompleted;
        }

        private void OnEnable()
        {
            _onSettingsSaveRequested?.Subscribe(HandleSaveRequested);
            _onSettingsLoadRequested?.Subscribe(HandleLoadRequested);
        }

        private void OnDisable()
        {
            _onSettingsSaveRequested?.Unsubscribe(HandleSaveRequested);
            _onSettingsSaveCompleted?.Unsubscribe(HandleSaveCompleted);
        }
        
        #endregion
    }
}