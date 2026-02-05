using System;
using System.Reflection;
using NUnit.Framework;
using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Managers;
using UnityEngine;

namespace SkillSystem.Tests.PlayMode
{
    public sealed class PlayModeTests
    {
        #region Constants
        private const string TestSkill = "TestSkill";
        private const string SoPath = "SkillSystem/TestSkillData";
        #endregion
        
        #region Setup
        [SetUp]
        public void Setup()
        {
            SkillManager.InitializeManager();

            Assert.NotNull(Resources.Load<SkillData>(SoPath), "SkillData must exist in Resources for PlayMode tests.");
        }
        #endregion

        #region Tests
        [Test]
        public void Initialize_Skill()
        {
            SkillManager.InitializeSkill(TestSkill);
            
            SkillManager.TryGetSkill(TestSkill, out _, out object instance);
            
            Assert.NotNull(instance, "Initialize should return a valid instance on first request.");
        }
        [Test]
        public void Change_State_Skill()
        {
            Initialize_Skill();
            
            SkillManager.ChangeSkillState(TestSkill, SkillState.Unlockable);
            
            SkillManager.TryGetSkill(TestSkill, out _, out object instance);

            FieldInfo fieldInfo = FindField(instance.GetType(), "SkillState");
            
            object value = fieldInfo?.GetValue(instance);

            Assert.AreEqual(SkillState.Unlockable, value, "SkillState should be the same after ChangeSkillState.");
        }
        #endregion

        #region Executes
        private static FieldInfo FindField(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                
                if (fieldInfo != null)
                    return fieldInfo;

                type = type.BaseType;
            }
            return null;
        }
        #endregion
    }
}
