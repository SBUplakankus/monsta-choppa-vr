using UnityEngine;

namespace Systems.Trailer_Camera
{
    public class DisableXROutput : MonoBehaviour
    {
        private void Awake()
        {
            var cam = GetComponent<Camera>();
            cam.stereoTargetEye = StereoTargetEyeMask.None;
        }
    }
}
