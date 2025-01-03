using Leap.Unity.HandsModule;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.XR;
using System.IO;
using System;

public class ShiftlyUserStudyBlindTouch : MonoBehaviour
{
    #region varibles
    public bool debugInputs = false; 
    public ShiftlyUserSutdyBlindTouchKeyboardInput keyCodes;
    public ShiftlyUserStudyBlindTouchAxisSetup axisSetup = new ShiftlyUserStudyBlindTouchAxisSetup();
    public ShiftlyUserStudyBlindTouchViews views;
    public StepperControllerClient stepperController;
    public ShiftlyVisualModelController visualModelController;
    public TextMeshProUGUI generalInformationText;
    public StepperMotorSpeeds motorSpeed;
    public int numberOfSteps = 24;
    public ShiftlyUserSutdyBlindTouchHands hands;

    [Range(-1,17)]
    public int testcaseConfiguration = 0;
    public int userRunId = -1; 
    public int testCaseCount = 0;
    public float timeTillTestcaseIsLoaded = 4;
    public ShiftlyUserStudyBlindTouchTestcase[] testcases = {};
    private string userStudyTimeStart = "XXX";
    private string userStudyTimeFinished = "XXX";

    private ShiftlyUserStudyBlindTouchStates state = ShiftlyUserStudyBlindTouchStates.Interpolation;
    private AlembicStreamPlayer almebicPlayer;
    private List<ShiftlyUserStudyBlindTouchCaseData> caseDatas = new List<ShiftlyUserStudyBlindTouchCaseData>();
    private ShiftlyUserStudyBlindTouchCaseData caseData;
    private float playerStepSize;
    private int selectedTimeStampIndex = 0; 
    private float[] allSelectableTimeStamps; 

    private string logFileName = "blinduserstudy-run"; // name if the user study 
    public string directoryOfResults = "blind-user-study-test-logs";  // the subfolder where logfiles are stored
    private string suffix = ".csv"; // file appendix 
    private string logFilePath; // full path of the logfile
    private string logFilePathEnd; 
    #endregion

    #region MonoBehaviour
    void Start()
    {
        almebicPlayer = views.InterpolationView.view.GetComponentInChildren<AlembicStreamPlayer>(); 
        if (almebicPlayer == null)
        {
            Debug.LogError("User Study has no Alembic Player"); 
        } else
        {
            float playerLength = almebicPlayer.EndTime - almebicPlayer.StartTime;
            playerStepSize = (almebicPlayer.EndTime - almebicPlayer.StartTime) / (float)(numberOfSteps - 1);
            allSelectableTimeStamps = new float[numberOfSteps];
            for (var i = 0; i < numberOfSteps; i++)
            {
                allSelectableTimeStamps[i] = almebicPlayer.StartTime + i * playerStepSize;
            }
            

            // Load Testcases
            testcases = ShiftlyUserStudyBlindTouchTestcases.GetOrdered(testcaseConfiguration);
            testCaseCount = 0;

            TurnOffHands();
            TurnOnDominandHand();
            TurnOffNonDominandController();

            StartUserStudy();
        }
    }

    void Update()
    {
        CheckKeyboardInput();
        if (axisSetup.noControllerInput <= 0.0f)
        {
            HandleControllerInput();
        } else
        {
            axisSetup.noControllerInput -= Time.deltaTime; 
        }

        AlignUserStudyWithShiftly();
        AlignHandSpheresWithHand();
        if(state == ShiftlyUserStudyBlindTouchStates.Touch) AlingTouchIndicators();
    }

    #endregion

    #region User Study Ciycle 
    public void StartUserStudy()
    {
        state = ShiftlyUserStudyBlindTouchStates.Start;
        TurnOffAllViews();
        if (views.StartUserStudyView.view != null) views.StartUserStudyView.view.SetActive(true);
        generalInformationText.text = views.StartUserStudyView.generalInstructionText + "\n" + userRunId + "," + testcaseConfiguration;

        // Bringing Shiftly To 000 state for adjustments
        stepperController.setAndUploadDegree("1", 0, motorSpeed);
        stepperController.setAndUploadDegree("2", 0, motorSpeed);
        stepperController.setAndUploadDegree("3", 0, motorSpeed);
        stepperController.stepper1Speed = motorSpeed;
        stepperController.stepper2Speed = motorSpeed;
        stepperController.stepper3Speed = motorSpeed; 

        // Turning Off Indicators based on Settings 
        views.TouchView.touchPointIndicator.SetActive(views.TouchView.displayTouchPointIndicator);
        views.TouchView.arrowIndicator.SetActive(views.TouchView.displayArrowIndicator);

        userStudyTimeStart = GetCurrentTimeAsString(); 
        CreateLogFile();
    }

