using UnityEngine;

using UnityEngine.SceneManagement;

public class EscMenu : MonoBehaviour
{
    public static EscMenu instance;

    public bool paused = false;
    //Turn off all UI objects which shouldn't be in the main menu, like: Velocity

    //public GameObject pausedText;

    public GameObject pausedObjects;//Objects which are not supposed to be in the scene when the game is paused

    public GameObject escMenu;//TODO Paused text is black, hard 2 see at night

    public GameObject settings;

    public GameObject back;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" || SceneManager.GetActiveScene().buildIndex == 0)//0 Should be main menu //Or any other UI based scene
        {
            Cursor.lockState = CursorLockMode.Locked;//REMEMBER Closes cursor on middle of the screen //Esc recks it
        }

        pausedObjects.SetActive(true);
        escMenu.SetActive(false);

        settings.SetActive(false);
        back.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu" || Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().name != "MainMenu")//Or any other UI based scene
        {
            if (settings.activeInHierarchy)
            {
                Back();
            }
            else if (CheckForPause())
            {
                Closing();
            }
            else if (!CheckForPause())
            {
                Opening();
            }
        }
    }

    public void ResumeGame()
    {
        Closing();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        back.SetActive(true);

        pausedObjects.SetActive(false);
        escMenu.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);//MainMenu //Not Baked Panel
    }

    public void Opening()
    {
        Time.timeScale = 0;
        Debug.Log("Paused");

        pausedObjects.SetActive(false);
        escMenu.SetActive(true);

        back.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        paused = true;
    }

    public void Closing()
    {
        Time.timeScale = 1;
        Debug.Log("UnPaused");

        pausedObjects.SetActive(true);
        escMenu.SetActive(false);

        back.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        paused = false;
    }

    public void Back()
    {
        if (settings.activeInHierarchy)//Settings are active
        {
            settings.SetActive(false);
            Opening();
        }
    }

    private bool CheckForPause()
    {
        if (paused)
        {
            return true;
        }
        else if (!paused)
        {
            return false;
        }

        Debug.LogError("Error: EscMenu");
        return false;//Error
    }
}