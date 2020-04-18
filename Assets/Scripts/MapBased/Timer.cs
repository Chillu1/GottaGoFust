using UnityEngine;

using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    //Make it so it pauses when U press ESC?
    //Splits

    public static float time;
    public float[] mapNames;
    public float[] timePerMap = new float[3];//AmountOfMaps
    public int currentMap = 0;//Which map U are on currently //Array start with 0

    public float[] highScores;

    public float highScoreThisMap = 120;//Goal Or Average

    public UnityEngine.UI.Text mapName;
    public UnityEngine.UI.Text timer;
    public UnityEngine.UI.Text highScore;
    public UnityEngine.UI.Text velocityText;

    public bool itsNight = false;//So text is white instead of black

    private CollisionDetector collisionDetector;

    private void Start()
    {
        //time doesnt = time, so it basically resets
        time = 0;
        timePerMap = GlobalControl.Instance.timePerMap;
        currentMap = GlobalControl.Instance.currentMap;

        mapNames = new float[GlobalControl.Instance.mapNames.Length];
        timePerMap = new float[GlobalControl.Instance.timePerMap.Length];//AmountOfMaps
        highScores = new float[GlobalControl.Instance.timePerMap.Length];//AmountOfMaps
        highScores = GlobalControl.Instance.timePerMap;//Works

        for (int i = 0; i < highScores.Length; i++)
        {
            if (highScoreThisMap > highScores[i] && highScores[i] != 0)
            {
                highScoreThisMap = highScores[i];
            }
        }

        collisionDetector = GetComponentInChildren<CollisionDetector>();
    }

    private void Update()
    {
        if (collisionDetector.started == false)
        {
            time = 0;
        }
        else
            time += Time.deltaTime;

        ChangeTextColor();

        //On load map: timePerMap[currentMap] = time; currentMap += 1;
        if (Input.GetKeyDown(KeyCode.M))//TODO Only for testing, make dev mode?
        {
            timePerMap[currentMap] = time;
            currentMap += 1;

            SaveData();
            SceneManager.LoadScene(1);//Doesnt save data inbetween maps, Global Control?
        }

        timer.text = "Time: " + time.ToString("F2");
        highScore.text = "Highscore: " + highScoreThisMap.ToString("F2");//Displays 2 decimals
        mapName.text = GlobalControl.Instance.currentMapName;

        velocityText.text = "Velocity: " + GetComponent<MovementPlayer>().velocity.ToString("F2");//Displays magnitude velocity, //Round it to 1-2 decimals
                                                                                                  //velocityText.text = GetComponent<Movement>().velocity.ToString("F2");//CHANGE
    }

    public void SaveData()
    {
        GlobalControl.Instance.time = time;
        GlobalControl.Instance.timePerMap = timePerMap;
        GlobalControl.Instance.currentMap = currentMap;
    }

    private void ChangeTextColor()
    {
        if (itsNight)
        {
            mapName.color = Color.white;
            timer.color = Color.white;
            highScore.color = Color.white;
            velocityText.color = Color.white;
        }
        else if (!itsNight)
        {
            mapName.color = Color.black;
            timer.color = Color.black;
            highScore.color = Color.black;
            velocityText.color = Color.black;
        }
    }

    //If hit finish/loading new map stop timer/split
    //Prob when loading starts
}