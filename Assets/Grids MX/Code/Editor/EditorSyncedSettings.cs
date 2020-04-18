using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace mx
{
	namespace Grids
	{
		/// <summary>
		/// Keeps a list of members in an object and syncs them with 
		/// </summary>
		public class EditorSyncedSettings
		{
			private const string USER_VERSION_KEY = "userversion";

			private SyncedSettings m_syncedSettings = null;
			private SerializedObject m_serializedSettings = null;
			private string m_prefsKeyPrefix = string.Empty;

			private SerializedProperty m_userVersionProperty = null;
			private List<SerializedProperty> m_properties = null;

			private string userVersionKey { get { return GetPrefsKey(USER_VERSION_KEY); } }

			public bool hasSettings { get { return m_serializedSettings != null && m_serializedSettings.targetObject != null; } }

			public EditorSyncedSettings(SyncedSettings settingsObject, string prefsKeyPrefix)
			{
				if (settingsObject == null)
				{
					Debug.LogError("Settings object must not be null!");
				}

				m_syncedSettings = settingsObject as SyncedSettings;
				m_serializedSettings = new SerializedObject(settingsObject);
				m_prefsKeyPrefix = prefsKeyPrefix;

				if (m_syncedSettings == null)
				{
					Debug.LogError("Settings object must implement interface ISyncedSettings.");
				}
				if (m_prefsKeyPrefix == string.Empty)
				{
					Debug.LogError("Prefs key prefix must not be empty."
						+ "This is to prevent collisions with other data, so make it unique please!");
				}

				string userVersionName = m_syncedSettings.GetUserVersionPropertyName();
                m_userVersionProperty = m_serializedSettings.FindProperty(userVersionName);

				m_properties = new List<SerializedProperty>();

				BindingFlags bindingFlags =
					(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				FieldInfo[] fieldInfos = settingsObject.GetType().GetFields(bindingFlags);
				foreach (FieldInfo fieldInfo in fieldInfos)
				{
					if (fieldInfo.GetCustomAttributes(typeof(NonSyncedAttribute), false).Length > 0)
					{
						continue;
					}

					SerializedProperty prop = m_serializedSettings.FindProperty(fieldInfo.Name);
					if (prop != null)
					{
						m_properties.Add(prop);
					}
				}
            }

			/// <summary>
			/// Retrieves property with given name.
			/// 
			/// Remember to call Update and ApplyModifiedProperties on this object as you
			/// would with a normal SerializedObject.
			/// </summary>
			public SerializedProperty GetProperty(string propertyName)
			{
				return m_properties.Find((SerializedProperty element) => { return element.name == propertyName; });
			}

			/// <summary>
			/// Returns a list of SerializedProperties with the given names.
			/// 
			/// Remember to call Update and ApplyModifiedProperties on this object as you
			/// would with a normal SerializedObject.
			/// </summary>
			public List<SerializedProperty> GetPropertiesInclusive(params string[] propertyNames)
			{
				List<SerializedProperty> requestedProperties = new List<SerializedProperty>();
				for (int i = 0; i < m_properties.Count; ++i)
				{
					foreach (string name in propertyNames)
					{
						if (m_properties[i].name == name)
						{
							requestedProperties.Add(m_properties[i]);
							break;
						}
					}
				}
				return requestedProperties;
			}

			/// <summary>
			/// Returns a list of SerializedProperties that do NOT have the given names.
			/// 
			/// Remember to call Update and ApplyModifiedProperties on this object as you
			/// would with a normal SerializedObject.
			/// </summary>
			public List<SerializedProperty> GetPropertiesExclusive(params string[] propertyNames)
			{
				List<SerializedProperty> requestedProperties = new List<SerializedProperty>();
				for (int i = 0; i < m_properties.Count; ++i)
				{
					bool hasName = false;
					foreach (string name in propertyNames)
					{
						if (m_properties[i].name == name)
						{
							hasName = true;
                            break;
						}
					}

					if (hasName)
					{
						requestedProperties.Add(m_properties[i]);
					}
				}
				return requestedProperties;
			}

			/// <summary>
			/// Clears all associated editor preferences for the settings object. Does NOT delete the settings object itself.
			/// </summary>
			public void ClearPrefs()
			{
				// @todo also needs to delete keys for all others while we're at it.. for cleanliness
				EditorPrefs.DeleteKey(userVersionKey);

				foreach (SerializedProperty property in m_properties)
				{
					string key = GetPrefsKey(property.name);
					EditorPrefs.DeleteKey(key);
				}
			}

			/// <summary>
			/// Updates the state of the serialized object
			/// </summary>
			public void Update()
			{
				m_serializedSettings.Update();
			}

			/// <summary>
			/// Applys any modified properties of the serialized object and syncs them with editor prefs
			/// </summary>
			public void ApplyModifiedProperties()
			{
				m_serializedSettings.ApplyModifiedProperties();
				IncrementVersion();
				Sync();
			}

			/// <summary>
			/// Sync settings in asset and prefs to whichever is more recent.
			/// </summary>
			public void Sync()
			{
				int assetUserVersion = m_syncedSettings.userVersion;
				int prefsUserVersion = EditorPrefs.GetInt(userVersionKey, -1);
				if (assetUserVersion > prefsUserVersion)
				{
					SyncFromAsset();
				}
				else if (assetUserVersion < prefsUserVersion)
				{
					SyncFromPrefs();
				}
			}

			private void SyncFromAsset()
			{
				foreach (SerializedProperty property in m_properties)
				{
					string key = GetPrefsKey(property.name);
					switch (property.propertyType)
					{
						case SerializedPropertyType.Enum:		EditorPrefs.SetInt(key, property.enumValueIndex); break;
						case SerializedPropertyType.Integer:	EditorPrefs.SetInt(key, property.intValue); break;
						case SerializedPropertyType.Float:		EditorPrefs.SetFloat(key, property.floatValue); break;
						case SerializedPropertyType.Boolean:	EditorPrefs.SetBool(key, property.boolValue); break;
						case SerializedPropertyType.Color:		PrefsSetColor(key, property.colorValue); break;
						default:
							Debug.LogError(string.Format("SyncedSettings -- Unsupported property type: ", property.propertyType));
							break;
					}
				}
			}
			
			private void SyncFromPrefs()
			{
				m_serializedSettings.Update();

				foreach (SerializedProperty property in m_properties)
				{
					string key = GetPrefsKey(property.name);
					switch (property.propertyType)
					{
						case SerializedPropertyType.Enum:		property.enumValueIndex =	EditorPrefs.GetInt(key); break;
						case SerializedPropertyType.Integer:	property.intValue =			EditorPrefs.GetInt(key); break;
						case SerializedPropertyType.Float:		property.floatValue =		EditorPrefs.GetFloat(key); break;
						case SerializedPropertyType.Boolean:	property.boolValue =		EditorPrefs.GetBool(key); break;
						case SerializedPropertyType.Color:		property.colorValue =		PrefsGetColor(key); break;
						default:
							Debug.LogError(string.Format("SyncedSettings -- Unsupported property type: ", property.propertyType));
							break;
					}
				}

				m_serializedSettings.ApplyModifiedProperties();
			}

			private void IncrementVersion()
			{
				m_serializedSettings.Update();

				m_userVersionProperty.intValue = (m_userVersionProperty.intValue + 1);
				if (m_userVersionProperty.intValue == int.MaxValue)
				{
					// you know... safety first...
					m_userVersionProperty.intValue = 0;
				}

				m_serializedSettings.ApplyModifiedProperties();
			}

			private string GetPrefsKey(string name)
			{
				return string.Format("{0}_{1}", m_prefsKeyPrefix, name);
			}

			private void PrefsSetColor(string key, Color color)
			{
				EditorPrefs.SetFloat(key + "_r", color.r);
				EditorPrefs.SetFloat(key + "_g", color.g);
				EditorPrefs.SetFloat(key + "_b", color.b);
				EditorPrefs.SetFloat(key + "_a", color.a);
			}

			private Color PrefsGetColor(string key)
			{
				Color c = new Color();
				c.r = EditorPrefs.GetFloat(key + "_r", 1f);
				c.g = EditorPrefs.GetFloat(key + "_g", 1f);
				c.b = EditorPrefs.GetFloat(key + "_b", 1f);
				c.a = EditorPrefs.GetFloat(key + "_a", 1f);
				return c;
			}
		}
	}
}