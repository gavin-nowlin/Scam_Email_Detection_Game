using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public enum CategoryType
{
    Grammar, Sender, Link, Urgency
}

// List of week days
public enum Weekday
{
    Monday, Tuesday, Wednesday, Thursday, Friday
}

// Email class with necessary attributes
[System.Serializable]
public class Email
{
    public string From;
    public string Subject;
    public string Body;
    // This is needed for parsing enums from a json file because
    // Unity's built in one is apparently lacking
    [JsonConverter(typeof(StringEnumConverter))]
    public Weekday Day;
    public bool IsScam;
    public List<CategoryType> Categories;
}

// Wrapper class
[System.Serializable]
public class EmailList
{
    public List<Email> Emails;
}

public class EmailLoader : MonoBehaviour
{
    // All loaded emails
    public List<Email> loadedEmails;
    // All emails with weekday as key
    public Dictionary<Weekday, List<Email>> emailsByDay = new Dictionary<Weekday, List<Email>>();

    void Awake()
    {
        loadedEmails = LoadEmails();
        SplitEmailsByDay();
    }

    // Loads and parses emails from json file
    public List<Email> LoadEmails()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Emails/emails");
        if (jsonFile == null)
        {
            Debug.LogError("emails.json not found!");
            return null;
        }

        // New settings are needed to correctly deserialize the categories
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        // Actually parsing all the emails from the json file
        List<Email> emails = JsonConvert.DeserializeObject<List<Email>>(jsonFile.text, settings);
        return emails;
    }

    // Splits emails by weekday
    void SplitEmailsByDay()
    {
        emailsByDay.Clear();

        foreach (Email email in loadedEmails)
        {
            // Initialize the list if the day doesn't exist yet
            if (!emailsByDay.ContainsKey(email.Day))
                emailsByDay[email.Day] = new List<Email>();

            // Add the email to the corresponding day
            emailsByDay[email.Day].Add(email);
        }

        // Outputting number of emails per day for debugging
        // foreach (var kvp in emailsByDay)
        //     Debug.Log($"{kvp.Key}: {kvp.Value.Count} emails");
    }
}
