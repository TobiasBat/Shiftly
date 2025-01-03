using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.Video;

public class SliderScreen : MonoBehaviour
{

    public Leap.Unity.Interaction.InteractionSlider slider;
    private SpriteRenderer spriteRenderer;
    private VideoPlayer videoPlayer; 

    // Start is called before the first frame update
    void Start()
    {
        if (slider)
        {
            Debug.Log("I have a slider");
        }
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>(); 
        if (!spriteRenderer)
        {
            Debug.Log("Noting to change"); 
        }
        videoPlayer = gameObject.GetComponent<VideoPlayer>(); 
        if (!videoPlayer)
        {
            Debug.Log("Could not Find a Videopalyer"); 
        }
        videoPlayer.Pause();
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentSliderValue = slider.HorizontalSliderValue;
        videoPlayer.time = currentSliderValue * videoPlayer.length; 
    }
}
