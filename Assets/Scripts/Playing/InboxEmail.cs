using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InboxEmail : MonoBehaviour
{
    // ===== Initializer =====
    public void Init(Email email, InboxManager inboxManager) {
        // If no email is provided that is very bad, so we make the game object go bye-bye and log a warning
        if (email == null) {
            Debug.LogWarning("[InboxEmail/InboxEmail] No email provided in constructor! Destroying InboxEmail game object.");
            Destroy(gameObject);
            return;
        }
        // If no inbox is provided that is very bad, so we make the game object go bye-bye and log a warning
        if (inboxManager == null) {
            Debug.LogWarning("[InboxEmail/InboxEmail] No inbox provided in constructor! Destroying InboxEmail game object.");
            Destroy(gameObject);
            return;
        }

        _myEmail = email;
        _myInboxManager = inboxManager;

        // Setting subject text
        _subjectText.text = email.Subject;
    }

    // ===== Attributes =====
    // Reference to the email this inbox email object/button holds
    [SerializeField]
    private Email _myEmail;

    // Reference to inbox
    [SerializeField]
    private InboxManager _myInboxManager;

    // ===== Variables =====
    // Reference to button component
    [SerializeField]
    private Button _inboxEmailButton;

    // Reference to image component
    [SerializeField]
    private Image _buttonImage;

    // Reference to normal button image
    [SerializeField]
    private Sprite _normalButtonImage;
    // Reference to pressed button image
    [SerializeField]
    private Sprite _pressedButtonImage;

    // Reference to button text for subject
    [SerializeField]
    private TextMeshProUGUI _subjectText;

    // Reference to AudioManager
    private AudioManager _audioManager;


    // ===== Private Methods =====
    // Runs as soon as the script is active
    private void Awake()
    {
        // Getting reference to audio manager
        _audioManager = FindAnyObjectByType<AudioManager>();

        // Getting references if we need to (might be broken btw so just assign it in inspector)
        if (_inboxEmailButton == null) {
            // Getting a reference to the button this script is attached to
            _inboxEmailButton = gameObject.GetComponent<Button>();
            if (!_inboxEmailButton)
                Debug.LogWarning("[InboxEmail/Start] Button not found!");
                
            // Getting a reference to the text on the button we just got
            _subjectText = _inboxEmailButton.GetComponentInChildren<TextMeshProUGUI>();
            if (!_subjectText)
                Debug.LogWarning("[InboxEmail/Start] Button subject text not found!");
        }
        
        // Adding OnClick() events
        _inboxEmailButton.onClick.RemoveAllListeners(); // Cleaning it first
        _inboxEmailButton.onClick.AddListener(PlayClickSound);
        _inboxEmailButton.onClick.AddListener(SelectEmail);
    }

    // ===== Public Methods ======

    public void PlayClickSound() {
        _audioManager.PlayClickSound();
    }

    public void SelectEmail() {
        _myInboxManager.ShowEmail(_myEmail, gameObject);

        // Changing button to appear selected
        _buttonImage.sprite = _pressedButtonImage;
    }

    public void DeselectEmail() {     
        // Changing button to appear normal
        _buttonImage.sprite = _normalButtonImage;
    }
}
