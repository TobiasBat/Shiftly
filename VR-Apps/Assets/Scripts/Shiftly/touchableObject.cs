using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchableObject : MonoBehaviour
{

    public GameObject visualRepresentation;
    public GameObject touchIndicator;
    public GameObject shiftlyTouchPoint;
    public Vector3 offsetFromCenter = new Vector3(0.0f, 0.0f, 0.0f);
    public bool RenderVisualRepresentation = true;


    // Start is called before the first frame update
    void Start()
    {
        if (!RenderVisualRepresentation)
        {
            TurnOffTouchObjectRendering();
            TurnOffTouchIndicatorRendering(); 
       }   
    }

    /*
     * Disables the visual representation of the touch surface and the touch indicator
     */
    public void TurnOffTouchObjectRendering()
    {
        var meshRenderer = visualRepresentation.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public void TurnOffTouchIndicatorRendering()
    {
        var touchIndicaterMeshrender = touchIndicator.GetComponent<MeshRenderer>();
        touchIndicaterMeshrender.enabled = false;
    }

    public void TurnOnTouchObjectRendering()
    {
        var meshRenderer = visualRepresentation.GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
    }

    public void TurnOnTouchIndicatorRendering()
    {
        var touchIndicaterMeshrender = touchIndicator.GetComponent<MeshRenderer>();
        touchIndicaterMeshrender.enabled = true;
    }

    public void turnOnTouchIndicator()
    {
        
        if (touchIndicator)
        {
            var renderer = touchIndicator.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = true; 
            }
        }
    }

    public void turnOffTouchIndicator()
    {
        if (touchIndicator)
        {
            var renderer = touchIndicator.GetComponent<Renderer>();
            if (renderer)
            {
                // renderer.materials[0].SetFloat("_alpha", 0.01f)
                renderer.enabled = false; 
            }
        }
    }

    public void TurnOnTouchInteraction()
    {
        if (touchIndicator)
        {
            var renderer = touchIndicator.GetComponent<Renderer>(); 
            if (renderer)
            {
                // renderer.materials[0].SetFloat("_FresnelPower", 1.0f); 
            }
        }
    }

}
