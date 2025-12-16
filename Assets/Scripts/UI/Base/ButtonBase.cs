using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI.Base
{
    [Serializable]
    public class ButtonBase : MonoBehaviour
    {
        [SerializeField] private string buttonName;
        [SerializeField] private UnityEvent onButtonClicked = new();
        private Button mButton;

        private void Start()
        {
            var uiToolkitDoc = GetComponent<UIDocument>();
            if (uiToolkitDoc == null) return;
            
            var root = uiToolkitDoc.rootVisualElement;

            if (string.IsNullOrEmpty(buttonName)) return;
            
            var nameSelector = buttonName.StartsWith("#") 
                ? buttonName 
                : "#" + buttonName;
            
            mButton = root.Q<Button>(nameSelector);

            if (mButton == null) return;
            
            mButton.clicked += HandleButtonClicked;
        }
        
        private void OnDisable()
        {
            if(mButton == null) return;
            mButton.clicked -= HandleButtonClicked;
        } 

        private void HandleButtonClicked()
        {
            onButtonClicked?.Invoke();
            Debug.Log(buttonName);
        }
    }
}
