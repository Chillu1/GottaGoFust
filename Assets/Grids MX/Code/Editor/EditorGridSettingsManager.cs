using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace mx
{
	namespace Grids
	{
		public class EditorGridSettingsManager
		{
			private static EditorGridSettingsManager m_instance = null;
			public static EditorGridSettingsManager instance
			{
				get
				{
					if (m_instance == null)
					{
						m_instance = new EditorGridSettingsManager();
					}
					return m_instance;
				}
			}

			private static Vector2 s_scrollViewPosition;

			private EditorSyncedSettings m_editorSyncedSettings;

			private EditorGridSettingsManager() { }
			
			public void CreateSettingsAsset()
			{
				string resourcesPath = EditorUtil.ROOT_FOLDER + "/Resources/" + GridSettings.RESOURCES_FOLDER_PATH;
				GridSettings settings = ScriptableObject.CreateInstance<GridSettings>();
				AssetDatabase.CreateAsset(settings, resourcesPath + "/" + GridSettings.RESOURCES_FILE_NAME + ".asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			public void Reset()
			{
				CreateSettingsAsset();
				
				m_editorSyncedSettings.ClearPrefs();
				m_editorSyncedSettings = null;
				GetOrCreateSyncedSettings();
				m_editorSyncedSettings.Sync();

				GridWindow.ResetSceneGrid();
			}

			public void DrawSettingsGUI()
			{
				GetOrCreateSyncedSettings();
				m_editorSyncedSettings.Update();

				DrawProperties(m_editorSyncedSettings.GetPropertiesInclusive(
					"m_gridWindowLayout",
					"m_showScrollBars",
					"m_showDataManagementOptions",
                    "m_showGlobalSnappingOptions",
                    "m_showXAxisOptions",
					"m_showYAxisOptions",
					"m_showZAxisOptions",
					"m_showGridMovementOptions",
					"m_showGridFollowOptions",
					"m_xAxisColor",
					"m_yAxisColor",
					"m_zAxisColor",
					"m_majorLineSpacing",
					"m_majorLineOpacity"
					));


				SerializedProperty unfocusedStyle = m_editorSyncedSettings.GetProperty("m_unfocusedStyle");
				EditorGUILayout.PropertyField(unfocusedStyle);
				EditorGUI.BeginDisabledGroup((GridSettings.VisualStyle)unfocusedStyle.enumValueIndex == GridSettings.VisualStyle.Off);
				{
					DrawProperties(m_editorSyncedSettings.GetPropertiesInclusive(
							"m_unfocusedColor"
                        ));
				}
				EditorGUI.EndDisabledGroup();

				DrawProperties(m_editorSyncedSettings.GetPropertiesInclusive(
					"m_measurementUnit"
                    ));
				
				SerializedProperty coordinateDisplay = m_editorSyncedSettings.GetProperty("m_coordinateDisplay");
				EditorGUILayout.PropertyField(coordinateDisplay);
				EditorGUI.BeginDisabledGroup((CoordinateDisplay)coordinateDisplay.enumValueIndex == CoordinateDisplay.Off);
				{
					DrawProperties(m_editorSyncedSettings.GetPropertiesInclusive(
						"m_coordinateAnchor",
						"m_coordinateColor",
						"m_coordinateSize",
						"m_useAxisColorForComponents"
						));
				}
				EditorGUI.EndDisabledGroup();

				SerializedProperty majorLineSpacing = m_editorSyncedSettings.GetProperty("m_majorLineSpacing");
				SerializedProperty majorLineOpacity = m_editorSyncedSettings.GetProperty("m_majorLineOpacity");
				SerializedProperty coordinateSize = m_editorSyncedSettings.GetProperty("m_coordinateSize");
				majorLineSpacing.intValue = Mathf.Clamp(majorLineSpacing.intValue, 1, int.MaxValue);
				majorLineOpacity.floatValue = Mathf.Clamp(majorLineOpacity.floatValue, 0f, 1f);
				coordinateSize.intValue = Mathf.Clamp(coordinateSize.intValue, 1, int.MaxValue);

				DrawReferenceLineProperties("m_planeReferenceDisplay", "m_planeReferenceStyle", "m_planeReferenceColor");
				DrawReferenceLineProperties("m_axisReferenceDisplay", "m_axisReferenceStyle", "m_axisReferenceColor");
				
				if (GUI.changed)
				{
					m_editorSyncedSettings.ApplyModifiedProperties();
					GridWindow.RepaintIfOpen();
					GridWindow.ResetSceneGrid();
				}
				
				if (GUILayout.Button("Reset to Defaults"))
				{
					Reset();
				}
				
				if (GUILayout.Button("Open Grids MX Toolbar"))
				{
					GridWindow.Open();
				}
			}

			private void DrawProperties(List<SerializedProperty> properties)
			{
				foreach (SerializedProperty property in properties)
				{
					EditorGUILayout.PropertyField(property);
				}
			}

			private void DrawReferenceLineProperties(string display, string style, string color)
			{
				SerializedProperty displayProperty = m_editorSyncedSettings.GetProperty(display);
				EditorGUILayout.PropertyField(displayProperty);
				EditorGUI.BeginDisabledGroup((ReferenceLineDisplay)displayProperty.enumValueIndex == ReferenceLineDisplay.Off);
				{
					SerializedProperty styleProperty = m_editorSyncedSettings.GetProperty(style);
					EditorGUILayout.PropertyField(styleProperty);
					EditorGUI.BeginDisabledGroup((ReferenceLineStyle)styleProperty.enumValueIndex == ReferenceLineStyle.UseAxisColors);
					{
						SerializedProperty colorProperty = m_editorSyncedSettings.GetProperty(color);
						EditorGUILayout.PropertyField(colorProperty);
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUI.EndDisabledGroup();
			}
			
			private void GetOrCreateSyncedSettings()
			{
                if (m_editorSyncedSettings == null || !m_editorSyncedSettings.hasSettings)
				{
					if (GridSettings.instance == null)
					{
						CreateSettingsAsset();
					}
					m_editorSyncedSettings = new EditorSyncedSettings(GridSettings.instance, "mx_gridsettings");
				}
			}
			
			[PreferenceItem("Grids MX")]
			private static void OnPreferencesGUI()
			{
				s_scrollViewPosition = EditorGUILayout.BeginScrollView(s_scrollViewPosition);
				{
					EditorGridSettingsManager.instance.DrawSettingsGUI();

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Keybindings", EditorStyles.boldLabel);
					KeybindingManager.instance.DrawKeybindingsGUI();
				}
				EditorGUILayout.EndScrollView();
			}
		}
	}
}