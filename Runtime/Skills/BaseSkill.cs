using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Interfaces;

namespace SkillSystem.Runtime.Skills
{
    public abstract class BaseSkill<TSkillData> : ISkillLogic<TSkillData> where TSkillData : SkillData
    {
        #region Fields
        protected TSkillData SkillData;
        protected SkillState SkillState;
        #endregion

        #region Getters
        public SkillState GetSkillState => SkillState;
        #endregion
        
        #region Core
        public virtual void Initialize(TSkillData skillData) => SkillData = skillData;
        public virtual void ChangeState(SkillState newSkillState) => SkillState = newSkillState;
        #endregion
    }
}