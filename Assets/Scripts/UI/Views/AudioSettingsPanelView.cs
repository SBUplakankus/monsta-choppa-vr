using System;
using Constants;
using Factories;
using UI.Extensions;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class AudioSettingsPanelView : BasePanelView
    {
        #region Fields

        public Slider MasterVolume {get; private set;}
        public Slider MusicVolume {get; private set;}
        public Slider AmbienceVolume {get; private set;}
        public Slider UIVolume {get; private set;}
        public Slider SfxVolume {get; private set;}

        #endregion

        #region Constructors

        public AudioSettingsPanelView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root);
        }

        #endregion

        #region Methods

        private static VisualElement CreateSlider(string key, out Slider slider)
        {
            var container = UIToolkitFactory.CreateContainer(UIToolkitStyles.SettingsSliderRow);

            container.Add(
                UIToolkitFactory.CreateLabel(key, UIToolkitStyles.SliderTitle)
            );

            slider = UIToolkitFactory.CreateSlider(classNames: UIToolkitStyles.SettingsSlider);
            container.Add(slider);
            
            return container;
        }

        protected sealed override void GenerateUI(VisualElement root)
        {
            Container = UIToolkitFactory.CreateContainer(
                UIToolkitStyles.Container
            );
            
            var header = UIToolkitFactory.CreateContainer(UIToolkitStyles.PanelHeader);
            
            var title = UIToolkitFactory.CreateLabel(
                LocalizationKeys.AudioSettings,
                UIToolkitStyles.PanelTitle);
            header.Add(title);
            
            Container.Add(header);
            
            var content = UIToolkitFactory.CreateContainer(UIToolkitStyles.PanelContent);

            content.Add(CreateSlider(LocalizationKeys.Master, out var master));
            content.Add(CreateSlider(LocalizationKeys.Music, out var music));
            content.Add(CreateSlider(LocalizationKeys.Ambience, out var ambience));
            content.Add(CreateSlider(LocalizationKeys.SFX, out var sfx));
            content.Add(CreateSlider(LocalizationKeys.UI, out var ui));

            MasterVolume = master;
            MusicVolume = music;
            AmbienceVolume = ambience;
            SfxVolume = sfx;
            UIVolume = ui;
            
            Container.Add(content);

            root.Add(Container);
        }

        #endregion
    }
}
