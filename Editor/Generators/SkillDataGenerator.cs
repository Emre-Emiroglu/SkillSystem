using System;
using System.IO;
using SkillSystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Editor.Generators
{
    public static class SkillDataGenerator
    {
        #region Constants
        private const string SkillDataFolder = "Assets/Resources/SkillSystem";
        private const string SkillDataClassFolder = "Assets/SkillSystem/Runtime/Data";
        private const string Indent = "    ";
        #endregion

        #region Executes
        public static void CreateSkillData(string skillName)
        {
            Debug.Log($"[SkillDataGenerator] Requested data class generate → {skillName}");

            if (!Directory.Exists(SkillDataFolder))
            {
                Directory.CreateDirectory(SkillDataFolder);
                
                Debug.Log($"[SkillDataGenerator] Created folder → {SkillDataFolder}");
            }

            if (!Directory.Exists(SkillDataClassFolder))
            {
                Directory.CreateDirectory(SkillDataClassFolder);
                
                Debug.Log($"[SkillDataGenerator] Created folder → {SkillDataClassFolder}");
            }

            string className = $"{skillName}Data";
            string classFilePath = $"{SkillDataClassFolder}/{className}.cs";
            string assetPath = $"{SkillDataFolder}/{className}.asset";

            if (File.Exists(classFilePath))
            {
                Debug.LogWarning($"[SkillDataGenerator] SKIPPED. Class already exists → {classFilePath}");
                
                return;
            }

            if (File.Exists(assetPath))
                Debug.LogWarning(
                    $"[SkillDataGenerator] Warning: Asset already exists (SO will only be created manually) → {assetPath}");

            Debug.Log($"[SkillDataGenerator] Creating data class file → {classFilePath}");

            string content =
$@"using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem.Runtime.Data
{{
{Indent}[CreateAssetMenu(fileName = ""{className}"",menuName = ""SkillSystem/SkillData/{className}"")]
{Indent}public sealed class {className} : SkillData
{Indent}{{
{Indent}}}
}}";

            File.WriteAllText(classFilePath, content);
            
            AssetDatabase.Refresh();

            Debug.Log($"[SkillDataGenerator] ✔ Created data class → {className}");
        }
        public static void CreateSo(string skillName)
        {
            Debug.Log($"[SkillDataGenerator] Requested SO create → {skillName}");

            string className = $"{skillName}Data";
            string targetPath = $"{SkillDataFolder}/{className}.asset";

            if (File.Exists(targetPath))
            {
                Debug.LogWarning($"[SkillDataGenerator] SKIPPED. SO already exists → {targetPath}");
                
                return;
            }
            
            Debug.Log($"[SkillDataGenerator] Searching script for → {className}");

            string[] guids = AssetDatabase.FindAssets($"{className} t:MonoScript");

            if (guids.Length == 0)
            {
                Debug.LogError(
                    $"[SkillDataGenerator] ERROR: No MonoScript found for {className}. Is the class name correct?");
                
                return;
            }

            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            
            MonoScript mono = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            
            if (!mono)
            {
                Debug.LogError("[SkillDataGenerator] ERROR: MonoScript failed to load!");
                
                return;
            }

            Type type = mono.GetClass();

            if (type == null)
            {
                Debug.LogError($"[SkillDataGenerator] ERROR: Type not compiled yet → {className}. Is it compiling?");
                
                return;
            }
            
            Debug.Log($"[SkillDataGenerator] Creating instance of {type.FullName}");

            ScriptableObject instance = ScriptableObject.CreateInstance(type);

            if (instance is SkillData data)
            {
                data.SkillName = skillName;
                
                EditorUtility.SetDirty(data);
                
                Debug.Log($"[SkillDataGenerator] ✔ Set AbilityName = {skillName}");
            }

            AssetDatabase.CreateAsset(instance, targetPath);
            
            AssetDatabase.SaveAssets();
            
            AssetDatabase.Refresh();

            Selection.activeObject = instance;
            
            EditorGUIUtility.PingObject(instance);

            Debug.Log($"[SkillDataGenerator] ✔ Created Skill SO → {targetPath}");
        }
        #endregion
    }
}