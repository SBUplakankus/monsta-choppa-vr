using System;
using Events;
using Events.Registries;
using UnityEngine;
using Utilities;

namespace Systems.Hub
{
    public class AutoSaveController : MonoBehaviour, IUpdateable
    {
        private float _timer;
        private const int AutoSaveInterval = 30;
        
        private void OnEnable()
        {
            _timer = 0;
            GameUpdateManager.Instance.Register(this, UpdatePriority.High);
        }

        private void OnDisable()
        {
            GameUpdateManager.Instance.Unregister(this);
        }

        private void AutoSave()
        {
            _timer = 0;
            SystemEvents.PlayerSaveRequested.Raise();
        }

        public void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
            if(_timer < AutoSaveInterval) return;
            AutoSave();
        }
    }
}