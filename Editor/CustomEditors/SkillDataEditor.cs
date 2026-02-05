using System.Collections.Generic;
using System.Linq;
using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Managers;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(SkillData), true)]
    public sealed class SkillDataEditor : UnityEditor.Editor
    {
        #region Fields
        private SkillData _skillData;
        private SerializedProperty _skillNameProp;
        private SerializedProperty _prerequisitesProp;
        #endregion

        #region Core
        private void OnEnable()
        {
            _skillData = (SkillData)target;
            
            _skillNameProp = serializedObject.FindProperty("skillName");
            
            _prerequisitesProp = serializedObject.FindProperty("prerequisites");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeaderField();
            
            DrawSkillName();
            
            DrawRuntimeState();
            
            DrawPrerequisites();
            
            DrawValidation();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Executes
        private static void DrawHeaderField()
        {
            GUILayout.Space(4);
            
            EditorGUILayout.LabelField("Skill Data", EditorStyles.boldLabel);
        }
        private void DrawSkillName()
        {
            EditorGUI.BeginDisabledGroup(true);
            
            EditorGUILayout.PropertyField(_skillNameProp);
            
            EditorGUI.EndDisabledGroup();
        }
        private void DrawRuntimeState()
        {
            SkillState state = GetRuntimeState();

            Color color = state switch
            {
                SkillState.Unlocked => Color.green,
                SkillState.Unlockable => Color.yellow,
                _ => Color.red
            };

            GUI.color = color;
            
            EditorGUILayout.LabelField("Runtime State", state.ToString(), EditorStyles.helpBox);
            
            GUI.color = Color.white;
        }
        private void DrawPrerequisites()
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Prerequisites", EditorStyles.boldLabel);

            for (int i = 0; i < _prerequisitesProp.arraySize; i++)
            {
                SerializedProperty element = _prerequisitesProp.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(element, GUIContent.none);

                if (GUILayout.Button("X", GUILayout.Width(32)))
                {
                    _prerequisitesProp.DeleteArrayElementAtIndex(i);
                    
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Prerequisite"))
                _prerequisitesProp.InsertArrayElementAtIndex(_prerequisitesProp.arraySize);
        }
        private void DrawValidation()
        {
            if (HasSelfReference())
                EditorGUILayout.HelpBox("Skill cannot reference itself as a prerequisite.", MessageType.Error);
            else if (HasCircularDependency())
                EditorGUILayout.HelpBox("Circular dependency detected!", MessageType.Error);
            else
                EditorGUILayout.HelpBox("Prerequisites configuration is valid.", MessageType.Info);
        }
        private bool HasSelfReference() => _skillData.Prerequisites.Any(p => p == _skillData);
        private bool HasCircularDependency()
        {
            HashSet<SkillData> visited = new();
            
            return CheckCircular(_skillData, visited);
        }
        private static bool CheckCircular(SkillData current, HashSet<SkillData> visited)
        {
            if (!current)
                return false;

            if (!visited.Add(current))
                return true;

            if (current.Prerequisites != null)
                if (current.Prerequisites.Where(prereq => prereq).Any(prereq => CheckCircular(prereq, visited)))
                    return true;

            visited.Remove(current);
            
            return false;
        }
        private SkillState GetRuntimeState() =>
            SkillManager.TryGetSkillState(_skillData.SkillName, out SkillState state) ? state : SkillState.Locked;
        #endregion
    }
}