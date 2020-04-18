using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		[CustomEditor(typeof(GridData))]
		public class GridDataEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				this.serializedObject.Update();

				DrawPropertiesExcluding(this.serializedObject, "m_gridOrigin", "m_gridSize", "m_rotationSnapAngle",
                    "m_xAxisProperties", "m_yAxisProperties", "m_zAxisProperties");

				GridData gridData = (GridData)target;
				gridData.gridOrigin = GridPointField("Origin Point", gridData.gridOrigin);

				if (GUI.changed)
				{
					EditorUtility.SetDirty(target);
					this.serializedObject.ApplyModifiedProperties();

					GridWindow.ResetSceneGrid();
				}

				if (GUILayout.Button("Open Grids MX Toolbar"))
				{
					GridWindow.Open();
	            }
			}

			private GridPoint GridPointField(string label, GridPoint gridPoint)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GridData gridData = (GridData)target;
					Vector3 v = EditorGUILayout.Vector3Field(label, (Vector3)gridPoint);
					v.x = Mathf.Clamp(v.x, 0, gridData.gridSize);
					v.y = Mathf.Clamp(v.y, 0, gridData.gridSize);
					v.z = Mathf.Clamp(v.z, 0, gridData.gridSize);
					gridPoint = new GridPoint(v);
				}
				EditorGUILayout.EndHorizontal();
				return gridPoint;
			}
		}
	}
}