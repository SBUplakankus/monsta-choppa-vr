using UnityEngine;
using UnityEngine.Localization;

namespace Data.Settings
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Settings/Language")]
    public class LanguageSettingsConfig : ScriptableObject
    {
        #region Fields

        [Header("Language Settings")]
        [SerializeField] private Locale language;

        #endregion
        
        #region Properties
        
        public Locale Language
        {
            get => language;
            set => language = value;
        }
        
        #endregion
    }
}