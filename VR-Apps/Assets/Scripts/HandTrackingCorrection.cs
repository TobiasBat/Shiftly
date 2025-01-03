using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandTrackingCorrection : MonoBehaviour
{

    public bool correctRightHandPositionWithTracker = true; 
    public GameObject rightHandWrist;
    public GameObject rightHandTracker;
    public InputActionProperty rightHandCallibrationInputProperty;
    public Vector3 rightHandDelta = new Vector3(0.04f, 0.05f, -0.03f);

    public Material trackerCallibrationMaterial;

    private MeshRenderer rightHandTrackerRenderer; 
    private Material trackerAktiveMaterial; 

    private bool callibratingRightHand = false; 
    private InputAction rightHandCallibrationAction;
    private List<Vector3> rightHandCallibrationDelta;
    private List<Vector3> rightHandCallibrationRotation; 
    

    // Start is called before the first frame update
    void Start()
    {
        rightHandCallibrationAction = rightHandCallibrationInputProperty.action;
        rightHandTrackerRenderer = rightHandTracker.GetComponentInChildren<MeshRenderer>();
        trackerAktiveMaterial = rightHandTrackerRenderer.material; 
    }

    private void LateUpdate()
    {
        if (correctRightHandPositionWithTracker)
        {
            RightHandCorrection(); 
        }
    }

    private void RightHandCorrection()
    {
        if (rightHandCallibrationAction.ReadValue<float>() > 0.5f)
        {
            if (!callibratingRightHand)
            {
                // First frame after right hand callibration button was pressed
                callibratingRightHand = true;
                rightHandCallibrationDelta = new List<Vector3>();
                rightHandCallibrationRotation = new List<Vector3>(); 
                rightHandTrackerRenderer.material = trackerCallibrationMaterial; 
            }

            // Computing Delta at frame and storing
            var delta = rightHandWrist.transform.localPosition - rightHandTracker.transform.localPosition;
            Vector3 rotatedDelta = (rightHandTracker.transform.localRotation) * delta; 
            // Vector3 rotatedDelta = delta;
            rightHandCallibrationDelta.Add(
              rotatedDelta  
            );

            Vector3 rotationDelta = (rightHandWrist.transform.eulerAngles - rightHandTracker.transform.eulerAngles);
            rightHandCallibrationRotation.Add(rotationDelta);

        }
        else if (callibratingRightHand)
        {
            // Frame after right hand callibration button was released
            callibratingRightHand = false;
            // Perform the allignment
            PerformRightHandCallibrationAllignment();
            rightHandTrackerRenderer.material = trackerAktiveMaterial; 
        }

        if (!callibratingRightHand)
        {
            // Assigns the Possitional Delta
            // rightHandWrist.transform.position = rightHandTracker.transform.position + (rightHandTracker.transform.rotation * constRightRotation) * rightHandDelta;
            // rightHandWrist.transform.position = rightHandTracker.transform.position + (rightHandTracker.transform.rotation) * rightHandDelta;
            // rightHandWrist.transform.position = rightHandTracker.transform.position + rightHandTracker.transform.rotation * (new Vector3(0.0f,1.0f,-1.0f)).normalized * rightHandDelta.magnitude;
            // rightHandWrist.transform.position = rightHandTracker.transform.position + rightHandTracker.transform.rotation * (new Vector3(-rightHandDelta.x, -rightHandDelta.y, -rightHandDelta.z));
            rightHandWrist.transform.position = rightHandTracker.transform.position + rightHandTracker.transform.rotation * (new Vector3(rightHandDelta.x, rightHandDelta.y, rightHandDelta.z));
        }
    }

    // Does Not work Yet!
    private void PerformRightHandCallibrationAllignment()
    {
        Debug.Log("Performing the right hand allignment");
        Vector3 deltaSum = Vector3.zero; 
        foreach (Vector3 delta in rightHandCallibrationDelta)
        {
            deltaSum += delta;
        }
        Vector3 avgDelta = deltaSum / (float)rightHandCallibrationDelta.Count;
        rightHandDelta = avgDelta; 


    }
}
