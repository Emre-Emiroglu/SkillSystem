using System.IO;
using UnityEditor;

namespace SkillSystem.Editor.Generators
{
    /// <summary>
    /// Generates runtime skill logic classes based on a given skill name.
    /// </summary>
    public static class SkillClassGenerator
    {
        #region Constants
        private const string SkillClassFolder = "Assets/SkillSystem/Runtime/Skills";
        private const string Indent = "    ";
        #endregion
        
        #region Executes
        /// <summary>
        /// Creates a concrete skill class file inheriting from BaseSkill for the given skill name.
        /// </summary>
        /// <param name="skillName">Name of the skill to generate the class for.</param>
        public static void CreateSkillClass(string skillName)
        {
            if (!Directory.Exists(SkillClassFolder))
                Directory.CreateDirectory(SkillClassFolder);

            string className = $"{skillName}Skill";
            string filePath = $"{SkillClassFolder}/{className}.cs";
            string dataName = $"{skillName}Data";

            if (File.Exists(filePath))
                return;

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
        }
        #endregion
    }
}