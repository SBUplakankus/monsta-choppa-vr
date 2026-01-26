using UnityEngine.Localization;

namespace Events.Registries
{
    public static class SystemEvents
    {
        #region Events
        
        // Save Data
        public static readonly EventChannel SettingsSaveRequested = new();
        public static readonly EventChannel SettingsLoadRequested = new();
        public static readonly EventChannel PlayerSaveRequested = new();
        public static readonly EventChannel PlayerLoadRequested = new();
        
        // Locale
        public static readonly EventChannel<Locale> LocaleChangeRequested = new();

        #endregion

        #region Methods

        public static void Clear()
        {
            SettingsSaveRequested.Clear();
            SettingsLoadRequested.Clear();
            PlayerSaveRequested.Clear();
            PlayerLoadRequested.Clear();
            LocaleChangeRequested.Clear();
        }

        #endregion
    }
}