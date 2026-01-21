using Data.Core;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Databases/Particle")]
    public class ParticleDatabase : DatabaseBase<ParticleData>
    {
        protected override string GetKey(ParticleData entry) =>  entry.ID;
    }
}