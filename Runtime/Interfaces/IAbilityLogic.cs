using AbilitySystem.Runtime.Data;

namespace AbilitySystem.Runtime.Interfaces
{
    /// <summary>
    /// Interface defining the standard ability lifecycle: initialize, execute, cancel.
    /// </summary>
    /// <typeparam name="TAbilityData">The AbilityData type used by this ability.</typeparam>
    public interface IAbilityLogic<in TAbilityData> where TAbilityData : AbilityData
    {
        /// <summary>
        /// Initializes the ability using the provided data.
        /// </summary>
        /// <param name="abilityData">The data used to initialize the ability.</param>
        public void Initialize(TAbilityData abilityData);
        
        /// <summary>
        /// Executes the ability logic.
        /// </summary>
        public void Execute();
        
        /// <summary>
        /// Cancels the ability logic.
        /// </summary>
        public void Cancel();
    }
}