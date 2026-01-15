using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

namespace Systems.Core
{
    [DefaultExecutionOrder(-100)]
    public class SpaceWarpCameraExtension : MonoBehaviour
    {
        private void Awake() => SpaceWarpFeature.SetSpaceWarp(true);

        private void LateUpdate()
        { 
            SpaceWarpFeature.SetAppSpacePosition(transform.position);
            SpaceWarpFeature.SetAppSpaceRotation(transform.rotation);
        }
    }
}
