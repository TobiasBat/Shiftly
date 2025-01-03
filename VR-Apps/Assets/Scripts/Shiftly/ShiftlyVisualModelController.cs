using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// Handels the Digital Twin of Shiflty. 
/// </summary>
public class ShiftlyVisualModelController : MonoBehaviour
{
    public StepperControllerClient stepperControllerClient;

    public bool DisplayTouchPointIndicators = true;
    public bool DisplayShiftlyFrameElements = true; 

    public FoldingElementTransformationController foldingElement1;
    public FoldingElementTransformationController foldingElement2;
    public FoldingElementTransformationController foldingElement3;

    // To connect the Edges of the corresponding folding Elemens 
    public GameObject[] motorSideConnected = new GameObject[2];
    public GameObject[] otherSodeConnected = new GameObject[2];
    public GameObject[] shiftlyFrameElements; 

    public GameObject FixedHingeIndicator; 
    public GameObject TopEdgeIndicator;
    public GameObject BackEdgeIndicator;

    public GameObject FrontSideModuleCenterIndicator; 
    public GameObject BackSideModuleCenterIndicator;
    public GameObject BottomSideModuleCenterIndicator;

    public GameObject FrontSideSurfaceIndicator;
    public GameObject BackSideSurfaceIndicator;
    public GameObject BottomSideSurfaceIndicator;

    public bool accuratWidth = false;

    public bool updateVisualController = true; 



    [HideInInspector]
    public float AlphaDegree = 60.0f;  // Inner angle between module 3 and 2, corresponds to angle of 2
    [HideInInspector]
    public float BetaDegree = 60.0f;   // Inner angle between module 3 and 1; corresponds to angle of 1 

    [HideInInspector]
    public Vector3 TopEdge = Vector3.zero;
    [HideInInspector]
    public Vector3 FrontEdge = Vector3.zero;
    [HideInInspector]
    public Vector3 BackEdge = Vector3.zero;

    [HideInInspector]
    public Vector3 FrontSideModuleCenter = Vector3.zero;
    [HideInInspector]
    public Vector3 BackSideModuleCenter = Vector3.zero;
    [HideInInspector]
    public Vector3 BottomSideModuleCenter = Vector3.zero;

    [HideInInspector]
    public Vector3 FrontSideSurfaceCenter = Vector3.zero;
    [HideInInspector]
    public Vector3 BackSideSurfaceCenter = Vector3.zero;
    [HideInInspector]
    public Vector3 BottomSideSurfaceCenter = Vector3.zero; 

    private string positionsSettingsFile = "Shiftly_init_position.txt";



    // Start is called before the first frame update
    void Start()
    {
        FrontEdge = FixedHingeIndicator.transform.localPosition; 

        // Display is Turn off, Remove renderer for all indicators
        if (!DisplayTouchPointIndicators)
        {
            // Endge Indicators
            FixedHingeIndicator.GetComponent<MeshRenderer>().enabled = false;
            TopEdgeIndicator.GetComponent<MeshRenderer>().enabled = false;
            BackEdgeIndicator.GetComponent<MeshRenderer>().enabled = false;
            // Surface Indicators
            FrontSideSurfaceIndicator.GetComponent<MeshRenderer>().enabled = false;
            BackSideSurfaceIndicator.GetComponent<MeshRenderer>().enabled = false;
            BottomSideSurfaceIndicator.GetComponent<MeshRenderer>().enabled = false;
            // Center Indicators 
            FrontSideModuleCenterIndicator.GetComponent<MeshRenderer>().enabled = false;
            BackSideModuleCenterIndicator.GetComponent<MeshRenderer>().enabled = false;
            BottomSideModuleCenterIndicator.GetComponent<MeshRenderer>().enabled = false; 
            // Edge indicators of folding elements 
            foldingElement1.topLeft.GetComponent<MeshRenderer>().enabled = false;
            foldingElement1.bottomLeft.GetComponent<MeshRenderer>().enabled = false; 
           
        }
        if (!DisplayShiftlyFrameElements)
        {
            for ( int i = 0; i < shiftlyFrameElements.Length; i++)
            {
                var obj = shiftlyFrameElements[i];
                obj.SetActive(false);
            }
        }
        updateModuleWidths();
        updateAngles();
        updateEdgePoints();
        updateSidePoints();
        alignHinges();

        // Check if Pose Driver is present 
        var poseDriver = gameObject.GetComponent<TrackedPoseDriver>();
        if (poseDriver.isActiveAndEnabled)
        {
            Debug.Log("Updating Shiftly Position Based on Pose Driver");
        }
        else
        {
            Debug.Log("Setting Position from Pos setting file");
            InitPositionFromPresetFile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateVisualController)
        {
            updateModuleWidths();
            updateAngles();
            updateEdgePoints();
            updateSidePoints();
            alignHinges();
        }
    }

