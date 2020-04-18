using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;



public class LevelGenerator : EditorWindow 
{
	Texture2D map; // The Image That Generates The Map
	
	List<Color> colorList = new List<Color>(); // List Of Colors Present In The Image Source
	List<Object> prefabList = new List<Object>(); // List Of Prefabs To Be Generated
	List<string> objectList = new List<string>(); // List Of Names To Organize The Objects

	Vector3 instPos; // Position Of Each Object
	
	Color[,] colorMap; // Tile of Color Extracted From The Image Source
	
	[MenuItem("Tools/Level Generator Tool/Generate Map")]
	private static void showEditor()
	{
		EditorWindow.GetWindow<LevelGenerator>(false,"Generate Map");
	} 
		
	void GetColors()  
	{
		for(int ix=0; ix<colorMap.GetLength(0); ix++)  // Read The Color Map in X
		{
			for(int iy=0; iy<colorMap.GetLength(1); iy++) // Read The Color MAp in Y
			{
				if(!colorList.Contains(colorMap[ix, iy])) // Find a new Color
				{
					// Important. adding a new instance in the prefab and name lists 
					// sync the indexes with the color list, so all the list shere the 
					// same index for a object
					
					colorList.Add(colorMap[ix,iy]); // Add it To the Color List
					prefabList.Add(null); // set a new empty instance in the prefab list
					objectList.Add("Object " + (objectList.Count + 1)); // set the object's name
				}
			}
		}
	}// Separate The Colors Present In The Image Source
	
	bool GotMap()
	{
		if(map) 
		{			
			colorMap = new Color[map.width, map.height]; // set the array size based in the source image's heigh and width
			
			for(int y=0; y<map.height; y++) // read the source image's pixels in Y
			{			
				for(int x=0; x<map.width; x++)// read the source image's pixels in X
				{
					colorMap[x, y] = map.GetPixel(x, y); // read and store the pixel color
				}
			}
			GetColors(); // separete the colors
			return true;
		}
		else
		{
			return false;
		}
	}
	
	void generteParent()
	{
		GameObject level = null;
		GameObject parent = null;
		
		if(!GameObject.Find("Level"))
			level = new GameObject("Level"); // create a level folder if it doesn't exist yet
		for(int i=0; i<objectList.Count; i++) // for each object created
		{
			if(!GameObject.Find(objectList[i]))// if the folder doesn't exist yet
				parent = new GameObject(objectList[i]);// create the folder
			parent.transform.parent = level.transform;// set the object in existent folder
		}
	} // Organize the new objects in the scene in proper folders
	
	void Generate()
	{
		GameObject obj = null;
		
		generteParent(); // create the folders for organization
		
		for(int ty=0; ty<colorMap.GetLength(1) ; ty++) // read the colors in the color map in the Y axix
			{			
				for(int tx=0; tx<colorMap.GetLength(0); tx++) // read the colors in the color map in the X axix
				{
					instPos = new Vector3((tx), 0, (ty)); // set the object's position based in the color position in the tale
					
					for(int c=0; c<colorList.Count; c++) // read the color to find the right kind of object to create
					{
						if(colorMap[tx,ty] == colorList[c])// find the object related to the color
						{
							if(prefabList[c])// if there is an object to be created
							{
								obj = Instantiate(prefabList[c], instPos, Quaternion.identity) as GameObject; // create the object
								obj.transform.parent = GameObject.Find(objectList[c]).transform; // set it in the folder
							}
						}
					}
				}
			}
	} // create all the objects based in the prefabs
	
	void ImportSettings()
	{
		if(map)
		{
			string path = AssetDatabase.GetAssetPath(map); // read the image's path
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; // create the importer settings
		
			if(textureImporter)
			{
				if(textureImporter.isReadable // if The settings are Right, 
				&& textureImporter.textureFormat == TextureImporterFormat.AutomaticTruecolor
				&& textureImporter.npotScale == TextureImporterNPOTScale.None )
					return; // Retuns
				
				// otherwise set the right settings
				textureImporter.isReadable = true;
				textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				textureImporter.npotScale = TextureImporterNPOTScale.None;
				
				// refresh the image source with the new settings
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			}
		}
	} // set the right Import Settings for the image source
	
	void OnGUI()
	{	
		// Get The Image Source
		map = EditorGUILayout.ObjectField("Map: ", map, typeof(Texture2D), false) as Texture2D;
		
		ImportSettings(); // change the image settings
		
		if(GotMap()) 
		{
			//Debug.Log(colorList.Count);
			//Debug.Log(prefabs.Lenght);
			for(int i=0; i<colorList.Count; i++) // show the colors from the image, and offer the inputs
			{
				EditorGUILayout.BeginHorizontal();
				objectList[i] = EditorGUILayout.TextField(objectList[i]); // for the name for the folders,
				colorList[i] = EditorGUILayout.ColorField(colorList[i]); // the colors from the image
				prefabList[i] = EditorGUILayout.ObjectField(prefabList[i], typeof(Object), true); // and the prfabs to be created.				
				EditorGUILayout.EndHorizontal();
			}
			
			EditorGUILayout.Space();
			if(GUILayout.Button("Generate")) // generate the map
			{
				//Debug.Log("Generating!");
				Generate();
			}
		}
	} // Show in the window
}
