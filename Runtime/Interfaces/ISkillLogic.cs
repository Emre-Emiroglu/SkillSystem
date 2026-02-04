using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;

namespace SkillSystem.Runtime.Interfaces
{
    public interface ISkillLogic<in TSkillData> where TSkillData : SkillData
    {
        public SkillState GetSkillState { get; }
        public void Initialize(TSkillData skillData);
        public void ChangeState(SkillState newSkillState);
    }
}