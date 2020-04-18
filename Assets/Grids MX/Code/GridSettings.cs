using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public enum CoordinateDisplay
		{
			Off,
			OrthographicOnly,
			PerspectiveOnly,
			Always
		}

		public enum CoordinateAnchor
		{
			AttachToSelection,
			UpperLeft,
			UpperCenter,
			UpperRight,
			MiddleLeft,
			MiddleRight,
			LowerLeft,
			LowerCenter,
			LowerRight
		}

		public enum ReferenceLineDisplay
		{
			Off,
			MainSelection,
			FullSelection
		}

		public enum ReferenceLineStyle
		{
			SingleColor,
			UseAxisColors
		}

		public enum LayoutDirection
		{
			Vertical,
			Horizontal
		}

		public class GridSettings : SyncedSettings
		{
			public enum VisualStyle
			{
				Off,
				Outline
			}

			public const string RESOURCES_FOLDER_PATH = "GridsMXData";
			public const string RESOURCES_FILE_NAME = "GridSettings";
			public const string RESOURCES_FILE_PATH = RESOURCES_FOLDER_PATH + "/" + RESOURCES_FILE_NAME;

			private const float LINE_FADE_START_DIST = 15f;
			private const float LINE_MAX_DIST = 20f;

			[System.NonSerialized]
			private static GridSettings m_instance;
			public static GridSettings instance
			{
				get
				{
					if (m_instance == null)
					{
						m_instance = Resources.Load<GridSettings>(RESOURCES_FILE_PATH);

						// when called from editor, we should handle null and recreate the asset.
						if (Application.isPlaying && m_instance == null)
						{
							Debug.LogError(string.Format("Grids MX -- ERROR! Missing GridSettings file in folder <Grids MX Root>/Resources/{0}", RESOURCES_FOLDER_PATH));
						}
					}
					return m_instance;
				}
			}

			[Header("Grid Toolbar Options")]
			[SerializeField] private LayoutDirection m_gridWindowLayout = LayoutDirection.Horizontal;
			[SerializeField] private bool m_showDataManagementOptions = true;
			[SerializeField] private bool m_showScrollBars = false;
			[SerializeField] private bool m_showGlobalSnappingOptions = true;
			[SerializeField] private bool m_showXAxisOptions = true;
			[SerializeField] private bool m_showYAxisOptions = true;
			[SerializeField] private bool m_showZAxisOptions = true;
			[SerializeField] private bool m_showGridMovementOptions = true;
			[SerializeField] private bool m_showGridFollowOptions = true;

			// default colors are unity editor axis colors
			[Header("Axis Styles")]
			[SerializeField] private Color m_xAxisColor = new Color(0.859f, 0.243f, 0.114f, 0.25f);
			[SerializeField] private Color m_yAxisColor = new Color(0.604f, 0.953f, 0.242f, 0.25f);
			[SerializeField] private Color m_zAxisColor = new Color(0.227f, 0.478f, 0.973f, 0.25f);

			[Tooltip("Changing to 'Outline' will show an outline of the entire grid, even behind other grid planes."
				+ " This is useful when using a non-standard grid to show the full bounds of the grid.")]
			[SerializeField] private VisualStyle m_unfocusedStyle = VisualStyle.Off;

			[Tooltip("The color of the lines behind other grid planes when the 'Unfocused Style' is set to something other than 'Off'.")]
			[SerializeField] private Color m_unfocusedColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);
			
			[Tooltip("A 'major' line will be displayed every n lines. Recommend spacing of 5 of 10 for better visual clarity.")]
			[SerializeField] private int m_majorLineSpacing = 10;

			[Tooltip("The opacity that 'major' lines are drawn with. Recommend a value at least 0.2 greater than the axis colors' opacity.")]
			[SerializeField] private float m_majorLineOpacity = 0.5f;

			[Header("Global Grid Options")]
			[Tooltip("This option will affect any fields that have a unit abbreviation after them (e.g. 123 m). The actual value"
				+ " will not change, it will be converted to the new unit. This is mostly useful when specifying how large you"
				+ " would like your cells to be. Unity default units are meters.")]
			[SerializeField] private Unit m_measurementUnit = Unit.Meter;

			[Header("Coordinate Display Options")]
			[Tooltip("What type of cameras would you like coordinates to display on? Coordinates show the closest grid point value.")]
			[SerializeField] private CoordinateDisplay m_coordinateDisplay = CoordinateDisplay.Off;

			[Tooltip("Anchors the coordinates to the screen or to the selected object.")]
			[SerializeField] private CoordinateAnchor m_coordinateAnchor = CoordinateAnchor.LowerCenter;
			[SerializeField] private Color m_coordinateColor = Color.grey;
			[SerializeField] private int m_coordinateSize = 15;

			[Tooltip("The components of the coordinate display will display with the color values appropriate to the axis they represent"
				+ "(e.g. default: red for X, green for Y, blue for Z")]
			[SerializeField] private bool m_useAxisColorForComponents = true;

			[Header("Grid Reference Options")]
			[Tooltip("Lines will be displayed across each plane that show the position of the selected object(s).")]
			[SerializeField] private ReferenceLineDisplay m_planeReferenceDisplay = ReferenceLineDisplay.MainSelection;
			[SerializeField] private ReferenceLineStyle m_planeReferenceStyle = ReferenceLineStyle.SingleColor;
			[SerializeField] private Color m_planeReferenceColor = Color.yellow;

			[Tooltip("Lines will be displayed that connect the object to each plane, giving a better sense of its 3D position.")]
			[SerializeField] private ReferenceLineDisplay m_axisReferenceDisplay = ReferenceLineDisplay.MainSelection;
			[SerializeField] private ReferenceLineStyle m_axisReferenceStyle = ReferenceLineStyle.UseAxisColors;
			[SerializeField] private Color m_axisReferenceColor = Color.yellow;

			// grid window
			public LayoutDirection gridWindowLayout { get { return m_gridWindowLayout; } }
			public bool showScrollBars { get { return m_showScrollBars; } }
			public bool showDataManagementOptions { get { return m_showDataManagementOptions; } }
			public bool showGlobalSnappingOptions { get { return m_showGlobalSnappingOptions; } }
			public bool showXAxisOptions { get { return m_showXAxisOptions; } }
			public bool showYAxisOptions { get { return m_showYAxisOptions; } }
			public bool showZAxisOptions { get { return m_showZAxisOptions; } }
			public bool showGridMovementOptions { get { return m_showGridMovementOptions; } }
			public bool showGridFollowOptions { get { return m_showGridFollowOptions; } }

			// grid
			public Color xAxisColor { get { return m_xAxisColor; } }
			public Color yAxisColor { get { return m_yAxisColor; } }
			public Color zAxisColor { get { return m_zAxisColor; } }
			public Color unfocusedColor { get { return m_unfocusedColor; } }
			public VisualStyle unfocusedStyle { get { return m_unfocusedStyle; } }
            public Unit measurementUnit { get { return m_measurementUnit; } }
			public float majorLineOpacity { get { return m_majorLineOpacity; } }
			public int majorLineSpacing { get { return m_majorLineSpacing; } }

			// coordinate display
			public CoordinateDisplay coordinateDisplay { get { return m_coordinateDisplay; } }
			public CoordinateAnchor coordinateAnchor { get { return m_coordinateAnchor; } }
			public Color coordinateColor { get { return m_coordinateColor; } }
			public int coordinateSize { get { return m_coordinateSize; } }
			public bool useAxisColorForComponents { get { return m_useAxisColorForComponents; } }

			// reference lines
			public ReferenceLineDisplay planeReferenceDisplay { get { return m_planeReferenceDisplay; } }
			public ReferenceLineStyle planeReferenceStyle { get { return m_planeReferenceStyle; } }
			public Color planeReferenceColor { get { return m_planeReferenceColor; } }

			public ReferenceLineDisplay axisReferenceDisplay { get { return m_axisReferenceDisplay; } }
			public ReferenceLineStyle axisReferenceStyle { get { return m_axisReferenceStyle; } }
			public Color axisReferenceColor { get { return m_axisReferenceColor; } }
		}
	}
}