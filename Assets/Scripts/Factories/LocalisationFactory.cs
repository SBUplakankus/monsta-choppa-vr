using Constants;
using UnityEngine;
using UnityEngine.Localization;

namespace Factories
{
    /// <summary>
    /// Factory class for creating localized string instances.
    /// </summary>
    public static class LocalizationFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Creates a <see cref="LocalizedString"/> using the main localization table.
        /// </summary>
        /// <param name="key">The localization key from <see cref="LocalizationKeys"/>.</param>
        /// <returns>A configured <see cref="LocalizedString"/> instance.</returns>
        public static LocalizedString CreateString(string key)
        {
            if (string.IsNullOrEmpty(key))
                Debug.LogWarning("Localization key is null or empty");
            
            var trimKey = key?.Trim().ToLower();
            return new LocalizedString(LocalizationKeys.MainTable, trimKey);
        }
    }
}