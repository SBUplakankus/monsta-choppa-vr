using UnityEngine;

namespace Data.Core
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/Core/Audio")]
    public class AudioClipData : ScriptableObject
    {
        [SerializeField] private AudioClip audioClipData;
        
        public AudioClip Clip => audioClipData;
        public string ID => name;
    }
}
