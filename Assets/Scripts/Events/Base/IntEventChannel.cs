using System;
using UnityEngine;

namespace Events.Base
{
    [CreateAssetMenu(fileName = "IntEventChannel", menuName = "Scriptable Objects/Event Channels/Int")]
    public class IntEventChannel : ScriptableObject
    {
        private event Action<int> Handlers;
        
        /// <summary>
        /// Call the events handlers
        /// </summary>
        public void Raise(int value)
        { 
            var handlers = Handlers;
            if (handlers == null) return;

            foreach (var @delegate in handlers.GetInvocationList())
            {
                var handler = (Action<int>)@delegate;
                try
                {
                    handler(value);
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
        public void Subscribe(Action<int> handler) => Handlers += handler;
        
        /// <summary>
        /// Unsubscribe an Action to be called from the event
        /// </summary>
        /// <param name="handler">Action to be removed</param>
        public void Unsubscribe(Action<int> handler) => Handlers -= handler;
        
        /// <summary>
        /// Clear the Event on Disable
        /// </summary>
        private void OnDisable() => Handlers = null;
    }
}

