using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class InboxManager : MonoBehaviour
{
    // ===== Variables =====
    // List of reports, cleared at the end of every day
    [SerializeField]
    private List<Report> _curDayReports = new List<Report>();

    // Reference to the inbox email prefab
    public GameObject InboxEmailPrefab;

    // Names of tags to get objects by tag
    [SerializeField]
    private string _inboxBoxTagName = "InboxBox";

    // List of emails currently in the inbox
    public List<GameObject> inboxEmails = new List<GameObject>();
    // Holds a reference to the currently open email
    [SerializeField]
    private Email _curEmail;
    // Holds a reference to the currently open email object
    [SerializeField]
    private GameObject _curEmailObject;

    // Inbox box (holds all email game objects)
    [SerializeField]
    private GameObject _inboxBox;

    // EmailLoader object to parse all the emails from the emails.json file
    [SerializeField]
    private EmailLoader _emailLoader;

    // Tag for from text
    [SerializeField]
    private string _fromTag;
    // Reference to from text
    [SerializeField]
    private TextMeshProUGUI _fromText;

    // Tag for subject text
    [SerializeField]
    private string _subjectTag;
    // Reference to subject text
    [SerializeField]
    private TextMeshProUGUI _subjectText;

    // Tag for body text
    [SerializeField]
    private string _bodyTag;
    // Reference to body text
    [SerializeField]
    private TextMeshProUGUI _bodyText;

    // Reference for good button (set from inspector)
    [SerializeField]
    private GoodButton _goodButton;
    // Reference for report button (set from inspector)
    [SerializeField]
    private ReportButton _reportButton;


    // ===== Private Methods =====
    private void Awake()
    {
        // Getting the email loader
        _emailLoader = gameObject.GetComponent<EmailLoader>();

        // Getting the inbox box
        _inboxBox = GameObject.FindWithTag(_inboxBoxTagName).gameObject;

        // Getting TMPro text objects
        _fromText = GameObject.FindWithTag(_fromTag).gameObject.GetComponent<TextMeshProUGUI>();
        _subjectText = GameObject.FindWithTag(_subjectTag).gameObject.GetComponent<TextMeshProUGUI>();
        _bodyText = GameObject.FindWithTag(_bodyTag).gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // // Adding all emails in the inbox box at the start of the scene/day
        // int childCount = _inboxBox.transform.childCount;
        // for (int i = 0; i < childCount; i++)
        //     inboxEmails.Add(_inboxBox.transform.GetChild(i).gameObject);

        // Hiding all emails to make sure we start clean
        HideAllEmails();
        // Deactivate buttons becase no email is selected on start
        DeactivateButtons();
    }

    // Clears all emails and resets inbox
    private void ClearInbox() {
        // Deactivate buttons and hiding all emails
        DeactivateButtons();
        HideAllEmails();
        // Destroying and deleting every email from list of emails in inbox
        foreach (GameObject email in inboxEmails) {
            // Destroying the email object
            if (email != null)
                Destroy(email);
            else
                Debug.LogWarning($"[InboxManager/ClearInbox] Deleting null email game object!\nemail: {email}");
            // Removing it from the inbox list of emails
            inboxEmails.Remove(email);
        }
        _curEmail = null;
        _curEmailObject = null;
        _curDayReports.Clear();
    }

    // Adds an email to the inbox
    private void AddEmail(Email email) {
        // Instantiates new inbox email object from prefab as a child of the inbox box
        GameObject newInboxEmail = Instantiate(InboxEmailPrefab, _inboxBox.transform);
        // Gets the InboxEmail script attached to the newly instantiated object
        InboxEmail newEmail = newInboxEmail.GetComponent<InboxEmail>();
        // Initializes the new InboxEmail with the email and a reference to this InboxManager
        newEmail.Init(email, this);
        // Adds the new inbox email game object to a list
        inboxEmails.Add(newInboxEmail);
    }

    // Hides all emails
    private void HideAllEmails() {
        // Setting all texts to nothing
        _fromText.text = "";
        _subjectText.text = "";
        _bodyText.text = "";

        // Settings current email to null because there is no email selected
        _curEmail = null;

        // Marking all emails as deselected
        foreach (GameObject emailObject in inboxEmails) {
            emailObject.GetComponent<InboxEmail>().DeselectEmail();
        }
    }

    // Activates buttons when an email is selected
    private void ActivateButtons() {
        _goodButton.Activate();
        _reportButton.Activate();
    }

    // Deactivates buttons when no email is selected
    private void DeactivateButtons() {
        _goodButton.Deactivate();
        _reportButton.Deactivate();
    }

    // Removes an email from the inbox, returns true if email was deleted and false if not
    private bool DeleteCurrentEmail() {
        if (_curEmail == null || _curEmailObject == null || InboxEmpty())
            return false;

        // Deactivate buttons becase no email is selected
        DeactivateButtons();

        // Since we can only delete an email that we currently have selected we hide them all
        HideAllEmails();

        // Setting current email to null
        _curEmail = null;

        // Removing current email object from inbox emails list
        inboxEmails.Remove(_curEmailObject);

        // Deleting email game object
        Destroy(_curEmailObject);

        // Setting current email object to null
        _curEmailObject = null;

        // Checking if the last email was deleted
        if(InboxEmpty()) {
            // Calling DayFinished() method in game manager if the last email was deleted and passing list of reports
            FindFirstObjectByType<GameManager>().GetComponent<GameManager>().DayFinished(new List<Report>(_curDayReports));
            ClearInbox();
        }

        return true;
    }

    // ===== Public Methods =====
    // Returns true if the inbox is empty and false otherwise
    public bool InboxEmpty() {
        return !inboxEmails.Any();
    }

    // Adds all emails from given day into the inbox, if there are emails for that day
    public void FillInboxByDay(Weekday day) {
        if (_emailLoader?.emailsByDay?.TryGetValue(day, out var emails) == true)
        {
            ClearInbox();
            foreach (Email email in emails)
                AddEmail(email);
        }
        else
        {
            Debug.Log($"[InboxManager/FillInboxByDay] No emails found for {day} or EmailLoader not initialized.");
        }
    }

    // Selects email to view
    public void ShowEmail(Email email, GameObject emailObject) {
        // Making sure we don't try to do stuff with a null email
        if (email == null) {
            Debug.LogWarning("[InboxManager/ShowEmail] Null email!");
            return;
        }

        HideAllEmails();

        // Setting email we are showing as the currently open email
        _curEmail = email;

        // Setting current email object
        _curEmailObject = emailObject;

        // Activating buttons when an email is selected
        ActivateButtons();

        _fromText.text = email.From;
        _subjectText.text = email.Subject;
        _bodyText.text = email.Body;
    }

    // Marks email as normal
    public void GoodEmail() {
        // Making sure we don't try to do stuff with a null email
        if (_curEmail == null) {
            Debug.LogWarning("[InboxManager/GoodEmail] Null email!");
            return;
        }

        // Making report of the correct vs guessed answers
        _curDayReports.Add(new Report(_curEmail, false));

        if (!DeleteCurrentEmail()) {
            Debug.LogWarning("[InboxManager/GoodEmail] Tried to delete invalid email!");
        }
    }

    // Marks email as reported
    public void ReportEmail(List<CategoryType> categoryTypes) {
        // Making sure we don't try to do stuff with a null email
        if (_curEmail == null) {
            Debug.LogWarning("[InboxManager/ReportEmail] Null email!");
            return;
        }
        // Making sure we didn't get a null list
        if (categoryTypes == null) {
            Debug.LogWarning("[InboxManager/ReportEmail] Null categoryTypes!");
            return;
        }

        // Adding report to end of current day
        _curDayReports.Add(new Report(_curEmail, true, categoryTypes));

        // Outputting category types for debugging
        // string categories = "";

        // foreach (CategoryType category in categoryTypes)
        //     categories += category.ToString() + ", ";

        // Debug.Log($"Category types: {categories}");


        if (!DeleteCurrentEmail()) {
            Debug.LogWarning("[InboxManager/ReportEmail] Tried to delete invalid email!");
        }
    }
}