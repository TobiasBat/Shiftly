using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AnimationSceneKeyboardShortCuts
{
    public KeyCode startCurrentlySelectAnimated = KeyCode.T;
    public KeyCode selectNextAnimation = KeyCode.RightArrow; 
}

public class ShiftlyAnimationSceneController : MonoBehaviour
{

    public List<AnimatedTouchableObject> animatedTouchableObjects;
    public ShiftlyVisualModelController shiftlyVisualModelController;
    public StepperControllerClient stepperControllerClient;
    public AnimationSceneKeyboardShortCuts keyboardShortCuts; 
    public int currentlyTrackedObjectIndex = -1;

    private bool animationIsRunning = false;
    public int animationFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (animatedTouchableObjects.Count > 0 && currentlyTrackedObjectIndex == -1)
        {
            currentlyTrackedObjectIndex = 0;
        }
        LoadCurrentlySelectedAnimationFrameToShiftly(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyTrackedObjectIndex > -1)
        {
            AlignAnimatedGameObjectWithShiftly();
        }
        CheckKeyboardInput();
    }

    private void AlignAnimatedGameObjectWithShiftly()
    {
        AnimatedTouchableObject animatedObject = animatedTouchableObjects[currentlyTrackedObjectIndex];       
        var touchPointPositionOnShiftly = animatedObject.animatedTochPointComponent.touchPointPosition;

        Vector3 touchPointPosition = animatedObject.shiftlyAnimatedTouchPoint.transform.position;

        switch (touchPointPositionOnShiftly)
        {
            case ShiftlyTouchPointPosition.TopEdge:
                Vector3 indicatorPosition = shiftlyVisualModelController.TopEdgeIndicator.transform.position;
                Vector3 delta = touchPointPosition - indicatorPosition;
                animatedObject.gameObject.transform.position -= delta;

                animatedObject.gameObject.transform.eulerAngles = shiftlyVisualModelController.gameObject.transform.eulerAngles;
                break;
            case ShiftlyTouchPointPosition.FrontSurface:
                animatedObject.gameObject.transform.eulerAngles = new Vector3(
                    0,
                    90.0f + shiftlyVisualModelController.FrontSideSurfaceIndicator.transform.eulerAngles.y,
                    90.0f + shiftlyVisualModelController.FrontSideSurfaceIndicator.transform.eulerAngles.x
                );



                Vector3 indicatorFront = shiftlyVisualModelController.FrontSideSurfaceIndicator.transform.position;
                animatedObject.gameObject.transform.position -= (touchPointPosition - indicatorFront);

                // Vector3 frontSideRotation = 
                // animatedObject.gameObject.transform.eulerAngles = shiftlyVisualModelController.gameObject.transform.eulerAngles + shiftlyVisualModelController.FrontSideSurfaceIndicator.transform.localEulerAngles;
                
                break;
            default:
                Debug.Log("Touch Point not Implemented");
                break; 
        }
       
       
    }

    private void LoadCurrentlySelectedAnimationFrameToShiftly()
    {
        Debug.Log("UPLOADING FRAME " + animationFrame);
        AnimatedTouchableObject animatedObject = animatedTouchableObjects[currentlyTrackedObjectIndex];

        var frame = animatedObject.animatedTochPointComponent.frames[animationFrame];
        animationFrame++;
        stepperControllerClient.setAndUploadDegree("1", frame.side_1_degree, frame.stepperMotorSpeeds);
        stepperControllerClient.setAndUploadDegree("2", frame.side_2_degree, frame.stepperMotorSpeeds);
        stepperControllerClient.setAndUploadDegree("3", frame.side_3_degree, frame.stepperMotorSpeeds);
        
    }

    /// <summary>
    /// Starts the currently selected Animation
    /// Communicates with Shiftly
    /// </summary>
    public void StartAnimationCurrentlySelectedObject() {
        
        if (!animationIsRunning)
        {
            Debug.Log("Starting Animation");
            
            // Stop updating the touch point indicator positions after each frame.            
            shiftlyVisualModelController.updateVisualController = false; 

            animationIsRunning = true; // ensure that animation can only be started once
            
            AnimatedTouchableObject animatedObject = animatedTouchableObjects[currentlyTrackedObjectIndex];
            
            animatedObject.turnOffTouchIndicator(); 
            animatedObject.playAnimation(); 

            var frames = animatedObject.animatedTochPointComponent.frames;

            // The first duration Equals the offset when the second frame should be envoked
            var accumulatedSeconds = 0.0f; // animatedObject.animatedTochPointComponent.frames[0].animationDuration; 
            
            // Sends the transformation commands after the defined durations
            // Starts at 1 because 0 describes the starting position the is loaded when animation is selected
            // TODO the last Invoke is never fired; Dont know why
            for (int i = 0; i < frames.Count; i++)
            {
                
                Debug.Log("Initialising " + i + " will be invoked after: " + accumulatedSeconds);
                Invoke("LoadCurrentlySelectedAnimationFrameToShiftly", accumulatedSeconds);
                accumulatedSeconds += animatedObject.animatedTochPointComponent.frames[i].animationDuration * animatedObject.animatedTochPointComponent.animationSpeed;


                Debug.Log(accumulatedSeconds); 
            }
            // Invoke("LoadCurrentlySelectedAnimationFrameToShiftly", accumulatedSeconds);
            Invoke("AnimationDone", accumulatedSeconds + 3.0f); 
        }
    }

    private void AnimationDone()
    {
        Debug.Log("Animation is Done"); 
        shiftlyVisualModelController.updateVisualController = true; 
       // // animationFrame = 0; // thats problematic because if the timing is not right
       animationIsRunning = false;
       // 
       AnimatedTouchableObject animatedObject = animatedTouchableObjects[currentlyTrackedObjectIndex];
       animatedObject.turnOnTouchIndicator();
    }

    private void CheckKeyboardInput()
    {
        if (Input.GetKeyUp(keyboardShortCuts.startCurrentlySelectAnimated))
        {
            animationFrame = 0;
            StartAnimationCurrentlySelectedObject();
        }
        if (Input.GetKeyUp(keyboardShortCuts.selectNextAnimation))
        {
            if (animationIsRunning)
            {
                Debug.Log("Animation is currently Running"); 
            } else
            {
                currentlyTrackedObjectIndex = 0;
                animationFrame = 0;
                // Load the First 
                LoadCurrentlySelectedAnimationFrameToShiftly();
            }
           
        }
    }

}


