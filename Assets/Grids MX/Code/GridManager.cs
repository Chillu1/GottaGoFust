using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class GridManager
		{
			public const string GRID_DATA_RESOURCES_PATH = "GridsMXData";
			public const string GRID_DATA_RESOURCES_FORMAT = GRID_DATA_RESOURCES_PATH + "/{0}";

			private static GridManager s_instance;
			/// <summary>
			/// GridManager follows the Singleton pattern: an instance will be created the first time you request it.
			/// </summary>
			public static GridManager instance
			{
				get
				{
					if (s_instance == null)
					{
						s_instance = new GridManager();
					}
					return s_instance;
				}
			}

			private Dictionary<string, GridData> m_gridData;

			private GridManager()
			{
				m_gridData = new Dictionary<string, GridData>();
			}

			/// <summary>
			/// Retrieves the grid with the given name. If the grid has not been loaded, it will be loaded immediately.
			/// You can frontload loading operations with LoadGrids() or LoadAllGrids().
			/// </summary>
			public GridData GetGrid(string name)
			{
				GridData gridData = null;
				if (!m_gridData.TryGetValue(name, out gridData))
				{
					LoadGrids(name);
					if (!m_gridData.TryGetValue(name, out gridData))
					{
						Debug.LogError(string.Format("Grid -- Unable to find grid with name: {0}", name));
					}
				}
				return gridData;
			}

			/// <summary>
			/// Loads all GridData with the given names in all 'GridsMXData' Resources folders
			/// </summary>
			public void LoadGrids(params string[] gridNames)
			{
				foreach (string gridName in gridNames)
				{
					GridData data = Resources.Load<GridData>(string.Format(GRID_DATA_RESOURCES_FORMAT, gridName));
					AddGrid(data);
				}
			}

			/// <summary>
			/// Loads all GridData in all 'GridsMXData' Resources folders
			/// </summary>
			public void LoadAllGrids()
			{
				GridData[] loadedData = Resources.LoadAll<GridData>(GRID_DATA_RESOURCES_PATH);
				foreach (GridData data in loadedData)
				{
					AddGrid(data);
                }
			}

			/// <summary>
			/// Releases references to GridData with the given names so they may be garbage collected.
			/// You may also need to call Resources.UnloadUnusedAssets() for a full unload.
			/// </summary>
			public void ReleaseGridReferences(params string[] gridNames)
			{
				foreach (string gridName in gridNames)
				{
					m_gridData.Remove(gridName);
				}
			}

			/// <summary>
			/// Releases all references to GridData so they may be garbage collected.
			/// You may also need to call Resources.UnloadUnusedAssets() for a full unload.
			/// </summary>
			public void ReleaseAllGridReferences()
			{
				m_gridData.Clear();
			}

			private void AddGrid(GridData data)
			{
				if (data == null)
				{
					Debug.LogError("Attempting to add null grid data.");
					return;
				}

				if (m_gridData.ContainsKey(data.name))
				{
					Debug.LogError(string.Format("Grid -- Found two Grids with the same name while loading: [{0}]. Please ensure all GridData have unique names.", data.name));
				}
				else
				{
					m_gridData.Add(data.name, data);
				}
			}
		}
	}
}