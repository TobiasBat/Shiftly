using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class UserStudyInterpolationObjectPaths
{
    public string nameOfPrefab_a; 
    public string nameOfPrefab_b;
}
public class UserStudyManager : MonoBehaviour
{
    #region Varibles
    public string Directory = "User-study-test-logs"; 
    public string LogFileName = "userstudy-run";
    public string UserStudyRun = "1";

    public GameObject testCasePrefab;
    public List<UserStudyInterpolationObjectPaths> userStudyInterpolationObjectPaths; 
    public List<UserStudyTestCase> userStudyTestCases;

    public StepperControllerClient stepperControllerClient;
    public ShiftlyVisualModelController shitlyModel;
    public GameObject objectToTrack;

    public Canvas testcaseCanvas;
    public TextMeshProUGUI testCaseText;  

    private int currentTestCaseIndex = 0; 

    private float timeTillTestStartSignAppears = 2.0f;

    private UserStudyLogging logger;
    private string logFileSufffix = ".json"; // file suffix of the log file
    #endregion

    #region MonoBehaviour 
    void Start()
    {
        // Creating new Logger for the testcase
        logger = new UserStudyLogging(Directory, LogFileName, UserStudyRun, logFileSufffix);

        // Disable All Testcases 
        foreach(UserStudyTestCase testcase in userStudyTestCases)
        {
            testcase.gameObject.SetActive(false); 
        }

        // Display the first testcase 
        displayTestCase();
    }
    void Update()
    {
        // Move circle to fit shfitly 
        gameObject.transform.position = objectToTrack.transform.position;
        gameObject.transform.rotation = objectToTrack.transform.rotation;
    }
    #endregion

    #region Handling test cases
    public void SubmittingTestCase(UserStudyTestCase testCase)
    {
        Debug.Log("Submitting a new Testcase");
        logger.WriteDownUserStudyTestCase(testCase);
        userStudyTestCases[currentTestCaseIndex].gameObject.SetActive(false);
        NextTestCase();
    }

    /// <summary>
    /// Prepairs testcases
    /// Updates the physical shiftly and starts the testcase after @timeTillTestStartSignAppears
    /// </summary>
    private void displayTestCase()
    {
        userStudyTestCases[currentTestCaseIndex].gameObject.SetActive(true); 
        UserStudyTestCase currentTestCase = userStudyTestCases[currentTestCaseIndex];
        currentTestCase.InitTestCase(shitlyModel, this);
        
        // Update Physical Shiftly
        touchableObject touchObject = currentTestCase.touchObject;
        UpdatePhysicalPrototype(touchObject);

        // update Text
        TextInfoPrepairingTestcase();
        Invoke("TextInfoTouchObject", timeTillTestStartSignAppears); // Time where shiftly should be in correct configuration
        Invoke("StartCurrentTestCase", timeTillTestStartSignAppears); 
    }

    /// <summary>
    ///  Starts the toucting part of the new Testcase
    /// </summary>
    private void StartCurrentTestCase()
    {
        userStudyTestCases[currentTestCaseIndex].StartTestCase(); 
    }
    private void NextTestCase()
    {
        currentTestCaseIndex++;
        if (currentTestCaseIndex == userStudyTestCases.Count)
        {
            logger.FinsihLogFile();
            TextInfoSubmittingTestResult();
            Invoke("TextInfoFinishStudy", 2.0f); 
        } else
        {
            TextInfoSubmittingTestResult();
            Invoke("displayTestCase", 2.0f);
        }
       
    }
    #endregion

    #region Physcial Shiftly controll
    private void UpdatePhysicalPrototype(touchableObject obj)
    {
        ShiftlyTouchPoint touchPoint = obj.shiftlyTouchPoint.GetComponent<ShiftlyTouchPoint>();
        if (touchPoint)
        {
            Debug.Log("Transforming Shiftly to: " + touchPoint.side_1_extension + ", " + touchPoint.side_2_extension + ", " + touchPoint.side_3_extension);
            stepperControllerClient.setAndUploadextensionValues("1", touchPoint.side_1_extension, stepperControllerClient.stepper1Speed);
            stepperControllerClient.setAndUploadextensionValues("2", touchPoint.side_2_extension, stepperControllerClient.stepper2Speed);
            stepperControllerClient.setAndUploadextensionValues("3", touchPoint.side_3_extension, stepperControllerClient.stepper3Speed);

        }
        else
        {
            Debug.LogError("Touchobject has not Shiftly Touch Point assigned");
        }
    }
    #endregion

    #region Handling User Study Manager Canvas input filed
    private void TextInfoPrepairingTestcase()
    {
        Debug.Log("Prepairing new testcase"); 
        testCaseText.text = "Prepairing new Testcase ... "; 
    }

    private void TextInfoTouchObject()
    {
        testCaseText.text = "Touch Object";
    }

    private void TextInfoSubmittingTestResult()
    {
        testCaseText.text = "Submitting testcase"; 
    }

    private void TextInfoFinishStudy()
    {
        testCaseText.text = "User study completed"; 
    }

    public void TextInfoClear()
    {
        testCaseText.text = ""; 
    }
    # endregion

}
