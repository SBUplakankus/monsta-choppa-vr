using UI.Controllers;
using UI.Views;

namespace UI.Hosts
{
    public class LoadingScreenHost : BasePanelHost
    {
        private LoadingScreenView _view;
        
        public override void Generate()
        {
            Dispose();
            _view = new LoadingScreenView(uiDocument.rootVisualElement, styleSheet);
        }
        
        private void UpdateProgress(LoadProgress loadProgress) => _view?.UpdateLoadingBar(loadProgress);
        
        public void DisplayLoadingScreen(LoadProgress loadProgress)
        {
            Generate();
            UpdateProgress(loadProgress);
            Show();
        }
        
        public void UpdateLoadingScreen(LoadProgress loadProgress) => UpdateProgress(loadProgress);
        public void HideLoadingScreen() => Hide();
        
        protected override void Dispose()
        {
            _view?.Dispose();
            _view = null;
        }
    }
}