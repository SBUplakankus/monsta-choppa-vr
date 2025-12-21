using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public interface IUpdateable
    {
        void OnUpdate(float deltaTime);
    }
    
    public enum UpdatePriority {High, Medium, Low}
    
    /// <summary>
    /// Replaces the Unity Update function for better performance. Register components based on their update priority,
    /// High - Every Frame, Medium - Every 20ms, Low - Every 40ms
    /// </summary>
    public class GameUpdateManager : MonoBehaviour
    {
        #region Fields
        
        public static GameUpdateManager Instance { get; private set; }
        
        private readonly List<IUpdateable> _highPriorityUpdates = new();
        private readonly List<IUpdateable> _mediumPriorityUpdates = new(); 
        private readonly List<IUpdateable> _lowPriorityUpdates = new();

        private const float MediumPriorityInterval = 0.2f;
        private const float LowPriorityInterval = 0.4f;

        private float _mediumPriorityTimer;
        private float _lowPriorityTimer;
        
        #endregion
        
        #region Class Functions

        private void UpdateHighPriority()
        {
            for (var i = 0; i < _highPriorityUpdates.Count; i++)
            {
                _highPriorityUpdates[i].OnUpdate(Time.deltaTime);
            }
        }

        private void UpdateMediumPriority()
        {
            _mediumPriorityTimer += Time.deltaTime;
            if (!(_mediumPriorityTimer >= MediumPriorityInterval)) return;
            
            for (var i = 0; i < _mediumPriorityUpdates.Count; i++)
            {
                _mediumPriorityUpdates[i].OnUpdate(_mediumPriorityTimer);
            }
            _mediumPriorityTimer = 0f;
        }

        private void UpdateLowPriority()
        {
            _lowPriorityTimer += Time.deltaTime;
            if (!(_lowPriorityTimer >= LowPriorityInterval)) return;
            
            for (var i = 0; i < _lowPriorityUpdates.Count; i++)
            {
                _lowPriorityUpdates[i].OnUpdate(_lowPriorityTimer);
            }
            _lowPriorityTimer = 0f;
        }
        
        /// <summary>
        /// Register a component to the Update Manager
        /// </summary>
        /// <param name="updateable">Component</param>
        /// <param name="priority">Priority to Update at</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Register(IUpdateable updateable, UpdatePriority priority)
        {
            switch (priority)
            {
                case UpdatePriority.High:
                    _highPriorityUpdates.Add(updateable);
                    break;
                case UpdatePriority.Medium:
                    _mediumPriorityUpdates.Add(updateable);
                    break;
                case UpdatePriority.Low:
                    _lowPriorityUpdates.Add(updateable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            }
        }
        
        /// <summary>
        /// Remove a component from the manager
        /// </summary>
        /// <param name="updateable">Component to Remove</param>
        public void Unregister(IUpdateable updateable)
        {
            _highPriorityUpdates.Remove(updateable);
            _mediumPriorityUpdates.Remove(updateable);
            _lowPriorityUpdates.Remove(updateable);
        }

        #endregion
        
        #region Unity Functions
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        private void Update()
        {
            UpdateHighPriority();
            UpdateMediumPriority();
            UpdateLowPriority();
        }
        
        #endregion
    }
}
