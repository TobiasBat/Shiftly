using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEditor.VersionControl;
using UnityEngine.XR.LegacyInputHelpers;
using UnityEditor;

[System.Serializable]
public class UserStudyKeyobardIntput
{
    public KeyCode JumpToInterpolationScreen = KeyCode.DownArrow;
    public KeyCode IncreaseInterpolationValue = KeyCode.RightArrow;
    public KeyCode DecreaseInterpolationValue = KeyCode.LeftArrow;
    public KeyCode SaveTestCase = KeyCode.S; 
}

public class UserStudyTestCase : MonoBehaviour
{
    #region varibles
    public GameObject touchGameObject;
    public Leap.Unity.Interaction.InteractionSlider slider;
    public Slider canvasSlider;
    public GameObject InterPolatorPrefab;

    private GameObject GameObjectInterpolator; 
    private IndexBasedLinearInterpolation GeometryInterpolator;

    private GameObject instantedTouchGameObject; 
    [HideInInspector] public touchableObject touchObject; 

    public string testcaseName;
    public GameObject interpolationObject_a;
    public GameObject interPolationObject_b;
    [HideInInspector] public string object_a;
    [HideInInspector] public Mesh mesh_a;
    [HideInInspector] public string object_b;
    [HideInInspector] public Mesh mesh_b;
    public float correctValue;
    [HideInInspector]public float selectedValue;
    private int numberOfDiscreteSteps = 20; 
    public GameObject UIObject;
    public GameObject InterpolatingObject;
    public UserStudyKeyobardIntput keyBoardInputs;

    // Times
    [HideInInspector] public float testCaseStartedTime; // The time where the new testcase was set active. Consider when informaton to touch the object was given. 
    [HideInInspector] public float userStartTouchingObjectTime = -1.0f;
    [HideInInspector] public float userStopsTouchingObjectTime = -1.0f;
    [HideInInspector] public float buttonPressedTime;
    [HideInInspector] public float testCaseEndedTime;

    private ShiftlyVisualModelController VisualModellOfShiftly;

    private bool jumpToInterpolationScreenAutomatically = false; 

    private UserStudyManager userStudyManager; 

    private bool updateSliderValueEveryFrame = false;
    private float updateSliderValueStepByKeyboard = 0.05f;
    private float offsetSliderValue = 0.3f; 

    // Submit Button 
    bool buttonIsCurrentlyPressed = false;
    float timeTillButtonFires = 1.0f;


    #endregion

    void Start()
    {

        // Find Object 
        mesh_a = interpolationObject_a.GetComponentInChildren<MeshFilter>().sharedMesh;
        mesh_b = interPolationObject_b.GetComponentInChildren<MeshFilter>().sharedMesh;
        object_a = interpolationObject_a.name;
        object_b = interPolationObject_b.name;

        Debug.Log("------------------"); 
        Debug.Log(object_a);
        Debug.Log(object_b);
        Debug.Log("---------------------");


        Debug.Log("Init new TestCase: " + testcaseName);


        // touchObject = touchGameObject.GetComponent<touchableObject>();
        GameObjectInterpolator = Instantiate(InterPolatorPrefab, Vector3.zero, Quaternion.identity);

        GameObjectInterpolator.transform.localPosition = InterpolatingObject.transform.localPosition;
        GameObjectInterpolator.transform.localEulerAngles = InterpolatingObject.transform.localEulerAngles;
        GameObjectInterpolator.transform.parent = InterpolatingObject.transform;
        GeometryInterpolator = GameObjectInterpolator.GetComponent<IndexBasedLinearInterpolation>();
        GeometryInterpolator.mesh0 = mesh_a;
        GeometryInterpolator.mesh1 = mesh_b;
        GeometryInterpolator.Init();

    }

    void Update()
    {
        KeyBoardControlls(); 
    }

