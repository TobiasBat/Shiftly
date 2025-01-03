using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ObjectCircleLogging 
{
    public string logFileName;
    public string folder;
    public string suffix;
    public string runName;
    public string logFilePath;

    private List<String> touchRecordings = new List<string>();
    private List<String> questionRecordings = new List<string>();
    private List<String> generalInformation = new List<string>(); 

   public ObjectCircleLogging(string _folder, string _logFileName, string _runName, string _suffix)
   {
        logFileName = _logFileName;
        folder = _folder;
        runName = _runName;
        suffix = _suffix;

        string DateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");
        generalInformation.Add(ValueLineFormated("File_Created", DateString));

        logFilePath = Application.streamingAssetsPath
            + "/" + folder
            + "/" + logFileName
            + "-" + runName
            + DateString
            + suffix;

        Directory.CreateDirectory(Application.streamingAssetsPath + "/" + folder + "/");

        if (!File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, "This is a new empthy Log file\n");
        }
        else
        {
            Debug.Log("Log File already Exist: " + logFilePath);
        }
        UpdateLogFile();
    }

    public void AppendGeneralInformation(string label, string value)
    {
        generalInformation.Add(ValueLineFormated(label, value)); 
    }

    public void AppendGeneralInformation(string label, bool value)
    {
        generalInformation.Add(ValueLineFormated(label, value));
    }

    public void AppendGeneralInformation(string label, float value)
    {
        generalInformation.Add(ValueLineFormated(label, value));
    }

    /// <summary>
    /// Add a new Touch point state to the recordings
    /// </summary>
    /// <param name="touchPoint">current state will be recorded</param>
    public void AppendShiftlyTouchPointData(ShiftlyTouchPoint touchPoint)
    {
        var jsonString = touchPoint.tooJSONDictString(1, "   ");
        touchRecordings.Add(jsonString);
        UpdateLogFile();
    }

    public void AppendAnswerData(string jsonDataString)
    {
        questionRecordings.Add(jsonDataString);
        UpdateLogFile(); 
    }

    public void ClearRecordeedAnswers()
    {
        questionRecordings = new List<string>(); 
        questionRecordings = new List<string>(); 
    }

    /// <summary>
    /// Updates the log file based on the stored values
    /// </summary>
    public void UpdateLogFile()
    {
        string logFileText = "{\n";
        // Generall Information
        for (int i = 0; i < generalInformation.Count; i++)
        {
            if (i != 0)
            {
                logFileText += ",\n";
            }
            logFileText += generalInformation[i];
        }

        // Touch events
        logFileText += AddText(",\n\"touch_events\": [\n", 1);
        for (int i = 0; i < touchRecordings.Count; i++)
        {
            if (i != 0)
            {
                logFileText += ",\n"; 
            }
            logFileText += touchRecordings[i];
        }
        logFileText += AddText("\n],\n", 1); 

        // Questions
        logFileText += AddText("\"Questions\": [\n", 1);
        for (int i = 0; i < questionRecordings.Count; i++)
        {
            if(i != 0)
            {
                logFileText += ", \n"; 
            }
            logFileText += questionRecordings[i]; 
        }
        logFileText += AddText("]\n"); 
        logFileText += "}";

        File.WriteAllText(logFilePath, logFileText);
    }


    /// <summary>
    /// Appends final bracked and writes down file
    /// </summary>
    public void FinsihLogFile()
    {
        string DateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");
        generalInformation.Add(ValueLineFormated("File_Finsihed_After_Testrun_Complete", DateString));
        UpdateLogFile(); 
        // File.WriteAllText(logFilePath, File.ReadAllText(logFilePath));
    }


    #region Line Formating
    /// <summary>
    /// Creates a json line with indent key and value 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="indentLevel"></param>
    /// <returns></returns>
    private string ValueLineFormated(string name, float value, int indentLevel = 1)
    {
        return ValueLineFormated(name, "" + value, indentLevel);
    }

    private string ValueLineFormated(string name, bool value, int indentLevel = 1)
    {
        return ValueLineFormated(name, "" + value, indentLevel);

    }

    /// <summary>
    ///  Creates a json line with indent key and value 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="indentLevel"></param>
    /// <returns></returns>

    private string ValueLineFormated(string name, string value, int indentLevel = 1)
    {
        string result = "";
        for (int i = 0; i < indentLevel; i++)
        {
            result += "   ";
        }

        result += "\"" + name + "\": ";
        result += "\"" + value + "\"";
        return result;
    }


    /// <summary>
    /// To start new array / object that has a key
    /// </summary>
    /// <param name="name"></param>
    /// <param name="bracket"></param>
    /// <param name="indentLevel"></param>
    /// <returns></returns>
    private string NamedBrackedFormated(string name, string bracket, int indentLevel = 1)
    {
        string result = "";
        for (int i = 0; i < indentLevel; i++)
        {
            result += "   ";
        }

        result += "\"" + name + "\": ";
        result += bracket + "\n";
        return result;
    }

    /// <summary>
    ///  Returns the text with spaces before, corresponding to indent level
    /// </summary>
    /// <param name="text"></param>
    /// <param name="indentLevel"></param>
    /// <returns></returns>
    private string AddText(string text, int indentLevel = 1)
    {
        string result = "";
        for (int i = 0; i < indentLevel; i++)
        {
            result += "   ";
        }
        result += text;
        return result;
    }
    #endregion
}
