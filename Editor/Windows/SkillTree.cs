using System.Collections.Generic;
using System.Linq;
using SkillSystem.Runtime.Data;
using SkillSystem.Runtime.Enums;
using SkillSystem.Runtime.Managers;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Editor.Windows
{
    public sealed class SkillTree : EditorWindow
    {
        #region Constants
        private const float NodeWidth = 192f;
        private const float NodeHeight = 64f;
        private const float GridSpacing = 32f;
        #endregion

        #region ReadonlyFields
        private readonly Dictionary<SkillData, Rect> _nodeRects = new();
        #endregion

        #region Fields
        private Vector2 _panOffset;
        private Vector2 _dragOffset;
        private SkillData _draggedSkill;
        private SkillData _linkFrom;
        #endregion

        #region Core
        [MenuItem("Tools/Skill Tree")]
        public static void Open() => DrawEditor();
        private void OnEnable()
        {
            SkillManager.InitializeManager();
            
            RefreshNodes();
        }
        private void OnGUI()
        {
            DrawGrid();
            
            DrawConnections();
            
            DrawNodes();
            
            HandleEvents(Event.current);

            if (_linkFrom)
                DrawLinkPreview(Event.current.mousePosition);

            if (GUI.changed)
                Repaint();
        }
        #endregion

        #region Executes
        private static void DrawEditor()
        {
            SkillTree window = GetWindow<SkillTree>();
            
            window.titleContent = new GUIContent("Skill Tree");
            
            window.minSize = new Vector2(512, 256);
            window.maxSize = new Vector2(1024, 512);
            
            window.Show();
        }
        private void DrawGrid()
        {
            Handles.color = new Color(0.25f, 0.25f, 0.25f);

            for (float x = 0; x < position.width; x += GridSpacing)
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, position.height));

            for (float y = 0; y < position.height; y += GridSpacing)
                Handles.DrawLine(new Vector3(0, y), new Vector3(position.width, y));

            Handles.color = Color.white;
        }
        private void DrawConnections()
        {
            foreach ((SkillData skill, Rect rect) in _nodeRects)
            {
                foreach (SkillData prereq in skill.Prerequisites)
                {
                    if (!prereq || !_nodeRects.TryGetValue(prereq, out Rect from))
                        continue;

                    Handles.DrawBezier(from.center + _panOffset, rect.center + _panOffset,
                        from.center + Vector2.right * 64f, rect.center + Vector2.left * 64f, Color.white, null, 2f);
                }
            }
        }
        private void DrawNodes()
        {
            foreach ((SkillData skill, Rect rect) in _nodeRects.ToList())
            {
                Rect moved = rect;
                
                moved.position += _panOffset;

                GUI.color = GetNodeColor(skill);
                
                GUI.Box(moved, GUIContent.none, EditorStyles.helpBox);
                
                GUI.color = Color.white;

                GUI.Label(new Rect(moved.x + 8, moved.y + 4, moved.width - 32, 16), skill.SkillName,
                    EditorStyles.boldLabel);

                GUI.Label(new Rect(moved.xMax - 24, moved.y + 4, 16, 16), GetStateIcon(skill));

                Rect btnRect = new(moved.x + 4, moved.yMax - 24, moved.width - 12, 16);
                
                DrawStateButtons(skill, btnRect);
            }
        }
        private void HandleEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    OnMouseDown(e);
                    break;

                case EventType.MouseDrag:
                    OnMouseDrag(e);
                    break;

                case EventType.MouseUp:
                    OnMouseUp(e);
                    break;

                case EventType.ContextClick:
                    ShowContextMenu();
                    break;
            }
        }
        private void DrawLinkPreview(Vector2 mousePos) => Handles.DrawBezier(_nodeRects[_linkFrom].center + _panOffset,
            mousePos, _nodeRects[_linkFrom].center + Vector2.right * 64f, mousePos + Vector2.left * 64f, Color.cyan,
            null, 3f);
        private static Color GetNodeColor(SkillData skill) =>
            SkillManager.TryGetSkillState(skill.SkillName, out SkillState state)
                ? state switch
                {
                    SkillState.Unlocked => new Color(0.3f, 0.8f, 0.3f),
                    SkillState.Unlockable => new Color(0.9f, 0.8f, 0.2f),
                    _ => new Color(0.8f, 0.3f, 0.3f)
                }
                : Color.gray;
        private static GUIContent GetStateIcon(SkillData skill) =>
            SkillManager.TryGetSkillState(skill.SkillName, out SkillState state)
                ? state switch
                {
                    SkillState.Unlocked => EditorGUIUtility.IconContent("TestPassed"),
                    SkillState.Unlockable => EditorGUIUtility.IconContent("TestNormal"),
                    _ => EditorGUIUtility.IconContent("TestFailed")
                }
                : GUIContent.none;
        private static void DrawStateButtons(SkillData skill, Rect rect)
        {
            GUILayout.BeginArea(rect);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Unlock"))
                SkillManager.ChangeSkillState(skill.SkillName, SkillState.Unlocked);

            if (GUILayout.Button("Lock"))
                SkillManager.ChangeSkillState(skill.SkillName, SkillState.Locked);

            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }
        private void OnMouseDown(Event e)
        {
            foreach ((SkillData skill, Rect rect) in _nodeRects)
            {
                Rect moved = rect;
                
                moved.position += _panOffset;

                if (!moved.Contains(e.mousePosition))
                    continue;

                switch (e.button)
                {
                    case 0:
                        _draggedSkill = skill;
                        
                        _dragOffset = e.mousePosition - moved.position;
                        
                        e.Use();
                        break;
                    case 1:
                        _linkFrom = skill;
                        
                        e.Use();
                        break;
                }

                return;
            }
        }
        private void OnMouseDrag(Event e)
        {
            if (!_draggedSkill)
                return;
            
            Rect r = _nodeRects[_draggedSkill];
            
            r.position = e.mousePosition - _dragOffset - _panOffset;
            
            _nodeRects[_draggedSkill] = r;

            _draggedSkill.EditorPosition = r.position;
            
            EditorUtility.SetDirty(_draggedSkill);

            GUI.changed = true;
            
            e.Use();
        }
        private void OnMouseUp(Event e)
        {
            if (_linkFrom)
            {
                foreach ((SkillData target, Rect rect) in _nodeRects)
                {
                    Rect moved = rect;
                    
                    moved.position += _panOffset;

                    if (!moved.Contains(e.mousePosition) || target == _linkFrom)
                        continue;
                    
                    TryAddPrerequisite(target, _linkFrom);
                    
                    break;
                }
            }

            _draggedSkill = null;
            
            _linkFrom = null;
            
            e.Use();
        }
        private void ShowContextMenu()
        {
            GenericMenu menu = new();
            
            menu.AddItem(new GUIContent("Refresh"), false, RefreshNodes);
            
            menu.AddItem(new GUIContent("Auto Layout"), false, AutoLayout);
            
            menu.ShowAsContext();
        }
        private static void TryAddPrerequisite(SkillData skill, SkillData prereq)
        {
            if (skill == prereq)
                return;
            
            if (skill.Prerequisites.Contains(prereq))
                return;
            
            if (HasCircular(skill, prereq, new HashSet<SkillData>()))
                return;

            skill.Prerequisites.Add(prereq);
            
            EditorUtility.SetDirty(skill);
        }
        private static bool HasCircular(SkillData start, SkillData current, HashSet<SkillData> visited) =>
            !visited.Add(current) || current.Prerequisites.Any(p => p == start || HasCircular(start, p, visited));
        private void RefreshNodes()
        {
            _nodeRects.Clear();

            foreach (SkillData skill in Resources.LoadAll<SkillData>("SkillSystem"))
            {
                Vector2 pos = skill.EditorPosition == Vector2.zero ? new Vector2(128, 128) : skill.EditorPosition;

                _nodeRects[skill] = new Rect(pos, new Vector2(NodeWidth, NodeHeight));
            }
        }
        private void AutoLayout()
        {
            float y = 96f;

            foreach (SkillData root in _nodeRects.Keys.Where(s => s.Prerequisites.Count == 0))
                LayoutRecursive(root, 96f, ref y);
        }
        private void LayoutRecursive(SkillData skill, float x, ref float y)
        {
            _nodeRects[skill] = new Rect(new Vector2(x, y), new Vector2(NodeWidth, NodeHeight));
            
            skill.EditorPosition = new Vector2(x, y);
            
            EditorUtility.SetDirty(skill);

            y += NodeHeight + 32f;

            foreach (SkillData child in _nodeRects.Keys.Where(s => s.Prerequisites.Contains(skill)))
                LayoutRecursive(child, x + 256f, ref y);
        }
        #endregion
    }
}
