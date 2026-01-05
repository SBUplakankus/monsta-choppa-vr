using System;
using System.Collections.Generic;
using Constants;
using Factories;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class VideoSettingsPanelView : BasePanelView
    {
        #region Fields
        
        public DropdownField Quality {get; private set;}
        public DropdownField AntiAliasing {get; private set;}
        public DropdownField RenderScale {get; private set;}
        
        private readonly List<string> _qualityOptions = new List<string>
        {
            LocalizationKeys.VeryLow,
            LocalizationKeys.Low, 
            LocalizationKeys.Medium, 
            LocalizationKeys.High, 
            LocalizationKeys.VeryHigh,
            LocalizationKeys.Ultra
        };
        private readonly List<string> _antiAliasingOptions = new List<string>
        {
            LocalizationKeys.Off, 
            LocalizationKeys.TimesTwo, 
            LocalizationKeys.TimesFour,
            LocalizationKeys.TimesEight,
        };

        private readonly List<string> _renderScaleOptions = new List<string>
        {
            "0.8",
            "0.9",
            "1.0",
            "1.1",
            "1.2",
        };

        #endregion

        #region Constructors

        public VideoSettingsPanelView(VisualElement root, StyleSheet styleSheet)
        {
            if (!root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            GenerateUI(root);
        }

        #endregion

        #region Methods

        private static DropdownField CreateDropdown(string key, List<string> options, int index)
        {
            return UIToolkitFactory.CreateDropdown(key, options, index, classNames: UIToolkitStyles.SettingsDropdown);
        }

        protected sealed override void GenerateUI(VisualElement root)
        {
            Container = UIToolkitFactory.CreateContainer(UIToolkitStyles.Container);
            
            var header = UIToolkitFactory.CreateContainer(UIToolkitStyles.PanelHeader);
            
            var title = UIToolkitFactory.CreateLabel(LocalizationKeys.VideoSettings, UIToolkitStyles.PanelTitle);
            header.Add(title);
            Container.Add(header);
            
            var content = UIToolkitFactory.CreateContainer(UIToolkitStyles.TabContent);

            var row = UIToolkitFactory.CreateContainer(UIToolkitStyles.SettingsDropdownRow);
            Quality = CreateDropdown(LocalizationKeys.Quality, _qualityOptions, 2);
            row.Add(Quality);
            content.Add(row);

            row = UIToolkitFactory.CreateContainer(UIToolkitStyles.SettingsDropdownRow);
            AntiAliasing = CreateDropdown(LocalizationKeys.AntiAliasing, _antiAliasingOptions, 2);
            row.Add(AntiAliasing);
            content.Add(row);
            
            row = UIToolkitFactory.CreateContainer(UIToolkitStyles.SettingsDropdownRow);
            RenderScale = CreateDropdown(LocalizationKeys.RenderScale, _renderScaleOptions, 2);
            row.Add(RenderScale);
            content.Add(row);
            
            Container.Add(content);
            root.Add(Container);
        }

        #endregion
    }
}
