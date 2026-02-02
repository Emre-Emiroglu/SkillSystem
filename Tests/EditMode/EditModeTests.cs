using System.IO;
using AbilitySystem.Editor.Generators;
using AbilitySystem.Runtime.Data;
using NUnit.Framework;
using UnityEditor;

namespace AbilitySystem.Tests.EditMode
{
    public sealed class EditModeTests
    {
        #region Constants
        private const string TestAbility = "TestAbility";
        private const string DataFolder = "Assets/Resources/AbilitySystem";
        private const string ClassFolder = "Assets/AbilitySystem/Runtime";
        #endregion

        #region Tests
        [Test]
        public void GenerateAbilityClasses_Test()
        {
            AbilityDataGenerator.CreateAbilityData(TestAbility);
            
            Assert.IsTrue(File.Exists($"{ClassFolder}/Data/{TestAbility}Data.cs"));

            AbilityClassGenerator.CreateAbilityClass(TestAbility);
            
            Assert.IsTrue(File.Exists($"{ClassFolder}/Abilities/{TestAbility}Ability.cs"));
        }
        [Test]
        public void CreateSo_Test()
        {
            AbilityDataGenerator.CreateSo(TestAbility);

            AbilityData so = AssetDatabase.LoadAssetAtPath<AbilityData>($"{DataFolder}/{TestAbility}Data.asset");

            Assert.IsNotNull(so, "ScriptableObject should be created.");
            
            Assert.AreEqual(TestAbility, so.AbilityName);
        }
        #endregion
    }
}