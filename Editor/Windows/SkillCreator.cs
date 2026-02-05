using SkillSystem.Editor.Generators;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Editor.Windows
{
    public sealed class SkillCreator : EditorWindow
    {
        #region Fields
        private string _skillName;
        #endregion
        
        #region Core
        [MenuItem("Tools/Skill Creator")]
        private static void ShowWindow() => DrawEditor();
        private void OnGUI()
        {
            DrawSkillNameField();

            GUILayout.Space(8);

            if (GenerateSkillClasses())
                return;

            GUILayout.Space(4);

            CreateSkillScriptableObject();
        }
        #endregion
        
        #region Executes
        private static void DrawEditor()
        {
            SkillCreator window = GetWindow<SkillCreator>();

            window.titleContent = new GUIContent("Skill Creator");

            window.minSize = new Vector2(256, 128);
            window.maxSize = new Vector2(512, 256);

            window.Show();
        }
        private void DrawSkillNameField()
        {
            GUILayout.Label("Create New Skill", EditorStyles.boldLabel);

            _skillName = EditorGUILayout.TextField("Skill Name", _skillName);
        }
        private bool GenerateSkillClasses()
        {
            if (!DrawGenerateSkillClassesButton())
                return false;

            if (CheckSkillNameIsEmpty())
                return true;

            string sanitizedSkillName = GetSanitizedSkillName();

            Debug.Log($"[SkillCreator] Generating classes for → {sanitizedSkillName}");

            SkillDataGenerator.CreateSkillData(sanitizedSkillName);
            
            SkillClassGenerator.CreateSkillClass(sanitizedSkillName);

            Debug.Log($"[SkillCreator] ✔ Generated Skill + Data classes → {sanitizedSkillName}");

            return false;
        }
        private void CreateSkillScriptableObject()
        {
            if (!DrawCreateScriptableObjectButton())
                return;

            if (CheckSkillNameIsEmpty())
                return;

            string sanitizedSkillName = GetSanitizedSkillName();

            Debug.Log($"[SkillCreator] Creating Skill SO → {sanitizedSkillName}");

            SkillDataGenerator.CreateSo(sanitizedSkillName);

            Debug.Log($"[SkillCreator] ✔ Skill SO created → {sanitizedSkillName}");
        }
        private static bool DrawGenerateSkillClassesButton() => GUILayout.Button("Generate Skill Classes");
        private static bool DrawCreateScriptableObjectButton() => GUILayout.Button("Create Skill ScriptableObject");
        private bool CheckSkillNameIsEmpty()
        {
            if (!string.IsNullOrWhiteSpace(_skillName))
                return false;

            Debug.LogError("[SkillCreator] ERROR: Skill name cannot be empty!");

            return true;
        }
        private string GetSanitizedSkillName() => _skillName.Trim().Replace(" ", string.Empty);
        #endregion
    }
}