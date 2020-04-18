using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace mx
{
	namespace Grids
	{
		public class GridAxisDrawer
		{
			private enum Quadrant
			{
				Q = 1 << 0,
				W = 1 << 1,
				E = 1 << 2,
				R = 1 << 3,

				QW = Q | W,
				QE = Q | E,
				QR = Q | R,
				WE = W | E,
				WR = W | R,
				ER = E | R,

				QWER = Q | W | E | R
			}

			private const float MIN_SIZE = 0.0001f;
			private const float MIN_AXIS_ROTATION = -180f;
			private const float MAX_AXIS_ROTATION = 180f;

			private const string FOLDOUT_OPEN_KEY_FORMAT = "mx_grids_{0}axisfoldoutopen";

			private Axis m_axis;
			private GridData m_gridData;
			private Material m_gridMaterial = null;
			private Dictionary<Quadrant, MeshBuilder> m_quadrantGrids = null;
			private Dictionary<Quadrant, MeshBuilder> m_quadrantOutlines = null;
			private bool m_isFoldoutOpen = true;

			private Axis aAxis { get { return (m_axis == Axis.X ? Axis.Y : (m_axis == Axis.Y ? Axis.X : Axis.X)); } }
			private Axis bAxis { get { return (m_axis == Axis.X ? Axis.Z : (m_axis == Axis.Y ? Axis.Z : Axis.Y)); } }

			public GridAxisDrawer(Axis axis, GridData gridData)
			{
				m_axis = axis;
				m_gridData = gridData;

				ResetGridMaterial();
				ResetGridMesh();

				LoadPrefs();
			}

			public void DrawProperties()
			{
				if ((m_axis == Axis.X && !GridSettings.instance.showXAxisOptions)
					|| (m_axis == Axis.Y && !GridSettings.instance.showYAxisOptions)
					|| (m_axis == Axis.Z && !GridSettings.instance.showZAxisOptions))
				{
					return;
				}

				AxisProperties axisProperties = m_gridData.GetAxisProperties(m_axis);

				EditorGUI.BeginChangeCheck();

				Color originalBGColor = GUI.backgroundColor;
				GUI.backgroundColor = GetAxisColor(m_axis);

				GUIStyle style = new GUIStyle(GUI.skin.box);
				style.normal.background = EditorTextures.GetByName(EditorTextures.BORDER_TEX_NAME);
				style.border = new RectOffset(5, 5, 5, 5);

				if (GridSettings.instance.gridWindowLayout == LayoutDirection.Vertical)
				{
					EditorGUILayout.BeginVertical(style);
				}
				else
				{
					EditorGUILayout.BeginHorizontal(style);
				}
				{
					GUI.backgroundColor = originalBGColor;

					m_isFoldoutOpen = EditorGUILayout.Foldout(m_isFoldoutOpen, m_axis + " Axis");

					if (m_isFoldoutOpen)
					{
						// VISIBLE
						axisProperties.isVisibleInEditor =
							EditorGUIExtensions.IconToggle(EditorTextures.GetTexturePath(EditorTextures.HIDDEN_ICON),
								EditorTextures.GetTexturePath(EditorTextures.VISIBLE_ICON),
								axisProperties.isVisibleInEditor,
								"Hide the " + m_axis + " grid plane",
								"Show the " + m_axis + " grid plane");

						// SNAP
						axisProperties.isSnapEnabled =
							EditorGUIExtensions.IconToggle(EditorTextures.GetTexturePath(EditorTextures.POSITION_SNAP_DISABLED_ICON),
								EditorTextures.GetTexturePath(EditorTextures.POSITION_SNAP_ENABLED_ICON),
								axisProperties.isSnapEnabled,
								"Disable snapping along the " + m_axis + " axis",
								"Enable snapping along the " + m_axis + " axis");

						string unitAbbrev = UnitUtil.Abbreviation(GridSettings.instance.measurementUnit);

						EditorGUILayout.BeginVertical();
						{
							// CELL SIZE
							EditorGUIExtensions.DrawIconPrefixedField(EditorTextures.GetTexturePath(EditorTextures.CELL_SIZE_ICON),
								"Size of cells along the " + m_axis + " axis",
								delegate
								{
									axisProperties.cellSize = (float)EditorUnitUtil.DrawConvertedSizeField(axisProperties.cellSize,
										Unit.Meter, GridSettings.instance.measurementUnit, GridWindow.MIN_FIELD_WIDTH);
									axisProperties.cellSize = Mathf.Max(axisProperties.cellSize, MIN_SIZE);

									EditorGUILayout.LabelField(unitAbbrev, GUILayout.Width(GridWindow.UNIT_LABEL_WIDTH));
								});

							// GRID OFFSET
							EditorGUIExtensions.DrawIconPrefixedField(EditorTextures.GetTexturePath(EditorTextures.GRID_OFFSET_ICON),
								"Offset of the entire grid along the " + m_axis + " axis",
								delegate
								{
									axisProperties.offset = (float)EditorUnitUtil.DrawConvertedSizeField(axisProperties.offset, Unit.Meter,
										GridSettings.instance.measurementUnit, GridWindow.MIN_FIELD_WIDTH);

									EditorGUILayout.LabelField(unitAbbrev, GUILayout.Width(GridWindow.UNIT_LABEL_WIDTH));
								});
						}
						EditorGUILayout.EndVertical();

						// ROTATION
						EditorGUIExtensions.DrawIconPrefixedField(EditorTextures.GetTexturePath(EditorTextures.ROTATION_SNAP_ANGLE_ICON),
							"Rotation of the grid around the " + m_axis + " axis",
							delegate
							{
								axisProperties.rotation = EditorGUILayout.FloatField(axisProperties.rotation,
									GUILayout.MinWidth(GridWindow.MIN_FIELD_WIDTH));
								axisProperties.rotation = Util.Wrap(axisProperties.rotation, MIN_AXIS_ROTATION, MAX_AXIS_ROTATION);

								// degree symbol
								EditorGUILayout.LabelField(('\u00B0').ToString(), GUILayout.Width(GridWindow.UNIT_LABEL_WIDTH));
							});
					}

					GUI.backgroundColor = GetAxisColor(m_axis);
				}
				if (GridSettings.instance.gridWindowLayout == LayoutDirection.Vertical)
				{
					EditorGUILayout.EndVertical();
				}
				else
				{
					EditorGUILayout.EndHorizontal();
				}

				GUI.backgroundColor = originalBGColor;

				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(m_gridData);
					SavePrefs();
                }
			}

			public void ResetGridMesh()
			{
				m_quadrantGrids = new Dictionary<Quadrant, MeshBuilder>(4);
				m_quadrantOutlines = new Dictionary<Quadrant, MeshBuilder>(4);

				CreatePlaneMesh();

				foreach (MeshBuilder builder in m_quadrantGrids.Values)
				{
					builder.Build(HideFlags.HideAndDontSave | HideFlags.HideInInspector, MeshTopology.Lines);
				}

				foreach (MeshBuilder builder in m_quadrantOutlines.Values)
				{
					builder.Build(HideFlags.HideAndDontSave | HideFlags.HideInInspector, MeshTopology.Lines);
				}
			}

			public void DrawPlane(Camera camera)
			{
				if (!m_gridData.GetAxisProperties(m_axis).isVisibleInEditor)
				{
					return;
				}

				Matrix4x4 gridMatrix = m_gridData.GetGridToWorldMatrix();

				float forwardAbsDot = Mathf.Abs(Vector3.Dot(camera.transform.forward, Vector3.forward));
				float rightAbsDot = Mathf.Abs(Vector3.Dot(camera.transform.right, Vector3.right));
				float upAbsDot = Mathf.Abs(Vector3.Dot(camera.transform.up, Vector3.up));
				bool isViewAlongAxis = Mathf.Approximately(forwardAbsDot, 1f)
					|| Mathf.Approximately(rightAbsDot, 1f)
					|| Mathf.Approximately(upAbsDot, 1f);

				Quadrant focused = (camera.orthographic && isViewAlongAxis ? Quadrant.QWER : GetFocusedQuadrants());
				DrawQuadrant(Quadrant.Q, focused, gridMatrix);
				DrawQuadrant(Quadrant.W, focused, gridMatrix);
				DrawQuadrant(Quadrant.E, focused, gridMatrix);
				DrawQuadrant(Quadrant.R, focused, gridMatrix);
			}

			private void DrawQuadrant(Quadrant quadrant, Quadrant focused, Matrix4x4 matrix)
			{
				MeshBuilder builder = null;
				if ((quadrant & focused) != 0)
				{
					m_quadrantGrids.TryGetValue(quadrant, out builder);
					DrawSelectionReferenceLines(quadrant);
				}
				else if (GridSettings.instance.unfocusedStyle == GridSettings.VisualStyle.Outline)
				{
					m_quadrantOutlines.TryGetValue(quadrant, out builder);
				}

				if (builder != null)
				{
					m_gridMaterial.SetPass(0);
					Graphics.DrawMeshNow(builder.mesh, matrix);
				}
			}

			private Quadrant GetFocusedQuadrants()
			{
				float aCameraPos = GetCameraAxisPosition(aAxis);
				float bCameraPos = GetCameraAxisPosition(bAxis);

				bool isAVisible = m_gridData.GetAxisProperties(aAxis).isVisibleInEditor;
				bool isBVisible = m_gridData.GetAxisProperties(bAxis).isVisibleInEditor;

				// focus the quadrant the camera is in for better visual clarity
				Quadrant cameraQuadrant = 0;
				if (aCameraPos < 0 && bCameraPos < 0) cameraQuadrant |= Quadrant.Q;
				else if (aCameraPos > 0 && bCameraPos < 0) cameraQuadrant |= Quadrant.W;
				else if (aCameraPos < 0 && bCameraPos > 0) cameraQuadrant |= Quadrant.E;
				else if (aCameraPos > 0 && bCameraPos > 0) cameraQuadrant |= Quadrant.R;

				// if the origin is all the way to one extent, we show both quadrants anyway, otherwise we get into a state=
				int aOrigin = m_gridData.gridOrigin.GetComponent(aAxis);
				int bOrigin = m_gridData.gridOrigin.GetComponent(bAxis);
				bool isAOriginAtExtents = (aOrigin == 0 || m_gridData.gridSize - aOrigin == 0);
				bool isBOriginAtExtents = (bOrigin == 0 || m_gridData.gridSize - bOrigin == 0);

				// if some axis grids are off, we can include other quadrants as 'focused' since they won't be blocked
				Quadrant focused = 0;
				if ((!isAVisible && !isBVisible) || (isAOriginAtExtents && isBOriginAtExtents))
				{
					focused = Quadrant.QWER;
				}
				else
				{
					focused = cameraQuadrant;
					if (!isAVisible || isAOriginAtExtents)
					{
						if ((cameraQuadrant & Quadrant.QW) != 0)
						{
							focused |= Quadrant.QW;
						}
						else if ((cameraQuadrant & Quadrant.ER) != 0)
						{
							focused |= Quadrant.ER;
						}
					}

					if (!isBVisible || isBOriginAtExtents)
					{
						if ((cameraQuadrant & Quadrant.QE) != 0)
						{
							focused |= Quadrant.QE;
						}
						else if ((cameraQuadrant & Quadrant.WR) != 0)
						{
							focused |= Quadrant.WR;
						}
					}
				}

				return focused;
			}

			private void DrawSelectionReferenceLines(Quadrant quadrant)
			{
				if (Selection.activeTransform == null)
				{
					return;
				}

				Matrix4x4 originalMatrix = Handles.matrix;
				Handles.matrix = m_gridData.GetGridToWorldMatrix();

				if (GridSettings.instance.axisReferenceDisplay == ReferenceLineDisplay.MainSelection)
				{
					DrawSelectionAxisReferenceLines(Selection.activeTransform);
				}

				if (GridSettings.instance.planeReferenceDisplay == ReferenceLineDisplay.MainSelection)
				{
					DrawSelectionPlaneReferenceLines(quadrant, Selection.activeTransform);
				}

				if (GridSettings.instance.axisReferenceDisplay == ReferenceLineDisplay.FullSelection
                    || GridSettings.instance.planeReferenceDisplay == ReferenceLineDisplay.FullSelection)
				{
					foreach (Transform t in Selection.transforms)
					{
						if (GridSettings.instance.axisReferenceDisplay == ReferenceLineDisplay.FullSelection)
						{
							DrawSelectionAxisReferenceLines(t);
						}
						if (GridSettings.instance.planeReferenceDisplay == ReferenceLineDisplay.FullSelection)
						{
							DrawSelectionPlaneReferenceLines(quadrant, t);
						}
					}
				}

				Handles.matrix = originalMatrix;
			}

			private void DrawSelectionPlaneReferenceLines(Quadrant quadrant, Transform transform)
			{
				Color originalColor = Handles.color;
				Color lineColor = GridSettings.instance.planeReferenceStyle == ReferenceLineStyle.SingleColor ?
					GridSettings.instance.planeReferenceColor : GetAxisColor(m_axis);
				lineColor.a = 1f;
				Handles.color = lineColor;

				Vector3 gridPosition = m_gridData.WorldToGridPosition(transform.position);

				float aStart = ((quadrant & Quadrant.QE) != 0) ? -m_gridData.gridOrigin.GetComponent(aAxis) : 0f;
				float bStart = ((quadrant & Quadrant.QW) != 0) ? -m_gridData.gridOrigin.GetComponent(bAxis) : 0f;
				float aEnd = ((quadrant & Quadrant.QE) != 0) ? 0f : m_gridData.gridSize - m_gridData.gridOrigin.GetComponent(aAxis);
				float bEnd = ((quadrant & Quadrant.QW) != 0) ? 0f : m_gridData.gridSize - m_gridData.gridOrigin.GetComponent(bAxis);

				Vector3 start = gridPosition.GetComponentVector(m_axis) + AxisUtil.GetVector(aAxis) * aStart;
				Vector3 end = gridPosition.GetComponentVector(m_axis) + AxisUtil.GetVector(aAxis) * aEnd;
				Handles.DrawLine(start, end);

				start = gridPosition.GetComponentVector(m_axis) + AxisUtil.GetVector(bAxis) * bStart;
				end = gridPosition.GetComponentVector(m_axis) + AxisUtil.GetVector(bAxis) * bEnd;
				Handles.DrawLine(start, end);

				Handles.color = originalColor;
			}

			private void DrawSelectionAxisReferenceLines(Transform transform)
			{
				Color originalColor = Handles.color;
				Color lineColor = GridSettings.instance.axisReferenceStyle == ReferenceLineStyle.SingleColor ?
					GridSettings.instance.axisReferenceColor : GetAxisColor(m_axis);
				lineColor.a = 1f;
				Handles.color = lineColor;

				Vector3 gridPosition = m_gridData.WorldToGridPosition(transform.position);
				Vector3 planePosition = Vector3.Scale(gridPosition, Vector3.one - AxisUtil.GetVector(m_axis));
				
				if (!Mathf.Approximately(gridPosition.GetComponent(aAxis), 0f)
					&& !Mathf.Approximately(gridPosition.GetComponent(bAxis), 0f))
				{
					Handles.DrawLine(gridPosition, planePosition);
				}
				
				Handles.color = originalColor;
			}

			private void CreatePlaneMesh()
			{
				// Create the grid plane as a combination of 4 quadrants. This allows us to draw them differently
				// depending on various factors e.g. camera position.

				// Each quadrant can be defined by a min and a max point. Each quadrant is labeled below for reference.
				// 
				// min ----------------- a axis
				//     |   Q   |   W   |	
				//     -----------------    <- center point is grid origin (0, 0, 0)
				//     |   E   |   R   |	
				//     -----------------
				// b axis			   max
				
                GridPoint aAxisOriginOffset = -1 * GridPoint.Scale(m_gridData.gridOrigin, AxisUtil.GetVector(aAxis));
				GridPoint bAxisOriginOffset = -1 * GridPoint.Scale(m_gridData.gridOrigin, AxisUtil.GetVector(bAxis));
				
                GridPoint aAxisMaxOffset = m_gridData.GridPositionToGridPoint(AxisUtil.GetVector(aAxis) * m_gridData.gridSize) + aAxisOriginOffset;
				GridPoint bAxisMaxOffset = m_gridData.GridPositionToGridPoint(AxisUtil.GetVector(bAxis) * m_gridData.gridSize) + bAxisOriginOffset;
				
				GridPoint qMin = aAxisOriginOffset + bAxisOriginOffset;
				GridPoint wMin = bAxisOriginOffset;
				GridPoint eMin = aAxisOriginOffset;
				GridPoint rMin = GridPoint.zero;
				GridPoint qMax = rMin;
				GridPoint wMax = aAxisMaxOffset;
				GridPoint eMax = bAxisMaxOffset;
				GridPoint rMax = aAxisMaxOffset + bAxisMaxOffset;
				
				CreateMeshQuadrant(Quadrant.Q, qMin, qMax, aAxis, bAxis);
                CreateMeshQuadrant(Quadrant.W, wMin, wMax, aAxis, bAxis);
                CreateMeshQuadrant(Quadrant.E, eMin, eMax, aAxis, bAxis);
				CreateMeshQuadrant(Quadrant.R, rMin, rMax, aAxis, bAxis);
			}

			private void CreateMeshQuadrant(Quadrant quadrant, GridPoint minGridPoint, GridPoint maxGridPoint, Axis aAxis, Axis bAxis)
			{
				if ((minGridPoint.x == maxGridPoint.x && m_axis != Axis.X)
					|| (minGridPoint.y == maxGridPoint.y && m_axis != Axis.Y)
					|| (minGridPoint.z == maxGridPoint.z && m_axis != Axis.Z))
				{
					// one or more dimensions has 0 area, nothing to draw!
					// note that components for this axis should be the same since that's the level we're drawing the plane at.
					return;
				}

				Color minorColor;
				Color majorColor;

				int aMin = minGridPoint.GetComponent(aAxis);
				int bMin = minGridPoint.GetComponent(bAxis);
				int aMax = maxGridPoint.GetComponent(aAxis);
				int bMax = maxGridPoint.GetComponent(bAxis);
				
				minorColor = GetAxisColor(m_axis);
				majorColor = minorColor;
				majorColor.a = GridSettings.instance.majorLineOpacity;
				
				GridPoint startA = GridPoint.Scale(minGridPoint, AxisUtil.GetVector(aAxis));
				GridPoint startB = GridPoint.Scale(minGridPoint, AxisUtil.GetVector(bAxis));
				GridPoint endA = GridPoint.Scale(maxGridPoint, AxisUtil.GetVector(aAxis));
				GridPoint endB = GridPoint.Scale(maxGridPoint, AxisUtil.GetVector(bAxis));

				MeshBuilder gridBuilder = new MeshBuilder();
				
				Vector3 aAxisVector = AxisUtil.GetVector(aAxis);
				Vector3 bAxisVector = AxisUtil.GetVector(bAxis);
				
                int line = aMin;
				for (int a = aMin; a < aMax + 1; ++a)
				{
					Color color = (line % GridSettings.instance.majorLineSpacing) == 0 ? majorColor : minorColor;
					++line;

					GridPoint aPoint = m_gridData.GridPositionToGridPoint(a * aAxisVector);
					Vector3 start = m_gridData.GridPointToGridPosition(aPoint + startB);
					Vector3 end = m_gridData.GridPointToGridPosition(aPoint + endB);

					gridBuilder.AddVertex(start, color);
					gridBuilder.AddVertex(end, color);
					gridBuilder.AddIndex(gridBuilder.vertexCount - 2);
					gridBuilder.AddIndex(gridBuilder.vertexCount - 1);
				}

				line = bMin;
				for (int b = bMin; b < bMax + 1; ++b)
				{
					Color color = (line % GridSettings.instance.majorLineSpacing) == 0 ? majorColor : minorColor;
					++line;

					GridPoint bPoint = m_gridData.GridPositionToGridPoint(b * bAxisVector);
					Vector3 start = m_gridData.GridPointToGridPosition(bPoint + startA);
					Vector3 end = m_gridData.GridPointToGridPosition(bPoint + endA);
						
					gridBuilder.AddVertex(start, color);
					gridBuilder.AddVertex(end, color);
					gridBuilder.AddIndex(gridBuilder.vertexCount - 2);
					gridBuilder.AddIndex(gridBuilder.vertexCount - 1);
				}

				m_quadrantGrids.Add(quadrant, gridBuilder);


				// add outlines
				{
					MeshBuilder outlineBuilder = new MeshBuilder();

					Color color = GridSettings.instance.unfocusedColor;

					Vector3 a = m_gridData.GridPointToGridPosition(startA + startB);
					Vector3 b = m_gridData.GridPointToGridPosition(startA + endB);
					Vector3 c = m_gridData.GridPointToGridPosition(endA + endB);
					Vector3 d = m_gridData.GridPointToGridPosition(endA + startB);

					outlineBuilder.AddVertex(a, color);
					outlineBuilder.AddVertex(b, color);
					outlineBuilder.AddVertex(c, color);
					outlineBuilder.AddVertex(d, color);

					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 4);
					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 3);

					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 3);
					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 2);

					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 2);
					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 1);

					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 1);
					outlineBuilder.AddIndex(outlineBuilder.vertexCount - 4);

					m_quadrantOutlines.Add(quadrant, outlineBuilder);
				}
			}

			private void ResetGridMaterial()
			{
				Shader gridShader = Shader.Find("Hidden/GridShader");
				if (gridShader != null)
				{
					m_gridMaterial = new Material(gridShader);
					m_gridMaterial.hideFlags = (HideFlags.HideAndDontSave | HideFlags.HideInInspector);
				}
				else
				{
					Debug.LogError("Unable to find grid shader. You may need to"
						+ " reimport the file from the asset store package.");
					return;
				}
			}

			private Color GetAxisColor(Axis axis)
			{
				switch (axis)
				{
					case Axis.X:
						return GridSettings.instance.xAxisColor;
					case Axis.Y:
						return GridSettings.instance.yAxisColor;
					case Axis.Z:
						return GridSettings.instance.zAxisColor;
					default:
						Debug.LogError(string.Format("Unknown axis [{0}].", m_axis));
						return Color.clear;
				}
			}

			private float GetCameraAxisPosition(Axis axis)
			{
				Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
				cameraPosition = m_gridData.WorldToGridPosition(cameraPosition);
                return cameraPosition.GetComponent(axis);
			}

			private void SavePrefs()
			{
				EditorPrefs.SetBool(string.Format(FOLDOUT_OPEN_KEY_FORMAT, m_axis), m_isFoldoutOpen);
			}

			private void LoadPrefs()
			{
				m_isFoldoutOpen = EditorPrefs.GetBool(string.Format(FOLDOUT_OPEN_KEY_FORMAT, m_axis), true);
			}
		}
	}
}