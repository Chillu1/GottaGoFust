using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class GridMover : MonoBehaviour
		{
			[SerializeField] private int m_speed = 1;
			[SerializeField] private string[] m_gridNames;

			private GridData m_gridData = null;

			private GridPoint m_gridPosition;
			private Vector3 m_offset;

			private void Awake()
			{
				if (m_gridNames.Length > 0)
				{
					// You can easily access your grids at runtime via their "ID" property on the GridData object.
					// GridManager follows the Singleton pattern: it creates itself the first time you call it.
					m_gridData = GridManager.instance.GetGrid(m_gridNames[0]);
				}

				ResetGridPosition();
            }

			private void Update()
			{
				// Some simple arrow key movement using GridPoints.

				// Technically GridPoints are almost exactly the same as Vector3, except that they use
				// integers instead of floats for their components. This allows you to be more precise and clear
				// when you're using grid-space calculations vs when you're using unity-space.

				GridPoint direction = GridPoint.zero;
				if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
				{
					direction += GridPoint.forward;
				}
				if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
				{
					direction -= GridPoint.forward;
				}
				if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
				{
					direction += GridPoint.right;
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
				{
					direction -= GridPoint.right;
				}

				if (direction != GridPoint.zero)
				{
					Move(direction * m_speed);
				}
			}

			private void Move(GridPoint delta)
			{
				// Change the gridposition by delta, but GridPoint has no context what grid its related to,
				// so for it to obey clamping/wrapping etc. we ask the Grid to snap our new GridPoint.
				m_gridPosition = m_gridData.SnapGridPoint(m_gridPosition + delta);

				// This is the actual position update of the GameObject.
                this.transform.position = m_gridData.GridPointToWorldPosition(m_gridPosition);
			}

			private void ResetGridPosition()
			{
				// Ensure our grid position represents our world position appropriately
				m_gridPosition = m_gridData.WorldPositionToGridPoint(this.transform.position);

				// And make sure we're actually snapped to the grid
				this.transform.position = m_gridData.SnapPosition(this.transform.position);
			}

			private void OnGUI()
			{
				foreach (string gridName in m_gridNames)
				{
					GUI.color = (m_gridData.name == gridName ? Color.green : Color.white);
					if (GUILayout.Button(gridName))
					{
						m_gridData = GridManager.instance.GetGrid(gridName);
						ResetGridPosition();
					}
					GUI.color = Color.white;
				}

				GUILayout.Label("Current Grid: " + m_gridData.name);
				GUILayout.Label("Grid Coordinates: " + m_gridPosition);

				GUILayout.Label("");

				GUILayout.Label("Use the arrow keys or WASD to move the \npiece along the selected grid.");
			}
		}
	}
}