using System.Collections.Generic;
using UnityEngine;

public class Report
{
    // ===== Constructors =====
    // Constructor for guessed is not scam email
    public Report(Email email, bool guessedIsScam) {
        // Assigning variables
        Subject = email.Subject;
        Correct_IsScam = email.IsScam;
        Guessed_IsScam = guessedIsScam;

        Correct_Categories = email.Categories;
        Guessed_Categories = new List<CategoryType>();

        EmailDay = email.Day;
    }

    // Constructor for guessed is spam email
    public Report(Email email, bool guessedIsScam, List<CategoryType> guessedCategories) {
        // Assigning variables
        Subject = email.Subject;
        Correct_IsScam = email.IsScam;
        Guessed_IsScam = guessedIsScam;

        Correct_Categories = email.Categories;
        Guessed_Categories = guessedCategories;

        EmailDay = email.Day;
    }


    // ===== Variables =====
    private Weekday EmailDay;
    // Correct
    private string Subject;
    private bool Correct_IsScam;
    private List<CategoryType> Correct_Categories;
    // Guessed
    private bool Guessed_IsScam;
    private List<CategoryType> Guessed_Categories;


    // ===== Public Methods =====
    // Returns true if the guess is correct and false if not
    public bool IsCorrect() {
        return Correct_IsScam == Guessed_IsScam;
    }

    // Returns list of guessed categories
    public List<CategoryType> GetGuessedCategories() {
        return Guessed_Categories;
    }

    // Returns list of correct categories
    public List<CategoryType> GetCorrectCategories() {
        return Correct_Categories;
    }

    // Returns the day of the email
    public Weekday GetDay() {
        return EmailDay;
    }

    // Returns a copy of the subject string
    public string GetSubject() {
        return new string(Subject);
    }

    // Returns true if spam and false if not
    public bool IsScam() {
        return Correct_IsScam;
    }
}

public class ReportManager
{
    // List of reports
    [SerializeField]
    private Dictionary<Weekday, List<Report>> _allReports = new Dictionary<Weekday, List<Report>>();

    // ===== Public Methods =====
    // Adds a report to list of reports with a matching weekday
    public void AddReport(Report report) {
        if (report == null) {
            Debug.LogWarning("[ReportManager/AddReport] Attempting to add null report!");
            return;
        }

        // Initialize the list if the day doesn't exist yet
        if (!_allReports.ContainsKey(report.GetDay()))
            _allReports[report.GetDay()] = new List<Report>();

        // Add the email to the corresponding day
        _allReports[report.GetDay()].Add(report);
    }

    // Adds a list of reports to 
    public void AddReportList(List<Report> reports, Weekday day) {
        if (reports == null) {
            Debug.LogWarning("[ReportManager/AddReportList] Received a null report list!");
            return;
        }

        if (_allReports == null)
        _allReports = new Dictionary<Weekday, List<Report>>();

        // Initialize the list if the day doesn't exist yet
        if (!_allReports.ContainsKey(day))
            _allReports[day] = new List<Report>(reports);
        // Adding the given list to the end of the current list if the day does exist
        else
            _allReports[day].AddRange(reports);
    }

    // Returns a copy of the dictionary of all reports with key as a weekday
    public Dictionary<Weekday, List<Report>> GetReports() {
        return new Dictionary<Weekday, List<Report>>(_allReports);
    }

    // Returns a copy of the list of reports for a specified day
    public List<Report> GetReports(Weekday day) {
        return new List<Report>(_allReports[day]);
    }
}
