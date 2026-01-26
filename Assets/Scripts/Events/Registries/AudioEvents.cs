namespace Events.Registries
{
    public static class AudioEvents
    {
        #region Events

        public static readonly EventChannel<string> MusicRequested = new();
        public static readonly EventChannel<string> MusicFadeRequested = new();
        public static readonly EventChannel<string> AmbienceRequested = new();
        public static readonly EventChannel<string> SfxRequested = new();
        public static readonly EventChannel<string> UISfxRequested = new();

        #endregion
        
        #region Methods

        public static void Clear()
        {
            AmbienceRequested.Clear();
            MusicRequested.Clear();
            MusicFadeRequested.Clear();
            SfxRequested.Clear();
            UISfxRequested.Clear();
        }
        
        #endregion
    }
}