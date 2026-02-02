using UnityEngine;

namespace AbilitySystem.Runtime.Data
{
    /// <summary>
    /// Base class for all AbilityData ScriptableObjects.
    /// Stores metadata used by ability logic classes.
    /// </summary>
    public abstract class AbilityData : ScriptableObject
    {
        #region Fields
        [Header("Ability Data Fields")]
        [SerializeField] private string abilityName;
        #endregion
        
        #region Properities
        /// <summary>
        /// The unique name of this ability. Must match the related ability class.
        /// </summary>
        public string AbilityName
        {
            get => abilityName;
            set => abilityName = value;
        }
        #endregion
    }
}