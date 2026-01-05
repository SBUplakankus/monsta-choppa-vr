using UnityEngine;

namespace UI.Extensions
{
    public interface ITweenable
    {
        void Show();
        void Hide();
        void StopTween();
        bool IsTweening { get; }
    }
    public static class TweenExtensions
    {
        
    }
}