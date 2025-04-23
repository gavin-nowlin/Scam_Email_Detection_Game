using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoodButton : MonoBehaviour
{
    // ===== Variables =====
    // Reference to the button this script is attached to
    [SerializeField]
    private Button _button;

    // Reference to audio manager
    private AudioManager _audioManager;

    // Tag to find the good confirmation panel
    // [SerializeField]
    // private string _goodConfirmationTag = "GoodConfirmation";

    // Reference to the good confirmation panel
    [SerializeField]
    private GameObject _goodConfirmationPanel;

    // Reference to the model blocker panel
    [SerializeField]
    private GameObject _modalBlockerPanel;

    // Reference to button image
    [SerializeField]
    private Image _buttonImage;

    // Sprite for button active
    [SerializeField]
    private Sprite _activeButtonSprite;
    // Sprite for button inactive
    [SerializeField]
    private Sprite _inactiveButtonSprite;

    // ===== Private Methods =====

    private void Start()
    {
        // Getting reference to button image
        _buttonImage = GetComponent<Image>();

        // Getting reference to audio manager
        _audioManager = FindAnyObjectByType<AudioManager>();

        // Getting reference to confirm panel game object
        // _goodConfirmationPanel = GameObject.FindWithTag(_goodConfirmationTag);

        // Getting a reference to the button this script is attached to
        _button = gameObject.GetComponent<Button>();
        if (!_button)
            Debug.LogWarning("[GoodButton/Start] Button not found!");
        
        // Deactivating button because no email is open on start
        Deactivate();
    }


    // ===== Public Methods =====

    public void PlayClickSound() {
        _audioManager.PlayClickSound();
    }

    public void OpenConfirmPanel() {
        Deactivate();
        _goodConfirmationPanel.SetActive(true);
        _modalBlockerPanel.SetActive(true);
    }

    // Sets button as active
    public void Activate() {
        _button.onClick.RemoveAllListeners(); // Cleaning it first
        _button.onClick.AddListener(PlayClickSound);
        _button.onClick.AddListener(OpenConfirmPanel);
        _buttonImage.sprite = _activeButtonSprite;
    }

    // Sets button as inactive
    public void Deactivate() {
        _buttonImage.sprite = _inactiveButtonSprite;
        _button.onClick.RemoveAllListeners();
    }
}
