using Data.Weapons;
using Weapons;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Weapon")]
    public class WeaponDatabase : DatabaseBase<WeaponData>
    {
        protected override string GetKey(WeaponData entry) => entry.WeaponID;
    }
}