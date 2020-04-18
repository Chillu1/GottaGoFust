using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public static class EditorGUIExtensions
		{
			public static bool IconToggle(string disabledIconPath, string enabledIconPath, bool value,
				string tooltipWhenEnabled, string tooltipWhenDisabled)
			{
				string texturePath = (value ? enabledIconPath : disabledIconPath);
				Texture2D texture = EditorTextures.GetByPath(texturePath);

				string tooltip = (value ? tooltipWhenEnabled : tooltipWhenDisabled);
				if (GUILayout.Button(new GUIContent(texture, tooltip), GUILayout.Height(GridWindow.BUTTON_HEIGHT)))
				{
					return !value;
				}

				return value;
			}

			public delegate void IconPrefixedFieldCallback();
			public static void DrawIconPrefixedField(string iconPath, string tooltip, IconPrefixedFieldCallback callback)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUIContent guiContent = new GUIContent(EditorTextures.GetByPath(iconPath), tooltip);
					EditorGUILayout.LabelField(guiContent, GUILayout.Width(20f));
					callback();
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}