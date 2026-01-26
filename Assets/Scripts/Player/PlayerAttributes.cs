using Attributes;
using Constants;
using Esper.ESave;
using Events;
using Events.Registries;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Manages player attributes (gold, experience, level) with save/load functionality.
    /// Handles attribute changes through event subscriptions.
    /// </summary>
    public class PlayerAttributes  : MonoBehaviour
    {
        #region Fields
        
        [Header("Attributes")]
        [SerializeField] private IntAttribute playerGold;
        [SerializeField] private IntAttribute playerExperience;
        [SerializeField] private IntAttribute playerLevel;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets the current amount of player gold.
        /// </summary>
        /// <value>The gold amount from <see cref="playerGold"/>.</value>
        public int Gold => playerGold.Value;
        
        /// <summary>
        /// Gets the current amount of player experience.
        /// </summary>
        /// <value>The experience amount from <see cref="playerExperience"/>.</value>
        public int Experience => playerExperience.Value;
        
        /// <summary>
        /// Gets the current player level.
        /// </summary>
        /// <value>The level from <see cref="playerLevel"/>.</value>
        public int Level => playerLevel.Value;
        
        #endregion
        
        #region Class Functions

        
        
        #endregion

        #region Event Handlers

        private void HandleGoldIncrease(int amount) => playerGold.Value += amount;
        private void HandleExperienceIncrease(int amount) => playerExperience.Value += amount;
        private void HandleLevelIncrease(int amount) => playerLevel.Value += amount;
        
        private void SubscribeToEvents()
        {
            GameplayEvents.GoldChanged.Subscribe(HandleGoldIncrease);
            GameplayEvents.ExperienceChanged.Subscribe(HandleExperienceIncrease);
            GameplayEvents.LevelChanged.Subscribe(HandleLevelIncrease);
        }

        private void UnsubscribeToEvents()
        {
            GameplayEvents.GoldChanged.Unsubscribe(HandleGoldIncrease);
            GameplayEvents.ExperienceChanged.Unsubscribe(HandleExperienceIncrease);
            GameplayEvents.LevelChanged.Unsubscribe(HandleLevelIncrease);
        }

        #endregion
        
        #region Unity Functions

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();
        
        #endregion
        
    }
}