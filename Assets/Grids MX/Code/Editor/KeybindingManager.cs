using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class KeybindingManager : ScriptableObject
		{
			[System.NonSerialized]
			private static KeybindingManager s_instance;
			public static KeybindingManager instance
			{
				get
				{
					if (s_instance == null)
					{
						s_instance = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(KeybindingManager)) as KeybindingManager;
						if (s_instance == null)
						{
							s_instance = ScriptableObject.CreateInstance<KeybindingManager>();
							AssetDatabase.CreateAsset(s_instance, assetsPath);
							AssetDatabase.SaveAssets();
							AssetDatabase.Refresh();
						}
					}
					return s_instance;
				}
			}

			private const float FIELD_WIDTH = 45f;
			private static string assetsPath { get { return EditorUtil.ROOT_FOLDER + "/Resources/"
				+ GridManager.GRID_DATA_RESOURCES_PATH + "/Keybindings.asset"; } }
			
			[SerializeField] private List<Keybinding> m_keybindings;

			private void Awake()
			{
				if (m_keybindings == null)
				{
					m_keybindings = new List<Keybinding>();
				}
			}

			public void UpdateKeybindings()
			{
				foreach (Keybinding keybinding in m_keybindings)
				{
					if (GetKeybindingState(keybinding))
					{
						keybinding.callback();
                    }
				}
			}

			public void AddKeybinding(Keybinding keybinding, Keybinding.Callback callback)
			{
				Keybinding existingKB = GetKeybindingByID(keybinding.id);
				if (existingKB == null)
				{
					existingKB = keybinding;
                    m_keybindings.Add(keybinding);
					EditorUtility.SetDirty(this);
				}

				existingKB.callback = callback;
			}

			public void DrawKeybindingsGUI()
			{
				if (m_keybindings.Count == 0)
				{
					EditorGUILayout.LabelField("Keybindings not initialized! Please open the Grids MX window from the 'Window' menu");
					return;
				}

				EditorGUI.BeginChangeCheck();

				foreach (Keybinding keybinding in m_keybindings)
				{
					DrawKeybinding(keybinding);
				}

				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(this);
				}
			}

			public string GetKeybindingDescription(string id)
			{
				Keybinding kb = GetKeybindingByID(id);
				return (kb != null ? kb.ToString() : string.Empty);
			}

			private Keybinding GetKeybindingByID(string id)
			{
				for (int i = 0; i < m_keybindings.Count; ++i)
				{
					if (m_keybindings[i].id == id)
					{
						return m_keybindings[i];
					}
				}
				return null;
			}

			private bool GetKeybindingState(Keybinding keybinding)
			{
				if (!keybinding.enabled)
				{
					return false;
				}

				if (Event.current.type != EventType.KeyDown)
				{
					return false;
				}

				if (keybinding.alt != Event.current.alt)
				{
					return false;
				}

				if (keybinding.control != Event.current.control)
				{
					return false;
				}

				if (keybinding.shift != Event.current.shift)
				{
					return false;
				}

				if (keybinding.key != Event.current.keyCode)
				{
					return false;
				}

				return true;
			}

			private void DrawKeybinding(Keybinding keybinding)
			{
				EditorGUILayout.BeginHorizontal();
				{
					keybinding.enabled = EditorGUILayout.Toggle(keybinding.enabled, GUILayout.MinWidth(14f), GUILayout.Width(14f));

					EditorGUI.BeginDisabledGroup(!keybinding.enabled);
					{
						EditorGUILayout.LabelField(keybinding.label, GUILayout.MinWidth(126f));
						keybinding.control = EditorGUILayout.ToggleLeft("Ctrl", keybinding.control, GUILayout.MinWidth(FIELD_WIDTH));
						keybinding.alt = EditorGUILayout.ToggleLeft("Alt", keybinding.alt, GUILayout.MinWidth(FIELD_WIDTH));
						keybinding.shift = EditorGUILayout.ToggleLeft("Shift", keybinding.shift, GUILayout.MinWidth(FIELD_WIDTH));
						keybinding.key = (KeyCode)EditorGUILayout.EnumPopup(keybinding.key, GUILayout.MinWidth(FIELD_WIDTH));
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}