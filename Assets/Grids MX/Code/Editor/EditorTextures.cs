using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	namespace Grids
	{
		public static class EditorTextures
		{
			private const string TEXTURES_GRIDSMX_PATH_FORMAT = "Textures/{0}.png";
			private const string TEXTURES_GRIDSMX_SKINNED_PATH_FORMAT = "Textures/{0}/{1}.png";

			public const string MISSING_TEX_NAME = "mia";
			public const string BORDER_TEX_NAME = "border";
			public const string HIDDEN_ICON = "hidden";
			public const string VISIBLE_ICON = "visible";
			public const string POSITION_SNAP_DISABLED_ICON = "position-snap-disabled";
			public const string POSITION_SNAP_ENABLED_ICON = "position-snap-enabled";
			public const string ROTATION_SNAP_DISABLED_ICON = "rotation-snap-disabled";
			public const string ROTATION_SNAP_ENABLED_ICON = "rotation-snap-enabled";
			public const string ROTATION_SNAP_ANGLE_ICON = "rotation-snap-angle";
			public const string GRIDSIZE_ICON = "gridsize";
			public const string ADD_NEW_ICON = "add-new";
			public const string DUPLICATE_ICON = "duplicate";
			public const string CELL_SIZE_ICON = "cell-size";
			public const string GRID_OFFSET_ICON = "grid-offset";
			public const string GRID_TO_SELECTION_POSITION_ICON = "grid-to-selection-position";
			public const string GRID_TO_SELECTION_ROTATION_ICON = "grid-to-selection-rotation";
			public const string GRID_TO_SELECTION_POSITION_UNLOCKED_ICON = "grid-to-selection-position-unlocked";
			public const string GRID_TO_SELECTION_POSITION_LOCKED_ICON = "grid-to-selection-position-locked";
			public const string GRID_TO_SELECTION_ROTATION_UNLOCKED_ICON = "grid-to-selection-rotation-unlocked";
			public const string GRID_TO_SELECTION_ROTATION_LOCKED_ICON = "grid-to-selection-rotation-locked";
			public const string SELECT_ICON = "select";
			public const string SETTINGS_ICON = "settings-gear";

			private static Dictionary<string, Texture2D> s_loadedTextures;

			public static Texture2D GetByPath(string path)
			{
				if (s_loadedTextures == null)
				{
					s_loadedTextures = new Dictionary<string, Texture2D>();
				}

				Texture2D tex2D;
				if (s_loadedTextures.TryGetValue(path, out tex2D))
				{
					return tex2D;
				}

				tex2D = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
				if (tex2D == null)
				{
					tex2D = AssetDatabase.LoadAssetAtPath(EditorTextures.GetTexturePath(EditorTextures.MISSING_TEX_NAME),
						typeof(Texture2D)) as Texture2D;
				}

				return tex2D;
			}

			public static Texture2D GetByName(string name)
			{
				return GetByPath(GetTexturePath(name));
			}

			public static string GetTexturePath(string iconName)
			{
				// Check more specific first
				string skinnedPath = EditorUtil.ROOT_FOLDER + "/" + string.Format(TEXTURES_GRIDSMX_SKINNED_PATH_FORMAT,
					(EditorGUIUtility.isProSkin ? "dark_skin" : "light_skin"), iconName);

				if (EditorUtil.FileExists(skinnedPath))
				{
					return skinnedPath;
				}

				return EditorUtil.ROOT_FOLDER + "/" + string.Format(TEXTURES_GRIDSMX_PATH_FORMAT, iconName);
            }
		}
	}
}