    /*
     * Starts testcase, updates shiftly to testobject
     */
    public void InitTestCase(ShiftlyVisualModelController visualModellOfShiftly, UserStudyManager _userStudyManager)
    {
        Debug.Log("Starting the Testcase: " + testcaseName);
        userStudyManager = _userStudyManager; 

        // Init Touchobject
        instantedTouchGameObject = Instantiate(touchGameObject, Vector3.zero, Quaternion.identity);
        touchObject = instantedTouchGameObject.GetComponent<touchableObject>();
        touchObject.TurnOffTouchObjectRendering(); 
        instantedTouchGameObject.transform.parent = gameObject.transform;
        instantedTouchGameObject.transform.localPosition = Vector3.zero;

        VisualModellOfShiftly = visualModellOfShiftly;
        AlignTestcaseWithShiftly(visualModellOfShiftly);

        // Disable Slide UI 
        UIObject.SetActive(false);
        InterpolatingObject.SetActive(false);
        touchObject.TurnOffTouchIndicatorRendering(); 
    }

    /*
     * Starts the touching part of the test case 
     */
    public void StartTestCase() 
    {
        AlignTestcaseWithShiftly(VisualModellOfShiftly);

        XRSimpleInteractable xrTouchpintInteractable = touchObject.touchIndicator.GetComponent<XRSimpleInteractable>();
        xrTouchpintInteractable.hoverEntered.AddListener(UserStartsTouchingObject);
        xrTouchpintInteractable.hoverExited.AddListener(UserStopsHoverEvent);
        Debug.Log(xrTouchpintInteractable);
        touchObject.TurnOnTouchIndicatorRendering();
        testCaseStartedTime = Time.time;
    }
    
    public void IncreaseInterpolationValue()
    {
        selectedValue += 1.0f / (float)numberOfDiscreteSteps;
        UpdateInterpolating();
    }

    public void DecreaseteInterpolationValue()
    {
        selectedValue -= 1.0f / (float)numberOfDiscreteSteps;
        UpdateInterpolating();
    }

    private void UpdateInterpolating()
    {
        if (GeometryInterpolator.interValue != selectedValue)
        {            
            float mappedValue = (Mathf.Cos(selectedValue * Mathf.PI + Mathf.PI)) / 2.0f + 0.5f;
            GeometryInterpolator.updateInterpolateMesh(mappedValue);
        }
    }

    /*
     * After Calling Function the slider value and the geometry is updated every frame.
     */
    public void StartToUpdateSliderVale()
    {
        updateSliderValueEveryFrame = true; 

    }
    /*
     * After Calling this Function the slider value and the geometry is not updated every frame.
     */
    public void StopToUpdateSliderValue()
    {
        updateSliderValueEveryFrame = false; 
    }

    /*
     * Function is called to record the time wehn user starts to touch object
     */
    public void UserStartsTouchingObject(HoverEnterEventArgs arg0)
    {
        if (userStartTouchingObjectTime < 0.0f)
        {
            userStartTouchingObjectTime = Time.time;
            touchObject.TurnOnTouchInteraction();
        } else
        {
            Debug.Log("User has already touched the object once"); 
        }
    }

    private void UserStopsHoverEvent(HoverExitEventArgs arg0)
    {
        if (userStartTouchingObjectTime > 2.0f)
        {
            UserStopsTouchingObject();
        }

    }

    /// <summary>
    /// Function is called to record the time when the user stops to touch the object 
    /// Displays UI for the value input
    /// </summary>
    public void UserStopsTouchingObject()
    {
        if (userStopsTouchingObjectTime < 0.0f)
        {
            userStopsTouchingObjectTime = Time.time;
            if (jumpToInterpolationScreenAutomatically) DisplayInterpolationScreen(); 

        } else
        {
            Debug.Log("User has alread stops touching the object"); 
        }
    }



