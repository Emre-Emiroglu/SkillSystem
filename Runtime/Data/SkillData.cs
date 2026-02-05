using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem.Runtime.Data
{
    public abstract class SkillData : ScriptableObject
    {
        #region Fields
        [Header("Skill Data Field")]
        [SerializeField] private string skillName;
        [SerializeField] private List<SkillData> prerequisites = new();
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
        
#if UNITY_EDITOR
        [SerializeField] private Vector2 editorPosition;
        public Vector2 EditorPosition
        {
            get => editorPosition;
            set => editorPosition = value;
        }
#endif
    }
}