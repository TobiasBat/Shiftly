using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls a single folding element 
 */
public class FoldingElementTransformationController : MonoBehaviour
{

    public List<GameObject> motorSideObjects;
    public List<GameObject> otherSideObjects;
    public GameObject largeWheel;
    public GameObject connectingArm; 

    [Range(0.0f, 1.0f)]
    public float extended = 0.0f;

    private float minOffset = 0.0f;
    public float maxOffset = 0.058f;
    
    [HideInInspector]
    public float MaxWidth = 0.0f;
    [HideInInspector]
    public float MinWidth = 0.0f; 

    public Vector3 extensionAxis = new Vector3(0f, 1.0f, 0.0f);

    private List<Vector3> initPosMotorSide = new List<Vector3>();
    private List<Vector3> initPosOtherSide = new List<Vector3>();

    public GameObject topLeft;
    public GameObject bottomLeft; 


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in motorSideObjects)
        {
            Vector3 initPosition = obj.gameObject.transform.localPosition;
            initPosMotorSide.Add(initPosition); 
        }
        foreach (GameObject obj in otherSideObjects)
        {
            Vector3 initPosition = obj.gameObject.transform.localPosition;
            initPosOtherSide.Add(initPosition); 
        }

        // Normalize Extension Axis
        extensionAxis = extensionAxis.normalized;

        Vector3 deltaLengthModule = topLeft.transform.position - bottomLeft.transform.position;
        MinWidth = deltaLengthModule.magnitude;
        MaxWidth = MinWidth + maxOffset; 




    }


    // Update is called once per frame
    void Update()
    {
        TranslateOtherSide();
        RotateWheel();
        // Debug log width of object
        float currentModuleDistance = (topLeft.transform.position - bottomLeft.transform.position).magnitude;
    }

    public void TranslateOtherSide()
    {
 
        for (var i = 0; i < initPosOtherSide.Count; i++)
        {
            Vector3 initPosition = initPosOtherSide[i];
            Vector3 maxPosition = initPosition + extensionAxis * maxOffset;
            Vector3 positon = extended * maxPosition + (1.0f - extended) * initPosition;

            otherSideObjects[i].gameObject.transform.localPosition = positon;
        }
    }

    void RotateWheel()
    {
        largeWheel.transform.localEulerAngles = new Vector3(largeWheel.transform.localEulerAngles.x, largeWheel.transform.localEulerAngles.y, extended * 180.0f);  
    }

    /**
     * Computes the width of the module  
     **/
    public float getCurrentWidth()
    {
        return (topLeft.transform.position - bottomLeft.transform.position).magnitude;
    }
}