    public Vector3 getOffsetFromFixedEdge(ShiftlyTouchPoint touchPoint)
    {
        ShiftlyTouchPointPosition touchPosition = touchPoint.touchPointPosition;
        switch (touchPosition)
        {
            case ShiftlyTouchPointPosition.FrontSurface:
                return FrontSideSurfaceIndicator.transform.position - FixedHingeIndicator.transform.position;
            case ShiftlyTouchPointPosition.TopEdge:
                return TopEdgeIndicator.transform.position - FixedHingeIndicator.transform.position;
            case ShiftlyTouchPointPosition.BackSurface:
                return BackSideSurfaceIndicator.transform.position - FixedHingeIndicator.transform.position;


        }
            
        return Vector3.zero; 
    }


    /// <summary>
    /// Takes an Shitly touchpoint and returns the indicator gameobject that corresponds to the 
    /// touchpoint.touchPointPosition. E.g. the gameobject that is located in the center of the 
    /// the front surface.
    /// </summary>
    /// <returns>Gameobject of the corresponding touch indicator</returns>
    public GameObject getTouchPointIndicatorObjectFromTouchPoint(ShiftlyTouchPoint touchPoint)
    {
        ShiftlyTouchPointPosition touchPosition = touchPoint.touchPointPosition;
        return GetTouchPointIndicatorObjectFromPointPosition(touchPosition);
    }

    public GameObject GetTouchPointIndicatorObjectFromPointPosition(ShiftlyTouchPointPosition touchPosition)
    {
        switch (touchPosition)
        {
            case ShiftlyTouchPointPosition.FrontSurface:
                return FrontSideSurfaceIndicator;
            case ShiftlyTouchPointPosition.TopEdge:
                return TopEdgeIndicator;
        }
        return FixedHingeIndicator;
    }
    

    private void updateSidePoints()
    {
        // Computing center side points based on the center of the 
        // corresponding edge points. 
        // corresponds to the center of each module 
        FrontSideModuleCenter = (FrontEdge - TopEdge) * 0.5f + TopEdge;
        FrontSideModuleCenterIndicator.transform.localPosition = FrontSideModuleCenter;
        
        BackSideModuleCenter = (BackEdge - TopEdge) * 0.5f + TopEdge;
        BackSideModuleCenterIndicator.transform.localPosition = BackSideModuleCenter;

        BottomSideModuleCenter = (FrontEdge - BackEdge) * 0.5f + BackEdge;
        BottomSideModuleCenterIndicator.transform.localPosition = BottomSideModuleCenter;

        // Computing the position of the center of the paper 
        // When contracted, offset to the frame center point 
        // Because of Curved Folding
        // Source of the computation for the offset h: https://en.wikipedia.org/wiki/Circular_segment

        // Front Side
        Vector3 vFront = (TopEdge - FrontEdge).normalized;
        float nAngleFront = (BetaDegree + 90.0f)* Mathf.Deg2Rad; 
        Vector3 nFront = (new Vector3(
            0.0f,
            vFront.y * Mathf.Sin(nAngleFront) + vFront.x * Mathf.Cos(nAngleFront),
            vFront.y * Mathf.Cos(nAngleFront) - vFront.x * Mathf.Sin(nAngleFront)
            )).normalized;
        float hFront = Mathf.Sqrt(
            Mathf.Pow(foldingElement1.MaxWidth / 2.0f, 2) - 
            Mathf.Pow(foldingElement1.getCurrentWidth() / 2.0f, 2)
            );
        if (!float.IsNaN(hFront))
        {
            FrontSideSurfaceCenter = FrontSideModuleCenter + (nFront * hFront);
        } else
        {
            FrontSideSurfaceCenter = FrontSideModuleCenter; 
        }
        FrontSideSurfaceIndicator.transform.localPosition = FrontSideSurfaceCenter;
        FrontSideSurfaceIndicator.transform.localEulerAngles = foldingElement1.gameObject.transform.localEulerAngles;

        // Back Side
        Vector3 vBack = (TopEdge - BackEdge).normalized;
        float nAngleBack = (-AlphaDegree + 90.0f) * Mathf.Deg2Rad;
        Vector3 nBack = (new Vector3(
            0.0f,
            vBack.y * Mathf.Sin(nAngleBack) + vBack.x * Mathf.Cos(nAngleBack),
            vBack.y * Mathf.Cos(nAngleBack) - vBack.x * Mathf.Sin(nAngleBack)
            )).normalized;
        float hBack = Mathf.Sqrt(
            Mathf.Pow(foldingElement2.MaxWidth / 2.0f,2) -
            Mathf.Pow(foldingElement2.getCurrentWidth() / 2.0f, 2)
            );
        if (!float.IsNaN(hBack))
        {
            BackSideSurfaceCenter = BackSideModuleCenter + (nBack * hBack);
        } else
        {
            BackSideSurfaceCenter = BackSideModuleCenter; 
        }
        BackSideSurfaceIndicator.transform.localPosition = BackSideSurfaceCenter;

        // Bottom Side
        Vector3 nBottom = new Vector3(0.0f, -1.0f, 0.0f);
        float hBottom = Mathf.Sqrt(
            Mathf.Pow(foldingElement3.MaxWidth / 2.0f, 2) -
            Mathf.Pow(foldingElement3.getCurrentWidth() / 2.0f, 2)
            );
        if (!float.IsNaN(hBottom))
        {
            BottomSideSurfaceCenter = BottomSideModuleCenter + (nBottom * hBottom);

        } else
        {
            BottomSideSurfaceCenter = BottomSideModuleCenter; 
        }
        BottomSideSurfaceIndicator.transform.localPosition = BottomSideSurfaceCenter;
    }

