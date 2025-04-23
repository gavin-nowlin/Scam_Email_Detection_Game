using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string playingSceneName = "Playing";
    [SerializeField]
    private string loginSceneName = "Login";

    // Opens the playing screen
    public void PlayGame()
    {
        SceneManager.LoadScene(playingSceneName);
    }

    // Opens Login screen
    public void LogOut()
    {
        SceneManager.LoadScene(loginSceneName);
    }
}
