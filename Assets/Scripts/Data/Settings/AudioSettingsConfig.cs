using Attributes;
using UnityEngine;

namespace Data.Settings
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Settings/Audio")]
    public class AudioSettingsConfig : ScriptableObject
    {
        #region Fields

        [Header("Audio Settings")]
        [SerializeField] private FloatAttribute masterVolume;
        [SerializeField] private FloatAttribute musicVolume;
        [SerializeField] private FloatAttribute ambienceVolume;
        [SerializeField] private FloatAttribute sfxVolume;
        [SerializeField] private FloatAttribute uiVolume;

        #endregion
        
        #region Properties
        
        public FloatAttribute MasterVolume => masterVolume;
        public FloatAttribute MusicVolume => musicVolume;
        public FloatAttribute AmbienceVolume => ambienceVolume;
        public FloatAttribute SfxVolume => sfxVolume;
        public FloatAttribute UIVolume => uiVolume;
        
        #endregion
    }
}