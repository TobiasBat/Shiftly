using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


/// <summary>
/// Utility to create a json logfile for a UserStudyTestCase. 
/// </summary>
public class UserStudyLogging {

    public string LogFileName = "userstudy-run"; // name if the user study 
    public string DirectoryOfResults = "User-study-test-logs";  // the subfolder where logfiles are stored
    private string suffix = ".json"; // file appendix 
    public string UserStudyRun;

    private string logFilePath; // full path of the logfile

    public UserStudyLogging(string directory, string logFileName, string userStudyRun, string _suffix)
    {
        DirectoryOfResults = directory; 
        LogFileName = logFileName;
        suffix = _suffix;
        UserStudyRun = userStudyRun;
        string DateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");

        logFilePath = Application.streamingAssetsPath + "/" + directory + "/" + logFileName + "-" + userStudyRun + "-" + DateString + suffix; 

        Directory.CreateDirectory(Application.streamingAssetsPath + "/" + directory + "/"); 

        if (!File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, "This is a new empthy Log file\n");
        } else
        {
            Debug.Log("Log File already Exist: " + logFilePath); 
        }
        InitLogFile(); 
    }

     /// <summary>
     ///  Gets and extends logifle by the lines of the testcases to the log file
     /// </summary>
     /// <param name="testCase"></param>
    public void WriteDownUserStudyTestCase(UserStudyTestCase testCase)
    {
        Debug.Log("Writting Down a new user study test case result " + testCase.name);
        string prevText = File.ReadAllText(logFilePath);
        string text = prevText;
        text += ",\n"; 
        text += NamedBrackedFormated(testCase.name, "{");
        
        text += ValueLineFormated("object_a", testCase.object_a, indentLevel: 2);
        text += ",\n"; 
        text += ValueLineFormated("object_b", testCase.object_b, indentLevel: 2);
        text += ",\n";
        text += ValueLineFormated("selected_value", testCase.selectedValue, indentLevel: 2);
        text += ",\n";
        text += ValueLineFormated("correct_value", testCase.correctValue, indentLevel: 2);
        text += ",\n";
        text += ValueLineFormated("delta_value", testCase.correctValue - testCase.correctValue, indentLevel: 2);
        text += ",\n";
        text += ValueLineFormated("testCaseStartedTime", testCase.testCaseStartedTime, indentLevel: 2);
        text += ",\n";
        text += ValueLineFormated("userStartTouchingObjectTime", testCase.userStartTouchingObjectTime, indentLevel: 2); 
        text += ",\n";
        text += ValueLineFormated("userStopsTouchingObjectTime", testCase.userStopsTouchingObjectTime, indentLevel: 2); 
        text += ",\n";
        text += ValueLineFormated("buttonPressedTime", testCase.buttonPressedTime, indentLevel: 2); 
        text += ",\n";
        text += ValueLineFormated("testCaseEndedTime", testCase.testCaseEndedTime, indentLevel: 2);
        text += "\n";
        text += AddText("}"); 
        File.WriteAllText(logFilePath, text);
    }

    /// <summary>
    /// Appends final bracked and writes down file
    /// </summary>
    public void FinsihLogFile()
    {
        File.WriteAllText(logFilePath, File.ReadAllText(logFilePath) + "\n}"); 
    }

    
    /// <summary>
    ///  Initialises Logfile with study name and create time 
    /// </summary>
    private void InitLogFile()
    {
        string text = "{\n";
        text += ValueLineFormated("study_run_name", UserStudyRun);
        text += ",\n";
        text += ValueLineFormated("log_file_create_time", DateTime.Now.ToString());
        File.WriteAllText(logFilePath, text); 
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
        result +=  bracket + "\n";
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
