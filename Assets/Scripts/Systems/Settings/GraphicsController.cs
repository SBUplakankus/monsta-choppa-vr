using Attributes;
using Data.Settings;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

namespace Systems.Settings
{
    
    public class GraphicsController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private VideoSettingsConfig videoSettings;

        private static void SetQuality(int quality)
        {
            QualitySettings.SetQualityLevel(quality, true);
        }

        private static void SetAliasing(int aliasing)
        {
            QualitySettings.antiAliasing = aliasing switch
            {
                0 => 0,
                1 => 2,
                2 => 4,
                3 => 8,
                _ => 4
            };
        }

        private void SetRenderScale(int renderScale)
        {
            
        }

        private void SetVideoSettings()
        {
            SetQuality(videoSettings.Quality.Value);
            SetAliasing(videoSettings.Aliasing.Value);
            SetRenderScale(videoSettings.RenderScale.Value);
        }

        private void OnEnable()
        {
            videoSettings.Quality.OnValueChanged += SetQuality;
            videoSettings.Aliasing.OnValueChanged += SetAliasing;
            videoSettings.RenderScale.OnValueChanged += SetRenderScale;
            SetVideoSettings();
        }

        private void OnDisable()
        {
            videoSettings.Quality.OnValueChanged -= SetQuality;
            videoSettings.Aliasing.OnValueChanged -= SetAliasing;
            videoSettings.RenderScale.OnValueChanged -= SetRenderScale;
        }
    }
}
