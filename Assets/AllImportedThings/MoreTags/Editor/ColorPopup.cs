using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreTags
{
    public class ColorPopup : PopupWindowContent
    {
        private string m_Tag;
        private Color m_Color;

        public ColorPopup(string tag)
        {
            m_Tag = tag;
            m_Color = TagSystem.GetTagColor(tag);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(80, EditorGUIUtility.singleLineHeight + 4);
        }

        public override void OnGUI(Rect rect)
        {
            var r = new Rect(rect);
            r.xMin += 2;
            r.yMin += 2;
            r.xMax -= 2;
            r.yMax -= 2;

            var col = EditorGUI.ColorField(r, m_Color);
            if (m_Color != col)
            {
                m_Color = col;
                TagSystem.SetTagColor(m_Tag, m_Color);
                for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneAt(i));
            }
        }
    }
}