using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class GridData : ScriptableObject
		{
			public enum EdgeType
			{
				Infinite,
				Free,
                Clamp,
				Wrap
			}
			
			[Tooltip("Infinite: Grid snapping continues after edge of visible grid. Size of grid is effectively display distance."
				+ "\n\nFree: Grid snapping stops after edge of grid."
				+ "\n\nClamp: Movement is clamped at grid edge, objects cannot be moved outside of it."
				+ "\n\nWrap: Objects moved outside of the grid wrap around to the other side of the grid.")]
			[SerializeField] private EdgeType m_edgeType = EdgeType.Infinite;

			[SerializeField] private GridPoint m_gridOrigin = new GridPoint(50, 50, 50);
			[SerializeField] private int m_gridSize = 100;
			[SerializeField] private float m_rotationSnapAngle = 0.0f;
			[SerializeField] private AxisProperties m_xAxisProperties;
			[SerializeField] private AxisProperties m_yAxisProperties;
			[SerializeField] private AxisProperties m_zAxisProperties;

			public GridPoint gridOrigin { get { return m_gridOrigin; } set { m_gridOrigin = value; } }
			public int gridSize { get { return m_gridSize; } set { m_gridSize = value; } }
			public float rotationSnapAngle { get { return m_rotationSnapAngle; } set { m_rotationSnapAngle = value; } }
			
			public EdgeType edgeType { get { return m_edgeType; } }
			public AxisProperties xAxisProperties { get { return m_xAxisProperties; } }
			public AxisProperties yAxisProperties { get { return m_yAxisProperties; } }
			public AxisProperties zAxisProperties { get { return m_zAxisProperties; } }

			/// <summary>
			/// Returns the closest gridpoint representation of the given world position.
			/// IGNORES edge type. Use Snap functions to get appropriate edge behaviour.
			/// </summary>
			public GridPoint WorldPositionToGridPoint(Vector3 worldPosition)
			{
				return GridPositionToGridPoint(WorldToGridPosition(worldPosition));
			}

			/// <summary>
			/// Returns the gridpoint representation of the given grid position.
			/// IGNORES edge type. Use Snap functions to get appropriate edge behaviour.
			/// </summary>
			public GridPoint GridPositionToGridPoint(Vector3 gridPosition)
			{
				return new GridPoint(gridPosition);
			}

			/// <summary>
			/// Returns the world position equivalent to the given grid point
			/// IGNORES edge type. Use Snap functions to get appropriate edge behaviour.
			/// </summary>
			public Vector3 GridPointToWorldPosition(GridPoint gridPoint)
			{
				return GridToWorldPosition(GridPointToGridPosition(gridPoint));
			}

			/// <summary>
			/// Returns the grid position equivalent to the given grid point
			/// IGNORES edge type. Use Snap functions to get appropriate edge behaviour.
			/// </summary>
			public Vector3 GridPointToGridPosition(GridPoint gridPoint)
			{
				return (Vector3)gridPoint;
			}

			/// <summary>
			/// Returns the nearest world position that aligns with the grid along the given axis.
			/// </summary>
			public Vector3 SnapPosition(Vector3 worldPosition, Axis axis, bool snapEvenIfDisabled = false)
			{
				Vector3 newPosition = WorldToGridPosition(worldPosition);
				GridPoint snappedPoint = WorldPositionToGridPoint(worldPosition);

				float value = SnapPositionInternal(snappedPoint, newPosition, axis, snapEvenIfDisabled);
				switch (axis)
				{
					case Axis.X:	newPosition.x = value; break;
					case Axis.Y:	newPosition.y = value; break;
					case Axis.Z:	newPosition.z = value; break;
					default:
						Debug.LogError(string.Format("Grids MX -- Unknown axis [{0}].", axis));
						break;
				}

				return GridToWorldPosition(newPosition);
			}

			/// <summary>
			/// Returns the nearest world position that aligns with the grid
			/// </summary>
			public Vector3 SnapPosition(Vector3 worldPosition, bool snapEvenIfDisabled = false)
			{
				Vector3 newPosition = WorldToGridPosition(worldPosition);
				GridPoint snappedPoint = WorldPositionToGridPoint(worldPosition);

				newPosition.x = SnapPositionInternal(snappedPoint, newPosition, Axis.X, snapEvenIfDisabled);
				newPosition.y = SnapPositionInternal(snappedPoint, newPosition, Axis.Y, snapEvenIfDisabled);
				newPosition.z = SnapPositionInternal(snappedPoint, newPosition, Axis.Z, snapEvenIfDisabled);

				return GridToWorldPosition(newPosition);
			}

			/// <summary>
			/// Rotates from current to target by snapped angles based on the rotation snap settings of the grid
			/// </summary>
			public Quaternion SnapRotation(Quaternion currentRot, Quaternion targetRot)
			{
				float angle = Quaternion.Angle(currentRot, targetRot);
#if UNITY_5_2
				while (angle > m_rotationSnapAngle * 0.5f)
				{
					currentRot = Quaternion.SlerpUnclamped(currentRot, targetRot, m_rotationSnapAngle / angle);
					angle -= m_rotationSnapAngle;
				}
#else
				// Works and is accurate, but doesn't feel quite as good as the unity 5 version
				if (angle > m_rotationSnapAngle)
				{
					return targetRot;
				}
#endif
				return currentRot;
			}

			/// <summary>
			/// Returns a gridpoint that obeys the edge type and size of the grid
			/// </summary>
			public GridPoint SnapGridPoint(GridPoint gridPoint, bool snapEvenIfDisabled = false)
			{
				Vector3 gridPosition = GridPointToGridPosition(gridPoint);

				gridPosition.x = SnapPositionInternal(gridPoint, gridPosition, Axis.X, snapEvenIfDisabled);
				gridPosition.y = SnapPositionInternal(gridPoint, gridPosition, Axis.Y, snapEvenIfDisabled);
				gridPosition.z = SnapPositionInternal(gridPoint, gridPosition, Axis.Z, snapEvenIfDisabled);

				return GridPositionToGridPoint(gridPosition);
			}

			/// <summary>
			/// Converts a world position to a gridspace position
			/// </summary>
			public Vector3 WorldToGridPosition(Vector3 worldPosition)
			{
				return GetGridToWorldMatrix().inverse.MultiplyPoint3x4(worldPosition);
            }

			/// <summary>
			/// Converts a gridspace position to a world position
			/// </summary>
			public Vector3 GridToWorldPosition(Vector3 gridPosition)
			{
				return GetGridToWorldMatrix().MultiplyPoint3x4(gridPosition);
			}

			/// <summary>
			/// The rotation of the grid itself
			/// </summary>
			public Quaternion GetGridRotation()
			{
				return Quaternion.Euler(xAxisProperties.rotation, yAxisProperties.rotation, zAxisProperties.rotation);
			}

			/// <summary>
			/// The minimum point on the grid in all directions
			/// </summary>
			public GridPoint GetGridMin()
			{
				return -1 * m_gridOrigin;
			}

			/// <summary>
			/// The maximum point on the grid in all directions
			/// </summary>
			public GridPoint GetGridMax()
			{
				return (GridPoint.one * m_gridSize) + GetGridMin();
			}

			/// <summary>
			/// Returns the properties object for the given axis.
			/// </summary>
			public AxisProperties GetAxisProperties(Axis axis)
			{
				switch (axis)
				{
					case Axis.X:
						return xAxisProperties;
					case Axis.Y:
						return yAxisProperties;
					case Axis.Z:
						return zAxisProperties;
					default:
						Debug.LogError(string.Format("Unknown axis [{0}].", axis));
						return null;
				}
			}

			/// <summary>
			/// Gets the matrix that converts from Grid to World space
			/// </summary>
			public Matrix4x4 GetGridToWorldMatrix()
			{
				Vector3 offset = new Vector3(xAxisProperties.offset,
											 yAxisProperties.offset,
											 zAxisProperties.offset);
				Vector3 size = new Vector3(xAxisProperties.cellSize, yAxisProperties.cellSize, zAxisProperties.cellSize);

				return Matrix4x4.TRS(offset, GetGridRotation(), size);
			}

			private float SnapPositionInternal(GridPoint snapTo, Vector3 gridspacePosition, Axis axis, bool snapEvenIfDisabled)
			{
                float gridspaceComponent = gridspacePosition.GetComponent(axis);

				AxisProperties properties = GetAxisProperties(axis);
				if (!properties.isSnapEnabled && !snapEvenIfDisabled)
				{
					return gridspaceComponent;
				}

				float snapToComponent = snapTo.GetComponent(axis);
				switch (m_edgeType)
				{
					case EdgeType.Infinite:
						// no changes, just snap to nearest
						return snapToComponent;

					case EdgeType.Free:
						return IsOutsideGrid(snapTo) ? gridspaceComponent : snapToComponent;

					case EdgeType.Clamp:
						{
							int gridMin = GetGridMin().GetComponent(axis);
							int gridMax = GetGridMax().GetComponent(axis);
							if (snapToComponent < gridMin)
							{
								return gridMin;
							}
							else if (snapToComponent > gridMax)
							{
								return gridMax;
							}
							else
							{
								return snapToComponent;
							}
						}

					case EdgeType.Wrap:
						{
							// while loops will bring snapTo in range of grid and wrap w/ overflow instead of clamping
							// e.g. on a 1-10 grid, a value of 13 would wrap to 2 (13 - gridSize of 10 = 3, -1 for the 'invisible'
							// grid space between edges)

							int gridMin = GetGridMin().GetComponent(axis);
							int gridMax = GetGridMax().GetComponent(axis);
							while (snapToComponent < gridMin)
							{
								snapToComponent += gridSize + 1;
							}

							while (snapToComponent > gridMax)
							{
								snapToComponent -= gridSize + 1;
							}
							return snapToComponent;
						}

					default:
						Debug.LogError(string.Format("Grids MX -- Unknown Edge Type: {0}", m_edgeType));
						return gridspaceComponent;
				}
			}

			private bool IsOutsideGrid(GridPoint point)
			{
				GridPoint gridMin = GetGridMin();
				GridPoint gridMax = GetGridMax();
				return (point.x < gridMin.x || point.y < gridMin.y || point.z < gridMin.z
					 || point.x > gridMax.x || point.y > gridMax.y || point.z > gridMax.z);
            }

			private float ClampRotation(float degrees, float clampAngle)
			{
				return Mathf.Round(degrees / clampAngle) * clampAngle;
			}
		}
	}
}