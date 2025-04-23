using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

// Wrapper class for buttons
[System.Serializable]
public class WeekdayButtonPair {
    public Weekday weekday;
    public Button button;
}

public class SummaryManager : MonoBehaviour
{
    // ===== Variables =====
    [SerializeField]
    private GameObject _emailSummaryPrefab;

    // ===== End of Day Variables =====
    [SerializeField]
    private GameObject _summaryScreen;

    [SerializeField]
    private TextMeshProUGUI _dayText;

    [SerializeField]
    private TextMeshProUGUI _emailRatioText;

    [SerializeField]
    private Transform _scrollContent;


    [SerializeField]
    private List<GameObject> _reportObjects;

    [SerializeField]
    private TextMeshProUGUI _nextDayButtonText;

    // ===== End of Week Variables =====
    [SerializeField]
    private GameObject EOW_summaryScreen;
    [SerializeField]
    private TextMeshProUGUI EOW_emailRatioText;

    [SerializeField]
    private Transform EOW_scrollContent;

    [SerializeField]
    private List<GameObject> EOW_reportObjects;

    private Dictionary<Weekday, List<Report>> _allReports;

    // Buttons
    [SerializeField]
    private List<WeekdayButtonPair> _buttonPairs;
    private Dictionary<Weekday, Button> EOW_dayButtons;

    [SerializeField]
    private Sprite _buttonActiveSprite;
    [SerializeField]
    private Sprite _buttonInactiveSprite;

    // Holds all correct and incorrect answers
    private Dictionary<Weekday, Tuple<int, int>> _allRatios = new Dictionary<Weekday, Tuple<int, int>>();

    // ===== Private Methods =====
    private void Awake() {
        // Building dictionary for day buttons
        EOW_dayButtons = new Dictionary<Weekday, Button>();
        foreach (WeekdayButtonPair pair in _buttonPairs) {
            EOW_dayButtons[pair.weekday] = pair.button;
            // Adding listeners to each weekday button to open the correct day summary in the end of week screen
            pair.button.onClick.AddListener(() => SummarizeDay(pair.weekday));
        }
    }

    // Clears all content
    private void ClearContent() {
        // Clears content for end of day
        foreach (GameObject report in _reportObjects) {
            if (report != null)
                Destroy(report);
        }
        _reportObjects.Clear();

        // Clears content for end of week
        foreach (GameObject report in EOW_reportObjects) {
            if (report != null)
                Destroy(report);
        }
        EOW_reportObjects.Clear();
    }


    // ===== Public Methods =====
    // Summarizes the current day with the given list of reports
    public void SummarizeDay(List<Report> reports) {
        ClearContent(); // Make sure we start with no reports

        // Making sure we didn't get a null report list
        if (reports == null) {
            Debug.LogWarning("EndOfDayScreen/FillContent] Given null report list!");
            return;
        }

        if (reports[0].GetDay() == Weekday.Friday)
            _nextDayButtonText.text = "WEEK SUMMARY";
        else
            _nextDayButtonText.text = "NEXT DAY";

        // Used to calculate the ratio of correct to incorrect emails
        int correctEmails = 0;
        int totalEmails = 0;

        // Adding an element for each report to screen
        foreach (Report report in reports) {
            // Spawns a new email summary object
            GameObject newSummaryObject = Instantiate(_emailSummaryPrefab, _scrollContent);
            // Adding new game object to list of report objects
            _reportObjects.Add(newSummaryObject);
            // Gets a reference to the email summary script on the new object
            EmailSummary newEmailSummary = newSummaryObject.GetComponent<EmailSummary>();
            // Initiates the new email summary object with the given report
            newEmailSummary.Init(report);

            ++totalEmails;

            if (report.IsCorrect())
                ++correctEmails;
        }

        // Changing the day text to correct day
        _dayText.text = reports[0].GetDay().ToString();
        // Changing ratio text
        _emailRatioText.text = $"{correctEmails}/{totalEmails}";

        // Adding list of reports to dictionary of all reports
        _allRatios[reports[0].GetDay()] = new Tuple<int, int>(correctEmails, totalEmails);
    }

    // Summarizes the given day
    public void SummarizeDay(Weekday day) {
        ClearContent(); // Make sure we start with no reports

        // Making sure button list for the given day is not null
        if (EOW_dayButtons == null || !EOW_dayButtons.ContainsKey(day)) {
            Debug.LogWarning($"No button found for day: {day}");
            return;
        }

        // Setting all day buttons to appear active
        foreach (Weekday _day in EOW_dayButtons.Keys)
            EOW_dayButtons[_day].image.sprite = _buttonActiveSprite;

        // Setting the selected day button to appear inactive
        EOW_dayButtons[day].image.sprite = _buttonInactiveSprite;

        // Adding an element for each report to screen
        foreach (Report report in _allReports[day]) {
            // Spawns a new email summary object
            GameObject newSummaryObject = Instantiate(_emailSummaryPrefab, EOW_scrollContent);
            // Adding new game object to list of report objects
            EOW_reportObjects.Add(newSummaryObject);
            // Gets a reference to the email summary script on the new object
            EmailSummary newEmailSummary = newSummaryObject.GetComponent<EmailSummary>();
            // Initiates the new email summary object with the given report
            newEmailSummary.Init(report);
        }

        // Changing ratio text
        EOW_emailRatioText.text = $"{_allRatios[day].Item1}/{_allRatios[day].Item2}";
    }

    // Summarizes the week
    public void SummarizeWeek(Dictionary<Weekday, List<Report>> allReports) {
        EOW_summaryScreen.SetActive(true);
        _summaryScreen.SetActive(false);
        _allReports = allReports;
        SummarizeDay(Weekday.Monday);
    }
}
