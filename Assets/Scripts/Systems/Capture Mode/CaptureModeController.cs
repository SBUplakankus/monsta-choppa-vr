using System;
using Systems.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Capture_Mode
{
    public class CaptureModeCamera : MonoBehaviour
    {
        public static CaptureModeCamera Instance { get; private set; }
        
        [Header("Capture Components")]
        public Camera captureCamera;
        public GameObject captureCanvas;
    
        [Header("Toggle Settings")]
        public Key toggleKey = Key.C;
        
        private bool _captureEnabled;

        private void OnEnable()
        {
            SetCaptureMode(false);
        }

        private void SetCaptureMode(bool toggle)
        {
            _captureEnabled = toggle;
    
            if (captureCamera)
                captureCamera.enabled = toggle;
    
            if (captureCanvas)
                captureCanvas.SetActive(toggle);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Update()
        {
            if (Keyboard.current != null &&
                Keyboard.current[toggleKey].wasPressedThisFrame)
            {
                SetCaptureMode(!_captureEnabled);
            }
        }
    }
}
