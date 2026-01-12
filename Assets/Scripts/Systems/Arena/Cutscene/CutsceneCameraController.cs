using UnityEngine;

namespace Systems.Arena.Cutscene
{
    public class CutsceneCameraController : MonoBehaviour
    {
        #region Fields
        
        [Header("Cutscene Camera Points")]
        [SerializeField] private Transform arenaIntroStart;
        [SerializeField] private Transform arenaIntroEnd;
        [SerializeField] private Transform bossIntroStart;
        [SerializeField] private Transform bossIntroEnd;
        
        private Camera _cutsceneCamera;
        private Transform _cameraTransform;
        
        #endregion
        
        #region Class Methods
        
        /// <summary>
        /// Aligns the cutscene camera to the tracked head position, allowing the player to look around.
        /// </summary>
        /// <param name="headTransform">The transform of the player's head during VR.</param>
        public void EnableCamera(Transform headTransform)
        {
            // Parent cutscene camera to the player's head during the cutscene
            _cameraTransform.SetParent(headTransform, false); // Using "false" to keep local position/rotation
            _cutsceneCamera.enabled = true;
        }

        public void DisableCamera()
        {
            _cameraTransform.SetParent(null); // Detach from the head
            _cutsceneCamera.enabled = false;
        }
        
        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            _cutsceneCamera = GetComponent<Camera>();
            _cameraTransform = _cutsceneCamera.transform;
            _cutsceneCamera.enabled = false;
        }
        
        #endregion
    }
}