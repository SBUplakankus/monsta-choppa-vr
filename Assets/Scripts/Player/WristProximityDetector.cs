using System;
using Systems;
using Systems.Core;
using UnityEngine;

namespace Player
{
    public class WristProximityDetector : MonoBehaviour, IUpdateable
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Transform wristTransform;
        [SerializeField] private Transform headTransform;
    
        [Header("Settings")]
        [SerializeField] private float proximityThreshold = 0.2f;

        public event Action OnWristEnterProximity;
        public event Action OnWristExitProximity;


        #endregion
        
        #region Class Methods
        
        private bool _isInProximity;

        private void DetectProximity()
        {
            if (!wristTransform || !headTransform)
            {
                Debug.LogWarning("Wrist or Head transform not assigned to WristProximityDetector.");
                return;
            }

            var distance = Vector3.Distance(wristTransform.position, headTransform.position);
            var isInProximity = distance < proximityThreshold;

            switch (isInProximity)
            {
                case true when !_isInProximity:
                    _isInProximity = true;
                    OnWristEnterProximity?.Invoke();
                    break;
                case false when _isInProximity:
                    _isInProximity = false;
                    OnWristExitProximity?.Invoke();
                    break;
            }
        }
        
        #endregion

        #region Unity Methods

        public void OnUpdate(float deltaTime)
        {
            DetectProximity();
        }

        private void OnEnable()
        {
            GameUpdateManager.Instance.Register(this, UpdatePriority.Low);
        }

        private void OnDisable()
        {
            GameUpdateManager.Instance.Unregister(this);
            OnWristEnterProximity = null;
            OnWristExitProximity = null;
        }

        #endregion
    }
}