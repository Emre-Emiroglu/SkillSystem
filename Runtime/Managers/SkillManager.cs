using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Interfaces;
using UnityEngine;

namespace SkillSystem.Runtime.Managers
{
    public static class SkillManager
    {
        #region Constants
        private const string ResourcesSkillDataFolder = "SkillSystem";
        #endregion

        #region ReadonlyFields
        private static readonly Dictionary<string, SkillData> DataMap = new();
        private static readonly Dictionary<string, object> InstanceMap = new();
        #endregion

        #region Executes
        public static void InitializeManager()
        {
            LoadAllSkillData();
            
            PrepareInstanceMap();
        }
        public static void InitializeSkill(string skillName)
        {
            if (!TryGetSkill(skillName, out SkillData data, out object instance))
                return;

            InvokeMethod(instance, "Initialize", new object[] { data });
        }
        public static void ChangeSkillState(string skillName, SkillState newState)
        {
            if (!TryGetSkill(skillName, out _, out object instance))
                return;

            InvokeMethod(instance, "ChangeState", new object[] { newState });
            
            CheckSkillPrerequisites();
        }
        private static void LoadAllSkillData()
        {
            DataMap.Clear();

            SkillData[] allData = Resources.LoadAll<SkillData>(ResourcesSkillDataFolder);

            foreach (SkillData data in allData)
                if (!string.IsNullOrWhiteSpace(data.SkillName))
                    DataMap[data.SkillName] = data;
        }
        private static void PrepareInstanceMap()
        {
            InstanceMap.Clear();

            foreach (string skillName in DataMap.Keys)
                InstanceMap[skillName] = null;
        }
        private static void CheckSkillPrerequisites()
        {
            foreach ((string skillName, SkillData data) in DataMap)
            {
                if (!TryGetSkill(skillName, out _, out object instance))
                    continue;

                if (instance is not ISkillLogic<SkillData> skill)
                    continue;

                if (skill.GetSkillState != SkillState.Locked)
                    continue;

                bool allUnlocked = data.Prerequisites.All(p =>
                    InstanceMap.TryGetValue(p.SkillName, out object prereqInstance) &&
                    prereqInstance is ISkillLogic<SkillData>
                        { GetSkillState: SkillState.Unlocked });

                if (allUnlocked)
                    ChangeSkillState(skillName, SkillState.Unlockable);
            }
        }
        private static bool TryGetSkill(string skillName, out SkillData data, out object instance)
        {
            data = null;
            instance = null;

            if (!DataMap.TryGetValue(skillName, out data))
                return false;

            if (InstanceMap.TryGetValue(skillName, out instance) && instance != null)
                return true;
            
            Type type = FindSkillType(skillName);
                
            if (type == null)
                return false;

            instance = Activator.CreateInstance(type);
                
            InstanceMap[skillName] = instance;

            return true;
        }
        private static Type FindSkillType(string skillName)
        {
            string expected = skillName + "Skill";

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            }).FirstOrDefault(t => t.Name == expected);
        }
        private static void InvokeMethod(object instance, string methodName, object[] parameters)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName);
                    
            method?.Invoke(instance, parameters);
        }
        #endregion
    }
}