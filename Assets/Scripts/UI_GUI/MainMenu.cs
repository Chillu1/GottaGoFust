using UnityEngine;

using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //For now
    //0 = MainMenu
    //1 = CityPrototype
    //2 = EddoCityMap
    //3 = First Tutorial
    //4 = MovementTestMap
    //5 = First Tutorial Eddo
    //6 = Huge Testing Map

    //Buttons
    public GameObject play;

    public GameObject settings;//Button
    public GameObject exit;

    //GameObjects
    public GameObject maps;

    public GameObject options;//Main Object

    public GameObject back;

    private void Start()
    {
        play.SetActive(true);
        settings.SetActive(true);
        exit.SetActive(true);

        maps.SetActive(false);

        options.SetActive(false);

        back.SetActive(false);
    }

    private void Update()
    {
    }

    public void PlayOpenMaps()
    {
        CloseMainMenu();

        maps.SetActive(true);

        Opening();
    }

    public void OpenSettings()
    {
        CloseMainMenu();

        options.SetActive(true);

        Opening();
    }

    public void Back()
    {
        if (maps.activeInHierarchy)//If play is pressed and maps are opened
        {
            OpenMainMenu();

            maps.SetActive(false);

            Closing();
        }
        else if (options.activeInHierarchy)
        {
            OpenMainMenu();

            options.SetActive(false);

            Closing();
        }
    }

    //0 = MainMenu
    //1 = CityPrototype
    //2 = EddoCityMap
    //3 = First Tutorial
    //4 = MovementTestMap
    //5 = First Tutorial Eddo
    //6 = Huge Testing Map

    public void OpenPrototypeMap()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenEddosMap()
    {
        SceneManager.LoadScene(2);//Eddos
    }

    public void OpenFirstTutorial()
    {
        SceneManager.LoadScene(3);
    }

    public void OpenMovemementTestMap()
    {
        SceneManager.LoadScene(4);
    }

    public void OpenFirstTutorialEddo()
    {
        SceneManager.LoadScene(5);
    }

    public void OpenHugeTestingMap()
    {
        SceneManager.LoadScene(6);
    }

    public void OpenEddoIndoorsMap()
    {
        SceneManager.LoadScene(7);
    }

    public void OpenRoofTopsMap()
    {
        SceneManager.LoadScene(8);
    }

    public void OpenOutSideRoamMap()
    {
        SceneManager.LoadScene(9);
    }

    public void OpenRoofMapTwo()
    {
        SceneManager.LoadScene(10);
    }

    public void OpenHardMap()
    {
        SceneManager.LoadScene(11);
    }

    public void OpenOutSideRoamMapTwo()
    {
        SceneManager.LoadScene(12);
    }

    public void Quit()
    {
#if UNITY_STANDALONE//REMEMBER Check if it works with mac/linux and others

        Application.Quit();

#endif

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#endif
    }

    public void Opening()
    {
        back.SetActive(true);
    }

    public void Closing()
    {
        back.SetActive(false);
    }

    public void OpenMainMenu()
    {
        play.SetActive(true);
        settings.SetActive(true);
        exit.SetActive(true);
    }

    public void CloseMainMenu()
    {
        play.SetActive(false);
        settings.SetActive(false);
        exit.SetActive(false);
    }
}