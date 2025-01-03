
public class SevenPointScaleData
{
    public string question = "Not Assigned"; 
    public float qustionStarted = -1; 
    public float qustionEnded = -1;
    public int selectedValue = -1; 
    public string labelValue0 = "Not Assigned";
    public string labelValue6 = "Not Assigned";
    public string notes = "";
 
    /// <summary>
    /// Converts the data object to a json string
    /// </summary>
    /// <param name="indentlevel">level of indent of the data lines</param>
    /// <param name="indent">"string of character used for indent (e.g. "   ")"</param>
    /// <returns>Formated json strong</returns>
    public string tooJSONDictString(int indentlevel, string indent)
    {
        string result = getIndentString(indentlevel, indent) + "{\n";
        result += valueLine("question", question, indentlevel+1, indent) + ",\n";
        result += valueLine("qustionStarted", qustionStarted, indentlevel+1, indent) + ",\n";
        result += valueLine("qustionStarted", qustionEnded, indentlevel+1, indent) + ",\n";
        result += valueLine("selectedValue", selectedValue, indentlevel+1, indent) + ",\n";
        result += valueLine("labelValue0", labelValue0, indentlevel+1, indent) + ",\n";
        result += valueLine("labelValue6", labelValue6, indentlevel+1, indent) + ",\n";
        result += valueLine("notes", notes, indentlevel+1, indent) + "\n";
        result += getIndentString(indentlevel, indent) + "}";
        return result; 
    }

    private string valueLine(string label, int value, int indentlevel, string indent)
    {
        return getIndentString(indentlevel, indent) + "\""+label+"\"" + ": " + value;
    }
    private string valueLine(string label, float value, int indentlevel, string indent)
    {
        return getIndentString(indentlevel, indent) + "\"" + label + "\"" + ": " + value;
    }
    private string valueLine(string label, string value, int indentlevel, string indent)
    {
        return getIndentString(indentlevel, indent) + "\"" + label + "\": " + "\"" + value + "\""; 
    }
    private string getIndentString(int indentLevel, string indent)
    {
        string result = "";
        for (int i = 0; i < indentLevel; i++) {
            result += indent;
        }
        return result; 
    }

}
