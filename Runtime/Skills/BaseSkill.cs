using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Interfaces;

namespace SkillSystem.Runtime.Skills
{
    /// <summary>
    /// Base implementation for runtime skill logic.
    /// </summary>
    /// <typeparam name="TSkillData">Type of SkillData used by the skill.</typeparam>
    public abstract class BaseSkill<TSkillData> : ISkillLogic<TSkillData> where TSkillData : SkillData
    {
        #region Fields
        protected TSkillData SkillData;
        protected SkillState SkillState;
        #endregion

        #region Core
        /// <summary>
        /// Initializes the skill with its associated data.
        /// </summary>
        /// <param name="skillData">Skill data instance.</param>
        public virtual void Initialize(TSkillData skillData) => SkillData = skillData;
        
        /// <summary>
        /// Changes the current runtime state of the skill.
        /// </summary>
        /// <param name="newSkillState">New skill state.</param>
        public virtual void ChangeState(SkillState newSkillState) => SkillState = newSkillState;
        #endregion
    }
}