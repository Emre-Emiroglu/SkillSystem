using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using UnityEngine;

namespace SkillSystem.Runtime.Managers
{
    /// <summary>
    /// Central manager responsible for loading, initializing and controlling skills.
    /// </summary>
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
        /// <summary>
        /// Initializes the skill system by loading all SkillData and preparing instances.
        /// </summary>
        public static void InitializeManager()
        {
            LoadAllSkillData();
            
            PrepareInstanceMap();
        }
        
        /// <summary>
        /// Initializes a specific skill by name.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        public static void InitializeSkill(string skillName)
        {
            if (!TryGetSkill(skillName, out SkillData data, out object instance))
                return;
            
            InvokeMethod(instance, "Initialize", new object[] {data});
        }
        
        /// <summary>
        /// Changes the runtime state of a specific skill.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        /// <param name="newSkillState">New state to apply.</param>
        public static void ChangeSkillState(string skillName, SkillState newSkillState)
        {
            if (!TryGetSkill(skillName, out _, out object instance))
                return;
            
            InvokeMethod(instance, "ChangeState", new object[] {newSkillState});
        }
        
        /// <summary>
        /// Attempts to retrieve the SkillData and runtime instance of a skill.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        /// <param name="data">Associated SkillData.</param>
        /// <param name="instance">Runtime skill instance.</param>
        /// <returns>True if the skill exists.</returns>
        public static bool TryGetSkill(string skillName, out SkillData data, out object instance)
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
            MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                    
            method?.Invoke(instance, parameters);
        }
        #endregion
    }
}