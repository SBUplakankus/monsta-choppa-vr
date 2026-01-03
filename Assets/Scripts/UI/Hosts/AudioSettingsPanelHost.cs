using System;
using Attributes;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Hosts
{
    public class AudioSettingsPanelHost : MonoBehaviour
    {
        #region Fields
        
        [Header("Audio Settings")]
        [SerializeField] private FloatAttribute masterVolume;
        [SerializeField] private FloatAttribute musicVolume;
        [SerializeField] private FloatAttribute sfxVolume;
        [SerializeField] private FloatAttribute ambienceVolume;
        [SerializeField] private FloatAttribute uiVolume;

        private Action _unbindAll;

        #endregion

        #region Methods

        private static Action BindSlider(Slider slider, FloatAttribute attribute)
        {
            if (slider == null || attribute == null) return null;

            slider.SetValueWithoutNotify(attribute.Value);

            EventCallback<ChangeEvent<float>> sliderCallback =
                e => attribute.Value = e.newValue;

            slider.RegisterValueChangedCallback(sliderCallback);
            attribute.OnValueChanged += slider.SetValueWithoutNotify;

            return () =>
            {
                slider.UnregisterValueChangedCallback(sliderCallback);
                attribute.OnValueChanged -= slider.SetValueWithoutNotify;
            };
        }
        
        public void BindViewSliders(AudioSettingsPanelView view)
        {
            _unbindAll = () => { };

            _unbindAll += BindSlider(view.MasterVolume, masterVolume);
            _unbindAll += BindSlider(view.MusicVolume, musicVolume);
            _unbindAll += BindSlider(view.AmbienceVolume, ambienceVolume);
            _unbindAll += BindSlider(view.SfxVolume, sfxVolume);
            _unbindAll += BindSlider(view.UIVolume, uiVolume);
        }

        public void Dispose()
        {
            _unbindAll?.Invoke();
            _unbindAll = null;
        }

        private void OnDisable() => Dispose();

        #endregion
    }
}
