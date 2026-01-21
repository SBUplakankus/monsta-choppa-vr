using System;
using System.Collections.Generic;
using System.Reflection;
using Constants;
using UI.Extensions;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Factories
{
    /// <summary>
    /// Holds references to all VisualElements that make up a health bar.
    /// Allows external systems to update visuals (e.g. fill width) without
    /// knowing how the health bar hierarchy is constructed.
    /// </summary>
    public struct BarElements
    {
        public VisualElement Container;
        public VisualElement Background;
        public VisualElement Fill;
    }
    
    /// <summary>
    /// Static factory class for creating and configuring UI Toolkit elements.
    /// Provides standardized methods for element creation with optional styling and event handling.
    /// </summary>
    public static class UIToolkitFactory
    {
        #region Core Factory Methods
        
        /// <summary>
        /// Creates a VisualElement of specified type with optional CSS classes.
        /// </summary>
        /// <typeparam name="T">Type of VisualElement to create.</typeparam>
        /// <param name="classNames">CSS class names to apply to the element.</param>
        /// <returns>Configured VisualElement instance.</returns>
        public static T CreateElement<T>(params string[] classNames) where T : VisualElement, new()
        {
            var element = new T();
            AddClasses(element, classNames);
            return element;
        }
        
        /// <summary>
        /// Creates a generic container VisualElement for grouping and layout.
        /// </summary>
        /// <param name="classNames">CSS class names to apply to the container.</param>
        /// <returns>Container VisualElement.</returns>
        public static VisualElement CreateContainer(params string[] classNames)
        {
            return CreateElement<VisualElement>(classNames);
        }
        
        #endregion
        
        #region Struct Factories
        
        /// <summary>
        /// Creates a complete health bar UI hierarchy.
        /// Structure:
        /// Container -> Background -> Fill
        /// </summary>
        /// <returns>
        /// A <see cref="BarElements"/> struct containing references
        /// to the container, background, and fill elements.
        /// </returns>
        public static BarElements CreateHealthBar()
        {
            var container = CreateContainer(UIToolkitStyles.HealthBarContainer);
            var background = CreateContainer(UIToolkitStyles.HealthBarBackground);
            var fill = CreateContainer(UIToolkitStyles.HealthBarFill);

            background.Add(fill);
            container.Add(background);

            return new BarElements
            {
                Container = container,
                Background = background,
                Fill = fill
            };
        }
        
        public static BarElements CreateLoadingBar()
        {
            var container = CreateContainer(UIToolkitStyles.LoadingBarContainer);
            var background = CreateContainer(UIToolkitStyles.LoadingBarBackground);
            var fill = CreateContainer(UIToolkitStyles.LoadingBarFill);

            background.Add(fill);
            container.Add(background);

            return new BarElements
            {
                Container = container,
                Background = background,
                Fill = fill
            };
        }
        
        #endregion
        
        #region Control Factories
        
        /// <summary>
        /// Creates a Button with text and optional click handler.
        /// </summary>
        /// <param name="localizationKey">Localization key for button text (optional).</param>
        /// <param name="onClick">Callback invoked on button click.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured Button element.</returns>
        public static Button CreateButton(string localizationKey = null, Action onClick = null, params string[] classNames)
        {
            var button = CreateElement<Button>(classNames);
            
            if(!string.IsNullOrEmpty(localizationKey))
            {
                var localizedString = LocalizationFactory.CreateString(localizationKey);
                button.SetBinding("text", localizedString);
            }
            
            if (onClick != null)
                button.clicked += onClick;
            
            button.AddAudioEvents();
            return button;
        }

        /// <summary>
        /// Creates a Label with specified text.
        /// </summary>
        /// <param name="localizationKey">Localization key for label text</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured Label element.</returns>
        public static Label CreateLabel(string localizationKey, params string[] classNames)
        {
            var label = CreateElement<Label>(classNames);
            
            if(!string.IsNullOrEmpty(localizationKey))
            {
                var localizedString = LocalizationFactory.CreateString(localizationKey);
                label.SetBinding(UIToolkitStyles.LabelTextBind, localizedString);
            }
            
            return label;
        }
        
        /// <summary>
        /// Creates a UI Toolkit <see cref="Label"/> that is bound to a property on a data source object.
        /// The label's <c>text</c> property will automatically update at runtime when the source property changes.
        /// </summary>
        /// <param name="dataSource">The object that contains the property to bind (e.g., a ScriptableObject or POCO).</param>
        /// <param name="dataSourcePath">The name of the property on the data source to bind to.</param>
        /// <param name="classNames">Optional USS class names to add to the label for styling.</param>
        /// <returns>A <see cref="Label"/> instance with runtime binding applied.</returns>
        public static Label CreateBoundLabel(
            object dataSource,
            string dataSourcePath,
            params string[] classNames)
        {
            var label = CreateElement<Label>(classNames);

            // Set data source on the element
            label.dataSource = dataSource;

            // Register a binding for the text property
            label.SetBinding(
                nameof(Label.text),
                new DataBinding
                {
                    dataSource = dataSource,
                    dataSourcePath = PropertyPath.FromName(dataSourcePath),
                    bindingMode = BindingMode.ToTarget
                });
            
            var prop = dataSource.GetType().GetProperty(dataSourcePath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
            {
                var value = prop.GetValue(dataSource);
                label.text = value != null ? value.ToString() : string.Empty;
            }
            else
            {
                Debug.LogWarning($"Property '{dataSourcePath}' not found on {dataSource}");
                label.text = string.Empty;
            }
            return label;
        }
        
        /// <summary>
        /// Creates a Slider with range configuration and value change callback.
        /// </summary>
        /// <param name="min">Minimum slider value.</param>
        /// <param name="max">Maximum slider value.</param>
        /// <param name="value">Initial slider value.</param>
        /// <param name="labelKey">Localization key for slider label (optional).</param>
        /// <param name="onValueChanged">Callback invoked when slider value changes.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured Slider element.</returns>
        public static Slider CreateSlider(float min = 0, float max = 1, float value = 1, 
            string labelKey = null, Action<float> onValueChanged = null, params string[] classNames)
        {
            var slider = CreateElement<Slider>(classNames);
            slider.lowValue = min;
            slider.highValue = max;
            slider.value = value;
            
            if(!string.IsNullOrEmpty(labelKey))
            {
                var localizedString = LocalizationFactory.CreateString(labelKey);
                slider.SetBinding("label", localizedString);
            }
            
            if (onValueChanged != null)
                slider.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            
            slider.AddAudioEvents();
            return slider;
        }
        
        /// <summary>
        /// Creates an Integer Slider with range configuration.
        /// </summary>
        /// <param name="min">Minimum integer value.</param>
        /// <param name="max">Maximum integer value.</param>
        /// <param name="value">Initial integer value.</param>
        /// <param name="labelKey">Localization key for slider label (optional).</param>
        /// <param name="onValueChanged">Callback invoked when value changes.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured SliderInt element.</returns>
        public static SliderInt CreateSliderInt(int min, int max, int value,
            string labelKey = null, Action<int> onValueChanged = null, params string[] classNames)
        {
            var slider = CreateElement<SliderInt>(classNames);
            slider.lowValue = min;
            slider.highValue = max;
            slider.value = value;
            
            if(!string.IsNullOrEmpty(labelKey))
            {
                var localizedString = LocalizationFactory.CreateString(labelKey);
                slider.SetBinding("label", localizedString);
            }
            
            if (onValueChanged != null)
                slider.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
                
            return slider;
        }
        
        /// <summary>
        /// Creates a Toggle (checkbox) with label and state change callback.
        /// </summary>
        /// <param name="localizationKey">Localization key for toggle label.</param>
        /// <param name="value">Initial toggle state.</param>
        /// <param name="onValueChanged">Callback invoked when toggle state changes.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured Toggle element.</returns>
        public static Toggle CreateToggle(string localizationKey, bool value = false,
            Action<bool> onValueChanged = null, params string[] classNames)
        {
            var toggle = CreateElement<Toggle>(classNames);
            
            if(!string.IsNullOrEmpty(localizationKey))
            {
                var localizedString = LocalizationFactory.CreateString(localizationKey);
                toggle.SetBinding("label", localizedString);
            }
            
            toggle.value = value;
            
            if (onValueChanged != null)
                toggle.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
                
            return toggle;
        }
        
        /// <summary>
        /// Creates a TextField for text input with optional label and change callback.
        /// </summary>
        /// <param name="labelKey">Localization key for field label (optional).</param>
        /// <param name="value">Initial field value.</param>
        /// <param name="isMultiline">Whether the field supports multiline input.</param>
        /// <param name="onValueChanged">Callback invoked when text changes.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured TextField element.</returns>
        public static TextField CreateTextField(string labelKey = null, string value = "", bool isMultiline = false,
            Action<string> onValueChanged = null, params string[] classNames)
        {
            var textField = CreateElement<TextField>(classNames);
            
            if(!string.IsNullOrEmpty(labelKey))
            {
                var localizedString = LocalizationFactory.CreateString(labelKey);
                textField.SetBinding("label", localizedString);
            }
            
            textField.value = value;
            textField.multiline = isMultiline;
            
            if (onValueChanged != null)
                textField.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
                
            return textField;
        }
        
        /// <summary>
        /// Creates a DropdownField with specified options and selection callback.
        /// </summary>
        /// <param name="labelKey">Localization key for dropdown label.</param>
        /// <param name="choices">List of available options.</param>
        /// <param name="defaultIndex">Index of initially selected option.</param>
        /// <param name="onValueChanged">Callback invoked when selection changes.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured DropdownField element.</returns>
        public static DropdownField CreateDropdown(string labelKey, List<string> choices, int defaultIndex = 0,
            Action<string> onValueChanged = null, params string[] classNames)
        {
            var dropdown = CreateElement<DropdownField>(classNames);
            
            if(!string.IsNullOrEmpty(labelKey))
            {
                var localizedString = LocalizationFactory.CreateString(labelKey);
                dropdown.SetBinding("label", localizedString);
            }
            
            dropdown.choices = choices;
            dropdown.index = defaultIndex;
            
            if (onValueChanged != null)
                dropdown.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
                
            dropdown.AddAudioEvents();
            
            return dropdown;
        }
        
        /// <summary>
        /// Creates a ScrollView container for scrollable content.
        /// </summary>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured ScrollView element.</returns>
        public static ScrollView CreateScrollView(params string[] classNames)
        {
            return CreateElement<ScrollView>(classNames);
        }
        
         /// <summary>
        /// Creates an Image VisualElement from a sprite.
        /// </summary>
        /// <param name="sprite">The sprite to display inside the image.</param>
        /// <param name="classNames">Optional CSS class names for styling the image.</param>
        /// <returns>Configured Image element displaying the sprite.</returns>
        public static Image CreateImage(Sprite sprite, params string[] classNames)
        {
            var image = CreateElement<Image>(classNames);

            if (sprite != null)
            {
                image.sprite = sprite;
            }

            return image;
        }

        /// <summary>
        /// Creates an Image VisualElement from a texture.
        /// </summary>
        /// <param name="texture">The texture to display inside the image.</param>
        /// <param name="classNames">Optional CSS class names for styling the image.</param>
        /// <returns>Configured Image element displaying the texture.</returns>
        public static Image CreateImage(Texture2D texture, params string[] classNames)
        {
            var image = CreateElement<Image>(classNames);

            if (texture != null)
            {
                image.style.backgroundImage = new StyleBackground(texture);
            }

            return image;
        }

        /// <summary>
        /// Creates an Image VisualElement for icons with optional tooltip.
        /// </summary>
        /// <param name="sprite">The sprite to use as the image source.</param>
        /// <param name="tooltip">Optional tooltip text to show on hover.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured Image element.</returns>
        public static Image CreateIcon(Sprite sprite, string tooltip = null, params string[] classNames)
        {
            var image = CreateImage(sprite, classNames);

            // Add tooltip if provided
            if (!string.IsNullOrEmpty(tooltip))
            {
                image.tooltip = tooltip;
            }

            return image;
        }

        /// <summary>
        /// Creates a VisualElement for displaying a background with a texture.
        /// </summary>
        /// <param name="texture">The background texture.</param>
        /// <param name="classNames">CSS class names for styling the background.</param>
        /// <returns>Configured VisualElement with a background image.</returns>
        public static VisualElement CreateBackground(Texture2D texture, params string[] classNames)
        {
            var background = CreateElement<VisualElement>(classNames);

            if (texture != null)
            {
                background.style.backgroundImage = new StyleBackground(texture);
            }

            return background;
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Creates a horizontal or vertical group for element arrangement.
        /// </summary>
        /// <param name="isHorizontal">True for horizontal layout, false for vertical.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Configured VisualElement group.</returns>
        public static VisualElement CreateGroup(bool isHorizontal = true, params string[] classNames)
        {
            var group = CreateElement<VisualElement>(classNames);
            group.style.flexDirection = isHorizontal ? FlexDirection.Row : FlexDirection.Column;
            return group;
        }
        
        /// <summary>
        /// Creates a spacer element that expands to fill available space.
        /// </summary>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Spacer VisualElement.</returns>
        public static VisualElement CreateSpacer(params string[] classNames)
        {
            var spacer = CreateElement<VisualElement>(classNames);
            spacer.style.flexGrow = 1;
            return spacer;
        }
        
        /// <summary>
        /// Creates a separator line (horizontal or vertical).
        /// </summary>
        /// <param name="isVertical">True for vertical separator, false for horizontal.</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Separator VisualElement.</returns>
        public static VisualElement CreateSeparator(bool isVertical = false, params string[] classNames)
        {
            var separator = CreateElement<VisualElement>(classNames);
            
            if (isVertical)
            {
                separator.style.width = 1;
                separator.style.height = Length.Percent(100);
                separator.style.backgroundColor = Color.gray;
            }
            else
            {
                separator.style.height = 1;
                separator.style.width = Length.Percent(100);
                separator.style.backgroundColor = Color.gray;
            }
            
            return separator;
        }
        
        /// <summary>
        /// Creates a simple progress bar element.
        /// </summary>
        /// <param name="progress">Initial progress value (0 to 1).</param>
        /// <param name="labelKey">Localization key for progress bar label (optional).</param>
        /// <param name="classNames">CSS class names for styling.</param>
        /// <returns>Progress bar container with inner fill element.</returns>
        public static (VisualElement container, VisualElement fill) CreateProgressBar(float progress = 0f, 
            string labelKey = null, params string[] classNames)
        {
            var container = CreateElement<VisualElement>(classNames);
            container.style.height = 20;
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            
            var fill = CreateElement<VisualElement>("progress-fill");
            fill.style.height = Length.Percent(100);
            fill.style.backgroundColor = Color.blue;
            fill.style.width = Length.Percent(progress * 100);
            
            container.Add(fill);
            
            if(!string.IsNullOrEmpty(labelKey))
            {
                var label = CreateLabel(labelKey);
                container.Add(label);
            }
            
            return (container, fill);
        }

        #endregion
        
        #region Extension Methods (Fluent API)
        
        /// <summary>
        /// Adds CSS classes to a VisualElement and returns it for method chaining.
        /// </summary>
        public static T WithClasses<T>(this T element, params string[] classNames) where T : VisualElement
        {
            AddClasses(element, classNames);
            return element;
        }
        
        /// <summary>
        /// Sets the display name of a VisualElement for querying purposes.
        /// </summary>
        public static T WithName<T>(this T element, string name) where T : VisualElement
        {
            element.name = name;
            return element;
        }
        
        /// <summary>
        /// Sets the text content of a TextElement.
        /// </summary>
        public static T WithText<T>(this T element, string text) where T : TextElement
        {
            element.text = text;
            return element;
        }
        
        /// <summary>
        /// Sets a localized text binding on a TextElement.
        /// </summary>
        public static T WithLocalizedText<T>(this T element, string localizationKey) where T : TextElement
        {
            if(!string.IsNullOrEmpty(localizationKey))
            {
                var localizedString = LocalizationFactory.CreateString(localizationKey);
                element.SetBinding("text", localizedString);
            }
            return element;
        }
        
        /// <summary>
        /// Sets uniform margin on all sides of a VisualElement.
        /// </summary>
        public static T WithMargin<T>(this T element, float value) where T : VisualElement
        {
            element.style.marginTop = value;
            element.style.marginBottom = value;
            element.style.marginLeft = value;
            element.style.marginRight = value;
            return element;
        }
        
        /// <summary>
        /// Sets uniform padding on all sides of a VisualElement.
        /// </summary>
        public static T WithPadding<T>(this T element, float value) where T : VisualElement
        {
            element.style.paddingTop = value;
            element.style.paddingBottom = value;
            element.style.paddingLeft = value;
            element.style.paddingRight = value;
            return element;
        }
        
        /// <summary>
        /// Sets the flexGrow property of a VisualElement.
        /// </summary>
        public static T WithFlexGrow<T>(this T element, float value) where T : VisualElement
        {
            element.style.flexGrow = value;
            return element;
        }
        
        /// <summary>
        /// Sets the visibility of a VisualElement.
        /// </summary>
        public static T WithVisibility<T>(this T element, bool isVisible) where T : VisualElement
        {
            element.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            return element;
        }
        
        /// <summary>
        /// Sets the display style of a VisualElement.
        /// </summary>
        public static T WithDisplayStyle<T>(this T element, DisplayStyle displayStyle) where T : VisualElement
        {
            element.style.display = displayStyle;
            return element;
        }
        
        /// <summary>
        /// Applies a style sheet to a VisualElement.
        /// </summary>
        public static T WithStyleSheet<T>(this T element, StyleSheet styleSheet) where T : VisualElement
        {
            element.styleSheets.Add(styleSheet);
            return element;
        }
        
        #endregion
        
        #region Private Helper Methods
        
        /// <summary>
        /// Internal method to add CSS classes to a VisualElement.
        /// </summary>
        private static void AddClasses(VisualElement element, string[] classNames)
        {
            if (classNames == null) return;
            
            foreach (var className in classNames)
            {
                if (!string.IsNullOrEmpty(className))
                    element.AddToClassList(className);
            }
        }
        
        #endregion
    }
}