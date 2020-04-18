using UnityEngine;

namespace mx
{
	namespace Grids
	{
		[System.Serializable]
		public struct GridPoint
		{
			public static readonly GridPoint zero = new GridPoint(0, 0, 0);
			public static readonly GridPoint one = new GridPoint(1, 1, 1);
			public static readonly GridPoint right = new GridPoint(1, 0, 0);
			public static readonly GridPoint up = new GridPoint(0, 1, 0);
			public static readonly GridPoint forward = new GridPoint(0, 0, 1);

			public enum DistanceType
			{
				Straight,
				StraightOrDiagonal
			}

			[SerializeField] private int m_x;
			[SerializeField] private int m_y;
			[SerializeField] private int m_z;

			public int x { get { return m_x; } }
			public int y { get { return m_y; } }
			public int z { get { return m_z; } }

			public GridPoint(int x, int y, int z)
			{
				m_x = x;
				m_y = y;
				m_z = z;
			}

			public GridPoint(Vector3 v)
			{
				m_x = (int)v.x;
				m_y = (int)v.y;
				m_z = (int)v.z;
			}

			public void Wrap(GridPoint min, GridPoint max)
			{
				m_x = WrapInt(m_x, min.x, max.x);
				m_y = WrapInt(m_y, min.y, max.y);
				m_z = WrapInt(m_z, min.z, max.z);
			}

			public int GetComponent(Axis axis)
			{
				switch (axis)
				{
					case Axis.X:	return m_x;
					case Axis.Y:	return m_y;
					case Axis.Z:	return m_z;
					default:
						Debug.LogError(string.Format("Grids MX -- Unknown Axis: {0}", axis));
						return 0;
				}
			}

			public static int Distance(GridPoint lhs, GridPoint rhs, DistanceType type)
			{
				switch (type)
				{
					case DistanceType.Straight:
						return Mathf.Abs(lhs.x - rhs.x) + Mathf.Abs(lhs.y - rhs.y) + Mathf.Abs(lhs.z - rhs.z);
					case DistanceType.StraightOrDiagonal:
						return Mathf.Max(Mathf.Abs(lhs.x - rhs.x), Mathf.Abs(lhs.y - rhs.y), Mathf.Abs(lhs.z - rhs.z));
					default:
						Debug.LogError(string.Format("Grids MX --Unknown distance type: {0}", type));
						return 0;
				}
			}

			public static GridPoint Scale(GridPoint gridPoint, Vector3 scale)
			{
				return new GridPoint((int)(gridPoint.x * scale.x), (int)(gridPoint.y * scale.y), (int)(gridPoint.z * scale.z));
			}

			public static Vector3 AverageWorldPosition(params GridPoint[] points)
			{
				Vector3 worldPosition = new Vector3();
				for (int i = 1; i < points.Length; ++i)
				{
					worldPosition.x += points[i].x;
					worldPosition.y += points[i].y;
					worldPosition.z += points[i].z;
				}

				if (points.Length > 0)
				{
					worldPosition /= points.Length;
				}
				return worldPosition;
			}

			public override bool Equals(object other)
			{
				if (!(other is GridPoint))
				{
					return false;
				}
				return this.Equals((GridPoint)other);
			}

			public bool Equals(GridPoint other)
			{
				return (other == this);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hash = 17;
					hash = hash * 23 + this.x.GetHashCode();
					hash = hash * 23 + this.y.GetHashCode();
					hash = hash * 23 + this.z.GetHashCode();
					return hash;
				}
			}

			public override string ToString()
			{
				return string.Format("[{0}, {1}, {2}]", x, y, z);
			}

			private int WrapInt(int i, int min, int max)
			{
				if (i < min)
				{
					i = max;
				}
				if (i > max)
				{
					i = min;
				}
				return i;
			}

			public static bool operator ==(GridPoint a, GridPoint b)
			{
				return (a.x == b.x && a.y == b.y && a.z == b.z);
			}

			public static bool operator !=(GridPoint a, GridPoint b)
			{
				return !(a == b);
			}

			public static GridPoint operator +(GridPoint a, GridPoint b)
			{
				return new GridPoint(a.x + b.x, a.y + b.y, a.z + b.z);
			}

			public static GridPoint operator -(GridPoint a, GridPoint b)
			{
				return new GridPoint(a.x - b.x, a.y - b.y, a.z - b.z);
			}

			public static GridPoint operator *(GridPoint a, int scalar)
			{
				return new GridPoint(a.x * scalar, a.y * scalar, a.z * scalar);
			}

			public static GridPoint operator *(GridPoint a, float scalar)
			{
				return new GridPoint((int)(a.x * scalar), (int)(a.y * scalar), (int)(a.z * scalar));
			}

			public static GridPoint operator *(int scalar, GridPoint a)
			{
				return a * scalar;
			}

			public static explicit operator Vector3(GridPoint point)
			{
				return new Vector3(point.x, point.y, point.z);
			}
		}
	}
}