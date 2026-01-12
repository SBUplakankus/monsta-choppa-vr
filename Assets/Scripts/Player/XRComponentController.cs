using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class XRComponentController : MonoBehaviour
    {
        [Header("XR Rig Components to Toggle")]
        [SerializeField] private List<MonoBehaviour> componentsToToggle; // List of components to enable/disable

        /// <summary>
        /// Enables all the specified components in the XR Rig.
        /// </summary>
        public void EnableComponents()
        {
            foreach (var component in componentsToToggle)
            {
                if (component != null)
                    component.enabled = true;
            }
        }

        /// <summary>
        /// Disables all the specified components in the XR Rig.
        /// </summary>
        public void DisableComponents()
        {
            foreach (var component in componentsToToggle)
            {
                if (component != null)
                    component.enabled = false;
            }
        }
    }
}