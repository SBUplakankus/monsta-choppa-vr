using Constants;
using Events;
using UnityEngine.UIElements;

namespace UI.Extensions
{
    public static class AudioExtensions
    {
        public static void AddAudioEvents(this Button button) 
        {
            var audioChannel = GameEvents.OnSfxRequested;
            var isHovering = false;

            button.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (isHovering) return;
                isHovering = true;
                audioChannel.Raise(AudioKeys.ButtonEnter);
            });

            button.RegisterCallback<PointerLeaveEvent>(_ =>
            {
                if (!isHovering) return;
                isHovering = false;
                audioChannel.Raise(AudioKeys.ButtonExit);
            });

            button.clicked += () => audioChannel.Raise(AudioKeys.ButtonClick);
        }
        
        public static void AddAudioEvents(this DropdownField dropdown)
        {
            var audioChannel = GameEvents.OnSfxRequested;
            var hovering = false;

            dropdown.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (hovering) return;
                hovering = true;
                audioChannel.Raise(AudioKeys.ButtonEnter);
            });

            dropdown.RegisterCallback<PointerLeaveEvent>(_ =>
            {
                if (!hovering) return;
                hovering = false;
                audioChannel.Raise(AudioKeys.ButtonExit);
            });

            dropdown.RegisterValueChangedCallback(_ =>
            {
                audioChannel.Raise(AudioKeys.ButtonClick);
            });
        }
        
        public static void AddAudioEvents(this Slider slider)
        {
            var audioChannel = GameEvents.OnSfxRequested;
            var hovering = false;

            slider.RegisterCallback<PointerEnterEvent>(_ =>
            {
                if (hovering) return;
                hovering = true;
                audioChannel.Raise(AudioKeys.ButtonEnter);
            });

            slider.RegisterCallback<PointerLeaveEvent>(_ =>
            {
                if (!hovering) return;
                hovering = false;
                audioChannel.Raise(AudioKeys.ButtonExit);
            });

            slider.RegisterCallback<PointerDownEvent>(_ =>
            {
                audioChannel.Raise(AudioKeys.ButtonClick);
            });
        }
    }
}