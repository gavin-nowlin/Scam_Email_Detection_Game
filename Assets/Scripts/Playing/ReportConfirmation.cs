using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportConfirmation : MonoBehaviour
{
    // ===== Variables =====
    // Reference to accept button on this game object
    [SerializeField]
    private Button _submitButton;
    // Reference to decline button on this game object
    [SerializeField]
    private Button _cancelButton;

    // Reference to submit button image
    [SerializeField]
    private Image _submitButtonImage;
    // Sprite for button active
    [SerializeField]
    private Sprite _activeButtonSprite;
    // Sprite for button inactive
    [SerializeField]
    private Sprite _inactiveButtonSprite;

    // Is true if the submit button is active and is false if not
    private bool _submitButtonIsActive = false;

    // References to report category toggles
    [SerializeField]
    private Toggle _grammarToggle;
    [SerializeField]
    private Toggle _senderToggle;
    [SerializeField]
    private Toggle _linkToggle;
    [SerializeField]
    private Toggle _urgencyToggle;

    // Reference to audio manager
    private AudioManager _audioManager;

    // Reference to inbox manager
    private InboxManager _inboxManager;

    // Reference to the model blocker panel
    [SerializeField]
    private GameObject _modalBlockerPanel;

    // Reference to report button
    [SerializeField]
    private ReportButton _reportButton;


    // ===== Private Methods =====
    private void Start() {
        // Getting reference to audio manager
        _audioManager = FindAnyObjectByType<AudioManager>();

        // Getting reference to inbox manager
        _inboxManager = FindAnyObjectByType<InboxManager>();

        // Adding OnClick() events for submit button
        DeactivateSubmitButton();

        // Adding OnClick() events for cancel button
        _cancelButton.onClick.RemoveAllListeners(); // Cleaning it first
        _cancelButton.onClick.AddListener(PlayClickSound);
        _cancelButton.onClick.AddListener(CancelReport);

        // Making the toggles play the click sound when they are changed

        _grammarToggle.onValueChanged.RemoveAllListeners(); // Cleaning it first
        // Using a lambda expression to wrap PlayClickSound() so that it matches the required signature (bool)
        _grammarToggle.onValueChanged.AddListener(_ => PlayClickSound());

        _senderToggle.onValueChanged.RemoveAllListeners(); // Cleaning it first
        // Using a lambda expression to wrap PlayClickSound() so that it matches the required signature (bool)
        _senderToggle.onValueChanged.AddListener(_ => PlayClickSound());

        _linkToggle.onValueChanged.RemoveAllListeners(); // Cleaning it first
        // Using a lambda expression to wrap PlayClickSound() so that it matches the required signature (bool)
        _linkToggle.onValueChanged.AddListener(_ => PlayClickSound());

        _urgencyToggle.onValueChanged.RemoveAllListeners(); // Cleaning it first
        // Using a lambda expression to wrap PlayClickSound() so that it matches the required signature (bool)
        _urgencyToggle.onValueChanged.AddListener(_ => PlayClickSound());
    }

    private void Update() {
        // Checks every from to see if at least one toggle is on and activates the submit button
        // if it is, it deactivates it if not
        if (!_submitButtonIsActive && AnyToggleActive())
            ActivateSubmitButton();
        else if (_submitButtonIsActive && !AnyToggleActive())
            DeactivateSubmitButton();
    }

    // Returns true if any toggles are currently active
    private bool AnyToggleActive() {
        if (_grammarToggle.isOn)
            return true;
        if (_senderToggle.isOn)
            return true;
        if (_linkToggle.isOn)
            return true;
        if (_urgencyToggle.isOn)
            return true;
        
        return false;
    }

    // Deactivates the submit report button when no toggles are checked
    private void DeactivateSubmitButton() {
        _submitButtonIsActive = false;
        _submitButton.onClick.RemoveAllListeners();
        _submitButtonImage.sprite = _inactiveButtonSprite;
    }

    private void ActivateSubmitButton() {
        _submitButtonIsActive = true;
        _submitButton.onClick.AddListener(PlayClickSound);
        _submitButton.onClick.AddListener(ConfirmReport);
        _submitButtonImage.sprite = _activeButtonSprite;
    }

    // Resets toggles to off
    public void ClearToggles() {
        _grammarToggle.isOn = false;
        _senderToggle.isOn = false;
        _linkToggle.isOn = false;
        _urgencyToggle.isOn = false;
    }

    // ===== Public Methods =====
    // Plays a click sound
    public void PlayClickSound() {
        _audioManager.PlayClickSound();
    }

    // Confirms email report
    public void ConfirmReport() {
        List<CategoryType> categoryTypes = new List<CategoryType>();

        // Adding answered category types to report
        if (_grammarToggle.isOn) {
            categoryTypes.Add(CategoryType.Grammar);
        }
        if (_senderToggle.isOn) {
            categoryTypes.Add(CategoryType.Sender);
        }
        if (_linkToggle.isOn) {
            categoryTypes.Add(CategoryType.Link);
        }
        if (_urgencyToggle.isOn) {
            categoryTypes.Add(CategoryType.Urgency);
        }

        // Making sure we don't pass a null list
        if (categoryTypes == null) {
            Debug.LogWarning("[ReportConfirmation/ConfirmReport] Null categoryTypes!");
            return;
        }
        
        // Resets toggles
        ClearToggles();
        // Reporting email with chosen category types
        _inboxManager.ReportEmail(categoryTypes);
        // Hides blocker panel and modal confirmation window
        _modalBlockerPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    // Cancels email report
    public void CancelReport() {
        // Hides blocker panel and modal confirmation window
        _modalBlockerPanel.SetActive(false);
        gameObject.SetActive(false);
        // Reactivates button
        _reportButton.Activate();
        // Resets toggles
        ClearToggles();
    }
}
