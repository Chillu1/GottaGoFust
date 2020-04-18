using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using System.Linq;

public class GlobalControl : MonoBehaviour {

    public static GlobalControl Instance;

    public MouseMovement mouseMovement;

    public float time;
    public string[] mapNames = new string[20];//First everything, then extract times. New string for each line
    //public string[] numberMapNames = new string[20];//Only numbers no map name
    public float[] timePerMap = new float[20];
    public string currentMapName;
    public int currentMap;

    //Settings
    public float sensivity;//Default 5?

    public TextAsset textAsset;

    void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        currentMapName = SceneManager.GetActiveScene().name;
    }

    void Start () {

        //Debug.Log(ReadString());
        //mapNames = ReadString().Split(ReadString(), "\r\n?|\n");
        //textAsset = Resources.Load("test", typeof(TextAsset)) as TextAsset;//Doesnt work as supposed 2
        
        mapNames = textAsset.text.Split('\n');
        timePerMap = new float[mapNames.Length];

        if(mouseMovement.sensitivityX != mouseMovement.sensitivityY)
        {
            Debug.LogError("Global Control: Different sensivities somehow");
        }
        sensivity = float.Parse(ReadSensivity().Replace("Sensivity: ", ""));//TODO Idk if works, I think it does tho

        mouseMovement.sensitivityX = sensivity;
        mouseMovement.sensitivityY = sensivity;

        for(int i = 0; i < mapNames.Length; i++)
        {
            //timePerMap[i] = float.Parse("123");//Works
            //numberMapNames[i] = mapNames[i].Replace("FirstTutorial time: ", "");
            //numberMapNames[i] = numberMapNames[i].Replace(" ", ".");//Testing if its coma

            float parseOut;

            //timePerMap[i] = float.TryParse(mapNames[i].Replace(currentMapName, ""), out parseOut);
            if(float.TryParse(mapNames[i].Replace(currentMapName + " time: ", ""), out parseOut))//Parses out first but not second
            {
                //Gouchi
                // Debug.Log("Parse Out: " + parseOut);
                timePerMap[i] = float.Parse(mapNames[i].Replace(currentMapName + " time: ", ""));//TODO Hard coded //First map/times works
            }
            else
            {
                Debug.LogError("GlobalControl: Parse Out: " + parseOut);
            }

            //timePerMap[i] = float.Parse(mapNames[i].Replace(currentMapName + " time: ", ""));//TODO Hard coded //First map/times works
            //Debug.Log(float.Parse(mapNames[i].Replace(currentMapName + " ", "")));

            //Debug.Log(mapNames[i]);
            //currentMapName = mapNames[i].Replace(" time: ", "");//TODO//Hard coded/Buggy with new setup This bugs out highscores in Player>Timer
            //currentMapName = currentMapName.Remove(14);//Hard coded/Buggy with new setup

            mapNames[i] = mapNames[i].Replace(" time: ", "");
            mapNames[i] = mapNames[i].Remove(currentMapName.Length+2);

            if(mapNames[i] != currentMapName)
            {
                //mapNames[i].Remove(0);//Remove whole name?
            }
            

        }
        //Check if name of current map is the same as highscore map, then display the highscore

        //currentMapName = mapNames[1];
    }
	
	
	void Update () {

        //if(mouseMovement.sensitivityX != sensivity)
        //{
            ChangeInSensivity();
        //}

        currentMapName = SceneManager.GetActiveScene().name;

	}

    void ChangeInSensivity()//TODO Sensivity to settings afterwards?
    {
        //sensivity = mouseMovement.sensitivityX;//Neither works
        mouseMovement.sensitivityX = sensivity;
        mouseMovement.sensitivityY = sensivity;

    }

    public static List<Data> savedData = new List<Data>();

    public static void WriteString(string mapName, float time)//Works
    {
        string path = "Assets/Resources/test.txt";

        StreamWriter writer = new StreamWriter(path, true);

        //Write some shit in there
        writer.WriteLine(mapName + " time: " + time);//Works
        writer.Close();
        

        //TextAsset asset = Resources.Load("test");


    }

    public static string ReadString()
    {
        string path = "Assets/Resources/test.txt";

        
        StreamReader reader = new StreamReader(path);
        //Debug.Log(reader.ReadToEnd());


        return reader.ReadToEnd();

        reader.Close();//TODO , idk honestly


        //return reader.ReadToEnd();//Reads out first, then closes, I think
    }

    public static void WriteSensivity(float sensivity)
    {
        string path = "Assets/Resources/Settings.txt";

        StreamWriter writer = new StreamWriter(path, false);//False = erstatte

        //File.WriteAllText(@"Assets/Resources/Settings.txt", string.Empty);
        writer.WriteLine("Sensivity: " + sensivity);

        writer.Close();

    }

    public static string ReadSensivity()
    {
        string path = "Assets/Resources/Settings.txt";

        StreamReader reader = new StreamReader(path);

        return reader.ReadToEnd();

        reader.Close();

    }

}