    public void FinishUserStudy()
    {
        Debug.Log("Finishing User Study");
        state = ShiftlyUserStudyBlindTouchStates.EndUserStudy;
        userStudyTimeFinished = GetCurrentTimeAsString();
        TurnOffAllViews();
        WriteAllCaseDatasToCSV();
        generalInformationText.text = views.FinishUserStudyView.generalInstructionText;
        if (views.FinishUserStudyView.view != null)
        {
            views.FinishUserStudyView.view.SetActive(true);
        }
    }
    #endregion

    #region TestCase Circle 
    public void LoadNextTextCase()
    {
        Debug.Log("Loading Next Test Case");
        state = ShiftlyUserStudyBlindTouchStates.Loading;

        // Reset Recording Data
        caseData = new ShiftlyUserStudyBlindTouchCaseData();

        // Load next Testcase 
        ShiftlyUserStudyBlindTouchTestcase testcase = testcases[testCaseCount];
        caseData.testCaseInStudyRun = testCaseCount;
        caseData.correctAnswer = testcase.correctAnwser;
        caseData.correctAnswer2 = testcase.correctAnswer2; 
        float[] shifltyConfiguration = testcase.stepperConfiguration;
        caseData.shiftlyConfiguration = shifltyConfiguration;
        stepperController.setAndUploadDegree("1", shifltyConfiguration[0], motorSpeed);
        stepperController.setAndUploadDegree("2", shifltyConfiguration[1], motorSpeed);
        stepperController.setAndUploadDegree("3", shifltyConfiguration[2], motorSpeed);
        caseData.name = testcase.name;
        caseData.testCaseId = testcase.id; 

        // Setting up the interpolation player
        // float playerRangeLength = almebicPlayer.EndTime - almebicPlayer.StartTime; 
        int randomIndex = UnityEngine.Random.Range(0, numberOfSteps);
        selectedTimeStampIndex = randomIndex; 
        float randomStartingPoint = allSelectableTimeStamps[randomIndex];
        //     almebicPlayer.StartTime + randomIndex * playerRangeLength / (float)(numberOfSteps-1.0f);
        almebicPlayer.CurrentTime = randomStartingPoint;
        caseData.startValue = randomStartingPoint;
        caseData.currentlySelected = randomStartingPoint;

        // Change the View 
        TurnOffAllViews();
        views.LoadingView.view.SetActive(true);
        generalInformationText.text = views.LoadingView.generalInstructionText;

        // Start the touch after a delay
        Invoke("StartTouch", timeTillTestcaseIsLoaded);
    }

    public void StartTouch()
    {
        state = ShiftlyUserStudyBlindTouchStates.Touch;
        caseData.timeStartedTouch = GetCurrentTimeAsString();

        Debug.Log("Starting Touch");
        TurnOffAllViews();
        views.TouchView.view.SetActive(true);
        generalInformationText.text = views.TouchView.generalInstructionText;
        TurnOffHands(); 
    }

    public void StartInterpolation()
    {
        caseData.timeEndedTouch = GetCurrentTimeAsString();

        state = ShiftlyUserStudyBlindTouchStates.Interpolation;
        caseData.timeStartedInterpolation = GetCurrentTimeAsString();
        Debug.Log("Starting Interpolation");
        TurnOffAllViews();
        views.InterpolationView.view.SetActive(true);
        TurnOnDominandHand(); 

        generalInformationText.text = views.InterpolationView.generalInstructionText;
    }

    public void StartConfidenceQuestion()
    {
        caseData.timeEndedInterpolation = GetCurrentTimeAsString();
        state = ShiftlyUserStudyBlindTouchStates.Confidence;
        Debug.Log("Starting Confidence Question View");

        // reset liker scale so nothing is selected
        views.ConvidenceView.likerScaleUIElement.SetBackToDefault();
        
        TurnOffAllViews(); 
        views.ConvidenceView.view.SetActive(true);
        TurnOffHands();
        generalInformationText.text = views.ConvidenceView.generalInstructionText;
    }

