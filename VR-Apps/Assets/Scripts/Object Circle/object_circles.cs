using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;


public enum ObjectCircleOrder
{
    IndexOrder, 
    Random,
    Original
}
public class object_circles : MonoBehaviour
{
    #region Varibles
    public List<touchableObject> objects = new List<touchableObject>();
    public List<touchableObject> originalOrderedObjects; 
    public bool randomizeOrderOnStart = true;
    public bool indexedOrder = true; 
    public ObjectCircleOrder objectOrder = ObjectCircleOrder.IndexOrder;

    [Range(0.5f, 10.0f)]
    public float radius = 0.75f;
    [Range(0.0f, 360.0f)]
    public float rotation = 180.0f;
    public int selectedIndex = 0;
    public float rotationUpdateSpeed = 30.0f;
    public StepperControllerClient stepperControllerClient;
    public GameObject objectToTrack;
    public ShiftlyVisualModelController shitlyModel;

    public int lastOrderIndex = 0;
    public bool userStudyTestRun = true; 


    private Vector3 rotationPoint;
    private float circleRotation = 0.0f;
    private int previouselySelectedIndex = 0; 
    private string lastOrderIndexFilePath = Application.streamingAssetsPath + "/execution_OrderIndex_History" + ".txt";

    public ObjectCircleLogging circleLogging; 

    #endregion

    void Start()
    {
        if (stepperControllerClient == null)
        {
            Debug.Log("No client assigned");
        }
        // store original order of objects 
        originalOrderedObjects = new List<touchableObject>(); 
        for (int i = 0; i < objects.Count; i++)
        {
            var newObject = objects[i]; 
            originalOrderedObjects.Add( newObject );
        }

        // Define rotation point of the circle
        rotationPoint = gameObject.transform.localPosition - new Vector3(radius, 0.0f, 0.0f);
        string runName = "testrun"; 
        if (objectOrder == ObjectCircleOrder.IndexOrder)
        {
            // read from file what the last order was
            lastOrderIndex = ReadLastOrderIndexFromFile();
            int newOrderIndex = lastOrderIndex + 1;
            lastOrderIndex = UpdateObjectOderByIndexedOfDoubleBalancedLatinQuare(newOrderIndex) - 1;
            runName = "index_order_run_" + newOrderIndex + "_";
            Debug.Log("Order object with index: " + newOrderIndex);
        }

        // Randomize the Order of the object
        else if (objectOrder == ObjectCircleOrder.Random)
        {
            RandomizeOrder();
            runName = "random_order_run";
            Debug.Log("Order object random"); 
        }
        else
        {
            Debug.Log("Did not reorder objects"); 
        }


        circleLogging = new ObjectCircleLogging("circle-logging", "circle", runName, ".json");
        AddGeneralLogInformation(); 

        //updatePhysicalPrototype(objects[selectedIndex]);
        stepperControllerClient.FullyContracted();
        TurnOffAllTouchIndicators(); 
    }


    void Update()
    {
        // Check if Key down
        // CheckKeyInput();

        float targetRotation = (selectedIndex / (float) objects.Count) * 360.0f;
        float currentRotation = circleRotation;
        if (Mathf.Abs(currentRotation - targetRotation) < 0.1f)
        {
            // Turn glow for selected object an
            objects[selectedIndex].turnOnTouchIndicator();
        } else
        {
            // Compute update Rotation;
            float updateRotatin = Mathf.Min(Mathf.Abs(targetRotation - currentRotation), rotationUpdateSpeed * Time.deltaTime);
            if (currentRotation > targetRotation)
            {
                updateRotatin *= -1.0f; 
            }
            circleRotation += updateRotatin;
            if (circleRotation > 360.0f)
            {
                circleRotation = circleRotation - 360.0f;
            }
        }

        // Move circle to fit the current selected touch point 
        GameObject touchPointObject = objects[selectedIndex].shiftlyTouchPoint;
        ShiftlyTouchPoint shiftlyTouchPoint = touchPointObject.GetComponent<ShiftlyTouchPoint>();
        GameObject shiftlyTochIndicator = shitlyModel.getTouchPointIndicatorObjectFromTouchPoint(shiftlyTouchPoint);

        GameObject previouslyTouchPointObject = objects[previouselySelectedIndex].shiftlyTouchPoint;
        ShiftlyTouchPoint previouslyShiftlyTouchPoint = previouslyTouchPointObject.GetComponent<ShiftlyTouchPoint>();
        GameObject previousShiftlyTouchIndicator = shitlyModel.getTouchPointIndicatorObjectFromTouchPoint(previouslyShiftlyTouchPoint);

        float previouseTargetRotation = (previouselySelectedIndex / (float)objects.Count) * 360.0f;
        float k = 1.0f; 
        if (targetRotation - previouseTargetRotation != 0)
        {
            k = (currentRotation - previouseTargetRotation) / (targetRotation - previouseTargetRotation);
        }

        // Linear interpolate between current and next index when rotation is not completed
        gameObject.transform.position = (1.0f - k) * previousShiftlyTouchIndicator.transform.position + k * shiftlyTochIndicator.transform.position;
        gameObject.transform.rotation = objectToTrack.transform.rotation;



        UpdateCircleRotation();
    }

