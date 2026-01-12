using UI.Views;

namespace UI.Hosts
{
    public class BossIntroHost : BasePanelHost
    {
        private BossIntroView _bossIntroView;
        private string _bossKey;

        public void DisplayBossIntro(string bossKey)
        {
            _bossKey = bossKey;
            Generate();
            Show();
        }

        public void HideBossIntro()
        {
            gameObject.SetActive(false);
        }
        
        public override void Generate()
        {
            _bossIntroView = new BossIntroView(
                uiDocument.rootVisualElement, 
                styleSheet, 
                _bossKey);
        }

        public override void Dispose()
        {
            _bossIntroView.Dispose();
            _bossKey = null;
            _bossIntroView = null;
        }
    }
}