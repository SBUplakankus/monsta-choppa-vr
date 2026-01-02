using Constants;
using Events;
using UnityEngine.UIElements;

namespace UI.Extensions
{
    public static class ButtonAudioExtensions
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
    }
}