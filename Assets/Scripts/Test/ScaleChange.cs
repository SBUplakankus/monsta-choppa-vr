using Events;
using UnityEngine;

namespace Test
{
    public class ScaleChange : MonoBehaviour
    {
        public FloatEventChannel scaleChanged;
        public VoidEventChannel spinClicked;

        private float _targetScale = 1;
        private Vector3 scaleVel;
        private Quaternion _targetRotation;

        private void OnEnable()
        { 
            scaleChanged.Subscribe(OnScaleChanged);
            spinClicked.Subscribe(OnSpinClicked);
        }

        private void OnDisable()
        {
            scaleChanged.Unsubscribe(OnScaleChanged);
            spinClicked.Unsubscribe(OnSpinClicked);
        } 
        
        private void OnScaleChanged(float newScale)
        {
            _targetScale = newScale;
        }

        private void OnSpinClicked()
        {
            _targetRotation = transform.rotation * Quaternion.Euler(Random.insideUnitSphere * 360);
        }

        private void Update()
        {
            transform.localScale = 
                Vector3.SmoothDamp(transform.localScale,
                    _targetScale * Vector3.one,
                    ref scaleVel,
                    0.15f);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                _targetRotation, Time.deltaTime * 5);
        }
    }
}