    private void updateEdgePoints()
    {
        // Top Edge 
        // Rotating Vector based on angle of folding element 
        // andscaling according to the current length of the element
        TopEdge = (new Vector3(0,
            Mathf.Sin(BetaDegree * Mathf.Deg2Rad),
            Mathf.Cos(BetaDegree * Mathf.Deg2Rad)) 
            * foldingElement1.getCurrentWidth()) + FrontEdge;
        TopEdgeIndicator.transform.localPosition = TopEdge;

        // Back Edge
        // Horizontal offset by module length 3 of the fixed point 
        BackEdge = FrontEdge +
            (new Vector3(0.0f, 0.0f, 1.0f) *
            foldingElement3.getCurrentWidth());
        BackEdgeIndicator.transform.localPosition = BackEdge; 
    }

    private void updateModuleWidths()
    {
        float degree1 = stepperControllerClient.stepper1Degree;
        float degree2 = stepperControllerClient.stepper2Degree;
        float degree3 = stepperControllerClient.stepper3Degree;

        // Update the Widths
        float extend1 = degreeToExetendedLinear(degree1);
        float extend2 = degreeToExetendedLinear(degree2);
        float extend3 = degreeToExetendedLinear(degree3);

        if (accuratWidth)
        {
            extend1 = degreeToExtendedCorrect(degree1); 
            extend2 = degreeToExtendedCorrect(degree2);
            extend3 = degreeToExtendedCorrect(degree3); 
        }

        foldingElement1.extended = extend1;
        foldingElement1.TranslateOtherSide();
        foldingElement2.extended = extend2;
        foldingElement1.TranslateOtherSide();
        foldingElement3.extended = extend3;
        foldingElement3.TranslateOtherSide();
    }

    private void updateAngles()
    {
        // Rotate Folding Elements
        float lengthFoldingElement1 = foldingElement1.getCurrentWidth(); // extend1 * (maxWidthFoldingElement - minWidthFoldingElement) + minWidthFoldingElement;
        float lengthFoldingElement2 = foldingElement2.getCurrentWidth(); // extend2 * (maxWidthFoldingElement - minWidthFoldingElement) + minWidthFoldingElement;
        float lengthFoldingElement3 = foldingElement3.getCurrentWidth(); // extend3 * (maxWidthFoldingElement - minWidthFoldingElement) + minWidthFoldingElement;

        // Source: https://www.calculator.net/triangle-calculator.html?vc=&vx=72&vy=20&va=&vz=20&vb=&angleunits=d&x=51&y=6
        // TODO check and compute by my self
        float alpha = Mathf.Acos((lengthFoldingElement2 * lengthFoldingElement2 + lengthFoldingElement3 * lengthFoldingElement3 - lengthFoldingElement1 * lengthFoldingElement1) /
            (2.0f * lengthFoldingElement2 * lengthFoldingElement3));

        float beta = Mathf.Acos(
            (lengthFoldingElement1 * lengthFoldingElement1 + lengthFoldingElement3 * lengthFoldingElement3 - lengthFoldingElement2 * lengthFoldingElement2) /
            (2.0f * lengthFoldingElement1 * lengthFoldingElement3));

        // Converting to Degree
        AlphaDegree = alpha * 180.0f / Mathf.PI;
        BetaDegree = beta * 180.0f / Mathf.PI;
        float gammaDegree = foldingElement3.gameObject.transform.localEulerAngles.x;
        float foldingElement2Angle = gammaDegree + AlphaDegree + 180.0f;
        float foldingElement1Angle = gammaDegree - BetaDegree - 180.0f;
        // Applieng rotation
        foldingElement1.gameObject.transform.localEulerAngles = new Vector3(foldingElement1Angle, 0, 0);
        foldingElement2.gameObject.transform.localEulerAngles = new Vector3(foldingElement2Angle, 0, 180);

    }