    public void FinishTestCase()
    {
        Debug.Log("Finish Test Case");
        state = ShiftlyUserStudyBlindTouchStates.FinishTestCase;
        // Updating Case Data
        caseData.timeEnded = GetCurrentTimeAsString();
        caseData.selectedConfidence = views.ConvidenceView.likerScaleUIElement.selectedIndex;
        caseData.convidenceScaleLeftLabel = views.ConvidenceView.likerScaleUIElement.leftLabel.text;
        caseData.convidenceScaleRightLabel = views.ConvidenceView.likerScaleUIElement.rightLabel.text;
        caseData.finallySelected = caseData.currentlySelected;
        AppendCaseDataToCSV(caseData);
        caseDatas.Add(caseData);

        // Handling next view 
        TurnOffAllViews();
        generalInformationText.text = views.FinishView.generalInstructionText; 
        // In case in a demo run
        if (testCaseCount < 2)
        {
            generalInformationText.text = views.FinishView.generalInstructionText + " (Demo)";
        }
        if (views.FinishView.view != null)
        {
            views.FinishView.view.SetActive(true);
        }

        if (testCaseCount == 1)
        {
            // Do Noting 
            // Requires enter from instructure after second demo case
        }
        else if (testCaseCount + 1 < testcases.Length)
        {
            testCaseCount++;
            Invoke("LoadNextTextCase", 1.0f);
        }
        else
        {
            Invoke("FinishUserStudy", 0.5f);
        }
    }
    #endregion

    #region I/O and Shiftly Manipulation
    public void HandleControllerInput()
    {

        List<InputDevice> devices = new List<InputDevice> ();
        InputDeviceCharacteristics controllerCharacteristics =  InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices); 

        foreach (InputDevice device in devices)
        {
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2D);
            device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
          
