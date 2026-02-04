using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Interfaces;
using UnityEngine;

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
        public void Initialize(TSkillData skillData)
        {
            SkillData = skillData;
            SkillState = SkillState.Locked;
            
            LogMessage($"Skill: {SkillData.SkillName} Initialized");
        }
        public virtual void ChangeState(SkillState newSkillState)
        {
            SkillState = newSkillState;

            LogMessage($"Skill: {SkillData.SkillName} State Changed → {SkillState}");
        }
        #endregion
        
        #region Executes
        private static void LogMessage(string message) => Debug.Log(message);
        #endregion
    }
}