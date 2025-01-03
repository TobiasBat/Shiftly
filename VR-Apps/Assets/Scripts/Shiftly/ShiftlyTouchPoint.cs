using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class ShiftlyTouchPoint : MonoBehaviour
{
    // public Vector3 orientationEulerAngles = new Vector3(0, 1, 0);
    [Range(0.0f, 1.0f)]
    public float side_1_extension = 0.0f;
    [Range(0.0f, 1.0f)]
    public float side_2_extension = 0.0f;
    [Range(0.0f, 1.0f)]
    public float side_3_extension = 0.0f; 
    public ShiftlyTouchPointPosition touchPointPosition;


    #region datarecording
    public string touchPointName = "touch point"; 
    public List<float> touchStartedTimes = new List<float>();
    public List<float> touchEndedTimes = new List<float>();
    #endregion

    #region Data Collection
    public void RecordTouchStarted()
    {
        touchStartedTimes.Add(Time.time);
    }

    public void RecordTochEnded()
    {
        touchEndedTimes.Add(Time.time);
    }

    public string tooJSONDictString(int indentlevel, string indent)
    {
        Debug.Log("Converting Touch Point To data json string");
        string result = "{\n";
        result += GetArrayLine("tochStartedTimes", touchStartedTimes, indentlevel, indent);
        result += ",\n"; 
        result += GetArrayLine("touchEndnedTimes", touchEndedTimes, indentlevel, indent) + ",\n";
        result += valueLine("set_name", touchPointName, indentlevel, indent) + ",\n";
        result += valueLine("side_1_extension", side_1_extension, indentlevel, indent) + ",\n";
        result += valueLine("side_2_extension", side_2_extension, indentlevel, indent) + ",\n";
        result += valueLine("side_3_extension", side_3_extension, indentlevel, indent) + "\n";
        result += "}"; 
        return result;
    }

    public void ResetRecordingData()
    {
        touchStartedTimes = new List<float>();
        touchEndedTimes = new List<float>(); 
    }

    private string valueLine(string label, float value, int indentlevel, string indent)
    {
        return getIndentString(indentlevel, indent) + "\""+label+"\"" + ": " + value;
    }
    private string valueLine(string label, string value, int indentlevel, string indent)
    {
        return getIndentString(indentlevel, indent) + "\""+label +"\"" + ": \"" + value + "\"";
    }
    private string getIndentString(int indentLevel, string indent)
    {
        string result = "";
        for (int i = 0; i < indentLevel; i++)
        {
            result += indent;
        }
        return result;
    }

    private string GetArrayLine(string label, List<float> list, int indentlevel, string indent)
    {
       
        string result = getIndentString(indentlevel, indent) + "\""+ label + "\"" +": [";
        for (int i = 0; i < list.Count; i++)
        {
            if (i != 0)
            {
                result += ", "; 
            }
            float value = list[i];
            result += value; 
        }
        result += "]";

        return result; 
    }
    #endregion
}