    private void AlignTestcaseWithShiftly(ShiftlyVisualModelController visualModell)
    {
        touchObject.turnOnTouchIndicator();

        // GameObject touchPointObject = touchObject.shiftlyTouchPoint;
        // ShiftlyTouchPoint shiftlyTouchPoint = touchPointObject.GetComponent<ShiftlyTouchPoint>(); // Which side / edge if shiftly is touched
        // Vector3 offSet = visualModell.getOffsetFromFixedEdge(shiftlyTouchPoint);
        // Vector3 deltaTouchPointAndTestCase = gameObject.transform.position - touchPointObject.transform.position;
        // 
        // gameObject.transform.position += deltaTouchPointAndTestCase + offSet; 

        ShiftlyTouchPoint currentTouchPoint = touchObject.shiftlyTouchPoint.GetComponent<ShiftlyTouchPoint>();
        GameObject shiftlyTouchIndicator = visualModell.getTouchPointIndicatorObjectFromTouchPoint(currentTouchPoint);
        // Rotate testcase
        // gameObject.transform.eulerAngles = shiftlyTouchIndicator.transform.eulerAngles; 
        // Reset the rotations of all parts of the testcase 
        instantedTouchGameObject.transform.localEulerAngles = Vector3.zero;
        GameObjectInterpolator.transform.localEulerAngles = Vector3.zero;

        // Align Shape
        gameObject.transform.position -= touchObject.shiftlyTouchPoint.transform.position - shiftlyTouchIndicator.transform.position; 
    }

    #region IO
    public void submitButtonPressed()
    {
        Debug.Log("Pressed The Button");
        buttonIsCurrentlyPressed = true;
        buttonPressedTime = Time.time; 
    }

    public void submitButtonReleased()
    {
        Debug.Log("Released the submit button");
        buttonIsCurrentlyPressed = false;
        float timePressed = Time.time - buttonPressedTime; 
        Debug.Log("Button Was pressed: " + (Time.time - buttonPressedTime)); 
        if (timePressed > timeTillButtonFires)
        {
            Debug.Log("Button for testcase " + testcaseName +  " was pressed and fired");
            testCaseEndedTime = Time.time; 
            selectedValue = slider.HorizontalSliderValue;
            userStudyManager.SubmittingTestCase(this); 
        }

    }
    public void FastForwardSubmitButtonPressed()
    {
        testCaseEndedTime = Time.time;
        selectedValue = slider.HorizontalSliderValue;
        userStudyManager.SubmittingTestCase(this);
    }

    private void DisplayInterpolationScreen()
    {
        touchObject.TurnOffTouchIndicatorRendering();
        UIObject.SetActive(true);
        InterpolatingObject.SetActive(true);
        userStudyManager.TextInfoClear();
    }

    private void KeyBoardControlls()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     submitButtonPressed(); 
        // } 
        // 
        // if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        //     slider.HorizontalSliderValue = slider.HorizontalSliderValue - updateSliderValueStepByKeyboard;
        //     UpdateSliderValue(); 
        // }
        // 
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     slider.HorizontalSliderValue = slider.HorizontalSliderValue + updateSliderValueStepByKeyboard;
        //     UpdateSliderValue();
        // }
        // 
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     UserStopsTouchingObject();
        // }
        // 
        // if (Input.GetKeyUp(KeyCode.Space))
        // {
        //     submitButtonReleased(); 
        // }
        // 
        if (Input.GetKeyUp(keyBoardInputs.JumpToInterpolationScreen))
        {
            DisplayInterpolationScreen();
        }
        if (Input.GetKeyUp(keyBoardInputs.IncreaseInterpolationValue))
        {
            IncreaseInterpolationValue(); 
        }
        if (Input.GetKeyUp(keyBoardInputs.DecreaseInterpolationValue)) { 
            DecreaseteInterpolationValue();
        }
        if (Input.GetKeyUp(keyBoardInputs.SaveTestCase))
        {
            FastForwardSubmitButtonPressed(); 
        }
    }
    #endregion
}

