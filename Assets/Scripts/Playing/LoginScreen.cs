using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScreen : MonoBehaviour
{
    [SerializeField]
    private string registerSceneName = "Register";

    // Opens the register screen
    public void RegisterScreen()
    {
        SceneManager.LoadScene(registerSceneName);
    }

    // Quits the application
    public void QuitGame() {
        Application.Quit();
    }
}