    private void alignHinges()
    {
        // Align Hinges
        Vector3 motorSideTarget = motorSideConnected[0].transform.position;
        Vector3 motorSideSource = motorSideConnected[1].transform.position;
        Vector3 delta = motorSideTarget - motorSideSource;
        GameObject parentOfSource = motorSideConnected[1].transform.parent.gameObject;
        parentOfSource.transform.position += delta;

        Vector3 otherSideTarget = otherSodeConnected[0].transform.position;
        Vector3 otherSideSource = otherSodeConnected[1].transform.position;
        Vector3 deltaOtherSide = otherSideTarget - otherSideSource;
        GameObject parentOfSourceOtherSide = otherSodeConnected[1].transform.parent.gameObject;
        parentOfSourceOtherSide.transform.position += deltaOtherSide;
    }

    /// <summary>
    /// Reads the positions and rotational values of the settings file in streaming assets 
    /// and updates shiftly Position accordingly
    /// </summary>
    private void InitPositionFromPresetFile()
    {
        Vector3 position = Vector3.zero;
        Vector3 eulerAngles = Vector3.zero;
        string path = Application.streamingAssetsPath + "/" + positionsSettingsFile;
        if (File.Exists(path))
        {
            IEnumerable<string> lines = File.ReadLines(path);
            foreach (string line in lines)
            {
                Debug.Log(line);
                string[] words = line.Split(' ');
                if (words.Length == 4)
                {
                    float x = (float)Convert.ToDouble(words[1]);
                    float y = (float)Convert.ToDouble(words[2]);
                    float z = (float)Convert.ToDouble(words[3]);
                    switch (words[0])
                    {
                        case "position":
                            position = new Vector3(x, y, z);
                            break;
                        case "euler":
                            eulerAngles = new Vector3(x, y, z);
                            break;
                        default:
                            Debug.Log("Settings file containes unknown keywords");
                            break;
                    }

                    if (position != Vector3.zero && eulerAngles != Vector3.zero)
                    {
                        gameObject.transform.position = position;
                        gameObject.transform.eulerAngles = eulerAngles;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No positions settings file. Haven't updated Shiftly position");
        }
    }

    /// <summary>
    /// Maps the degree to the normalized extension. User a linear mapping between degree and extension
    /// </summary>
    /// <param name="degree">Motor Rotation in degree, zero degree equals completly contracted</param>
    private float degreeToExetendedLinear(float degree)
    {
        float extended = 0;
        if (degree > 180)
        {
            extended = (360.0f - degree) / 180.0f; 
        } else
        {
            extended = degree / 180.0f; 
        }

        if (extended == 0) return 0.0f;
        // sin( (x * pi) - pi/2 ) / 2 + 0.5
        // extended = Mathf.Sin((extended * Mathf.PI) - Mathf.PI / 2.0f) / 2.0f + 0.5f; 
        return extended;
    }

    /// <summary>
    /// Maps the degree to the normalized extension. Produces correct values. Because no linear relationship betwenn degree and extension.
    /// </summary>
    /// <param name="degree">Motor Rotation in degree, zero degree equals completly contracted</param>
    /// <returns></returns>
    public static float degreeToExtendedCorrect(float degree)
    {
        float hinge = 0.0675f;
        float radiusWheel = 0.0280f;
        float theta = degree * Mathf.Deg2Rad - Mathf.PI / 2.0f;
        // Describes the offset from the motor center in unity units. 
        float extended; 

        if (theta>= 0)
        {
            extended =
                MathF.Sqrt(hinge * hinge - radiusWheel * radiusWheel * Mathf.Pow(MathF.Cos(theta), 2)) + 
                MathF.Sqrt(radiusWheel * radiusWheel - radiusWheel * radiusWheel * MathF.Pow(MathF.Cos(theta),2)); 
        } else
        {
            extended = 
                MathF.Sqrt(hinge * hinge - radiusWheel * radiusWheel * MathF.Cos(theta) * MathF.Cos(theta)) -
                MathF.Sqrt(radiusWheel * radiusWheel - radiusWheel * radiusWheel * MathF.Cos(theta) * MathF.Cos(theta));
           
        }
        // Map to the interval and than normalize
        float extendedNorm = (extended - (hinge - radiusWheel)) / ((hinge + radiusWheel) - (hinge - radiusWheel));
        return extendedNorm; 
    }
}
