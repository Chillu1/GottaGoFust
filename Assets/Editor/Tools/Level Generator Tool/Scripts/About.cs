using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class About : EditorWindow
{	
	StreamReader reader;
	string about;
	
	[MenuItem("Tools/Level Generator Tool/About")]
	private static void showEditor()
	{
		EditorWindow.GetWindow<About>(false,"About");
	} 
	
	void OnGUI()
	{
		if(reader == null)
		{
			reader = new StreamReader("Assets/Editor/Tools/Level Generator Tool/About.txt");
			about = reader.ReadToEnd();
		}
		
		GUILayout.Label("Level Generator Tool");
		EditorGUILayout.Space();
		GUILayout.Label("by Guilherme A. Leite");
		GUILayout.Label("02/2014");
		EditorGUILayout.Space();
		GUILayout.Label(about);
	}
}
