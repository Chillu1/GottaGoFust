using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public enum Axis
		{
			X,
			Y,
			Z
		}

		public static class AxisUtil
		{
			public static Vector3 GetVector(Axis axis)
			{
				switch (axis)
				{
					case Axis.X:
						return Vector3.right;
					case Axis.Y:
						return Vector3.up;
					case Axis.Z:
						return Vector3.forward;
					default:
						Debug.LogError(string.Format("Grids MX -- Unknown axis [{0}].", axis));
						return Vector3.zero;
				}
			}

			public static Vector3 GetComponentVector(this Vector3 v, Axis axis)
			{
				return Vector3.Scale(v, GetVector(axis));
			}

			public static float GetComponent(this Vector3 v, Axis axis)
			{
				switch (axis)
				{
					case Axis.X: return v.x;
					case Axis.Y: return v.y;
					case Axis.Z: return v.z;
					default:
						Debug.LogError(string.Format("Grids MX -- Unknown Axis: {0}", axis));
						return 0;
				}
			}
		}
		
		[System.Serializable]
		public class AxisProperties
		{
			public bool isVisibleInEditor = true;
			public bool isSnapEnabled = true;
            public float cellSize = 1f;
			public float offset = 0f;
			public float rotation = 0f;
		}
	}
}