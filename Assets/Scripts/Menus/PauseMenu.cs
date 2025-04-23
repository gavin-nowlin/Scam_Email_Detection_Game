using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseScreen;
    public GameObject playingScreen;
    public GameObject settingsScreen;

    [SerializeField]
    private string _mainMenuSceneName = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        playingScreen.SetActive(true);
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        pauseScreen.SetActive(true);
        playingScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void Settings() {
        settingsScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_mainMenuSceneName);
    }
}