            switch (state)
            {
                case ShiftlyUserStudyBlindTouchStates.Start:
                    if (triggerButton && debugInputs)
                    {
                        LoadNextTextCase();
                        axisSetup.noControllerInput = axisSetup.brakeTillCheck;
                    }
                    break;
                case ShiftlyUserStudyBlindTouchStates.Touch:
                    if (triggerButton && debugInputs)
                    {
                        StartInterpolation();
                        axisSetup.noControllerInput = axisSetup.brakeTillCheck;
                    }
                    break;

                case ShiftlyUserStudyBlindTouchStates.Interpolation:
                    if (primary2D.x > axisSetup.thressholdTouchpadInput)
                    {
                        ForwardMorph();
                        axisSetup.noControllerInput = axisSetup.brakeTillCheck; 
                    }
                    else if (primary2D.x < -axisSetup.thressholdTouchpadInput)
                    {
                        BackwardMorph();
                        axisSetup.noControllerInput = axisSetup.brakeTillCheck; 
                    } 
                    if (triggerButton && debugInputs)
                    {
                        FinishTestCase(); 
                    }
                    break;
                case ShiftlyUserStudyBlindTouchStates.Confidence:
                    break;
                default:
                    break; 
            }
        }
    }

    // 17.okt 1130


    public void CheckKeyboardInput()
    {
        switch (state)
        {
            case ShiftlyUserStudyBlindTouchStates.Start:
                if (Input.GetKeyUp(keyCodes.startUserStudy))
                {
                    LoadNextTextCase(); 
                }
                break;
            case ShiftlyUserStudyBlindTouchStates.Touch:
                if (Input.GetKeyUp(keyCodes.next))
                {
                    StartInterpolation(); 
                }
                break;
            case ShiftlyUserStudyBlindTouchStates.Interpolation:
                if (Input.GetKeyUp(keyCodes.next))
                {
                    StartConfidenceQuestion(); 
                }
                if (Input.GetKeyUp(keyCodes.forwardMorph))
                {
                    ForwardMorph();
                } 
                if (Input.GetKeyUp(keyCodes.backWardsMorph)) 
                {
                    BackwardMorph(); 
                }
                break;
            case ShiftlyUserStudyBlindTouchStates.FinishTestCase: 
                if (Input.GetKeyUp(keyCodes.startUserStudy) && testCaseCount == 1)
                {
                    testCaseCount++;
                    Invoke("LoadNextTextCase", 1.0f);
                }
                break;
            default:
                break;
        }
    }

    public void AlignUserStudyWithShiftly()
    {
        gameObject.transform.position = stepperController.gameObject.transform.position;
        gameObject.transform.eulerAngles = stepperController.gameObject.transform.eulerAngles + new Vector3(0.0f, -180.0f, 0.0f); 
    }

    public void AlignHandSpheresWithHand()
    {
        if (hands.dominandHandIsRight) // right hand has a sphere
        {
            var hand = hands.rightHandBinder.GetLeapHand(); 
            if (hand != null)
            {
                hands.handTouchIndicator.SetActive(true); 
                hands.handTouchIndicator.transform.position = hand.PalmPosition;
            }
        } else
        {
            var hand = hands.leftHandBinder.GetLeapHand();
            if (hand != null)
            {
                hands.handTouchIndicator.SetActive(true);
                hands.handTouchIndicator.transform.position = hand.PalmPosition;
            }
        }
    }

    public void AlingTouchIndicators()
    {
        // Align arrow 
        var touchPointPosition = testcases[testCaseCount].touchpointPosition;
        if (views.TouchView.displayArrowIndicator)
        {
            views.TouchView.arrowIndicator.transform.position = visualModelController.GetTouchPointIndicatorObjectFromPointPosition(touchPointPosition).transform.position;
            switch (touchPointPosition)
            {
                case (ShiftlyTouchPointPosition.TopEdge):
                    views.TouchView.arrowIndicator.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 90.0f);
                    views.TouchView.arrowIndicator.transform.localPosition += new Vector3(0.0f, 0.1f, 0.0f);
                    break;
                case (ShiftlyTouchPointPosition.FrontSurface):

                    Vector3 panel = visualModelController.FixedHingeIndicator.transform.position - visualModelController.TopEdgeIndicator.transform.position;
                    Vector3 panelAngle = Quaternion.LookRotation(panel).eulerAngles;
                    Vector3 eulerAngles = new Vector3(0.0f, 180.0f, -panelAngle.x) + new Vector3(0.0f, 0.0f, 90.0f);
                    views.TouchView.arrowIndicator.transform.localEulerAngles = eulerAngles;
                    views.TouchView.arrowIndicator.transform.Translate(new Vector3(0.1f, 0.0f, 0.0f));

                    break;
            }
        }
        // Align Touch Point

        if (views.TouchView.displayTouchPointIndicator)
        {
            views.TouchView.touchPointIndicator.transform.position = visualModelController.GetTouchPointIndicatorObjectFromPointPosition(touchPointPosition).transform.position;
        }
    }

    private void TurnOffAllViews()
    {
        if (views.StartUserStudyView.view != null ) views.StartUserStudyView.view.SetActive(false);
        views.InterpolationView.view.SetActive(false); 
        views.TouchView.view.SetActive(false);
        views.LoadingView.view.SetActive(false);
        if (views.FinishUserStudyView.view != null) views.FinishUserStudyView.view.SetActive(false); 
        if (views.FinishView.view != null ) views.FinishView.view.SetActive(false);
        views.ConvidenceView.view.SetActive(false); 
    }

    private void TurnOnDominandHand()
    {
        if (hands.rightHandRenderer != null && hands.dominandHandIsRight) {
            hands.rightHandRenderer.enabled = true;
        }
        if (hands.leftHandRenderer != null && !hands.dominandHandIsRight)
        {
            hands.leftHandRenderer.enabled = true;
        }
        
    }

    private void TurnOffNonDominandController()
    {
        if (hands.dominandHandIsRight)
        {
            hands.rightController.SetActive(false); 
            hands.rightRayInteractor.SetActive(false);
        } else
        {
            hands.leftController.SetActive(false);
            hands.leftRayInteractor.SetActive(false);
        }
    }

    private void TurnOffHands()
    {
        if (hands.leftHandRenderer != null) hands.leftHandRenderer.enabled = false; 
        if (hands.rightHandRenderer != null) hands.rightHandRenderer.enabled = false;

    }

    // TODO 
    public void SetConfidenceValue()
    {
        caseData.selectedConfidence = -2; 
    }
  
    #endregion

    #region Controll Interpolation
    public void ForwardMorph()
    {
        //if (almebicPlayer.CurrentTime + playerStepSize >= almebicPlayer.EndTime)
        //{
        //    Debug.Log("Alembic Player set Back to Start Time");
        //    almebicPlayer.CurrentTime = almebicPlayer.StartTime; 
        //}
        //else
        //{
        //    almebicPlayer.CurrentTime += playerStepSize;
        //}
        selectedTimeStampIndex++;
        if (selectedTimeStampIndex >= allSelectableTimeStamps.Length) selectedTimeStampIndex = 0;
        // else if (selectedTimeStampIndex < 0) selectedTimeStampIndex = allSelectableTimeStamps.Length - 1;
        almebicPlayer.CurrentTime = allSelectableTimeStamps[selectedTimeStampIndex]; 
        caseData.currentlySelected = almebicPlayer.CurrentTime; 
    }

    public void BackwardMorph()
    {
        //if (almebicPlayer.CurrentTime <= almebicPlayer.StartTime)
        //{
        //    Debug.Log("Alembic Player set Forward to End Time");
        //    almebicPlayer.CurrentTime = almebicPlayer.EndTime; 
        //}
        //almebicPlayer.CurrentTime -= playerStepSize;
        selectedTimeStampIndex--;
        // if (selectedTimeStampIndex >= allSelectableTimeStamps.Length) selectedTimeStampIndex = 0;
        if (selectedTimeStampIndex < 0) selectedTimeStampIndex = allSelectableTimeStamps.Length - 1;
        almebicPlayer.CurrentTime = allSelectableTimeStamps[selectedTimeStampIndex];
        caseData.currentlySelected = almebicPlayer.CurrentTime; 
    }
    #endregion

    #region Logging 
    private void CreateLogFile()
    {
        string DateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");
        logFilePath = Application.streamingAssetsPath + "/" + directoryOfResults + "/" + logFileName + "-" + userRunId + "-" + DateString + suffix;
        logFilePathEnd = Application.streamingAssetsPath + "/" + directoryOfResults + "/" + logFileName + "-" + userRunId + "-" + DateString + "-end" + suffix;

        Directory.CreateDirectory(Application.streamingAssetsPath + "/" + directoryOfResults + "/");

        if (!File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, GetAllTestcaseOptions() + "\n" + GetallSelectableOptions() + "\n" + GetCSVHeader());
        }
        else
        {
            Debug.LogError("Log File already Exist: " + logFilePath);
        }
    }

    private void AppendCaseDataToCSV(ShiftlyUserStudyBlindTouchCaseData data)
    {
        string prevText = File.ReadAllText(logFilePath);
        string text = prevText;
        text += "\n";
        text += GetValuesAsCSVLine(data);
        File.WriteAllText(logFilePath, text);
    }

    private void WriteAllCaseDatasToCSV()
    {
        string text = GetCSVHeader();
        for (int i = 0; i < caseDatas.Count; i++)
        {
            text += "\n";
            text += GetValuesAsCSVLine(caseDatas[i]);
        }
        File.WriteAllText(logFilePathEnd, text);

    }

    private string GetCSVHeader()
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}", 
            "userRunId", 
            "testcaseConfiguration", 
            "numberOfSteps", 
            "almebicPlayer.StartTime", 
            "almebicPlayer.EndTime", 
            "userStudyTimeStart", 
            "userStudyTimeFinished", 
            "data.testCaseInStudyRun", 
            "data.name",
            "data.testCaseId",
            "data.shiftlyConfiguration", 
            "data.timeStartedTouch", 
            "data.timeEndedTouch", 
            "data.timeStartedInterpolation",
            "data.timeEndedInterpolation",
            "data.timeEnded", 
            "data.startValue", 
            "data.finallySelected", 
            "data.correctAnswer",
            "data.correctAnswer2",
            "data.selectedConfidence",
            "data.convidenceScaleLeftLabel",
            "data.convidenceScaleRightLabel"
            );
    }
    private string GetValuesAsCSVLine(ShiftlyUserStudyBlindTouchCaseData data)
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}", 
            userRunId,
            testcaseConfiguration,
            numberOfSteps,
            almebicPlayer.StartTime,
            almebicPlayer.EndTime,
            userStudyTimeStart,
            userStudyTimeFinished,
            data.testCaseInStudyRun,
            data.name,
            data.testCaseId,
            "\"(" + data.shiftlyConfiguration[0] + "," + data.shiftlyConfiguration[1] + "," + data.shiftlyConfiguration[2] + ")\"", 
            data.timeStartedTouch,
            data.timeEndedTouch,
            data.timeStartedInterpolation,
            data.timeEndedInterpolation,
            data.timeEnded, 
            data.startValue, 
            data.finallySelected,
            data.correctAnswer,
            data.correctAnswer2,
            data.selectedConfidence,
            data.convidenceScaleLeftLabel, 
            data.convidenceScaleRightLabel
            );
    }

    private string GetCurrentTimeAsString()
    {
        return "" + DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //DateTime.Now.ToString("yyyy_m_d_HH_mm"); 
    }

    private string GetallSelectableOptions()
    {
        string text = "// All Selectable Options";
        for (int i = 0; i < numberOfSteps; i++)
        {
            text += ",\n" + allSelectableTimeStamps[i];//(almebicPlayer.StartTime + i * playerStepSize); 
        }
        return text;
    }

    private string GetAllTestcaseOptions()
    {
        float numberOfSelectable = 23.0f; 
        string text = "// All TestCase Options";
        for (int i = 0; i < (int)numberOfSelectable; i++)
        {
            text += "\n" + (almebicPlayer.StartTime + i * (almebicPlayer.EndTime - almebicPlayer.StartTime)/(numberOfSelectable-1));
        }
        return text;
    }
    #endregion
}

