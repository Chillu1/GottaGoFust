using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		[CustomEditor(typeof(KeybindingManager))]
		public class KeybindingManagerEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				KeybindingManager.instance.DrawKeybindingsGUI();
			}
		}
	}
}