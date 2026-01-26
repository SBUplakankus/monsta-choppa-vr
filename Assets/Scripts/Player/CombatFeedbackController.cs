using UnityEngine;

namespace Player
{
    public class CombatFeedbackController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHitAudio hitAudio;
        [SerializeField] private PlayerHapticFeedback haptics;
    }
}