#region Unitility Classes 
[System.Serializable]
public class ShiftlyUserSutdyBlindTouchKeyboardInput
{
    public KeyCode next = KeyCode.RightArrow;
    public KeyCode prev = KeyCode.LeftArrow;
    public KeyCode forwardMorph = KeyCode.UpArrow;
    public KeyCode backWardsMorph = KeyCode.DownArrow;
    public KeyCode startUserStudy = KeyCode.Return;
}

[System.Serializable]
public class ShiftlyUserStudyBlindTouchViews
{
    [System.Serializable]
    public class ViewInformation
    {
        public string generalInstructionText;
        public GameObject view;
    }
    [System.Serializable]
    public class TouchViewInformation : ViewInformation
    {
        public GameObject arrowIndicator;
        public GameObject touchPointIndicator;
        public bool displayArrowIndicator = false;
        public bool displayTouchPointIndicator = true; 
    }
    [System.Serializable]
    public class LikerScaleView : ViewInformation
    {
        public SevenPointScaleUIElement likerScaleUIElement; 
    }

    public ViewInformation StartUserStudyView;
    public ViewInformation LoadingView;
    public ViewInformation InterpolationView;
    public TouchViewInformation TouchView;
    public LikerScaleView ConvidenceView; 
    public ViewInformation FinishView;
    public ViewInformation FinishUserStudyView;
}

