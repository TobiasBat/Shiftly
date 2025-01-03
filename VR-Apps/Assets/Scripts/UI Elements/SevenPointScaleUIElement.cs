using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SevenPointScaleUIElement : MonoBehaviour
{
    public List<RadionButtonLogic> buttons;
    public bool dontAllowEmpthyResults;
    public TMP_Text question; 
    public TMP_Text leftLabel;
    public TMP_Text rightLabel; 

    public Button submitButton;
    public int selectedIndex = -1;
    public SevenPointScaleData dataObject = new SevenPointScaleData();
    public string note = "_"; 

    public void Start()
    {
        if (dontAllowEmpthyResults)
        {
            submitButton.gameObject.SetActive(false); 
        }

        dataObject.question = question.text;
        dataObject.labelValue0 = leftLabel.text;
        dataObject.labelValue6 = rightLabel.text; 
    }

    public void SelectByIndex(int selectIndex)
    {
        submitButton.gameObject.SetActive(true);
        
        selectedIndex = selectIndex;
        dataObject.selectedValue = selectIndex; 

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != selectIndex)
            {
                buttons[i].Deselect(); 
            }
        }
    }

    public void SetTimeQuestionHasStarted()
    {
        dataObject.qustionStarted = Time.time; 
    }

    public void SetTimeQuestionWasSubmitted()
    {
        dataObject.qustionEnded = Time.time; 
    }

    public void SetBackToDefault()
    {
        selectedIndex = -1;
        submitButton.gameObject.SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
           buttons[i].Deselect();
        }

    }

    public SevenPointScaleData GetSevenPointScaleDataObject()
    {
        dataObject.notes = note; 
        return dataObject; 
    }
}
