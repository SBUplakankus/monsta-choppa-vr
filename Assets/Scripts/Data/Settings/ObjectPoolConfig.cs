using UnityEngine;

namespace Data.Settings
{
    public class ObjectPoolConfig : ScriptableObject
    {
        [SerializeField] private int preWarmCount = 5;
        [SerializeField] private int maxWarmCount = 5;
    }
}