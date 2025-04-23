using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EmailSummary : MonoBehaviour
{
    // ===== Initializer =====
    public void Init(Report report) {
        _report = report;
        // Setting text fields
        _subjectText.text = _report.GetSubject();
        _guessText.text = _report.IsCorrect() ? "--- CORRECT ---" : "--- INCORRECT ---";
        _isSpamText.text = _report.IsScam() ? "Scam" : "Normal";
        // Settings answer details fields
        _correctDetailsText.text = $"CORRECT:\n{DetailsFormater(_report.GetCorrectCategories())}";
        _guessedDetailsText.text = $"YOU GUESSED:\n{DetailsFormater(_report.GetGuessedCategories())}";
    }


    // ===== Variables =====
    [SerializeField]
    private Report _report;

    [SerializeField]
    private TextMeshProUGUI _subjectText;

    [SerializeField]
    private TextMeshProUGUI _guessText;

    [SerializeField]
    private TextMeshProUGUI _isSpamText;

    [SerializeField]
    private TextMeshProUGUI _correctDetailsText;

    [SerializeField]
    private TextMeshProUGUI _guessedDetailsText;


    // ===== Private Methods =====
    private string DetailsFormater(List<CategoryType> categories) {
        string result = new string("");

        // Checking for grammar
        if (categories.Contains(CategoryType.Grammar))
            result += "- Grammar";
        else
            result += "-";
        
        // Checking for sender
        if (categories.Contains(CategoryType.Sender))
            result += "\n- Sender";
        else
            result += "\n-";

        // Checking for link
        if (categories.Contains(CategoryType.Link))
            result += "\n- Link";
        else
            result += "\n-";

        // Checking for urgency
        if (categories.Contains(CategoryType.Urgency))
            result += "\n- Urgency";
        else
            result += "\n-";
        
        return result;
    }
}