    private void RandomizeOrder()
    {
        System.Random rng = new System.Random();
        int n = objects.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            touchableObject value = objects[k];
            objects[k] = objects[n];
            objects[n] = value;
        }
    }

    /// <summary>
    /// Updates the order of objects based on the index of the double balanced latin Square
    /// </summary>
    /// <param name="index">Row of double balanced lating square. In case index greated than row number. 0 is selected</param>
    /// <returns>The selected Index; Incase <param name="index"> was greated that double latin square, it returns 0</returns>
    private int UpdateObjectOderByIndexedOfDoubleBalancedLatinQuare(int index)
    {
        int[,] square;
        int rank = 0; 
        if (originalOrderedObjects.Count == 4)
        {
            rank = 4; 
            square = LatinSquares.doubleLatinSquare4Indices; 
        } else if (originalOrderedObjects.Count == 6)
        {
            rank = 6; 
            square = LatinSquares.doubleLatinSquare6Indices; 
        } else
        {
            Debug.Log("Number for latin square is not implemented");
            return index; 
        }

        if (rank > 0)
        {
            if (index >= square.GetLength(0))
            {
                index = 0;
                lastOrderIndex = -1; 
            }
            int[] sequence = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                sequence[i] = square[index, i];
            }
            print(sequence);

            // appliying sequence 
            objects = new List<touchableObject>();
            foreach (int i in sequence)
            {
                objects.Add(originalOrderedObjects[i]); 
            }
        }
        return index; 
        
    }
    public void IsAUserStudyRun()
    {
        userStudyTestRun = true; 
        circleLogging.AppendGeneralInformation("is_a_user_study_testrun", userStudyTestRun);
    }
    public void IsNotAUserStudyRun()
    {
        userStudyTestRun = false;
        circleLogging.AppendGeneralInformation("is_a_user_study_testrun", userStudyTestRun);
    }

    public void CompleteCircle()
    {
        if (userStudyTestRun)
        {
            lastOrderIndex = lastOrderIndex + 1;
            WriteLastOrderIndexToFile(lastOrderIndex); 
            // TODO store last Ordered Index
            circleLogging.AppendGeneralInformation("Completed and index stored", true); 
            Debug.Log("Increades the last ordered index");
        }
        else
        {
            Debug.Log("Not Increased Last Ordered Index");
        }
    }

    public void Restart()
    {
        // Todo latin square here
        userStudyTestRun = true;
        // RandomizeOrder(); 
        string runName = "testrun";
        if (indexedOrder)
        {
            // read from file what the last order was
            lastOrderIndex = ReadLastOrderIndexFromFile();
            int newOrderIndex = lastOrderIndex + 1;
            lastOrderIndex = UpdateObjectOderByIndexedOfDoubleBalancedLatinQuare(newOrderIndex) - 1;
            runName = "index_order_run_" + newOrderIndex + "_";
        }

        // Randomize the Order of the object
        if (randomizeOrderOnStart)
        {
            RandomizeOrder();
            runName = "random_order_run";
        }

        circleLogging = new ObjectCircleLogging("circle-logging", "circle", runName, ".json");
        AddGeneralLogInformation(); 


        selectedIndex = 0;
        circleRotation = 0.0f; 
        UpdateCircleRotation();
        updatePhysicalPrototype(objects[selectedIndex]);
        // stepperControllerClient.FullyContracted();
        TurnOffAllTouchIndicators();
        objects[selectedIndex].turnOnTouchIndicator();
    }

    public void TurnOffAllTouchIndicators()
    {
        foreach (var obj in objects)
        {
            obj.turnOffTouchIndicator();
        }
    }

    private void AddGeneralLogInformation()
    {
        circleLogging.AppendGeneralInformation("fully_extended_degree", stepperControllerClient.fullyExtendedDegree);
        circleLogging.AppendGeneralInformation("fully_contracted_degree", stepperControllerClient.fullyContractedDegree);
        circleLogging.AppendGeneralInformation("indexed_ordered_run", indexedOrder);
        circleLogging.AppendGeneralInformation("randomized_order_on_start", randomizeOrderOnStart);
        circleLogging.AppendGeneralInformation("order_index", lastOrderIndex + 1);

    }

    public void NextObject()
    {
        objects[selectedIndex].turnOffTouchIndicator();
        LogCurrentTouchPoint(); 
       
        previouselySelectedIndex = selectedIndex;
        selectedIndex++;
        correctSelectedIndex();
        updatePhysicalPrototype(objects[selectedIndex]);
    }

    private void LogCurrentTouchPoint()
    {
        // TODO
        circleLogging.AppendShiftlyTouchPointData(objects[selectedIndex].shiftlyTouchPoint.GetComponent<ShiftlyTouchPoint>()); 
        
        objects[selectedIndex].shiftlyTouchPoint.GetComponent<ShiftlyTouchPoint>().ResetRecordingData();
    }

    public void PreviouseObject()
    {
        objects[selectedIndex].turnOffTouchIndicator();
        LogCurrentTouchPoint(); 
        previouselySelectedIndex = selectedIndex;
        selectedIndex--;
        correctSelectedIndex();
        updatePhysicalPrototype(objects[selectedIndex]);
    }

    public void DisableObjectCircleObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].gameObject.SetActive(false); 
        }
    }

    public void EnableObjectCircleObjects()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].gameObject.SetActive(true);
        }
    }

    private void UpdateCircleRotation()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            touchableObject obj = objects[i];
            GameObject touchPointObject = objects[i].shiftlyTouchPoint;

            // Degree of object in the circle; Spot in circe
            float rotationDegree = -360.0f / objects.Count * i + circleRotation;

            // Rotate object to be tangetial to circle
            obj.transform.localEulerAngles = new Vector3(0.0f, -rotationDegree, 0.0f);

            // Compute position of object in circe
            float x = radius * Mathf.Cos(rotationDegree * Mathf.Deg2Rad);
            float z = radius * Mathf.Sin(rotationDegree * Mathf.Deg2Rad);

            obj.transform.localPosition = new Vector3(x, 0, z) // rotational positon
                + new Vector3(-radius, 0, 0)    // set backt to align circle border with shiftly
                - touchPointObject.transform.localPosition; // move object to align with touch point rater than with whole touchobject
        }
    }

    private void CheckKeyInput()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            PreviouseObject();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            NextObject(); 
        }
    }

    public void UpdatePhysicalPrototypeToSelectedIndex()
    {
        updatePhysicalPrototype(objects[selectedIndex]);
    }
    private void updatePhysicalPrototype(touchableObject obj)
    {
        ShiftlyTouchPoint touchPoint = obj.shiftlyTouchPoint.GetComponent<ShiftlyTouchPoint>();
        if (touchPoint)
        {
            Debug.Log("Transforming Shiftly to: " + touchPoint.side_1_extension + ", " + touchPoint.side_2_extension + ", " + touchPoint.side_3_extension);
            stepperControllerClient.setAndUploadextensionValues("1", touchPoint.side_1_extension, stepperControllerClient.stepper1Speed);
            stepperControllerClient.setAndUploadextensionValues("2", touchPoint.side_2_extension, stepperControllerClient.stepper2Speed);
            stepperControllerClient.setAndUploadextensionValues("3", touchPoint.side_3_extension, stepperControllerClient.stepper3Speed);

        } else
        {
            Debug.LogError("Touchobject has not Shiftly Touch Point assigned"); 
        }
    }

    private void correctSelectedIndex()
    {
        // check if selected Index Correct
        if (selectedIndex >= objects.Count)
        {
            selectedIndex = 0;
        }
        else if (selectedIndex < 0)
        {
            selectedIndex = objects.Count - 1;
        }
    }

    int ReadLastOrderIndexFromFile()
    {

        if (!File.Exists(lastOrderIndexFilePath))
        {
            string dateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");
            File.WriteAllText(lastOrderIndexFilePath, "File Created: " + dateString + "\n");
            return -1;
        }
        else
        {
            try
            {
                var lineList = new List<string>();
                var fileStream = new FileStream(lastOrderIndexFilePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lineList.Add(line);
                    }
                }
                string lastLine = lineList[lineList.Count - 1];
                string lastIndexString = lastLine.Split(" ")[0];
                int readLastIndex = Int32.Parse(lastIndexString);
                Debug.Log(readLastIndex);
                Debug.Log("-----------------");
                return readLastIndex;
            }
            catch
            {
                Debug.Log("Something went wront reading the last Index");
            }

        }

        return 0;
    }

    void WriteLastOrderIndexToFile(int index)
    {
        if (!File.Exists(lastOrderIndexFilePath))
        {
            string dateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");
            File.WriteAllText(lastOrderIndexFilePath, "File Created: " + dateString + "\n");
        }

        try
        {
            var lineList = new List<string>();
            var fileStream = new FileStream(lastOrderIndexFilePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lineList.Add(line);
                }
            }

            //Open the File
            StreamWriter sw = new StreamWriter(lastOrderIndexFilePath, true, Encoding.ASCII);
            // foreach(string line in lineList)
            // {
            //     sw.WriteLine(line); 
            // }
            string dateString = DateTime.Now.ToString("yyyy_m_d_HH_mm");
            sw.WriteLine(String.Format("{0} " + dateString, index));
            sw.Close();
        }
        catch
        {
            Debug.LogError("Something went wront writing to the file");
            Debug.LogError("Could not store the index: " + index + "in the last oder index file");
        }
    }


}
