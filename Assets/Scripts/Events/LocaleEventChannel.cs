using UnityEngine;
using UnityEngine.Localization;

namespace Events
{
    [CreateAssetMenu(fileName = "LocaleEventChannel", menuName = "Scriptable Objects/Event Channels/Locale")]
    public class LocaleEventChannel : TypeEventChannelBase<Locale> { }
}