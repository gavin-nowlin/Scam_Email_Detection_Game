using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ===== Variables =====
    [SerializeField]
    private string _mainMenuSceneName = "MainMenu";
    [SerializeField]
    private string _playingSceneName = "PlayingWeek";

    [SerializeField]
    private WebRequest _webRequest;

    // Inbox screen
    [SerializeField]
    private GameObject _inboxScreen;
    [SerializeField]
    private string _inboxScreenTag = "Inbox";

    // Summary screens
    [SerializeField]
    private SummaryManager _summaryManager;
    [SerializeField]
    private GameObject _endOfDayScreenObject;
    [SerializeField]
    private string _endOfDayScreenTag = "EndOfDayScreen";
    [SerializeField]
    private GameObject _endOfWeekScreenObject;
    [SerializeField]
    private string _endOfWeekScreenTag = "EndOfWeekScreen";

    // Reference to inbox manager
    [SerializeField]
    private InboxManager _inboxManager;

    // Report manager
    private ReportManager _reportManager = new ReportManager();

    // Holds current day
    [SerializeField]
    private Weekday _curDay;

    // Reference to current day text
    [SerializeField]
    private TextMeshProUGUI _curDayText;
    // Current day text tag name
    [SerializeField]
    private string _curDayTextTag = "CurrentDayText";


    // ===== Private Methods =====
    private void Start()
    {
        // Getting screens if we need to
        if (_inboxScreen == null)
            _inboxScreen = GameObject.FindWithTag(_inboxScreenTag);
        if (_endOfDayScreenObject == null)
            _endOfDayScreenObject = GameObject.FindWithTag(_endOfDayScreenTag);
        if (_endOfWeekScreenObject == null)
            _endOfWeekScreenObject = GameObject.FindWithTag(_endOfWeekScreenTag);

        // Getting inbox manager
        _inboxManager = FindFirstObjectByType<InboxManager>().GetComponent<InboxManager>();

        // Setting initial day to Monday
        _curDay = Weekday.Monday;

        // Getting current day text
        _curDayText = GameObject.FindWithTag(_curDayTextTag).GetComponent<TextMeshProUGUI>();

        // Setting current day text
        _curDayText.text = _curDay.ToString();

        // Start the week (Monday)
        FillInboxByDay(_curDay);
    }

    // Fills inbox with all emails for that day
    private void FillInboxByDay(Weekday day) {
        _inboxManager.FillInboxByDay(day);
    }

    // Returns true is the week is finished and false if not,
    // if not the end of week it fills the inbox with all emails for the next day 
    private bool TryNextDay() {
        switch (_curDay)
        {
            case Weekday.Monday:
                _curDay = Weekday.Tuesday;
                return false;
            case Weekday.Tuesday:
                _curDay = Weekday.Wednesday;
                return false;
            case Weekday.Wednesday:
                _curDay = Weekday.Thursday;
                return false;
            case Weekday.Thursday:
                _curDay = Weekday.Friday;
                return false;
            case Weekday.Friday:
                // Debug.Log("[GameManager/TryNextDay] Week complete.");
                return true;
            default:
                Debug.LogWarning($"[GameManager/TryNextDay] Invalid next day!\n_curDay: {_curDay}");
                return false;
        }
    }


    // ===== Public Methods =====
    // Finishes day and opens end of day report screen
    public void DayFinished(List<Report> reports) {
        // Showing end of day screen
        _endOfDayScreenObject.SetActive(true);
        // Hiding inbox screen
        _inboxScreen.SetActive(false);
        // Add the reports from this day to the dictionary of reports
        _reportManager.AddReportList(reports, _curDay);

        // Populate end of day screen with list of report details
        _summaryManager.SummarizeDay(reports);

        // Turns the current day into an integer from 1-5
        int dayInt = 0;
        switch (reports[0].GetDay()) {
            case Weekday.Monday:
                dayInt = 1;
                return;
            case Weekday.Tuesday:
                dayInt = 2;
                return;
            case Weekday.Wednesday:
                dayInt = 3;
                return;
            case Weekday.Thursday:
                dayInt = 4;
                return;
            case Weekday.Friday:
                dayInt = 5;
                return;
        }
        // Updates database with day
        _webRequest.UpdateField("day", dayInt);

        // Calculating number of correct answers
        int score = 0;
        foreach (Report report in reports) {
            if (report.IsCorrect())
                ++score;
        }
        // Updates database with score
        _webRequest.UpdateField("score", score);
    }

    // Shows the end of week summary after the last weekday is finished
    public void WeekFinished() {
        _summaryManager.SummarizeWeek(_reportManager.GetReports());
    }

    // Called whenever the player is done with the end of day summary screen
    public void NextDay() {
        // Checking if the week is complete
        if (TryNextDay()) {
            // Open end of week report screen after end of day since week is over
            WeekFinished();
            return;
        }

        // Showing inbox screen
        _inboxScreen.SetActive(true);
        // Hiding end of day screen
        _endOfDayScreenObject.SetActive(false);
        // Setting current day text
        _curDayText.text = _curDay.ToString();

        FillInboxByDay(_curDay);
    }

    // Returns a copy of the dictionary of all reports with key as a weekday
    public Dictionary<Weekday, List<Report>> GetReports() {
        return _reportManager.GetReports();
    }

    // Returns a copy of the list of reports for a specified day
    public List<Report> GetReports(Weekday day) {
        return _reportManager.GetReports(day);
    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_mainMenuSceneName);
    }

    public void PlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_playingSceneName);
    }
}
