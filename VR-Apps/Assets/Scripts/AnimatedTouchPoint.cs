using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatedTouchPointFrame
{
    [Range(0.0f, 180.0f)]
    public float side_1_degree = 0.0f;
    [Range(0.0f, 180.0f)]
    public float side_2_degree = 0.0f;
    [Range(0.0f, 180.0f)]
    public float side_3_degree = 0.0f;
    public StepperMotorSpeeds stepperMotorSpeeds;
    public float animationDuration; /// in seconds; Describes how long shiftly needs till reaches that confiuration from previouse frame
}
/// <summary>
/// Describes how the physical shiftly should behave
/// Defines the different configuration, and the times when they should be loaded
/// </summary>
public class AnimatedTouchPoint : MonoBehaviour
{
    public List<AnimatedTouchPointFrame> frames;
    public ShiftlyTouchPointPosition touchPointPosition;
    [Range(0.0f, 3.0f)]
    public float animationSpeed = 1.0f;
}