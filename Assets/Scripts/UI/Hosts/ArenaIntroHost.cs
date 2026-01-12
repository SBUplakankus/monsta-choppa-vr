using UI.Views;

namespace UI.Hosts
{
    public class ArenaIntroHost : BasePanelHost
    {
        private ArenaIntroView _arenaIntroView;
        private string _arenaKey, _difficultyKey;
        
        public void DisplayArenaIntro(string arenaKey, string difficultyKey)
        {
            _arenaKey = arenaKey;
            _difficultyKey = difficultyKey;
            Generate();
            Show();
        }

        public void HideArenaIntro()
        {
            Hide();
        }
        
        public override void Generate()
        {
            _arenaIntroView = new ArenaIntroView(
                uiDocument.rootVisualElement,
                styleSheet,
                _arenaKey,
                _difficultyKey);
        }

        public override void Dispose()
        {
            _arenaIntroView.Dispose();
            _arenaIntroView = null;
        }
    }
}