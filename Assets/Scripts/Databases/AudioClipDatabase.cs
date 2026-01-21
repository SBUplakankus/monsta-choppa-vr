using Data.Core;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Audio")]
    public class AudioClipDatabase : DatabaseBase<AudioClipData>
    {
        protected override string GetKey(AudioClipData entry) => entry.ID;
    }
}
