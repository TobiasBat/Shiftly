using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShiftlyUserStudyBlindTouchTestcases 
{
    public static float playerStartTime = 0.04166667f;
    public static float playerEndTime = 12.5f;
    public static float numberOfRealShapes = 23;

    public static ShiftlyUserStudyBlindTouchTestcase[] testCases =
    {
        new ShiftlyUserStudyBlindTouchTestcase(
            "Demo 1",
            -1,
            ShiftlyTouchPointPosition.FrontSurface,
            new float[]{ 0.0f, 0.0f, 0.0f },
             ComputeCorrectAnswerer(6),
             ComputeCorrectAnswerer(6)
        ),
        new ShiftlyUserStudyBlindTouchTestcase(
            "Demo 2",
            -2,
            ShiftlyTouchPointPosition.TopEdge,
            new float[] { 180.0f, 0.0f, 180.0f },
             ComputeCorrectAnswerer(7),
             ComputeCorrectAnswerer(6)
        ),
        new ShiftlyUserStudyBlindTouchTestcase(
            "flat 71.89",
            1,
            ShiftlyTouchPointPosition.FrontSurface,
            new float[]{ 180.0f, 180.0f, 0.0f },
            ComputeCorrectAnswerer(1),
            ComputeCorrectAnswerer(21)
        ),
        new ShiftlyUserStudyBlindTouchTestcase(
            "flat 36.21", 
            2,
            ShiftlyTouchPointPosition.FrontSurface, 
            new float[] { 180.0f, 0.0f, 180.0f },
             ComputeCorrectAnswerer(2)
            ,ComputeCorrectAnswerer(20)
        ),
        new ShiftlyUserStudyBlindTouchTestcase(
            "edge 60°",
            3,
            ShiftlyTouchPointPosition.TopEdge,
            new float[]{180.0f, 180, 180},
             ComputeCorrectAnswerer(5),
             ComputeCorrectAnswerer((int)(numberOfRealShapes - 1.0f - 5.0f))
        ),
        new ShiftlyUserStudyBlindTouchTestcase(
            "edge 36°",
            4,
            ShiftlyTouchPointPosition.TopEdge,
            new float[]{180, 180, 0},
             ComputeCorrectAnswerer(6),
             ComputeCorrectAnswerer((int)(numberOfRealShapes - 1 - 6))
        ),
        // Minimal Representable Circle
        new ShiftlyUserStudyBlindTouchTestcase(
            "cylinder 92mm",
            5,
            ShiftlyTouchPointPosition.FrontSurface,
            new float[] {0.0f, 180.0f, 180.0f},
            ComputeCorrectAnswerer(9),
            ComputeCorrectAnswerer((int)(numberOfRealShapes - 1 - 9))
        ),
        // Maximum representable circle
        new ShiftlyUserStudyBlindTouchTestcase(
            "cylinder 156mm",
            6,
            ShiftlyTouchPointPosition.TopEdge,
            new float[] {115.0f, 115.0f, 180.0f},
            ComputeCorrectAnswerer(10),
            ComputeCorrectAnswerer((int)(numberOfRealShapes - 1 - 10))
        )
    };

    private static float ComputeCorrectAnswerer(int correctIndex)
    {
        return playerStartTime + (float)(correctIndex) * (playerEndTime - playerStartTime) / (numberOfRealShapes-1.0f); 
    }

    public static ShiftlyUserStudyBlindTouchTestcase[] GetOrdered(int orderIndex)
    {
       if (orderIndex == -1)
        {
            return testCases; 
        } else
        {
            ShiftlyUserStudyBlindTouchTestcase[] orderedTestcases = new ShiftlyUserStudyBlindTouchTestcase[18 + 2];
            // insert the two demos
            orderedTestcases[0] = testCases[0];
            orderedTestcases[1] = testCases[1]; 
            
            for (int i = 0; i < 18; i++)
            {
                // take the next testcase - offset by two. 
                // i is the testcase of a user study run. 
                orderedTestcases[i + 2] = testCases[
                    LatinSquares.singleLatinSquare3x6IndicesRowColReanranged[orderIndex][i] + 2
                    ]; 
            }
            return orderedTestcases; 
        }
    }
}
