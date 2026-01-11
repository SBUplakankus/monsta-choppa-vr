using System;
using UnityEngine;

namespace Events
{
    public abstract class TypeEventChannelBase<T> : ScriptableObject
    {
        private event Action<T> Handlers;
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Call the events handlers
        /// </summary>
        public void Raise(T value) => Handlers?.Invoke(value);
        
        /// <summary>
        /// Subscribe an Action to be called with the event
        /// </summary>
        /// <param name="handler">Action to be called</param>
        public void Subscribe(Action<T> handler) => Handlers += handler;
        
        /// <summary>
        /// Unsubscribe an Action to be called from the event
        /// </summary>
        /// <param name="handler">Action to be removed</param>
        public void Unsubscribe(Action<T> handler) => Handlers -= handler;
        
        /// <summary>
        /// Clear the Event on Disable
        /// </summary>
        private void OnDisable() => Handlers = null;
    }
}
