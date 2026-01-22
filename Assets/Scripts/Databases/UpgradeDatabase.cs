using Data.Progression;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Upgrades")]
    public class UpgradeDatabase : DatabaseBase<UpgradeData>
    {
        protected override string GetKey(UpgradeData entry) => entry.ID;
    }
}