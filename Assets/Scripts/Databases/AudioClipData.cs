using TMPro;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/Audio Clip")]
    public class AudioClipData : ScriptableObject
    {
        [SerializeField] private AudioClip audioClipData;
        
        public AudioClip Clip => audioClipData;
        public string ID => name;
    }
}
