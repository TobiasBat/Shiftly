using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class ObjectCircleUILogic : MonoBehaviour
{
    public object_circles objectCircle;

    #region views
    public GameObject nextPreviewsView;
    public GameObject startCirceView;
    public GameObject realismQuestionView;
    public GameObject usabilityQuestionView;
    public GameObject utilityQuestionView; 
    public GameObject doneiew;
    #endregion

    #region ui elements conrolled by logic
    public Button nextButton;
    public Button previouseButton;
    public float secTillButtonsAktiveAgain = 2.0f;
    public SevenPointScaleUIElement[] likerScales;
    public SevenPointScaleUIElement realismScale;
    public SevenPointScaleUIElement usabilityScale;
    public SevenPointScaleUIElement utitlityScale;
    #endregion

    [SerializeReference]
    private KeyCode jumpToStartKeyCode = KeyCode.L; 

    #region parameters
    public bool showPreviouseButton = true;
    public bool showObjectCircleWhileRealismQuestion = true; 
    #endregion

    public void Start()
    {
        GetStartCircleView();
    }

    public void Update()
    {
        if (Input.GetKeyUp(jumpToStartKeyCode))
        {
            Debug.Log("Stopping the Circle Run");
            StopCircleRun(); 
        }
    }
    public void WriteDownAndClearAnswers()
    {
        ResetAllQuestions(); 
    }

    private void StopCircleRun()
    {
        ResetAllQuestions();
        GetStartCircleView(); 
    }

    private void LogScale(SevenPointScaleUIElement scale)
    {
        var scaleDataObject = scale.GetSevenPointScaleDataObject();
        Debug.Log("Current scale " + scale.name); 
        string jsonPartString = scaleDataObject.tooJSONDictString(1, "   ");
        objectCircle.circleLogging.AppendAnswerData(jsonPartString);
    }

    public void LogCurrentRealismQuestion()
    {
        LogScale(realismScale); 
    }

    public void LogCurrentUsabilityScale()
    {
        LogScale(usabilityScale); 
    }

    public void LogCurrentUtilityScale()
    {
        LogScale(utitlityScale); 
    }

    private void ResetAllQuestions()
    {
        Debug.Log("Reseting all Questions"); 
        for (int i = 0; i < likerScales.Length; i++)
        {
            likerScales[i].SetBackToDefault(); 
        }
    }

    public void GetNextPrevView()
    {
        objectCircle.EnableObjectCircleObjects();
        SetAllViewsInactive();
        nextPreviewsView.SetActive(true);
        if (showPreviouseButton && objectCircle.selectedIndex > 0)
        {
            previouseButton.gameObject.SetActive(true);
        }
        else
        {
            previouseButton.gameObject.SetActive(false);
        }
    }

    public void GetStartCircleView()
    {
        objectCircle.DisableObjectCircleObjects(); 
        SetAllViewsInactive();
        // TODO must be delayed
        startCirceView.SetActive(true);
    }

    public void GetDoneView()
    {
        objectCircle.circleLogging.FinsihLogFile();
        SetAllViewsInactive(); 
        doneiew.SetActive(true);
    }


    public void GetRealismQuestionViewForCurrentCircleobject()
    { 
        if (objectCircle.userStudyTestRun)
        {
            var sevenPoiontScale = realismQuestionView.GetComponentInChildren<SevenPointScaleUIElement>();
            sevenPoiontScale.note = objectCircle.objects[objectCircle.selectedIndex].shiftlyTouchPoint.GetComponent<ShiftlyTouchPoint>().touchPointName;
            sevenPoiontScale.SetTimeQuestionHasStarted();
            SetAllViewsInactive();
            realismQuestionView.SetActive(true);
            if (showObjectCircleWhileRealismQuestion)
            {
                objectCircle.TurnOffAllTouchIndicators();
                objectCircle.objects[objectCircle.selectedIndex].turnOffTouchIndicator(); 
            } else
            {
                objectCircle.DisableObjectCircleObjects();
            }
        }
        else
        {
            LogCurrentRealismQuestion();
            NextObject(); 
        }
       
    }

    public void GetUsabilityQuestionView()
    {
        SetAllViewsInactive();
        objectCircle.DisableObjectCircleObjects();
        usabilityQuestionView.SetActive(true);
        usabilityQuestionView.GetComponentInChildren<SevenPointScaleUIElement>().SetTimeQuestionHasStarted();  
    }

    public void GetUtilityQuestionView()
    {
        SetAllViewsInactive();
        objectCircle.DisableObjectCircleObjects();
        utilityQuestionView.SetActive(true);
        utilityQuestionView.GetComponentInChildren<SevenPointScaleUIElement>().SetTimeQuestionHasStarted(); 
    }

    public void NextObject()
    {
        // Echeck if objct circle would be done
        if (objectCircle.selectedIndex + 1 == objectCircle.objects.Count)
        {
            objectCircle.DisableObjectCircleObjects(); 
            if (objectCircle.userStudyTestRun)
            {
                GetUsabilityQuestionView();
            } else
            {
                objectCircle.CompleteCircle();
                objectCircle.stepperControllerClient.FullyContracted(); 
                GetDoneView(); 
            }
            
            
        } else
        {
            objectCircle.NextObject();
            StartedRotatedToNewObject();
            if (showPreviouseButton)
            {
                previouseButton.gameObject.SetActive(true);
            }
        }
    }

    public void PreviouseObject()
    {
        objectCircle.PreviouseObject();
        StartedRotatedToNewObject();
        if (showPreviouseButton && objectCircle.selectedIndex > 0)
        {
            previouseButton.gameObject.SetActive(true);
        } else
        {
            previouseButton.gameObject.SetActive(false);
        }
    }

    public void StartedRotatedToNewObject()
    {
        DisableButtons();
        Invoke("AktivateButtons", secTillButtonsAktiveAgain); 
    }

    public void DontShowPreviouseButton()
    {
        showPreviouseButton = false; 
    }

    public void ShowPreviouseButton()
    {
        showPreviouseButton = true; 
    }

    private void DisableButtons()
    {
        nextButton.interactable = false;
        previouseButton.interactable = false;
    }

    private void AktivateButtons()
    {
        nextButton.interactable = true; 
        previouseButton.interactable = true;
    }

    private void SetAllViewsInactive()
    {
        nextPreviewsView.SetActive(false);
        startCirceView.SetActive(false);
        realismQuestionView.SetActive(false);
        usabilityQuestionView.SetActive(false);
        utilityQuestionView.SetActive(false);
        doneiew.SetActive(false);
    }
}
