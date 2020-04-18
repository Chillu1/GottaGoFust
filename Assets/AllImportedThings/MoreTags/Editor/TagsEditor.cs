using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MoreTags
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Tags))]
    public class TagsEditor : Editor
    {
        private TagGUI m_GameObjectTag;

        public override void OnInspectorGUI()
        {
            var go = (target as Tags).gameObject;
            if (!go.scene.isLoaded)
            {
                EditorGUILayout.HelpBox("Tags should be attached to on scene GameObject. Please use Prefab Tags for off scene GameObject.", MessageType.Warning);
                return;
            }
            TagSystem.CheckTagManager(go.scene);

            var list = targets.Cast<Tags>().Select(item => item.gameObject);
            if (!list.Skip(1).Any())
                m_GameObjectTag.OnGUI(go.GetTags().OrderBy(tag => TagPreset.GetTagOrder(tag)));
            else
                TagManagerEditor.ShowGameObjectsTag(list);
        }

        void OnEnable()
        {
            var go = (target as Tags).gameObject;

            m_GameObjectTag = new TagGUI();
            m_GameObjectTag.OnItemString += TagManagerEditor.OnItemString;
            m_GameObjectTag.OnItemColor += TagManagerEditor.OnItemColor;
            m_GameObjectTag.OnRightClickItem += TagManagerEditor.OnRightClickItem;
            m_GameObjectTag.OnAddItem += (item) =>
            {
                if (!string.IsNullOrEmpty(item))
                    AddTag(go, item);
                else
                {
                    var menu = new GenericMenu();
                    foreach (var tag in TagPreset.GetPresets().Union(TagSystem.GetAllTags()).Except(go.GetTags()))
                        menu.AddItem(new GUIContent(tag), false, () => AddTag(go, tag));
                    menu.ShowAsContext();
                }
            };
            m_GameObjectTag.OnClickItem += (item) => { RemoveTag(go, item); };
        }

        private void RemoveTag(GameObject go, string tag)
        {
            go.RemoveTag(tag);
            EditorSceneManager.MarkSceneDirty(go.scene);
        }

        private void AddTag(GameObject go, string tag)
        {
            if (!TagSystem.AllTags().Contains(tag))
            {
                TagSystem.AddTag(tag);
                TagSystem.SetTagColor(tag, TagPreset.GetPresetColor(tag));
            }
            go.AddTag(tag);
            EditorSceneManager.MarkSceneDirty(go.scene);
        }
    }
}