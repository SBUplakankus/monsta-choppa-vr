using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

namespace Systems.Core
{
    public class SpaceWarpCameraExtension : MonoBehaviour, IUpdateable
    {
        private void Awake() => SpaceWarpFeature.SetSpaceWarp(true);
        private void OnEnable() => GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        private void OnDisable() => GameUpdateManager.Instance.Unregister(this);

        public void OnUpdate(float deltaTime)
        { 
            SpaceWarpFeature.SetAppSpacePosition(transform.position);
            SpaceWarpFeature.SetAppSpaceRotation(transform.rotation);
        }
    }
}
