namespace Constants
{
    public static class GameConstants
    {
        #region Attribute Keys
        
        public const string PlayerGoldKey = "PlayerGold";
        public const string PlayerExperienceKey = "PlayerExperience";
        public const string PlayerLevelKey = "PlayerLevel";
        
        #endregion

        #region Scene Names

        public const string Bootstrapper = "Bootstrapper";
        public const string StartMenu = "StartMenu";
        public const string Hub = "Hub";
        public const string GoblinCampDay = "GoblinCampDay";


        #endregion
        
        #region Arena Properties

        public const int PreludeDuration = 10;
        public const int WaveIntermissionDuration = 15;
        public const int BossIntermissionDuration = 15;

        #endregion

        #region Animation Keys

        public const int MageWeaponKey = 0;
        public const int TwoHandAxeKey = 1;
        public const int TwoHandSwordKey = 2;
        public const int SwordShieldKey = 3;
        public const int BowKey = 4;
        
        public const int LightAttackCount = 3;
        public const int HeavyAttackCount = 2;

        public const string LightAttackTrigger = "LightAttack";
        public const string LightAttackIndex = "LightAttackIndex";
        public const string HeavyAttackTrigger = "HeavyAttack";
        public const string HeavyAttackIndex = "HeavyAttackIndex";
        
        public const string HitLeftTrigger = "HitLeft";
        public const string HitRightTrigger = "HitRight";
        public const string HitFrontTrigger = "HitFront";
        
        public const string SpeedParam = "Speed";
        public const string IsMovingParam = "IsMoving";
        public const string IsAttackingParam = "IsAttacking";
        
        public const string WeaponTypeParam = "WeaponType";

        #endregion
        
        #region Combat Settings
        
        public const float DefaultMeleeAttackRange = 2.0f;
        public const float DefaultAttackCooldown = 1.5f;
        public const float MinSwingVelocity = 1.0f;
        public const float MaxSwingVelocity = 5.0f;
        
        public const float MinVelocityDamageMultiplier = 0.5f;
        public const float MaxVelocityDamageMultiplier = 2.0f;
        public const float InvincibilityDuration = 0.2f;
        
        #endregion
        
        #region Projectile Settings
        
        public const float DefaultProjectileSpeed = 20f;
        public const float DefaultProjectileLifetime = 5f;
        public const float DefaultBowDrawTime = 1.5f;
        public const float MinBowDrawStrength = 0.2f;
        
        #endregion

        #region Save Keys

        public const string MetaProgressionKey = "MetaProgression";
        public const string AudioSettingsKey = "AudioSettings";
        public const string VideoSettingsKey = "VideoSettings";
        public const string LocalizationSettingsKey = "LocalizationSettings";

        #endregion
    }
}
