using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AbilitySystem.Runtime.Data;
using UnityEngine;

namespace AbilitySystem.Runtime.Managers
{
    /// <summary>
    /// Manages to load, spawning, executing and pooling ability instances at runtime.
    /// 
    /// This manager:
    /// - Loads all AbilityData assets from Resources.
    /// - Automatically detects all ability classes derived from BaseAbility&lt;&gt;
    /// - Supports pooling of multiple instances per ability.
    /// - Allows execution and cancellation of specific or all active instances.
    /// 
    /// Designed to be fully dynamic and independent of gameplay code.
    /// </summary>
    public static class AbilityManager
    {
        #region Constants
        private const string ResourcesAbilityDataFolder = "AbilitySystem";
        #endregion
        
        #region Data Maps
        private static readonly Dictionary<string, AbilityData> DataMap = new();
        private static readonly Dictionary<string, Stack<object>> DeActiveAbilityInstanceMap = new();
        private static readonly Dictionary<string, HashSet<object>> ActiveAbilityInstanceMap = new();
        #endregion
        
        #region Executes
        /// <summary>
        /// Initializes the AbilityManager by loading AbilityData assets and preparing instance containers.
        /// Call this once at game startup before using any ability logic.
        /// </summary>
        public static void InitializeManager()
        {
            Debug.Log("<color=yellow>[AbilityManager] Initializing...</color>");

            LoadAllAbilityData();
            
            PrepareDictionaries();

            Debug.Log("<color=green>[AbilityManager] Ready.</color>");
        }
        private static void LoadAllAbilityData()
        {
            DataMap.Clear();

            AbilityData[] allData = Resources.LoadAll<AbilityData>(ResourcesAbilityDataFolder);

            foreach (var data in allData)
                if (!string.IsNullOrWhiteSpace(data.AbilityName))
                    DataMap[data.AbilityName] = data;

            Debug.Log($"[AbilityManager] Loaded {DataMap.Count} AbilityData assets.");
        }
        private static void PrepareDictionaries()
        {
            DeActiveAbilityInstanceMap.Clear();
            
            ActiveAbilityInstanceMap.Clear();

            foreach (string ability in DataMap.Keys)
            {
                DeActiveAbilityInstanceMap[ability] = new Stack<object>(8);
                
                ActiveAbilityInstanceMap[ability] = new HashSet<object>();
            }
        }

        /// <summary>
        /// Spawns a new ability instance.  
        /// If a pooled instance exists, it is reused.  
        /// Otherwise, a new instance is created via reflection.
        /// </summary>
        /// <param name="abilityName">The name of the ability to spawn.</param>
        /// <returns>The spawned ability instance, or null if spawning fails.</returns>
        public static object Spawn(string abilityName)
        {
            if (!DataMap.TryGetValue(abilityName, out AbilityData data))
            {
                Debug.LogError($"[AbilityManager] No AbilityData found for {abilityName}");
                
                return null;
            }

            object instance;

            if (DeActiveAbilityInstanceMap[abilityName].Count > 0)
                instance = DeActiveAbilityInstanceMap[abilityName].Pop();
            else
            {
                Type type = FindAbilityType(abilityName);
                
                if (type == null)
                {
                    Debug.LogError($"[AbilityManager] No Ability class found for {abilityName}");
                    
                    return null;
                }

                instance = Activator.CreateInstance(type);
            }

            MethodInfo initialize = instance.GetType().GetMethod("Initialize");
            
            initialize?.Invoke(instance, new object[] { data });

            ActiveAbilityInstanceMap[abilityName].Add(instance);

            return instance;
        }
        
        /// <summary>
        /// Releases an ability instance back into the pool.
        /// Cancels it if necessary and marks it as inactive.
        /// </summary>
        /// <param name="instance">The ability instance to release.</param>
        public static void Release(object instance)
        {
            if (instance == null)
                return;

            string abilityName = ExtractAbilityName(instance.GetType().Name);

            if (!ActiveAbilityInstanceMap.TryGetValue(abilityName, out HashSet<object> activeAbilityInstances))
            {
                Debug.LogWarning($"[AbilityManager] Tried to release unknown ability → {abilityName}");
                
                return;
            }

            MethodInfo cancel = instance.GetType().GetMethod("Cancel");
            
            cancel?.Invoke(instance, null);

            activeAbilityInstances.Remove(instance);
            
            DeActiveAbilityInstanceMap[abilityName].Push(instance);
        }
        
        /// <summary>
        /// Cancels and releases all active instances of the specified ability.
        /// </summary>
        /// <param name="abilityName">The ability to cancel all instances for.</param>
        public static void CancelAll(string abilityName)
        {
            if (!ActiveAbilityInstanceMap.TryGetValue(abilityName, out HashSet<object> activeAbilityInstances))
                return;

            List<object> snapshot = activeAbilityInstances.ToList();

            foreach (object instance in snapshot)
                Release(instance);
        }
        
        /// <summary>
        /// Executes a single ability instance.
        /// </summary>
        /// <param name="instance">The ability of instance to execute.</param>
        public static void Execute(object instance)
        {
            if (instance == null)
                return;

            MethodInfo execute = instance.GetType().GetMethod("Execute");
            
            execute?.Invoke(instance, null);
        }
        
        /// <summary>
        /// Executes all active instances of the specified ability.
        /// </summary>
        /// <param name="abilityName">The ability to execute all active instances for.</param>
        public static void ExecuteAll(string abilityName)
        {
            if (!ActiveAbilityInstanceMap.TryGetValue(abilityName, out HashSet<object> activeAbilityInstances))
                return;

            foreach (object instance in activeAbilityInstances)
                Execute(instance);
        }
        private static Type FindAbilityType(string abilityName)
        {
            string expected = abilityName + "Ability";

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
        private static string ExtractAbilityName(string typeName)
        {
            const string suffix = "Ability";
    
            return typeName.EndsWith(suffix) ? typeName[..^suffix.Length] : typeName;
        }
        #endregion
    }
}
