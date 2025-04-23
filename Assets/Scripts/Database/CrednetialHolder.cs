using UnityEngine;

[CreateAssetMenu(fileName = "CredentialHolder", menuName = "Scriptable Objects/CredentialHolder")]
public class CrednetialHolder : ScriptableObject
{
    private string user;
    public string User
    {
        get { return user; }
        set { user = value; }
    }
    
    private string phrase;
    public string Phrase
    {
        get { return phrase; }
        set { phrase = value; }
    }

    private RSAHandler rsa = null;
    public RSAHandler RSA
    {
        get { return rsa; }
        set { rsa = value; }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void ReleaseInfo()
    {
        user = phrase = "";
        rsa = null;
    }
}
