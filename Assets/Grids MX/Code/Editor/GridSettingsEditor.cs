using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		[CustomEditor(typeof(GridSettings))]
		public class GridSettingsEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				EditorGridSettingsManager.instance.DrawSettingsGUI();
			}
		}
	}
}