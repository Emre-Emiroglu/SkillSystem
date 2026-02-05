using UnityEngine;

namespace SkillSystem.Runtime.Data
{
    /// <summary>
    /// Base ScriptableObject class that holds static data for a skill.
    /// </summary>
    public abstract class SkillData : ScriptableObject
    {
        #region Fields
        [Header("Skill Data Field")]
        [SerializeField] private string skillName;
        #endregion

        #region Properities
        /// <summary>
        /// Unique name identifier of the skill.
        /// </summary>
        public string SkillName
        {
            get => skillName;
            set => skillName = value;
        }
        #endregion
    }
}