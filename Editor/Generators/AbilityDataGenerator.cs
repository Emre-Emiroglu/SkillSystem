using System;
using System.IO;
using AbilitySystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Editor.Generators
{
    /// <summary>
    /// Generates AbilityData classes and ScriptableObject instances.
    /// Handles file creation and linking ability names.
    /// </summary>
    public static class AbilityDataGenerator
    {
        #region Constants
        private const string AbilityDataFolder = "Assets/Resources/AbilitySystem";
        private const string AbilityDataClassFolder = "Assets/AbilitySystem/Runtime/Data";
        private const string Indent = "    ";
        #endregion

        #region Executes
        /// <summary>
        /// Generates the AbilityData C# class file for a given ability.
        /// </summary>
        /// <param name="abilityName">The base ability name (e.g. "Fireball").</param>
        public static void CreateAbilityData(string abilityName)
        {
            Debug.Log($"[AbilityDataGenerator] Requested data class generate → {abilityName}");

            if (!Directory.Exists(AbilityDataFolder))
            {
                Directory.CreateDirectory(AbilityDataFolder);
                
                Debug.Log($"[AbilityDataGenerator] Created folder → {AbilityDataFolder}");
            }

            if (!Directory.Exists(AbilityDataClassFolder))
            {
                Directory.CreateDirectory(AbilityDataClassFolder);
                Debug.Log($"[AbilityDataGenerator] Created folder → {AbilityDataClassFolder}");
                
            }

            string className = $"{abilityName}Data";
            string classFilePath = $"{AbilityDataClassFolder}/{className}.cs";
            string assetPath = $"{AbilityDataFolder}/{className}.asset";

            if (File.Exists(classFilePath))
            {
                Debug.LogWarning($"[AbilityDataGenerator] SKIPPED. Class already exists → {classFilePath}");
                
                return;
            }

            if (File.Exists(assetPath))
                Debug.LogWarning(
                    $"[AbilityDataGenerator] Warning: Asset already exists (SO will only be created manually) → {assetPath}");

            Debug.Log($"[AbilityDataGenerator] Creating data class file → {classFilePath}");

            string content =
$@"using UnityEngine;

namespace AbilitySystem.Runtime.Data
{{
{Indent}[CreateAssetMenu(fileName = ""{className}"", menuName = ""AbilitySystem/AbilityData/{className}"")]
{Indent}public sealed class {className} : AbilityData
{Indent}{{
{Indent}}}
}}";
            
            File.WriteAllText(classFilePath, content);
            
            AssetDatabase.Refresh();

            Debug.Log($"[AbilityDataGenerator] ✔ Created data class → {className}");
        }
        
        /// <summary>
        /// Creates a ScriptableObject instance of the generated AbilityData class.
        /// </summary>
        /// <param name="abilityName">The ability name corresponding to the AbilityData class.</param>
        public static void CreateSo(string abilityName)
        {
            Debug.Log($"[AbilityDataGenerator] Requested SO create → {abilityName}");

            string className = $"{abilityName}Data";
            string targetPath = $"{AbilityDataFolder}/{className}.asset";

            if (File.Exists(targetPath))
            {
                Debug.LogWarning($"[AbilityDataGenerator] SKIPPED: SO already exists → {targetPath}");
                
                return;
            }

            Debug.Log($"[AbilityDataGenerator] Searching script for → {className}");

            string[] guids = AssetDatabase.FindAssets($"{className} t:MonoScript");

            if (guids.Length == 0)
            {
                Debug.LogError(
                    $"[AbilityDataGenerator] ERROR: No MonoScript found for {className}. Is the class name correct?");
                
                return;
            }

            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            
            MonoScript mono = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

            if (!mono)
            {
                Debug.LogError("[AbilityDataGenerator] ERROR: MonoScript failed to load!");
                
                return;
            }

            Type type = mono.GetClass();

            if (type == null)
            {
                Debug.LogError(
                    $"[AbilityDataGenerator] ERROR: Type could not be loaded for class {className}. Is it compiling?");
                
                return;
            }

            Debug.Log($"[AbilityDataGenerator] Creating instance of {type.FullName}");

            ScriptableObject instance = ScriptableObject.CreateInstance(type);

            if (instance is AbilityData data)
            {
                data.AbilityName = abilityName;
                
                EditorUtility.SetDirty(data);

                Debug.Log($"[AbilityDataGenerator] ✔ Set AbilityName = {abilityName}");
            }

            AssetDatabase.CreateAsset(instance, targetPath);
            
            AssetDatabase.SaveAssets();
            
            AssetDatabase.Refresh();

            Selection.activeObject = instance;
            
            EditorGUIUtility.PingObject(instance);

            Debug.Log($"[AbilityDataGenerator] ✔ Created SO → {targetPath}");
        }
        #endregion        
    }
}
