using AbilitySystem.Runtime.Data;
using AbilitySystem.Runtime.Managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AbilitySystem.Tests.PlayMode
{
    public sealed class PlayModeTests
    {
        #region Constants
        private const string TestAbility = "TestAbility";
        private const string SoPath = "AbilitySystem/TestAbilityData";
        #endregion
        
        #region Setup
        [SetUp]
        public void Setup()
        {
            AbilityManager.InitializeManager();

            Assert.NotNull(Resources.Load<AbilityData>(SoPath),
                "AbilityData must exist in Resources for PlayMode tests.");
        }
        #endregion

        #region Tests
        [Test]
        public void Spawn_ReturnsNewInstance_WhenPoolEmpty()
        {
            object instance = AbilityManager.Spawn(TestAbility);

            Assert.NotNull(instance, "Spawn should return a valid instance on first request.");
        }
        [Test]
        public void Release_MovesInstanceToPool()
        {
            object instance = AbilityManager.Spawn(TestAbility);

            AbilityManager.Release(instance);

            object instance2 = AbilityManager.Spawn(TestAbility);

            Assert.AreSame(instance, instance2, "Instance should be the same after Release.");
        }
        [Test]
        public void CancelAll_CancelsEveryActiveInstance()
        {
            AbilityManager.Spawn(TestAbility);
            AbilityManager.Spawn(TestAbility);

            LogAssert.Expect(LogType.Log, $"Ability: {TestAbility} Canceled");
            LogAssert.Expect(LogType.Log, $"Ability: {TestAbility} Canceled");

            AbilityManager.CancelAll(TestAbility);
        }
        [Test]
        public void Execute_CallsAbilityExecute()
        {
            object instance = AbilityManager.Spawn(TestAbility);

            LogAssert.Expect(LogType.Log, $"Ability: {TestAbility} Executed");

            AbilityManager.Execute(instance);
        }
        [Test]
        public void ExecuteAll_ExecutesEveryActiveInstance()
        {
            AbilityManager.Spawn(TestAbility);
            AbilityManager.Spawn(TestAbility);

            LogAssert.Expect(LogType.Log, $"Ability: {TestAbility} Executed");
            LogAssert.Expect(LogType.Log, $"Ability: {TestAbility} Executed");

            AbilityManager.ExecuteAll(TestAbility);
        }
        #endregion
    }
}