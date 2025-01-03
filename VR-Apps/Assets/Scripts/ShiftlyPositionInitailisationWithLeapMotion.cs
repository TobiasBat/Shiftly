using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// To initilise the position of shiftly with leap motion controller
/// This is only usefull when shiftly is not constantly tracked
/// and savely mounted on table.
/// </summary>
public class ShiftlyPositionInitailisationWithLeapMotion : MonoBehaviour
{
    public GameObject Shiftly; 
    public LeapProvider leapProvider;
    public HandModelBase HandModelBaseLeft;
    private GameObject startPointIndicator;
    private GameObject endPointIndicator;
    private GameObject dragPointIndicator;
    [SerializeField] private KeyCode keyCodeToPressForActivation;
    private string positionsSettingsFile = "Shiftly_init_position.txt"; 

    private Vector3 onSpaceDownPosition = Vector3.zero; 


    // Start is called before the first frame update
    void Start()
    {
        Vector3 indicatorScaleVector = new Vector3(0.02f, 0.02f, 0.02f); 
        startPointIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        startPointIndicator.name = "Leap Motion Shiftly Top Edge Start point indicator"; 
        startPointIndicator.transform.localScale = indicatorScaleVector;
        endPointIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        endPointIndicator.name = "Leap Motion Shiftly Top Edge End point indicator";
        endPointIndicator.transform.localScale = indicatorScaleVector;
        dragPointIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        endPointIndicator.name = "Leap Motion Shiftly Top Edge Drag point indicator";
        dragPointIndicator.transform.localScale = indicatorScaleVector;
        startPointIndicator.SetActive(false);
        endPointIndicator.SetActive(false);
        dragPointIndicator.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        handleKeyInputCallibration();
    }

    Vector3 getLeftHandTipPosition()
    {
        for (int i = 0; i < leapProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand _hand = leapProvider.CurrentFrame.Hands[i];

            if (_hand.IsLeft)
            {
                Finger _index = _hand.GetIndex();
                Vector3 tipPos = _index.TipPosition;
                return tipPos; 
            }
        }
        return Vector3.zero; 
    }

    void handleKeyInputCallibration()
    {
        if (Input.GetKeyDown(keyCodeToPressForActivation))
        {
            startPointIndicator.SetActive(true);
            endPointIndicator.SetActive(true);
            dragPointIndicator.SetActive(true); 
            turnOnRenderingForIndicators();
            onSpaceDownPosition = getLeftHandTipPosition();
            startPointIndicator.transform.position = onSpaceDownPosition;
            endPointIndicator.transform.position = onSpaceDownPosition;
            Debug.Log("Getting an Shiftly position"); 
        }
        else if (Input.GetKey(keyCodeToPressForActivation))
        {
            startPointIndicator.SetActive(true);
            endPointIndicator.SetActive(true);
            dragPointIndicator.SetActive(true); 
            dragPointIndicator.transform.position = getLeftHandTipPosition();
        } 
        else if (Input.GetKeyUp(keyCodeToPressForActivation))
        {
            Vector3 endPosition = getLeftHandTipPosition();
            endPointIndicator.transform.position = endPosition;
            PerfomAlignmentOfShiftly(onSpaceDownPosition, endPosition);
            turnOffRenderingForIndicators();
            startPointIndicator.SetActive(false);
            endPointIndicator.SetActive(false);
            dragPointIndicator.SetActive(false); 
        }
    }

    private void WritePositionSettingsFile()
    {
        string settingsFilePath = Application.streamingAssetsPath + "/" + positionsSettingsFile; 

        if (!File.Exists(settingsFilePath))
        {
            Debug.Log("Writing new Shiftly Position File");   
        }
        else
        {
            Debug.Log("Overwriting Shiftly Position File");
        }
        string content = "position " + Shiftly.transform.position.x + " " + 
            Shiftly.transform.position.y + " " +
            Shiftly.transform.position.z + "\n";
        content += "euler " + Shiftly.transform.eulerAngles.x + " " +
            Shiftly.transform.eulerAngles.y + " " +
            Shiftly.transform.eulerAngles.z; 
        File.WriteAllText(settingsFilePath, content);
    }

    void PerfomAlignmentOfShiftly(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3 delta = endPosition - startPosition;
        if (delta.magnitude < 0.1f )
        {
            return; 
        }
        Vector3 pos = delta / 2.0f + startPosition;
        Shiftly.transform.rotation = Quaternion.LookRotation(delta);
        Shiftly.transform.eulerAngles += new Vector3(0, 180, 0);

        ShiftlyVisualModelController shiftlyController = Shiftly.GetComponent<ShiftlyVisualModelController>();
        Vector3 offset = Shiftly.transform.position - shiftlyController.TopEdgeIndicator.transform.position; 
        Shiftly.transform.position = pos + new Vector3(offset.x, offset.y, offset.z);
        WritePositionSettingsFile(); 
    }

    void turnOffRenderingForIndicators()
    {
        MeshRenderer renderer = startPointIndicator.GetComponent<MeshRenderer>();
        renderer.enabled = false; 
        MeshRenderer rendererEndPoint = endPointIndicator.GetComponent<MeshRenderer>();
        rendererEndPoint.enabled = false; 
        MeshRenderer renderDragPoint = dragPointIndicator.GetComponent<MeshRenderer>();
        renderDragPoint.enabled = false;
    }   

    void turnOnRenderingForIndicators()
    {
        MeshRenderer renderer = startPointIndicator.GetComponent<MeshRenderer>();
        renderer.enabled = true;
        MeshRenderer rendererEndPoint = endPointIndicator.GetComponent<MeshRenderer>();
        rendererEndPoint.enabled = true;
        MeshRenderer renderDragPoint = dragPointIndicator.GetComponent<MeshRenderer>();
        renderDragPoint.enabled = true;
    }
}
