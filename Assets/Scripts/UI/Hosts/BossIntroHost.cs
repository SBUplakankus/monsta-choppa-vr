using Data.Arena;
using Systems.Arena;
using UI.Views;
using UnityEngine;

namespace UI.Hosts
{
    public class BossIntroHost : BasePanelHost
    {
        [SerializeField] private ArenaData arenaData;
        private BossIntroView _bossIntroView;

        public void DisplayBossIntro()
        {
            Generate();
        }

        public void HideBossIntro()
        {
            gameObject.SetActive(false);
        }
        
        public override void Generate()
        {
            Dispose();
            
            _bossIntroView = new BossIntroView(
                uiDocument.rootVisualElement, 
                styleSheet, 
                arenaData.BossKey);
            
            Show();
        }

        protected override void Dispose()
        {
            _bossIntroView?.Dispose();
            _bossIntroView = null;
        }
        
        private void OnDisable() => Dispose();
    }
}