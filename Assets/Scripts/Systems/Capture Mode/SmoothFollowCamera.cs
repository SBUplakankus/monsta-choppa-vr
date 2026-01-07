using UnityEngine;

namespace Systems
{
    public class SmoothFollowCamera : MonoBehaviour
    {
        public Transform target;
        public bool smooth = true;

        [Header("Smoothing")]
        [Range(0f, 12f)] public float posRate = 8f;
        [Range(0f, 12f)] public float rotRate = 5f;

        [Header("Offsets")]
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        private void OnEnable()
        {
            if (target == null) return;

            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        private void LateUpdate()
        {
            if (!target) return;

            Vector3 targetPos =
                target.position + target.rotation * positionOffset;

            Quaternion targetRot =
                target.rotation * Quaternion.Euler(rotationOffset);

            if (smooth)
            {
                float posT = Mathf.Clamp01(posRate * Time.deltaTime);
                float rotT = Mathf.Clamp01(rotRate * Time.deltaTime);

                transform.position =
                    Vector3.Lerp(transform.position, targetPos, posT);

                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRot, rotT);
            }
            else
            {
                transform.position = targetPos;
                transform.rotation = targetRot;
            }
        }
    }
}
