using Characters.Enemies;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Enemy Database")]
    public class EnemyDatabase : DatabaseBase<EnemyData>
    {
        protected override string GetKey(EnemyData entry) => entry.EnemyId;
        
        public EnemyData[] Enemies => entries;
    }
}