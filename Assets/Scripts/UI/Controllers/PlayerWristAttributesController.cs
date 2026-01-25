using Player;
using UI.Hosts;
using UnityEngine;

namespace UI.Controllers
{
    public class PlayerWristAttributesController : MonoBehaviour
    {
        [Header("UI Hosts")]
        [SerializeField] private PlayerArenaAttributeHost health;
        [SerializeField] private PlayerArenaAttributeHost shield;
        
        [Header("Detection")] 
        [SerializeField] private WristProximityDetector wristProximityDetector;

        private void OnEnable()
        {
            if (wristProximityDetector != null)
            {
                wristProximityDetector.OnWristEnterProximity += Show;
                wristProximityDetector.OnWristExitProximity += Hide;
            }

            health.Generate();
            shield.Generate();
            Hide();
        }

        private void OnDisable()
        {
            if (wristProximityDetector == null) return;
            wristProximityDetector.OnWristEnterProximity -= Show;
            wristProximityDetector.OnWristExitProximity -= Hide;
        }

        private void Show()
        {
            health.Show();
            shield.Show();
        }

        private void Hide()
        {
            health.Hide();
            shield.Hide();
        }
    }
}