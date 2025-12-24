using Weapons;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Weapon Database")]
    public class WeaponDatabase : DatabaseBase<WeaponData>
    {
        public WeaponData[] Weapons => entries;
        protected override string GetKey(WeaponData entry) => entry.WeaponID;
    }
}