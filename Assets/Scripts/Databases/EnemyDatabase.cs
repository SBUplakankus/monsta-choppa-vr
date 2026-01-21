using Characters.Enemies;
using Data.Core;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Enemy")]
    public class EnemyDatabase : DatabaseBase<EnemyData>
    {
        protected override string GetKey(EnemyData entry) => entry.EnemyId;
    }
}