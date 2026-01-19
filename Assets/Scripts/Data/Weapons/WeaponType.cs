namespace Data.Weapons
{
    /// <summary>
    /// Categories of weapons available in the game.
    /// Used for animation switching and weapon behavior.
    /// </summary>
    public enum WeaponCategory
    {
        Sword,
        Bow,
        Staff,
        Axe,
        Dagger,
        Shield,
        ThrowingKnife,
        TwoHandSword,
        TwoHandAxe,
        Mace,
        Spear
    }
    
    /// <summary>
    /// Rarity tiers for weapons affecting stats and visual effects.
    /// </summary>
    public enum WeaponRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    /// <summary>
    /// Damage types for weapons and spells.
    /// Used for elemental effects and enemy resistances.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Fire,
        Frost,
        Lightning,
        Arcane,
        Poison
    }
}
