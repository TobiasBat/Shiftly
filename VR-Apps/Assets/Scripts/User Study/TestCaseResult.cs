using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestCaseResult 
{
    public string testCaseName;
    public DateTime startTime;
    public DateTime endTime;
    public string shiftlyConviguration;
    public string object_a;
    public string object_b;
    public string submittedValue;
    public string correctValue; 

    public TestCaseResult(string name)
    {
        testCaseName = name; 
    }
}
