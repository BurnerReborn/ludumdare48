using System.Collections;
using System.Collections.Generic;
using FMODUnityResonance;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public string levelSelect, mainMenu;

    public GameObject pauseScreen;
    public bool isPaused;

    FMOD.Studio.EventInstance mainSnapshot;
    FMOD.Studio.EventInstance pauseSnapshot;


    private void Awake()
    {
        instance = this;
        mainSnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/main");
        pauseSnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/pause");
        mainSnapshot.start();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        if(isPaused)
        {
            isPaused = false;
            pauseScreen.SetActive(false);
            mainSnapshot.start();
            pauseSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Time.timeScale = 1f;
        } else
        {
            isPaused = true;
            pauseScreen.SetActive(true);
            mainSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            pauseSnapshot.start();
            Time.timeScale = 0f;
            FMODUnity.RuntimeManager.CreateInstance("snapshot:/pause");
        }
    }

    public void LevelSelect()
    {
        PlayerPrefs.SetString("CurrentLevel", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(levelSelect);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
        Time.timeScale = 1f;
    }
}
