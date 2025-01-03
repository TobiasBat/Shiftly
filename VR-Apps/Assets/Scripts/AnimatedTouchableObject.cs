using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class AnimatedTouchableObject : MonoBehaviour
{

    public GameObject visualRepresentation;
    public GameObject touchIndicator;
    public GameObject shiftlyAnimatedTouchPoint;
    // public Vector3 offsetFromCenter = new Vector3(0.0f, 0.0f, 0.0f);
    public bool RenderVisualRepresentation = true;
    public float speed = 1.0f;

    private AlembicStreamPlayer player; 
    private bool animationIsRunning = false;
    
    public AnimatedTouchPoint animatedTochPointComponent; 

    // Start is called before the first frame update
    void Start()
    {
        if (visualRepresentation == null) { 
            Debug.LogError("No Visual Representation Defined"); 
            return; 
        }
        player = visualRepresentation.GetComponent<AlembicStreamPlayer>();  
        if (player == null)
        {
            Debug.LogError("Visual Representation does not have an Alembic Stream Player"); 
        }
        


        if (!RenderVisualRepresentation)
        {
            TurnOffTouchObjectRendering();
            TurnOffTouchIndicatorRendering();
        }

        if (!shiftlyAnimatedTouchPoint)
        {
            Debug.LogError("No Shiftly Animated Touch Point Object Defined"); 
        } 
    }

    private void Update()
    {
        if (animationIsRunning)
        {
            player.CurrentTime = Mathf.Min(
                player.CurrentTime + Time.deltaTime * speed, 
                player.EndTime - player.StartTime
            );
            if (player.CurrentTime >= player.EndTime - player.StartTime) {
                Debug.Log("Animation Done"); 
                animationIsRunning = false; 
            }
        }
    }

    public void playAnimation()
    {
        player.CurrentTime = 0.0f; 
        animationIsRunning = true;
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
