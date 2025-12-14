using System;
using UnityEngine;

namespace Events.Base
{
    [CreateAssetMenu(fileName = "VoidEventChannel", menuName = "Scriptable Objects/Event Channels/Void")]
    public class VoidEventChannel : ScriptableObject
    {
        private event Action Handlers;
        
        /// <summary>
        /// Call the events handlers
        /// </summary>
        public void Raise()
        { 
            var handlers = Handlers;
            if (handlers == null) return;

            foreach (var @delegate in handlers.GetInvocationList())
            {
                var handler = (Action)@delegate;
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        
        /// <summary>
        /// Subscribe an Action to be called with the event
        /// </summary>
        /// <param name="handler">Action to be called</param>
        public void Subscribe(Action handler) => Handlers += handler;
        
        /// <summary>
        /// Unsubscribe an Action to be called from the event
        /// </summary>
        /// <param name="handler">Action to be removed</param>
        public void Unsubscribe(Action handler) => Handlers -= handler;
        
        /// <summary>
        /// Clear the Event on Disable
        /// </summary>
        private void OnDisable() => Handlers = null;
    }
}
