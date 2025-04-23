using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterScreen : MonoBehaviour
{
    [SerializeField]
    private string loginSceneName = "Login";

    // Opens the register screen
    public void LoginScreen()
    {
        SceneManager.LoadScene(loginSceneName);
    }
}
