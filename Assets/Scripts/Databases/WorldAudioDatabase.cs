using Data.Core;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/World Audio")]
    public class WorldAudioDatabase : DatabaseBase<WorldAudioData>
    {
        protected override string GetKey(WorldAudioData entry) => entry.ID;
    }
}