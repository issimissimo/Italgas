using UnityEngine;
using System.IO;
using System;

public class LogDisplay : MonoBehaviour
{
    public bool WriteToFile;
    public bool ShowInView;
    
    
    private string logText = ""; // String to hold the log text
    private GUIStyle labelStyle; // GUIStyle for the label
    private bool styleInitialized = false; // Flag to check if the GUIStyle is initialized
    private string logFilePath = "log.txt";


    void Awake()
    {
        // Debug.Log(System.DateTime.UtcNow.Millisecond.ToString());
        long ticks = System.DateTime.Now.Ticks;
        long milliseconds = ticks / System.TimeSpan.TicksPerMillisecond;

        logFilePath = milliseconds.ToString() + " - " + logFilePath;
        logFilePath = Path.Combine(Application.persistentDataPath, logFilePath);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog; // Subscribe to log messages
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog; // Unsubscribe from log messages
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        
        // Append the log message to the logText string
        logText += logString + "\n";

        if (WriteToFile)
        {
            // Save the log message to the file
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine(formattedDateTime + " - " + logString);
            }
        }
    }

    public void ClearLog()
    {
        logText = ""; // Clear the logText variable
    }

    private void OnGUI()
    {
        if (ShowInView)
        {

            // Initialize the GUIStyle with larger font size on the first OnGUI call
            if (!styleInitialized)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = 20; // Set the desired font size
                styleInitialized = true;
            }

            // Display the logText in a scrollable text area with larger font size
            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
            GUILayout.BeginScrollView(Vector2.zero);
            GUILayout.Label(logText, labelStyle); // Use the custom GUIStyle
            GUILayout.EndScrollView();
            GUILayout.EndArea();

        }


    }
}
