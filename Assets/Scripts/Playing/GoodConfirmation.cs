using UnityEngine;
using UnityEngine.UI;

public class GoodConfirmation : MonoBehaviour
{
    // ===== Variables =====
    // Reference to accept button on this game object
    [SerializeField]
    private Button _yesButton;
    // Reference to decline button on this game object
    [SerializeField]
    private Button _noButton;

    // Reference to audio manager
    private AudioManager _audioManager;

    // Reference to inbox manager
    private InboxManager _inboxManager;

    // Reference to the model blocker panel
    [SerializeField]
    private GameObject _modalBlockerPanel;

    // Reference to good button
    [SerializeField]
    private GoodButton _goodButton;


    // ===== Private Methods =====
    private void Start() {
        // Getting reference to audio manager
        _audioManager = FindAnyObjectByType<AudioManager>();

        // Getting reference to inbox manager
        _inboxManager = FindAnyObjectByType<InboxManager>();

        // Adding OnClick() events for yes button
        _yesButton.onClick.RemoveAllListeners(); // Cleaning it first
        _yesButton.onClick.AddListener(PlayClickSound);
        _yesButton.onClick.AddListener(ConfirmReport);

        // Adding OnClick() events for no button
        _noButton.onClick.RemoveAllListeners(); // Cleaning it first
        _noButton.onClick.AddListener(PlayClickSound);
        _noButton.onClick.AddListener(CancelReport);
    }


    // ===== Public Methods =====
    // Plays a click sound
    public void PlayClickSound() {
        _audioManager.PlayClickSound();
    }

    // Confirms email report
    public void ConfirmReport() {
        // Sending report to inbox manager
        _inboxManager.GoodEmail();
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
        _goodButton.Activate();
    }
}
