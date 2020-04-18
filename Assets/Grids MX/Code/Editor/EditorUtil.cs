using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace mx
{
	namespace Grids
	{
		public static class EditorUtil
		{
			private const string REGEX_PATH_NAME = @"{0}$";
			public const string ROOT_FOLDER = "Assets/Grids MX";

			public static bool IsPathName(string name, string path)
			{
				return Regex.IsMatch(path, string.Format(REGEX_PATH_NAME, name));
			}

			public static bool FileExists(string assetsPath)
			{
				return File.Exists(GetFullPath(assetsPath));
			}

			public static string GetFullPath(string assetsPath)
			{
				return Application.dataPath.Replace("\\", "/").Replace("Assets", string.Empty) + assetsPath;
            }
		}
	}
}