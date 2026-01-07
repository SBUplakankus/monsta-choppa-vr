using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Capture_Mode
{
    public class CaptureModeCamera : MonoBehaviour, IUpdateable
    {
        [Header("Capture Components")]
        public Camera captureCamera;
        public GameObject captureCanvas;
    
        [Header("Toggle Settings")]
        public Key toggleKey = Key.C;
        
        private bool _captureEnabled;

        private void OnEnable()
        {
            SetCaptureMode(false);
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }

        private void OnDisable()
        {
            GameUpdateManager.Instance.Unregister(this);
        }

        private void SetCaptureMode(bool toggle)
        {
            _captureEnabled = toggle;
    
            if (captureCamera)
                captureCamera.enabled = toggle;
    
            if (captureCanvas)
                captureCanvas.SetActive(toggle);
        }

        public void OnUpdate(float deltaTime)
        {
            if (Keyboard.current != null &&
                Keyboard.current[toggleKey].wasPressedThisFrame)
            {
                SetCaptureMode(!_captureEnabled);
            }
        }
    }
}
