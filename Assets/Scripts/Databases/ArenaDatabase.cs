using Data.Arena;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Arena")]
    public class ArenaDatabase : DatabaseBase<ArenaData>
    {
        protected override string GetKey(ArenaData entry) => entry.ID;
    }
}