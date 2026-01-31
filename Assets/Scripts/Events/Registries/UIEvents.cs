namespace Events.Registries
{
    public static class UIEvents
    {
        #region Events

        public static readonly EventChannel FadeIn = new();
        public static readonly EventChannel FadeOut = new();
        
        #endregion
        
        #region Methods

        public static void Clear()
        {
            FadeIn.Clear();
            FadeOut.Clear();
        }
        
        #endregion
    }
}