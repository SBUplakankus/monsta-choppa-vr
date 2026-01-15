using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features.Meta;

namespace Systems.Core
{
    public class RefreshRateController : MonoBehaviour
    {
        private static void SetRefreshRate()
        {
            var displaySubsystem = XRGeneralSettings.Instance.Manager.activeLoader
                ?.GetLoadedSubsystem<XRDisplaySubsystem>();

            if (displaySubsystem == null)
            {
                Debug.LogWarning("XR Display Subsystem not found. Cannot set refresh rate.");
                return;
            }

            // Get the supported refresh rates (temporary allocator is fine for one frame)
            if (displaySubsystem.TryGetSupportedDisplayRefreshRates(Allocator.Temp, out var refreshRates))
            {
                // Try to find 90 Hz
                const float targetRate = 90f;
                var selectedRate = refreshRates.FirstOrDefault(r => Mathf.Approximately(r, targetRate));

                if (selectedRate > 0f)
                {
                    var success = displaySubsystem.TryRequestDisplayRefreshRate(selectedRate);
                    Debug.Log("Requested 90 Hz refresh rate: " + success);
                }
                else
                {
                    Debug.LogWarning("90 Hz not supported. Supported rates: " + string.Join(", ", refreshRates));
                }
            }
            else
            {
                Debug.LogWarning("Failed to get supported display refresh rates.");
            }
        }

        private void Start()
        {
            SetRefreshRate();
        }
    }
}
