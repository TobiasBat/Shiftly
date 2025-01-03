using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiftly_Kinematics : MonoBehaviour
{
    public StepperControllerClient StepperControllerClient;
    public FoldingElementTransformationController FoldingElement1;  // Back Facing Element  
    public FoldingElementTransformationController FoldingElement2;  // Front Facing Element
    public FoldingElementTransformationController FoldingElement3; // Botton element 

    // Indicators 
    public GameObject FixedPointIndicator;
    public GameObject TopEdgeIndicator;
    public GameObject FrontFaceIndicator;

    [HideInInspector]
    public float AlphaDegree = 60.0f;  // Inner angle between module 3 and 2, corresponds to angle of 2
    [HideInInspector]
    public float BetaDegree = 60.0f;   // Inner angle between module 3 and 1; corresponds to angle of 1 

    


    // Start is called before the first frame update
    void Start()
    {
        updateAngles();
        updateEdges();
        updateEdgeIndicators(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void updateAngles()
    {

    }

    private void updateEdges()
    {

    }

    private void updateEdgeIndicators()
    {

    }

    
}
