using UI.Views;

namespace UI.Hosts
{
    public class LabelHost : BasePanelHost
    {
        private LabelView _view;
        public override void Generate()
        {
            Dispose();
            _view = new LabelView(uiDocument.rootVisualElement, styleSheet);
        }

        protected override void Dispose()
        {
            _view?.Dispose();
        }
    }
}