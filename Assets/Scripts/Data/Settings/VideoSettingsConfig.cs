using Attributes;
using UnityEngine;

namespace Data.Settings
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Settings/Video")]
    public class VideoSettingsConfig : ScriptableObject
    {
        #region Fields

        [Header("Video Settings")]
        [SerializeField] private IntAttribute qualitySetting;
        [SerializeField] private IntAttribute aliasingSetting;
        [SerializeField] private IntAttribute renderScaleSetting;

        #endregion
        
        #region Properties
        
        public IntAttribute Quality => qualitySetting;
        public IntAttribute Aliasing => aliasingSetting;
        public IntAttribute RenderScale => renderScaleSetting;
        
        #endregion
    }
}