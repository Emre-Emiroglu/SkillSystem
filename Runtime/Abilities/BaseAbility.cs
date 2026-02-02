using AbilitySystem.Runtime.Data;
using AbilitySystem.Runtime.Interfaces;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    /// <summary>
    /// Base class for all ability implementations.
    /// Provides initialization, execution, and cancellation flow.
    /// </summary>
    /// <typeparam name="TAbilityData">The data type associated with the ability.</typeparam>
    public abstract class BaseAbility<TAbilityData> : IAbilityLogic<TAbilityData> where TAbilityData : AbilityData
    {
        #region Fields
        protected TAbilityData AbilityData;
        #endregion

        #region Core
        /// <summary>
        /// Initializes the ability with its associated data.
        /// </summary>
        /// <param name="abilityData">The data used for initialization.</param>
        public virtual void Initialize(TAbilityData abilityData)
        {
            AbilityData = abilityData;
            
            LogMessage($"Ability: {AbilityData.AbilityName} Initialized");
        }
        
        /// <summary>
        /// Executes the ability logic.
        /// </summary>
        public virtual void Execute() => LogMessage($"Ability: {AbilityData.AbilityName} Executed");
        
        /// <summary>
        /// Cancels the ability.
        /// </summary>
        public virtual void Cancel() => LogMessage($"Ability: {AbilityData.AbilityName} Canceled");
        #endregion

        #region Executes
        private static void LogMessage(string message) => Debug.Log(message);
        #endregion
    }
}