[System.Serializable]
public class ShiftlyUserStudyBlindTouchAxisSetup
{
    public float thressholdTouchpadInput = 0.8f;
    public float thressholdTouchpadInputFast = 0.9f; 
    public float brakeTillCheck = 0.3f;
    [HideInInspector] public float noControllerInput = 0.0f;
}

public enum ShiftlyUserStudyBlindTouchStates
{
    Start,
    Loading,
    Touch,
    Interpolation,
    Confidence,
    FinishTestCase,
    EndUserStudy
}

public class ShiftlyUserStudyBlindTouchCaseData
{
    public string name = "Not set";
    public int testCaseId = -1; 
    public float[] shiftlyConfiguration = { -1, -1, -1 };
    public string timeStartedTouch = "XXX";
    public string timeEndedTouch = "XXX";
    public string timeStartedInterpolation = "XXX";
    public string timeEndedInterpolation = "XXX"; 
    public string timeEnded = "XXX";
    public float startValue = -1.0f;
    public float currentlySelected = -1.0f;
    public float finallySelected = -1.0f;
    public int testCaseInStudyRun = 0;
    public float correctAnswer = -1.0f;
    public float correctAnswer2 = -2.0f;
    public int selectedConfidence = -1;
    public string convidenceScaleLeftLabel = "XXX";
    public string convidenceScaleRightLabel = "XXX"; 
}

[System.Serializable] 
public class ShiftlyUserSutdyBlindTouchHands
{
    public SkinnedMeshRenderer leftHandRenderer;
    public HandBinder leftHandBinder;

    public SkinnedMeshRenderer rightHandRenderer;
    public HandBinder rightHandBinder;

    public GameObject handTouchIndicator; 
    public bool dominandHandIsRight = true;

    public GameObject leftController;
    public GameObject leftRayInteractor; 

    public GameObject rightController;
    public GameObject rightRayInteractor; 
}

#endregion