using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem.Runtime.Data
{
    public abstract class SkillData : ScriptableObject
    {
        #region Fields
        [Header("Skill Data Field")]
        [SerializeField] private string skillName;
        [SerializeField] private List<SkillData> prerequisites;
        #endregion

        #region Properities
        public string SkillName
        {
            get => skillName;
            set => skillName = value;
        }
        public List<SkillData> Prerequisites
        {
            get => prerequisites;
            set => prerequisites = value;
        }
        #endregion
    }
}