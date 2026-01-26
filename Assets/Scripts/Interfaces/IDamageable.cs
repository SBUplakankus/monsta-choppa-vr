namespace Interfaces
{
    /// <summary>
    /// Interface for entities that can receive damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage to this entity.
        /// </summary>
        /// <param name="damage">Amount of damage.</param>
        void TakeDamage(int damage);
    }
}