using UnityEngine;

namespace Databases.Base
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/AudioClip Database")]
    public class AudioClipDatabase : DatabaseBase<AudioClipData>
    {
        protected override string GetKey(AudioClipData entry) => entry.id;
    }
    
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/TMP Font Database")]
    public class TMPFontDatabase : DatabaseBase<TMPFontData>
    {
        protected override string GetKey(TMPFontData entry) => entry.id;
    }
    
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Sprite Database")]
    public class SpriteDatabase : DatabaseBase<SpriteData>
    {
        protected override string GetKey(SpriteData entry) => entry.id;
    }
}