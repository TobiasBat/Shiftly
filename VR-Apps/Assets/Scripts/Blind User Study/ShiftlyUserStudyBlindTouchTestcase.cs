using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShiftlyUserStudyBlindTouchTestcase
{
    /// <summary>
    /// Name of the testcase. Describes testcase. Not shown to participant
    /// </summary>
    public string name = "Sample Test Case";
    /// <summary>
    /// Numberic id of the testcase. Must be unique to all other
    /// </summary>
    public int id;
    /// <summary>
    /// Defines where the user should touch shiftly
    /// </summary>
    public ShiftlyTouchPointPosition touchpointPosition = ShiftlyTouchPointPosition.FrontEdge;
    /// <summary>
    /// Degree of the motor Rotations. Musst be an array of length 3. 
    /// </summary>
    public float[] stepperConfiguration = { -1, -1, -1 };
    public float correctAnwser = -1.0f;
    public float correctAnswer2 = -2.0f; 

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_name">Name of the testcase. Describes testcase. Not shown to participant</param>
    /// <param name="_id">unique id of testcase</param>
    /// <param name="_touchpointPosition">Where the user touches SHIFTLY</param>
    /// <param name="_stepperConfiguration">Degree of the motor Rotations. Must be an array of length 3. </param>
    public ShiftlyUserStudyBlindTouchTestcase(
        string _name,
        int _id,
        ShiftlyTouchPointPosition _touchpointPosition,
        float[] _stepperConfiguration,
        float _correctAnwser,
        float _correctedAnswer2
    )
    {
        name = _name;
        id = _id; 
        touchpointPosition = _touchpointPosition;
        stepperConfiguration = _stepperConfiguration;
        correctAnwser = _correctAnwser;
        correctAnswer2 = _correctedAnswer2;
    }
}