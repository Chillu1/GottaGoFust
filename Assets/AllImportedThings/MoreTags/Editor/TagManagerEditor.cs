using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreTags
{
    [CustomEditor(typeof(TagManager))]
    public class TagManagerEditor : Editor
    {
        private TagGUI m_AllTag;

        private static bool s_Abbreviation = true;
        private static bool s_PresetFoldout = false;
        private static bool s_ManagerFoldout = true;
        private static bool s_AllTagFoldout = false;

        private static string s_TagRename = string.Empty;
        private static int s_TagRenameIndex;
        private static string s_ChangeTag = string.Empty;
        private static int s_ChangeTagIndex;

        private static bool s_WithChildren;
        private static string s_PatternString = "*";
        private static bool s_RemoveAll;

        private IEnumerable<GameObject> m_GameObjects = Enumerable.Empty<GameObject>();

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            s_PresetFoldout = EditorGUILayout.Foldout(s_PresetFoldout, "Preset", true);
            if (s_PresetFoldout)
                TagPreset.OnGUI();

            s_ManagerFoldout = EditorGUILayout.Foldout(s_ManagerFoldout, "Tag Manager", true);
            if (s_ManagerFoldout)
            {
                EditorGUI.BeginChangeCheck();
                s_Abbreviation = EditorGUILayout.Toggle("Abbreviation", s_Abbreviation);
                if (EditorGUI.EndChangeCheck())
                    EditorPrefs.SetBool(TagPreset.kPrefsPrefix + "Abbreviation", s_Abbreviation);

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("box");
                s_AllTagFoldout = EditorGUILayout.Foldout(s_AllTagFoldout, "All Tags", true);
                EditorGUI.indentLevel--;
                if (s_AllTagFoldout)
                {
                    var alltag = TagSystem.GetAllTags().OrderBy(tag => TagPreset.GetTagOrder(tag)).ToArray();
                    m_AllTag.OnGUI(alltag);
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    s_TagRename = EditorGUILayout.TextField(" ", s_TagRename);
                    var rect = GUILayoutUtility.GetLastRect();
                    rect.width = EditorGUIUtility.labelWidth - 4;
                    s_TagRenameIndex = Mathf.Clamp(s_TagRenameIndex, 0, alltag.Length - 1);
                    s_TagRenameIndex = EditorGUI.Popup(rect, s_TagRenameIndex, alltag);
                    if (GUILayout.Button("Rename", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                        TagSystem.RenameTag(alltag[s_TagRenameIndex], s_TagRename);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("Right click the tag to change color.", MessageType.Info);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                var pattern = TagSystem.pattern;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("All", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    TagSystem.SearchFrom();
                    if (string.IsNullOrEmpty(s_PatternString))
                        pattern = TagSystem.pattern.All();
                    else
                        pattern = TagHelper.StringToPattern(s_PatternString);
                    m_GameObjects = pattern.GameObjects().Where(go => go != null);
                }
                if (GUILayout.Button("Selected", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    if (!s_WithChildren)
                        TagSystem.SearchFrom(Selection.gameObjects);
                    else
                        TagSystem.SearchFrom(Selection.gameObjects.SelectMany(go => go.GetChildrenList()));
                    if (string.IsNullOrEmpty(s_PatternString))
                        pattern = TagSystem.pattern.All();
                    else
                        pattern = TagHelper.StringToPattern(s_PatternString);
                    m_GameObjects = pattern.GameObjects().Where(go => go != null);
                }
                s_WithChildren = GUILayout.Toggle(s_WithChildren, "With Children", GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Tag Pattern");
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                    s_PatternString = string.Empty;
                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("*"), false, () => s_PatternString += "*");
                    foreach (var tag in TagPreset.GetPresets().Union(TagSystem.GetAllTags()))
                        menu.AddItem(new GUIContent(tag), false, () => s_PatternString += tag);
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
                s_PatternString = EditorGUILayout.TextArea(s_PatternString);

                ShowGameObjectsTag(m_GameObjects);
            }
        }

        public static void ShowGameObjectsTag(IEnumerable<GameObject> list)
        {
            var taggui = new TagGUI();
            taggui.OnItemString += OnItemString;
            taggui.OnItemColor += OnItemColor;
            taggui.OnRightClickItem += OnRightClickItem;

            taggui.OnAddItem = (item) =>
            {
                if (!string.IsNullOrEmpty(item))
                    AddTagToGameObjects(item, list);
                else
                {
                    var menu = new GenericMenu();
                    foreach (var tag in TagPreset.GetPresets().Union(TagSystem.GetAllTags()))
                        menu.AddItem(new GUIContent(tag), false, () => AddTagToGameObjects(tag, list));
                    menu.ShowAsContext();
                }
            };
            taggui.OnClickItem = null;
            taggui.OnGUI(Enumerable.Empty<string>(), "Add Tag");
            s_RemoveAll = EditorGUILayout.Toggle("Remove All", s_RemoveAll);
            if (GUILayout.Button("Select Listed Game Object", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                if (list.Any())
                    Selection.objects = list.ToArray();
            EditorGUILayout.BeginHorizontal();
            {
                var alltag = TagSystem.GetAllTags().OrderBy(tag => TagPreset.GetTagOrder(tag)).ToArray();
                s_ChangeTag = EditorGUILayout.TextField(" ", s_ChangeTag);
                var rect = GUILayoutUtility.GetLastRect();
                rect.width = EditorGUIUtility.labelWidth - 4;
                s_ChangeTagIndex = Mathf.Clamp(s_ChangeTagIndex, 0, alltag.Length - 1);
                s_ChangeTagIndex = EditorGUI.Popup(rect, s_ChangeTagIndex, alltag);
                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    var menu = new GenericMenu();
                    foreach (var tag in TagPreset.GetPresets().Union(TagSystem.GetAllTags()))
                        menu.AddItem(new GUIContent(tag), false, () => s_ChangeTag = tag);
                    menu.ShowAsContext();
                }
                if (GUILayout.Button("Change", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                    ChangeGameObjectsTag(alltag[s_ChangeTagIndex], s_ChangeTag, list);
            }
            EditorGUILayout.EndHorizontal();

            if (list.Any())
            {
                EditorGUILayout.BeginVertical("box");
                taggui.OnAddItem = null;
                foreach (var go in list)
                {
                    var remove = s_RemoveAll ? list : new[] { go };
                    taggui.OnClickItem = (item) => RemoveTagInGameObjects(item, remove);
                    taggui.OnGUI(go.GetTags().OrderBy(tag => TagPreset.GetTagOrder(tag)), go.name);
                }
                EditorGUILayout.EndVertical();
            }
        }

        void OnEnable()
        {
            s_Abbreviation = EditorPrefs.GetBool(TagPreset.kPrefsPrefix + "Abbreviation", true);

            m_AllTag = new TagGUI();
            m_AllTag.OnItemString += OnItemString;
            m_AllTag.OnItemColor += OnItemColor;
            m_AllTag.OnRightClickItem += OnRightClickItem;
            m_AllTag.OnAddItem = (item) =>
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!TagSystem.GetAllTags().Contains(item))
                        AddTagToManager(item);
                }
                else
                {
                    var menu = new GenericMenu();
                    foreach (var tag in TagPreset.GetPresets().Except(TagSystem.GetAllTags()))
                        menu.AddItem(new GUIContent(tag), false, () => AddTagToManager(tag));
                    menu.ShowAsContext();
                }
                for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneAt(i));
            };
            m_AllTag.OnClickItem += (item) =>
            {
                TagSystem.RemoveTag(item);
                for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneAt(i));
            };
        }

        public static GUIContent OnItemString(string item)
        {
            if (!s_Abbreviation) return new GUIContent(item);
            var list = item.Split('.');
            var str = string.Join(".", list.Take(list.Length - 1).Select(s => s.First().ToString()).Concat(new[] { list.Last() }).ToArray());
            return new GUIContent(str, item);
        }

        public static Color OnItemColor(string item)
        {
            return TagSystem.GetTagColor(item);
        }

        public static void OnRightClickItem(Rect rect, string item)
        {
            var popup = new ColorPopup(item);
            PopupWindow.Show(rect, popup);
        }

        public static void AddTagToManager(string tag)
        {
            TagSystem.AddTag(tag);
            TagSystem.SetTagColor(tag, TagPreset.GetPresetColor(tag));
            for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneAt(i));
        }

        public static void AddTagToGameObjects(string tag, IEnumerable<GameObject> list)
        {
            if (!list.Any()) return;
            if (!TagSystem.AllTags().Contains(tag))
            {
                TagSystem.AddTag(tag);
                TagSystem.SetTagColor(tag, TagPreset.GetPresetColor(tag));
            }
            foreach (var go in list)
                go.AddTag(tag);
            EditorSceneManager.MarkSceneDirty(list.First().scene);
        }

        public static void RemoveTagInGameObjects(string tag, IEnumerable<GameObject> list)
        {
            if (!list.Any()) return;
            foreach (var go in list)
                go.RemoveTag(tag);
            EditorSceneManager.MarkSceneDirty(list.First().scene);
        }

        public static void ChangeGameObjectsTag(string old, string tag, IEnumerable<GameObject> list)
        {
            if (!list.Any()) return;
            foreach (var go in list)
                go.ChangeTag(old, tag);
            EditorSceneManager.MarkSceneDirty(list.First().scene);
        }

        private IEnumerable<string> GetInheritanceHierarchy(Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current.ToString();
        }

        private bool GameObjectWithComponent(GameObject go, string component)
        {
            if (string.IsNullOrEmpty(component)) return true;
            var c = go.GetComponents<Component>().Where(comp => comp != null);
            var s = c.SelectMany(comp => GetInheritanceHierarchy(comp.GetType())).Where(type => type.EndsWith(component));
            return s.Any();
        }
    }
}