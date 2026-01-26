using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Player
{
    public class PlayerHapticFeedback : MonoBehaviour
    {
        public void TriggerHapticFeedback(float intensity, float duration, XRNode controllerNode)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(controllerNode, devices);

            foreach (var device in devices)
            {
                if (!device.TryGetHapticCapabilities(out var hapticCapabilities) ||
                    !hapticCapabilities.supportsImpulse) continue;
                device.SendHapticImpulse(0, intensity, duration); // channel 0 for simple impulses
                Debug.Log($"Haptic feedback triggered on {controllerNode}");
            }
        }
    }
}