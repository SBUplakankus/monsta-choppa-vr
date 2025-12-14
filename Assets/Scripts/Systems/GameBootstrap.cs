using Events.Base;
using UnityEngine;

namespace Systems
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Player Events")] 
        [SerializeField] private IntEventChannel onPlayerDamaged;
        
        [Header("Audio Events")]
        [SerializeField] private StringEventChannel onMusicRequested;
        [SerializeField] private StringEventChannel onSfxRequested;

        private void Awake()
        {
            GameEvents.OnPlayerDamaged =  onPlayerDamaged;
            GameEvents.OnMusicRequested = onMusicRequested;
            GameEvents.OnSfxRequested = onSfxRequested;
        }
    }
}
