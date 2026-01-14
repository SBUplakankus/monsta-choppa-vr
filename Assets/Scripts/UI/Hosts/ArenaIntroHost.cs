using Systems.Arena;
using UI.Views;
using UnityEngine;

namespace UI.Hosts
{
    public class ArenaIntroHost : BasePanelHost
    {
        [SerializeField] private ArenaData arenaData;
        private ArenaIntroView _arenaIntroView;
        
        public void DisplayArenaIntro()
        {
            Generate();
            Show();
        }

        public void HideArenaIntro()
        {
            Hide();
        }
        
        public override void Generate()
        {
            Dispose();
            _arenaIntroView = new ArenaIntroView(
                uiDocument.rootVisualElement,
                styleSheet,
                arenaData.Location,
                arenaData.Difficulty);
        }

        protected override void Dispose()
        {
            _arenaIntroView?.Dispose();
            _arenaIntroView = null;
        }
        
        private void OnEnable() => Generate();
        private void OnDisable() => Dispose();
    }
}