using TMPro;
using UnityEngine;

namespace Databases.Base
{
    public static class DataTypes
    {
        [CreateAssetMenu(menuName = "Scriptable Objects/Data/AudioClip Data")]
        public class AudioClipData : ScriptableObject
        {
            public string id;
            public AudioClip clip;
            public float volume = 1f;
            public bool loop = false;
        }
        
        [CreateAssetMenu(menuName = "Scriptable Objects/Data/TMP Font Data")]
        public class TMPFontData : ScriptableObject
        {
            public string id;
            public TMP_FontAsset fontAsset;
        }
        
        [CreateAssetMenu(menuName = "Scriptable Objects/Data/Sprite Data")]
        public class SpriteData : ScriptableObject
        {
            public string id;
            public Sprite image;
        }
    }
}
