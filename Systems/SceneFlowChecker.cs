using System;
using System.IO;
using Lumina.XML;
using UnityEngine; 

/// <summary>
/// SceneFlowChecker is a workaround to the lack of error files from the game itself. This class continously checks for errors and lets the user know if any happened.
/// </summary>
public class SceneFlowChecker
{
    private static readonly string LogFilePath = Path.Combine(GlobalPaths.LocalLowDirectory, @"Colossal Order\Cities Skylines II\Logs\SceneFlow.Log");
    private static readonly TimeSpan FileAgeLimit = TimeSpan.FromMinutes(10); // File must be recent (e.g., within the last 10 minutes)
    private static readonly int MaxErrorSnippetLength = 1000; // Max length of error snippet to include in the exception

    private static string errorMessage = null;

    /// <summary>
    /// Checks for errors in SceneFlow.log file.
    /// </summary>
    /// <exception cref="Exception">Throws an exception.</exception>
    public static void CheckForErrors()
    {
        if (!File.Exists(LogFilePath))
        {
            Console.WriteLine("Log file does not exist.");
            return;
        }

        FileInfo fileInfo = new FileInfo(LogFilePath);
        DateTime fileLastWriteTime = fileInfo.LastWriteTime;

        // Check if the file is recent
        if (DateTime.Now - fileLastWriteTime > FileAgeLimit)
        {
            Console.WriteLine("Log file is not recent enough.");
            return;
        }

        // Read the file and check for 'ERROR'
        string logContent = File.ReadAllText(LogFilePath);
        if (logContent.Contains("ERROR"))
        {
            // Extract the error details from the log content
            string errorSnippet = ExtractErrorSnippet(logContent);
            errorMessage = $"Error found in log file:\n{errorSnippet}";

            // Throw an exception with the error details
            Debug.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }

    private static string ExtractErrorSnippet(string logContent)
    {
        // Find the index of the first occurrence of 'ERROR'
        int errorIndex = logContent.IndexOf("ERROR");
        if (errorIndex == -1)
        {
            return "No error details available.";
        }

        // Extract a snippet around the error
        int snippetStart = Math.Max(0, errorIndex - MaxErrorSnippetLength / 2);
        int snippetLength = Math.Min(MaxErrorSnippetLength, logContent.Length - snippetStart);
        return logContent.Substring(snippetStart, snippetLength);
    }
}