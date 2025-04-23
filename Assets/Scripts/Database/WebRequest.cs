using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Security.Cryptography;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class WebRequest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Display;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField re_entered;
    [SerializeField] private CrednetialHolder CH;
    private string IP = "http://3.23.245.184/";
    private float retryDelay = 3;
    private int maxRetries = 3;
    private string curPass = "";
    private string secondaryPass = "";
    private bool updating = false;
    private string Phrase;

    [SerializeField]
    private string mainMenuSceneName = "MainMenu";

    async public Task<bool> Handshake()
    {
        CH.Phrase = GenerateRandomString(20);
        WWWForm form = new WWWForm();
        form.AddField("phrase", CH.Phrase);

        using (UnityWebRequest request = UnityWebRequest.Post(IP + "key.php", form))
        {
            await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var key = request.downloadHandler.text;
                    CH.RSA = new RSAHandler(key);
                }
                else { return false; }
        }
        return true;
    }

    public async void Register()
    {
        if(!curPass.Equals(secondaryPass))
        {
            Display.text = "<color=red>Does Not Match Current Password</color>";
            return;
        }
        else
        {
            Display.text = "<color=green>Passwords Match</color>";
        }

        //Check if needed variables are set
        if(CH.RSA == null) {
            var conn = Handshake(); 
            await conn;
            if(!conn.Result) { DisplayResult("ff"); return; } 
        }
        
        print(CH.Phrase);
        //Fill out POST form
        WWWForm form = new WWWForm();
        form.AddField("user", CH.RSA.Encrypt(username.text));
        form.AddField("pass", CH.RSA.Encrypt(curPass));
        form.AddField("phrase", CH.Phrase);

        //Send request to server
        SendRequest(IP + "register.php", form);
        
        /*
        curPass = "";
        secondaryPass = "";
        password.text = "";
        re_entered.text = "";
        */

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    async public void UpdateField(string field, int value)
    {
        //Check if needed variables are set
        if(CH.RSA == null) 
        {
            var conn = Handshake(); 
            await conn;
            if(!conn.Result) { DisplayResult("ff"); return; } 
        }

        //Use the Encrypted Username that's stored and fill out POST form
        WWWForm form = new WWWForm();
        form.AddField("user", CH.User);
        form.AddField("where", CH.RSA.Encrypt(field));
        form.AddField("value", CH.RSA.Encrypt(value.ToString()));
        form.AddField("phrase", CH.Phrase);

        //Send request to server
        SendRequest(IP + "update.php", form);
    }

    public async void LogIn()
    {
        //Check if needed variables are set
        if(CH.RSA == null) {
            var conn = Handshake(); 
            await conn;
            if(!conn.Result) { DisplayResult("ff"); return; } 
        }

        print(curPass);
        //Fill out POST Form
        WWWForm form = new WWWForm();
        form.AddField("user", CH.RSA.Encrypt(username.text));
        form.AddField("pass", CH.RSA.Encrypt(curPass));
        form.AddField("phrase", CH.Phrase);
        SendRequest(IP + "login.php", form);

        //Save the encrypted Username
        CH.User = CH.RSA.Encrypt(username.text);
    }

    public void LogOut()
    {
        //Fill out POST form
        WWWForm form = new WWWForm();
        form.AddField("phrase", CH.Phrase);
        SendRequest(IP + "logout.php", form);

        CH.ReleaseInfo();
    }

    async void SendRequest(string Address, WWWForm form)
    {
        var attempt = 0;
        while (attempt < maxRetries)
        {   
            // Try to send request
            using (UnityWebRequest request = UnityWebRequest.Post(Address, form))
            {
                await request.SendWebRequest();

                //Check server response upon successful request
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Server Response: " + request.downloadHandler.text);
                    DisplayResult(request.downloadHandler.text);
                    return; // Exit the loop if successful
                }
                else
                {
                    Debug.LogWarning("Request Failed: " + request.error + " (Attempt " + (attempt + 1) + ")");
                }
            }

            attempt++;
            await Awaitable.WaitForSecondsAsync(retryDelay); // Wait before retrying
        }

        Debug.LogError("All retry attempts failed.");
    }

    public void VoidPass(TMP_InputField passwordField)
    {
        updating = true;
        if(passwordField == password)
        {
            if(curPass.Length < passwordField.text.Length) { curPass += passwordField.text[passwordField.text.Length-1]; }
            else if(curPass.Length == passwordField.text.Length) { return; }
            else if(curPass.Length != 0) {curPass = curPass[..^1]; return; }
            else { StopCoroutine(Timer); return; }
        }
        else
        {
            if(secondaryPass.Length < passwordField.text.Length) { secondaryPass += passwordField.text[passwordField.text.Length-1]; }
            else if(secondaryPass.Length == passwordField.text.Length) { return; }
            else if(secondaryPass.Length != 0) {secondaryPass = secondaryPass[..^1]; return; }
            else { StopCoroutine(Timer); return; }
        }

        print(curPass);
        var ofuscatedPass = "".PadRight(passwordField.text.Length-1, '*') + passwordField.text[passwordField.text.Length-1];
        passwordField.text = ofuscatedPass;

        if(updating) 
        { 
            if(Timer != null) { StopCoroutine(Timer); }
            Timer = StartCoroutine(timer(1, passwordField));
            return; 
        }
    }

    public void CheckPass()
    {
        if(curPass.Length < 10)
        {
            Display.text = "<color=red>Password Must Be At Least 10 Characters Long</color>";
            RegisterButton.interactable = false;
            return;
        }
        
        if(!Regex.IsMatch(curPass, @"(?=.*[a-z])"))
        {
            Display.text = "<color=red>Password Must Have An Lowercase Character</color>";
            RegisterButton.interactable = false;
            return;
        }
        
        if(!Regex.IsMatch(curPass, @"(?=.*[A-Z])"))
        {
            Display.text = "<color=red>Password Must Have An Uppercase Character</color>";
            RegisterButton.interactable = false;
            return;
        }

        if(!Regex.IsMatch(curPass, @"(?=.*\d)"))
        {
            Display.text = "<color=red>Password Must Have At Least One Number</color>";
            RegisterButton.interactable = false;
            return;
        }
        
        if(!Regex.IsMatch(curPass, @"(?=.*[\W_]).+"))
        {
            Display.text = "<color=red>Password Must Have At Least One Special Character</color>";
            RegisterButton.interactable = false;
            return;
        }
        
        Display.text = "<color=green>Valid Password</color>";
        RegisterButton.interactable = true;
    }

    Coroutine Timer;
    IEnumerator timer(float duration, TMP_InputField password)
    {
        yield return new WaitForSecondsRealtime(duration);
        updating = false;
        var ofuscatedPass = "*".PadRight(password.text.Length, '*');
        password.text = ofuscatedPass;
    } 


    string GenerateRandomString(int length)
    {
        byte[] buffer = new byte[length / 2];
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(buffer);
        }
        return BitConverter.ToString(buffer).Replace("-", "");
    }

    void DisplayResult(string ServerResponse)
    {
        if (Display == null) {
            Debug.LogWarning("Display is null!");
            return;
        }

        // Only a sever repsonse of 0 is a length of 1
        if(ServerResponse.Length == 1) {
            Display.text = "<color=green>Success</color>";
            // Load the main menu scene
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }
        switch(ServerResponse[..2])
        {           
            case "l4":
                Display.text = "<color=red>Invalid Username</color>";
                break;
            
            case "l5":
                Display.text = "<color=red>Invalid Password</color>";
                break;
            
            case "r3":
                Display.text = "<color=red>Username Already Exists</color>";
                break;

            case "u1":
            case "u2":
            case "r1":
            case "r5":
            case "l1":
            case "l2":
            case "ff":
                Display.text = "<color=red>Connection Failure</color>";
                break;
                            
            default:
                Display.text = "<color=red>Internal Server Error</color>";
                break;
        }
    }

    void OnApplicationQuit()
    {
        print(CH.Phrase);
        LogOut();
    }
}
