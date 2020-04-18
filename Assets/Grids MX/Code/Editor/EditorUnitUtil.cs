using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public static class EditorUnitUtil
		{
			public static double DrawConvertedSizeField(string label, double size, Unit storeAs, Unit displayAs, float minWidth)
			{
				size = UnitUtil.Convert(storeAs, displayAs, size);
#if UNITY_5
				size = EditorGUILayout.DoubleField(label, size, GUILayout.MinWidth(minWidth));
#else
				size = EditorGUILayout.FloatField(label, (float)size, GUILayout.MinWidth(minWidth));
#endif
				return UnitUtil.Convert(displayAs, storeAs, size);
			}

			public static double DrawConvertedSizeField(double size, Unit storeAs, Unit displayAs, float minWidth)
			{
				size = UnitUtil.Convert(storeAs, displayAs, size);
#if UNITY_5
				size = EditorGUILayout.DoubleField(size, GUILayout.MinWidth(minWidth));
#else
				size = EditorGUILayout.FloatField((float)size, GUILayout.MinWidth(minWidth));
#endif
				return UnitUtil.Convert(displayAs, storeAs, size);
			}
		}
	}
}
