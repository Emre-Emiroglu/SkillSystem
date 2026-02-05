using System.IO;
using NUnit.Framework;
using SkillSystem.Editor.Generators;
using SkillSystem.Runtime.Data;
using UnityEditor;

namespace SkillSystem.Tests.EditMode
{
    public sealed class EditModeTests
    {
        #region Constants
        private const string TestSkill = "TestSkill";
        private const string DataFolder = "Assets/Resources/SkillSystem";
        private const string ClassFolder = "Assets/SkillSystem/Runtime";
        #endregion

        #region Tests
        [Test]
        public void GenerateSkillClasses_Test()
        {
            SkillDataGenerator.CreateSkillData(TestSkill);
            
            Assert.IsTrue(File.Exists($"{ClassFolder}/Data/{TestSkill}Data.cs"));

            SkillClassGenerator.CreateSkillClass(TestSkill);
            
            Assert.IsTrue(File.Exists($"{ClassFolder}/Skills/{TestSkill}Skill.cs"));
        }
        [Test]
        public void CreateSo_Test()
        {
            SkillDataGenerator.CreateSo(TestSkill);

            SkillData so = AssetDatabase.LoadAssetAtPath<SkillData>($"{DataFolder}/{TestSkill}Data.asset");

            Assert.IsNotNull(so, "ScriptableObject should be created.");
            
            Assert.AreEqual(TestSkill, so.SkillName);
        }
        #endregion
    }
}