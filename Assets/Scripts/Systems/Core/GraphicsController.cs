using Attributes;
using UnityEngine;

namespace Systems
{
    public struct GraphicsSettings
    {
        public IntAttribute QualitySetting;
        public IntAttribute AliasingSetting;
        public IntAttribute RenderScaleSetting;
    }
    
    public class GraphicsController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private IntAttribute qualitySetting;
        [SerializeField] private IntAttribute aliasingSetting;
        [SerializeField] private IntAttribute renderScaleSetting;

        private static void SetQuality(int quality)
        {
            Debug.Log($"Setting quality index: {quality}");
            Debug.Log($"Quality name: {QualitySettings.names[quality]}");

            QualitySettings.SetQualityLevel(quality, true);
            
            Debug.Log($"Current quality index: {QualitySettings.GetQualityLevel()}");
            Debug.Log($"Current quality name: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
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

        private void OnEnable()
        {
            qualitySetting.OnValueChanged += SetQuality;
            aliasingSetting.OnValueChanged += SetAliasing;
            renderScaleSetting.OnValueChanged += SetRenderScale;
        }

        private void OnDisable()
        {
            qualitySetting.OnValueChanged -= SetQuality;
            aliasingSetting.OnValueChanged -= SetAliasing;
            renderScaleSetting.OnValueChanged -= SetRenderScale;
        }
    }
}
