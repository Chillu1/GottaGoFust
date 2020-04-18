using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public static class CoordinateWindow
		{
			private const string COORDINATE_LABEL_FORMAT = "<size={6}><color={7}><b>"
													+ "[<color={3}>{0}</color>,"
													+ " <color={4}>{1}</color>,"
													+ " <color={5}>{2}</color>]</b></color></size>";

			public static void Draw(SceneView sceneView, GridData gridData)
			{
				CoordinateDisplay coordinateStyle = GridSettings.instance.coordinateDisplay;
				if (coordinateStyle == CoordinateDisplay.Off)
				{
					return;
				}

				if (sceneView.camera.orthographic && coordinateStyle == CoordinateDisplay.PerspectiveOnly)
				{
					return;
				}

				if (!sceneView.camera.orthographic && coordinateStyle == CoordinateDisplay.OrthographicOnly)
				{
					return;
				}

				if (Selection.activeTransform != null)
				{
					GUIStyle style = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game).box)
					{
						richText = true,
						padding = new RectOffset(0, 0, 0, 0),
						margin = new RectOffset(0, 0, 0, 0)
					};

					string coordinateColor = GetHexRGB(GridSettings.instance.coordinateColor);
					bool useComponentColors = GridSettings.instance.useAxisColorForComponents;
					GridPoint gridPoint = gridData.WorldPositionToGridPoint(Selection.activeTransform.position);
					string label = string.Format(COORDINATE_LABEL_FORMAT,
						gridPoint.x, gridPoint.y, gridPoint.z,
						(useComponentColors ? GetHexRGB(GridSettings.instance.xAxisColor) : coordinateColor),
						(useComponentColors ? GetHexRGB(GridSettings.instance.yAxisColor) : coordinateColor),
						(useComponentColors ? GetHexRGB(GridSettings.instance.zAxisColor) : coordinateColor),
						GridSettings.instance.coordinateSize,
						coordinateColor);

					CoordinateAnchor coordinateAnchor = GridSettings.instance.coordinateAnchor;
					float size = GridSettings.instance.coordinateSize;
					Handles.BeginGUI();
					if (coordinateAnchor == CoordinateAnchor.AttachToSelection)
					{
						style.alignment = TextAnchor.UpperLeft;
						style.padding = new RectOffset(5, 0, 5, 0);
						style.margin = new RectOffset(5, 0, 5, 0);
						Handles.Label(Selection.activeTransform.position, label, style);
					}
					else
					{
						style.alignment = TextAnchor.MiddleCenter;

						int numCharacters = (gridPoint.x.ToString().Length + gridPoint.y.ToString().Length + gridPoint.z.ToString().Length);

						// arbitrary multipliers that look good
						float width = size * 6f + (numCharacters - 3) * size * 0.75f;
						float height = size * 1.8f;
						Rect rect = new Rect(0f, 0f, width, height);
						float padding = 10f;
						float paddingCenter = (sceneView.position.width - width) * 0.5f;
						float paddingMiddle = (sceneView.position.height - height) * 0.5f;
						float paddingRight = sceneView.position.width - padding - width;
						float paddingLower = sceneView.position.height - padding - height - 14f;

						switch (coordinateAnchor)
						{
							case CoordinateAnchor.UpperLeft:
								rect.x = padding;
								rect.y = padding;
								break;
							case CoordinateAnchor.MiddleLeft:
								rect.x = padding;
								rect.y = paddingMiddle;
								break;
							case CoordinateAnchor.LowerLeft:
								rect.x = padding;
								rect.y = paddingLower;
								break;
							case CoordinateAnchor.UpperCenter:
								rect.x = paddingCenter;
								rect.y = padding;
								break;
							case CoordinateAnchor.LowerCenter:
								rect.x = paddingCenter;
								rect.y = paddingLower;
								break;
							case CoordinateAnchor.UpperRight:
								rect.x = paddingRight;
								rect.y = padding;
								break;
							case CoordinateAnchor.MiddleRight:
								rect.x = paddingRight;
								rect.y = paddingMiddle;
								break;
							case CoordinateAnchor.LowerRight:
								rect.x = paddingRight;
								rect.y = paddingLower;
								break;
							default:
								Debug.LogError(string.Format("Grids MX -- Unsupported Coordinate Anchor: " + coordinateAnchor));
								break;
						}

						GUI.Label(rect, label, style);
					}
					Handles.EndGUI();
				}
			}

			private static string GetHexRGB(Color c)
			{
				return string.Format("#{0:X2}{1:X2}{2:X2}",
					(int)(c.r * 255),
					(int)(c.g * 255),
					(int)(c.b * 255));
			}
		}
	}
}