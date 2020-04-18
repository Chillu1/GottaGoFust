using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public class GridWindow : EditorWindow
		{
			private static GridWindow s_window = null;

			private const int CONTROL_ID_HASH = 82484683; // totally arbitrary.

			private const string SELECTED_GRID_DATA_KEY = "mx_grids_gridwindow_selectedgriddata";
			private const string GRID_VISIBLE_KEY = "mx_grids_gridvisible";
			private const string POSITION_SNAP_ENABLED_KEY = "mx_grids_positionsnapenabled";
			private const string ROTATION_SNAP_ENABLED_KEY = "mx_grids_rotationsnapenabled";
			private const string POSITION_FOLLOW_KEY = "mx_grids_followposition";
			private const string ROTATION_FOLLOW_KEY = "mx_grids_followrotation";

			private const string GRID_DATA_CHANGE_UNDO_NAME = "Grid Properties Modification";
			private const string DEFAULT_GRID_DATA_NAME = "DefaultGridData";
            private const string DEFAULT_GRID_DATA_PATH = GridManager.GRID_DATA_RESOURCES_PATH + "/" + DEFAULT_GRID_DATA_NAME;
			private const float MIN_POSITION_DELTA_FOR_SNAP = 0.0001f;
			private const float MIN_ROTATION_DELTA_FOR_SNAP = 0.0001f;
			private const float MIN_ROTATION_SNAP_ANGLE = 0.1f;
			private const float MAX_ROTATION_SNAP_ANGLE = 180f;
			private const int MIN_GRID_SIZE = 1;
			private const int MAX_GRID_SIZE = 16250; // Based on mesh constraints: 65,000 (max verts) / 4 = 16,250

			private const string VISIBILTY_TOGGLE_KEYBINDING_ID = "visibility_toggle";
            private const string POSITION_ENABLED_KEYBINDING_ID = "position_enabled";
			private const string ROTATION_ENABLED_KEYBINDING_ID = "rotation_enabled";
            private const string MOVE_TO_SELECTED_KEYBINDING_ID = "move_to_selected";
            private const string ROTATE_TO_SELECTED_KEYBINDING_ID = "rotate_to_selected";
            private const string FOLLOW_POSITION_KEYBINDING_ID = "follow_position";
            private const string FOLLOW_ROTATION_KEYBINDING_ID = "follow_rotation";
			
			private const float MIN_WINDOW_WIDTH = 85f;
			private const float MIN_WINDOW_HEIGHT = 46f;
			public const float MIN_FIELD_WIDTH = 30f;
			public const float UNIT_LABEL_WIDTH = 24f;
			public const float BUTTON_HEIGHT = 28f;

			private GridData m_gridData = null;
			private bool m_isGridVisible = true;
			private bool m_isPositionSnapEnabled = true;
			private bool m_isRotationSnapEnabled = false;
			private bool m_isFollowingPosition = false;
			private bool m_isFollowingRotation = false;
			private Vector3 m_followOffset;
			private List<GridAxisDrawer> m_planeDrawers;
			private Transform m_activeSelectionLastFrame;
			private Vector3 m_activeSelectionPositionLastFrame;
			private Vector3 m_toolPosition;
			private Quaternion m_toolRotation;
			private Tool m_cachedTool = Tool.None;
			private Vector2 m_scrollViewPosition;

			#region Window
			private void OnFocus()
			{
				SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
				SceneView.onSceneGUIDelegate += this.OnSceneGUI;

				EditorApplication.update -= this.OnEditorUpdate;
				EditorApplication.update += this.OnEditorUpdate;
			}

			private void OnDestroy()
			{
				SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
				EditorApplication.update -= this.OnEditorUpdate;
			}

			private void OnEditorUpdate()
			{
				if (Selection.activeObject != null)
				{
					GridData selectedGridData = Selection.activeObject as GridData;
					if (selectedGridData != null && selectedGridData != m_gridData)
					{
						SelectGridData(selectedGridData);
						this.Repaint();
                    }
				}
			}
			#endregion

			#region GUI
			private void OnGUI()
			{
				ResetReferences();

				Undo.RecordObject(m_gridData, GRID_DATA_CHANGE_UNDO_NAME);

				EditorGUI.BeginChangeCheck();
				
				KeybindingManager.instance.UpdateKeybindings();

				if (GridSettings.instance.showScrollBars)
				{
					m_scrollViewPosition = EditorGUILayout.BeginScrollView(m_scrollViewPosition);
				}

				if (GridSettings.instance.gridWindowLayout == LayoutDirection.Vertical)
				{
					DrawVerticalLayout();
				}
				else
				{
					DrawHorizontalLayout();
				}

				if (GridSettings.instance.showScrollBars)
				{
					EditorGUILayout.EndScrollView();
				}

				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(m_gridData);

					// force repaint for grid updates
					SceneView.RepaintAll();

					SavePrefs();
				}
			}

			private void DrawVerticalLayout()
			{
				EditorGUILayout.BeginVertical();
				{
                    if (GUILayout.Button(new GUIContent(EditorTextures.GetByName(EditorTextures.SETTINGS_ICON)), GUILayout.Height(UNIT_LABEL_WIDTH)))
					{
						Selection.activeObject = GridSettings.instance;
					}

					DrawBarItems();
				}
				EditorGUILayout.EndVertical();
			}

			private void DrawHorizontalLayout()
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(new GUIContent(EditorTextures.GetByName(EditorTextures.SETTINGS_ICON)), GUILayout.Width(UNIT_LABEL_WIDTH)))
					{
						Selection.activeObject = GridSettings.instance;
					}

					DrawBarItems();
				}
				EditorGUILayout.EndHorizontal();
			}

			private void DrawBarItems()
			{
				if (GridSettings.instance.showDataManagementOptions)
				{
					DrawGridField();
					EditorGUILayout.Space();
				}

				if (GridSettings.instance.showGlobalSnappingOptions)
				{
					DrawGlobalOptions();
				}

				DrawAxes();
				EditorGUILayout.Space();

				if (GridSettings.instance.showGridMovementOptions)
				{
					DrawGridMoveButtons();
					EditorGUILayout.Space();
				}
				
				if (GridSettings.instance.showGridFollowOptions)
				{
					DrawFollowToggles();
				}
            }

			private void DrawGlobalOptions()
			{
				string visibilityDesc = KeybindingManager.instance.GetKeybindingDescription(VISIBILTY_TOGGLE_KEYBINDING_ID);
				m_isGridVisible = EditorGUIExtensions.IconToggle(EditorTextures.GetTexturePath(EditorTextures.HIDDEN_ICON), EditorTextures.GetTexturePath(EditorTextures.VISIBLE_ICON),
					m_isGridVisible, "Hide grid " + visibilityDesc, "Show grid " + visibilityDesc);

				string posSnapDesc = KeybindingManager.instance.GetKeybindingDescription(POSITION_ENABLED_KEYBINDING_ID);
                m_isPositionSnapEnabled = EditorGUIExtensions.IconToggle(EditorTextures.GetTexturePath(EditorTextures.POSITION_SNAP_DISABLED_ICON),
					EditorTextures.GetTexturePath(EditorTextures.POSITION_SNAP_ENABLED_ICON), m_isPositionSnapEnabled,
					"Disable position snapping " + posSnapDesc, "Enable position snapping " + posSnapDesc);
				
				string rotSnapDesc = KeybindingManager.instance.GetKeybindingDescription(ROTATION_ENABLED_KEYBINDING_ID);
				m_isRotationSnapEnabled = EditorGUIExtensions.IconToggle(EditorTextures.GetTexturePath(EditorTextures.ROTATION_SNAP_DISABLED_ICON),
					EditorTextures.GetTexturePath(EditorTextures.ROTATION_SNAP_ENABLED_ICON), m_isRotationSnapEnabled,
					"Disable rotation snapping " + rotSnapDesc, "Enable rotation snapping " + rotSnapDesc);

				EditorGUILayout.BeginVertical();
				{
					EditorGUI.BeginDisabledGroup(!m_isRotationSnapEnabled);
					{
						EditorGUIExtensions.DrawIconPrefixedField(EditorTextures.GetTexturePath(EditorTextures.ROTATION_SNAP_ANGLE_ICON),
							"The angle in degrees used when rotation snapping is enabled",
							delegate
							{
								m_gridData.rotationSnapAngle = EditorGUILayout.FloatField(m_gridData.rotationSnapAngle,
									GUILayout.MinWidth(MIN_FIELD_WIDTH));
								m_gridData.rotationSnapAngle =
									Mathf.Clamp(m_gridData.rotationSnapAngle, MIN_ROTATION_SNAP_ANGLE, MAX_ROTATION_SNAP_ANGLE);

								// degree symbol
								EditorGUILayout.LabelField(('\u00B0').ToString(), GUILayout.Width(UNIT_LABEL_WIDTH));
							});
					}
					EditorGUI.EndDisabledGroup();

					EditorGUI.BeginChangeCheck();
					{
						string tooltip = (m_gridData.edgeType == GridData.EdgeType.Infinite
							? "Display distance as number of grid cells"
							: "Grid size (number of cells)");
						EditorGUIExtensions.DrawIconPrefixedField(EditorTextures.GetTexturePath(EditorTextures.GRIDSIZE_ICON), tooltip,
						delegate
						{
							m_gridData.gridSize = EditorGUILayout.IntField(m_gridData.gridSize, GUILayout.MinWidth(MIN_FIELD_WIDTH));
							m_gridData.gridSize = Mathf.Clamp(m_gridData.gridSize, MIN_GRID_SIZE, MAX_GRID_SIZE);
							EditorGUILayout.LabelField("u", GUILayout.Width(UNIT_LABEL_WIDTH));
						});
					}
					if (EditorGUI.EndChangeCheck())
					{
						GridWindow.ResetSceneGrid();
					}
				}
				EditorGUILayout.EndVertical();
			}

			private void DrawAxes()
			{
				foreach (GridAxisDrawer drawer in m_planeDrawers)
				{
					drawer.DrawProperties();
				}
			}

			private void DrawGridMoveButtons()
			{
				EditorGUI.BeginDisabledGroup(Selection.activeTransform == null);
				{
					if (GUILayout.Button(new GUIContent(EditorTextures.GetByName(EditorTextures.GRID_TO_SELECTION_POSITION_ICON),
						"Moves the entire grid to match the position of the active selected object "
						+ KeybindingManager.instance.GetKeybindingDescription(MOVE_TO_SELECTED_KEYBINDING_ID)),
						GUILayout.Height(BUTTON_HEIGHT)))
					{
						MoveGridToSelected();
                    }
					if (GUILayout.Button(new GUIContent(EditorTextures.GetByName(EditorTextures.GRID_TO_SELECTION_ROTATION_ICON),
						"Rotates the entire grid to match the rotation of the active selected object "
						+ KeybindingManager.instance.GetKeybindingDescription(ROTATE_TO_SELECTED_KEYBINDING_ID)),
						GUILayout.Height(BUTTON_HEIGHT)))
					{
						RotateGridToSelected();
					}
				}
				EditorGUI.EndDisabledGroup();
			}

			private void DrawFollowToggles()
			{
				EditorGUI.BeginChangeCheck();
					
				string followPosDesc = KeybindingManager.instance.GetKeybindingDescription(FOLLOW_POSITION_KEYBINDING_ID);
				m_isFollowingPosition = EditorGUIExtensions.IconToggle(
					EditorTextures.GetTexturePath(EditorTextures.GRID_TO_SELECTION_POSITION_UNLOCKED_ICON),
					EditorTextures.GetTexturePath(EditorTextures.GRID_TO_SELECTION_POSITION_LOCKED_ICON),
					m_isFollowingPosition,
					"Stop grid from moving when the selected object is moved " + followPosDesc,
					"Set grid to move when the selected object is moved " + followPosDesc);
					
				if (EditorGUI.EndChangeCheck() && m_isFollowingPosition)
				{
					ResetFollowOffset();
                }

				string followRotDesc = KeybindingManager.instance.GetKeybindingDescription(FOLLOW_ROTATION_KEYBINDING_ID);
				m_isFollowingRotation = EditorGUIExtensions.IconToggle(
					EditorTextures.GetTexturePath(EditorTextures.GRID_TO_SELECTION_ROTATION_UNLOCKED_ICON),
					EditorTextures.GetTexturePath(EditorTextures.GRID_TO_SELECTION_ROTATION_LOCKED_ICON),
					m_isFollowingRotation,
					"Stop grid from rotating when the selected object is rotated " + followRotDesc,
					"Set grid to rotate when the selected object is rotated " + followRotDesc);
			}

			private void DrawGridField()
			{
				EditorGUI.BeginChangeCheck();
				m_gridData = EditorGUILayout.ObjectField(m_gridData, typeof(GridData), false) as GridData;

				GUIContent addNewGridDataGUIContent = new GUIContent(EditorTextures.GetByName(EditorTextures.ADD_NEW_ICON),
					"Create new GridData with default settings");
				if (GUILayout.Button(addNewGridDataGUIContent, GUILayout.Height(BUTTON_HEIGHT)))
				{
					m_gridData = CreateNewGridData("New GridData");
					Selection.activeObject = m_gridData;
				}

				GUIContent duplicateGridDataGUIContent = new GUIContent(EditorTextures.GetByName(EditorTextures.DUPLICATE_ICON),
					"Duplicate the currently selected GridData");
				if (GUILayout.Button(duplicateGridDataGUIContent, GUILayout.Height(BUTTON_HEIGHT)))
				{
					string path = AssetDatabase.GetAssetPath(m_gridData);
					string copyPath = AssetDatabase.GenerateUniqueAssetPath(path.Replace(".asset", string.Empty) + " Copy.asset");
					AssetDatabase.CopyAsset(path, copyPath);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();

					m_gridData = AssetDatabase.LoadAssetAtPath(copyPath, typeof(GridData)) as GridData;
					Selection.activeObject = m_gridData;
				}

				if (EditorGUI.EndChangeCheck())
				{
					SelectGridData(m_gridData);
                }
			}
			#endregion

			#region SceneGUI
			private void OnSceneGUI(SceneView sceneView)
			{
				ResetReferences();

				KeybindingManager.instance.UpdateKeybindings();

				if (Event.current.type == EventType.Repaint)
				{
					DrawSceneGrid(sceneView);
					CoordinateWindow.Draw(sceneView, m_gridData);
                }

				if (DrawCustomTools())
				{
					UpdateSnapping();
				}

				if (Selection.activeTransform != null)
				{
					if (m_isFollowingPosition)
					{
						if (m_activeSelectionLastFrame != Selection.activeTransform)
						{
							ResetFollowOffset();
						}
						MoveGridToPosition(Selection.activeTransform.position + m_followOffset);
					}

					if (m_isFollowingRotation)
					{
						RotateGridTo(Selection.activeTransform.rotation);
					}
				}

				if (Selection.activeTransform != m_activeSelectionLastFrame)
				{
					this.Repaint();
				}

				m_activeSelectionLastFrame = Selection.activeTransform;
				if (Selection.activeTransform != null)
				{
					m_activeSelectionPositionLastFrame = Selection.activeTransform.position;
				}
			}

			private void DrawSceneGrid(SceneView sceneView)
			{
				if (m_isGridVisible && GridSettings.instance != null && m_planeDrawers != null)
				{
					foreach (GridAxisDrawer planeDrawer in m_planeDrawers)
					{
						planeDrawer.DrawPlane(sceneView.camera);
					}
                }
			}

			private bool DrawCustomTools()
			{
				if (Event.current.rawType == EventType.MouseDown && Selection.activeTransform != null)
				{
					m_toolRotation = Selection.activeTransform.localRotation;
				}

				bool isDragging = (Event.current.type == EventType.MouseDrag);

				if ((m_isRotationSnapEnabled && Tools.current == Tool.Rotate)
					|| (m_isPositionSnapEnabled && Tools.current == Tool.Move))
				{
					m_cachedTool = Tools.current;
					Tools.current = Tool.None;
					if (Selection.activeTransform != null)
					{
						m_toolPosition = Selection.activeTransform.position;
						m_toolRotation = Selection.activeTransform.localRotation;
					}
				}

				if (Tools.current == Tool.None && Selection.activeTransform != null)
				{
					if (m_activeSelectionLastFrame != Selection.activeTransform)
					{
						m_toolPosition = Selection.activeTransform.position;
						m_toolRotation = Selection.activeTransform.localRotation;
					}
					switch (m_cachedTool)
					{
						case Tool.Rotate:
							{
								m_toolRotation = Handles.RotationHandle(m_toolRotation, Selection.activeTransform.position);
							}
							break;
						case Tool.Move:
							{
								Quaternion gridRotation = m_gridData.GetGridRotation();

								Transform activeTransform = Selection.activeTransform;
								m_toolPosition = activeTransform.position;
                                Vector3 delta = Handles.PositionHandle(m_toolPosition, gridRotation) - activeTransform.position;

								foreach (Transform transform in Selection.transforms)
								{
									transform.position += delta;
								}
								
								float rectOffset = HandleUtility.GetHandleSize(m_toolPosition) * 1.1f;
								float rectSize = HandleUtility.GetHandleSize(m_toolPosition) * 0.1f;

								Vector3 position = m_toolPosition + gridRotation * Vector3.forward * rectOffset;
                                Quaternion rotation = gridRotation;
                                Handles.color = Handles.zAxisColor;
								Handles.RectangleHandleCap(-1, position, rotation, rectSize, EventType.Repaint);

								position = m_toolPosition + gridRotation * Vector3.right * rectOffset;
								rotation = gridRotation * Quaternion.LookRotation(Vector3.right);
								Handles.color = Handles.xAxisColor;
                                Handles.RectangleHandleCap(-1, position, rotation, rectSize, EventType.Repaint);

								position = m_toolPosition + gridRotation * Vector3.up * rectOffset;
								rotation = gridRotation * Quaternion.LookRotation(Vector3.up);
								Handles.color = Handles.yAxisColor;
                                Handles.RectangleHandleCap(-1, position, rotation, rectSize, EventType.Repaint);
							}
							break;
						default:
							// nothin
							break;
					}
				}

				if ((m_cachedTool == Tool.Move && !m_isPositionSnapEnabled)
                    || (m_cachedTool == Tool.Rotate && !m_isRotationSnapEnabled))
				{
					Tools.current = m_cachedTool;
					m_cachedTool = Tool.None;
				}

				return isDragging;
			}

			private void UpdateSnapping()
			{
				if (m_gridData == null || Selection.activeTransform == null
					|| Selection.activeTransform != m_activeSelectionLastFrame)
				{
					return;
				}

				if (m_isPositionSnapEnabled)
				{
					// snap position
					float xDelta = Mathf.Abs(Selection.activeTransform.position.x - m_activeSelectionPositionLastFrame.x);
					if (xDelta > MIN_POSITION_DELTA_FOR_SNAP)
					{
						this.SnapAllToGridPosition(Selection.transforms, Axis.X, false);
					}

					float yDelta = Mathf.Abs(Selection.activeTransform.position.y - m_activeSelectionPositionLastFrame.y);
					if (yDelta > MIN_POSITION_DELTA_FOR_SNAP)
					{
						this.SnapAllToGridPosition(Selection.transforms, Axis.Y, false);
					}

					float zDelta = Mathf.Abs(Selection.activeTransform.position.z - m_activeSelectionPositionLastFrame.z);
					if (zDelta > MIN_POSITION_DELTA_FOR_SNAP)
					{
						this.SnapAllToGridPosition(Selection.transforms, Axis.Z, false);
					}
				}
				
				if (m_isRotationSnapEnabled && m_cachedTool == Tool.Rotate)
				{
					// snap rotation
					float angleDelta = Quaternion.Angle(Selection.activeTransform.localRotation, m_toolRotation);
                    if (angleDelta > MIN_ROTATION_DELTA_FOR_SNAP)
					{
						this.SnapAllToRotation(Selection.transforms);
                    }
				}
			}

			private void SnapAllToGridPosition(Transform[] transforms, Axis axis, bool ignoreDisabled)
			{
				Undo.RecordObjects(transforms, "Grids MX: Snap Position");
				foreach (Transform t in transforms)
				{
					t.position = m_gridData.SnapPosition(t.position, axis, ignoreDisabled);
					EditorUtility.SetDirty(t);
				}
			}

			private void SnapAllToGridPosition(Transform[] transforms)
			{
				Undo.RecordObjects(transforms, "Grids MX: Snap Position");
				foreach (Transform t in transforms)
				{
					t.position = m_gridData.SnapPosition(t.position, true);
					EditorUtility.SetDirty(t);
				}
			}

			private void SnapAllToRotation(Transform[] transforms)
			{
				Undo.RecordObjects(transforms, "Grids MX: Snap Rotation");
				foreach (Transform t in transforms)
				{
					t.localRotation = m_gridData.SnapRotation(t.localRotation, m_toolRotation);
					EditorUtility.SetDirty(t);
				}
			}
			#endregion

			#region Data Management
			private GridData CreateNewGridData(string name)
			{
				GridData newGridData = GridData.CreateInstance<GridData>();
				string path = EditorUtil.ROOT_FOLDER + "/Resources/" + GridManager.GRID_DATA_RESOURCES_PATH + "/" + name + ".asset";
				path = AssetDatabase.GenerateUniqueAssetPath(path);
				AssetDatabase.CreateAsset(newGridData, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				return newGridData;
			}

			private void SelectGridData(GridData data)
			{
				m_gridData = data;

				EditorPrefs.SetString(SELECTED_GRID_DATA_KEY, AssetDatabase.GetAssetPath(m_gridData));
				m_planeDrawers = null;
				ResetDataReferences();
				GridWindow.ResetSceneGrid();
			}
			#endregion

			#region Grid Move/Follow
			private void ResetFollowOffset()
			{
				if (Selection.activeTransform != null)
				{
					Vector3 gridPos = new Vector3(m_gridData.xAxisProperties.offset, m_gridData.yAxisProperties.offset,
						m_gridData.zAxisProperties.offset);
					m_followOffset = gridPos - Selection.activeTransform.position;
				}
				else
				{
					m_followOffset = Vector3.zero;
				}
			}

			private void MoveGridToSelected()
			{
				if (Selection.activeTransform != null)
				{
					MoveGridToPosition(Selection.activeTransform.position);
				}
			}

			private void RotateGridToSelected()
			{
				if (Selection.activeTransform != null)
				{
					RotateGridTo(Selection.activeTransform.rotation);
				}
			}

			private void MoveGridToPosition(Vector3 position)
			{
				Undo.RecordObject(m_gridData, "Move Grid to Selected");
				m_gridData.xAxisProperties.offset = position.x;
				m_gridData.yAxisProperties.offset = position.y;
				m_gridData.zAxisProperties.offset = position.z;
			}

			private void RotateGridTo(Quaternion rotation)
			{
				Undo.RecordObject(m_gridData, "Rotate Grid to Selected");
				Vector3 eulerAngles = rotation.eulerAngles;
				m_gridData.xAxisProperties.rotation = eulerAngles.x;
				m_gridData.yAxisProperties.rotation = eulerAngles.y;
				m_gridData.zAxisProperties.rotation = eulerAngles.z;
			}

			private void ToggleGridFollowPosition()
			{
				m_isFollowingPosition = !m_isFollowingPosition;
				if (m_isFollowingPosition)
				{
					ResetFollowOffset();
				}
            }

			private void ToggleGridFollowRotation()
			{
				m_isFollowingRotation = !m_isFollowingRotation;
			}

			private void TogglePositionSnap()
			{
				m_isPositionSnapEnabled = !m_isPositionSnapEnabled;
			}

			private void ToggleRotationSnap()
			{
				m_isRotationSnapEnabled = !m_isRotationSnapEnabled;
			}
			#endregion

			#region Window Data Helpers
			private void AttachKeybindings()
			{
				KeybindingManager.instance.AddKeybinding(new Keybinding("Toggle Visibility", true, true, false, true, KeyCode.Q, VISIBILTY_TOGGLE_KEYBINDING_ID), ToggleGridVisiblity);
				KeybindingManager.instance.AddKeybinding(new Keybinding("Toggle Position Snap", true, true, false, true, KeyCode.W, POSITION_ENABLED_KEYBINDING_ID), TogglePositionSnap);
				KeybindingManager.instance.AddKeybinding(new Keybinding("Toggle Rotation Snap", true, true, false, true, KeyCode.E, ROTATION_ENABLED_KEYBINDING_ID), ToggleRotationSnap);
				KeybindingManager.instance.AddKeybinding(new Keybinding("Move to Selected", true, true, false, false, KeyCode.T, MOVE_TO_SELECTED_KEYBINDING_ID), MoveGridToSelected);
				KeybindingManager.instance.AddKeybinding(new Keybinding("Rotate to Selected", true, true, false, false, KeyCode.G, ROTATE_TO_SELECTED_KEYBINDING_ID), RotateGridToSelected);
				KeybindingManager.instance.AddKeybinding(new Keybinding("Follow Position", true, true, false, true, KeyCode.T, FOLLOW_POSITION_KEYBINDING_ID), ToggleGridFollowPosition);
				KeybindingManager.instance.AddKeybinding(new Keybinding("Follow Rotation", true, true, false, true, KeyCode.G, FOLLOW_ROTATION_KEYBINDING_ID), ToggleGridFollowRotation);
			}

			private void ResetDataReferences()
			{
				if (GridSettings.instance == null)
				{
					EditorGridSettingsManager.instance.CreateSettingsAsset();
				}

				if (m_gridData == null)
				{
					if (EditorPrefs.HasKey(SELECTED_GRID_DATA_KEY))
					{
						string assetPath = EditorPrefs.GetString(SELECTED_GRID_DATA_KEY);
						m_gridData = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GridData)) as GridData;
					}

					if (m_gridData == null)
					{
						m_gridData = Resources.Load<GridData>(DEFAULT_GRID_DATA_PATH);
					}

					if (m_gridData == null)
					{
						m_gridData = CreateNewGridData(DEFAULT_GRID_DATA_NAME);
                    }
				}

				if (m_gridData != null && m_planeDrawers == null)
				{
					m_planeDrawers = new List<GridAxisDrawer>(3);
					m_planeDrawers.Add(new GridAxisDrawer(Axis.X, m_gridData));
					m_planeDrawers.Add(new GridAxisDrawer(Axis.Y, m_gridData));
					m_planeDrawers.Add(new GridAxisDrawer(Axis.Z, m_gridData));
				}
			}

			private void ResetReferences()
			{
				ResetWindowReference();
				ResetDataReferences();
			}

			private void LoadPrefs()
			{
				m_isGridVisible = EditorPrefs.GetBool(GRID_VISIBLE_KEY, true);
				m_isPositionSnapEnabled = EditorPrefs.GetBool(POSITION_SNAP_ENABLED_KEY, true);
				m_isRotationSnapEnabled = EditorPrefs.GetBool(ROTATION_SNAP_ENABLED_KEY, false);
				m_isFollowingPosition = EditorPrefs.GetBool(POSITION_FOLLOW_KEY, false);
				m_isFollowingRotation = EditorPrefs.GetBool(ROTATION_SNAP_ENABLED_KEY, false);
			}

			private void SavePrefs()
			{
				EditorPrefs.SetBool(GRID_VISIBLE_KEY, m_isGridVisible);
				EditorPrefs.SetBool(POSITION_SNAP_ENABLED_KEY, m_isPositionSnapEnabled);
				EditorPrefs.SetBool(ROTATION_SNAP_ENABLED_KEY, m_isRotationSnapEnabled);
				EditorPrefs.SetBool(POSITION_FOLLOW_KEY, m_isFollowingPosition);
				EditorPrefs.SetBool(ROTATION_SNAP_ENABLED_KEY, m_isFollowingRotation);
			}
			#endregion

			private void ToggleGridVisiblity()
			{
				m_isGridVisible = !m_isGridVisible;
			}
			private void ErrorClose(string error, params object[] args)
			{
				Debug.LogError(string.Format("Grids MX -- " + error + "\n\nIf the problem persists, please contact support.", args));
				if (s_window != null)
				{
					s_window.Close();
				}
			}

			[MenuItem("Window/Grids MX Toolbar", false, 10000)]
			public static void Open()
			{
				ResetWindowReference();
                s_window.Show();
			}

			public static void RepaintIfOpen()
			{
				if (s_window != null)
				{
					s_window.Repaint();
				}
			}

			public static void ResetSceneGrid()
			{
				if (s_window != null)
				{
					foreach (GridAxisDrawer planeDrawer in s_window.m_planeDrawers)
					{
						planeDrawer.ResetGridMesh();
					}
				}
			}

			private static void ResetWindowReference()
			{
				if (s_window == null)
				{
					s_window = EditorWindow.GetWindow<GridWindow>("Grids MX Toolbar");
					s_window.LoadPrefs();
					s_window.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
					s_window.AttachKeybindings();
				}
			}
		}
	}
}