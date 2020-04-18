using UnityEngine;

using UnityEngine.SceneManagement;

public class FinishMenu : MonoBehaviour
{
    private GameObject player;

    private CollisionDetector collisionDetector;

    public bool finished = false;

    public GameObject finishMenu;

    public GameObject stats;

    public GameObject back;

    public UnityEngine.UI.Text time;
    public UnityEngine.UI.Text highscore;

    private bool somethingIsOpened = false;

    private void Start()
    {
        finishMenu.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").gameObject;//REMEMBER
        collisionDetector = player.GetComponent<CollisionDetector>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;

        finished = false;
    }

    private void Update()
    {
        if (IsFinished() && !somethingIsOpened)//Finished map
        {
            Time.timeScale = 0;//Stops time

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;//Should fix the no mouse "bug"

            finishMenu.SetActive(true);

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartMap();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Stats();
            }
        }
        else if (!IsFinished())
        {
            finishMenu.SetActive(false);
        }

        //GameObject.FindGameObjectWithTag("Player").GetComponent<Timer>().
        time.text = "" + Timer.time;//23.68/works
        if (player.GetComponent<Timer>().highScoreThisMap < Timer.time)//TODO Looks like it works properly, not 100% sure tho
        {
            highscore.text = "" + player.GetComponent<Timer>().highScore.text;//TODO Works now, but isn't update right away cuz of the file save? (If U get a new highscore)
        }
        else
            highscore.text = "" + Timer.time;
    }

    public void RestartMap()
    {
        //Save highscore/new time
        //Cant move
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//Refreshes scene/ loads it again. Prob not the fastest way to refresh it/restart
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;

        //TODO Prob could just reset time and position
    }

    public void Stats()
    {
        //TODO
        //Averages, graphs++, worst sectors/parts of them, best sectors. Fastest speed, efficiency
        stats.SetActive(true);
        finishMenu.SetActive(false);

        back.SetActive(true);
        somethingIsOpened = true;
    }

    public void Back()
    {
        if (stats.activeInHierarchy)
        {
            stats.SetActive(false);
            finishMenu.SetActive(true);
            back.SetActive(false);
        }

        somethingIsOpened = false;
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    private bool IsFinished()
    {
        if (finished)
        {
            return true;
        }
        else if (!finished)
        {
            return false;
        }

        Debug.LogError("FinishMenu Error");
        return false;
    }
}