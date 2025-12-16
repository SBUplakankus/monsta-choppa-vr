using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Panels
{
    public class IntroPanel : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private StyleSheet styleSheet;

        [SerializeField] private FloatEventChannel scaleChanged;
        [SerializeField] private VoidEventChannel spinClicked;

        private void Start()
        {
            StartCoroutine(Generate());
        }

        private void OnValidate()
        {
            if(Application.isPlaying) return;
            StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            yield return null;
            var root = uiDocument.rootVisualElement;
            root.Clear();
            
            root.styleSheets.Add(styleSheet);
            
            var container = CreateContainer("container");
            
            var viewBox = CreateContainer("view-box", "bordered-box");
            container.Add(viewBox);
            
            var controlBox = CreateContainer("control-box", "bordered-box");
            container.Add(controlBox);

            var spinBtn = Create<Button>();
            spinBtn.text = "Spin";
            spinBtn.clicked += () => spinClicked.Raise();
            controlBox.Add(spinBtn);

            var scaleSlider = Create<Slider>();
            scaleSlider.lowValue = 0.5f;
            scaleSlider.highValue = 2f;
            scaleSlider.value = 1f;
            scaleSlider.RegisterCallback<ChangeEvent<float>>(v=> scaleChanged.Raise(v.newValue));
            controlBox.Add(scaleSlider);
            
            root.Add(container);
            
        }

        private VisualElement CreateContainer(params string[] classNames)
        {
            return Create<VisualElement>(classNames);
        }

        private static T Create<T>(params string[] classNames) where T : VisualElement, new()
        {
            var element = new T();
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }
            return element;
        }
    }
}
