using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;

namespace SkillSystem.Runtime.Interfaces
{
    /// <summary>
    /// Defines the runtime logic interface for a skill.
    /// </summary>
    /// <typeparam name="TSkillData">Type of SkillData associated with this skill.</typeparam>
    public interface ISkillLogic<in TSkillData> where TSkillData : SkillData
    {
        /// <summary>
        /// Initializes the skill with its associated data.
        /// </summary>
        /// <param name="skillData">Skill data instance.</param>
        public void Initialize(TSkillData skillData);
        
        /// <summary>
        /// Changes the current runtime state of the skill.
        /// </summary>
        /// <param name="newSkillState">New skill state.</param>
        public void ChangeState(SkillState newSkillState);
    }
}