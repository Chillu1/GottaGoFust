using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MoreTags
{
    public class TagGUI
    {
        public Action<string> OnAddItem;
        public Action<string> OnClickItem;
        public Action<Rect, string> OnRightClickItem;
        public Func<string, GUIContent> OnItemString;
        public Func<string, Color> OnItemColor;

        private string m_NewItem = string.Empty;
        private GUIStyle m_BgStyle = null;

        public void InitStyle()
        {
            if (m_BgStyle != null) return;
            m_BgStyle = new GUIStyle("CN CountBadge");
            if (!EditorGUIUtility.isProSkin) return;

            var res = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (var tex in res)
                if (tex.name.Equals("ConsoleCountBadge") && tex != m_BgStyle.normal.background)
                    m_BgStyle.normal.background = tex;
        }

        public void OnGUI(IEnumerable<string> list, string header = null)
        {
            InitStyle();

            var guicolor = GUI.color;
            var tagstyle = new GUIStyle("OL Minus");
            tagstyle.normal.textColor = Color.white;
            tagstyle.font = EditorStyles.boldFont;
            var addstyle = new GUIStyle("OL Plus");
            var headstyle = new GUIStyle("label");
            headstyle.font = EditorStyles.boldFont;

            var xMax = EditorGUIUtility.currentViewWidth - 12;
            var height = EditorGUIUtility.singleLineHeight + 2;
            var newrect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
            var bgrect = new Rect(newrect);
            bgrect.xMin = 16;

            if (!string.IsNullOrEmpty(header))
            {
                var gc = new GUIContent(header);
                var w = headstyle.CalcSize(gc).x + 4;
                bgrect.width = w;
                GUI.color = new Color(0, 0, 0, 0);
                GUI.Box(bgrect, GUIContent.none, m_BgStyle);

                GUI.color = guicolor;
                var rect = new Rect(bgrect);
                rect.position += new Vector2(1, 1);
                GUI.Label(rect, gc, headstyle);
                bgrect.xMin = bgrect.xMax;
            }

            foreach (var item in list)
            {
                var s = item is string ? item as string : item.ToString();
                var gc = OnItemString != null ? OnItemString(item) : new GUIContent(s);
                var w = tagstyle.CalcSize(gc).x + 4;
                if (bgrect.xMin + w > xMax)
                {
                    newrect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                    bgrect = new Rect(newrect);
                    bgrect.xMin = string.IsNullOrEmpty(header) ? 16 : 30;
                }
                bgrect.width = w;
                var col = Color.white;
                if (OnItemColor != null) col = OnItemColor(item);
                GUI.color = col;
                GUI.Box(bgrect, GUIContent.none, m_BgStyle);

                var lum = col.grayscale > 0.5 ? col.grayscale - 0.5f : col.grayscale + 0.5f;
                GUI.color = new Color(lum, lum, lum, 1.0f);
                var rect = new Rect(bgrect);
                rect.position += new Vector2(1, 1);
                if (GUI.Button(rect, gc, tagstyle))
                {
                    if (Event.current.button == 0 && OnClickItem != null)
                        OnClickItem(item);
                    if (Event.current.button == 1 && OnRightClickItem != null)
                        OnRightClickItem(rect, item);
                }
                bgrect.xMin = bgrect.xMax;
                GUI.color = guicolor;
            }

            if (OnAddItem != null)
            {
                if (bgrect.xMin + 150 > xMax)
                {
                    newrect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
                    bgrect = new Rect(newrect);
                    bgrect.xMin = string.IsNullOrEmpty(header) ? 16 : 30;
                }
                bgrect.width = 150;
                GUI.Box(bgrect, GUIContent.none, m_BgStyle);
                var gc = GUIContent.none;
                var w = tagstyle.CalcSize(gc).x;
                var rect = new Rect(bgrect);
                rect.position += new Vector2(1, 1);
                rect.width = w;
                if (GUI.Button(rect, gc, addstyle))
                {
                    OnAddItem(m_NewItem);
                    m_NewItem = string.Empty;
                }
                rect = new Rect(bgrect);
                rect.yMin += 1;
                rect.yMax -= 1;
                rect.xMin += w;
                rect.xMax -= 8;
                m_NewItem = GUI.TextField(rect, m_NewItem);
                bgrect.xMin = bgrect.xMax;
            }
        }
    }
}