using System.IO;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Editor.Generators
{
    public static class SkillClassGenerator
    {
        #region Constants
        private const string SkillClassFolder = "Assets/SkillSystem/Runtime/Skills";
        private const string Indent = "    ";
        #endregion
        
        #region Executes
        public static void CreateSkillClass(string skillName)
        {
            Debug.Log($"[SkillClassGenerator] Requested skill class generate → {skillName}");

            if (!Directory.Exists(SkillClassFolder))
            {
                Directory.CreateDirectory(SkillClassFolder);
                
                Debug.Log($"[SkillClassGenerator] Created folder → {SkillClassFolder}");
            }

            string className = $"{skillName}Skill";
            string filePath = $"{SkillClassFolder}/{className}.cs";
            string dataName = $"{skillName}Data";

            if (File.Exists(filePath))
            {
                Debug.LogWarning($"[SkillClassGenerator] SKIPPED. Class already exists → {filePath}");
                
                return;
            }

            string content =
$@"using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;

namespace SkillSystem.Runtime.Skills
{{
{Indent}public sealed class {className} : BaseSkill<{dataName}>
{Indent}{{
{Indent}{Indent}public override void Initialize({dataName} skillData)
{Indent}{Indent}{{
{Indent}{Indent}{Indent}base.Initialize(skillData);

{Indent}{Indent}{Indent}// TODO: {className} initialize logic here
{Indent}{Indent}}}

{Indent}{Indent}public override void ChangeState(SkillState newSkillState)
{Indent}{Indent}{{
{Indent}{Indent}{Indent}base.ChangeState(newSkillState);

{Indent}{Indent}{Indent}// TODO: {className} change state logic here
{Indent}{Indent}}}
{Indent}}}
}}";

            File.WriteAllText(filePath, content);
            
            AssetDatabase.Refresh();

            Debug.Log($"[SkillClassGenerator] ✔ Created skill class → {className}");
        }
        #endregion
    }
}