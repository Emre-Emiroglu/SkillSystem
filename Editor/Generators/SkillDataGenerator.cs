using System;
using System.IO;
using SkillSystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Editor.Generators
{
    /// <summary>
    /// Generates SkillData script classes and ScriptableObject assets.
    /// </summary>
    public static class SkillDataGenerator
    {
        #region Constants
        private const string SkillDataFolder = "Assets/Resources/SkillSystem";
        private const string SkillDataClassFolder = "Assets/SkillSystem/Runtime/Data";
        private const string Indent = "    ";
        #endregion

        #region Executes
        /// <summary>
        /// Creates a SkillData-derived C# class for the given skill name.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        public static void CreateSkillData(string skillName)
        {
            if (!Directory.Exists(SkillDataFolder))
                Directory.CreateDirectory(SkillDataFolder);

            if (!Directory.Exists(SkillDataClassFolder))
                Directory.CreateDirectory(SkillDataClassFolder);

            string className = $"{skillName}Data";
            string classFilePath = $"{SkillDataClassFolder}/{className}.cs";

            if (File.Exists(classFilePath))
                return;

            string content =
$@"using UnityEngine;

namespace SkillSystem.Runtime.Data
{{
{Indent}[CreateAssetMenu(fileName = ""{className}"",menuName = ""SkillSystem/SkillData/{className}"")]
{Indent}public sealed class {className} : SkillData
{Indent}{{
{Indent}}}
}}";

            File.WriteAllText(classFilePath, content);
            
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// Creates a ScriptableObject instance of the generated SkillData class.
        /// </summary>
        /// <param name="skillName">Name of the skill.</param>
        public static void CreateSo(string skillName)
        {
            string className = $"{skillName}Data";
            string targetPath = $"{SkillDataFolder}/{className}.asset";

            if (File.Exists(targetPath))
                return;

            string[] guids = AssetDatabase.FindAssets($"{className} t:MonoScript");

            if (guids.Length == 0)
                return;

            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            
            MonoScript mono = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            
            if (!mono)
                return;

            Type type = mono.GetClass();

            if (type == null)
                return;
            
            ScriptableObject instance = ScriptableObject.CreateInstance(type);

            if (instance is SkillData data)
            {
                data.SkillName = skillName;
                
                EditorUtility.SetDirty(data);
            }

            AssetDatabase.CreateAsset(instance, targetPath);
            
            AssetDatabase.SaveAssets();
            
            AssetDatabase.Refresh();

            Selection.activeObject = instance;
            
            EditorGUIUtility.PingObject(instance);
        }
        #endregion
    }
}