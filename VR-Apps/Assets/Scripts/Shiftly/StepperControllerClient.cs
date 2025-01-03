using System.Collections;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Networking;

public enum StepperMotorSpeeds
{
    Full = 1,
    Half = 2,
    Quatar = 4,
    Eighteenth = 8,
    Sixtheenth = 16
}

[System.Serializable]
public class ShiftlyKeyboardInstructions
{
    public KeyCode contractAll = KeyCode.C;
    public KeyCode extendAll = KeyCode.E; 
    public KeyCode updateAll = KeyCode.A;
    [Space(10)]
    public KeyCode set = KeyCode.S;
    public KeyCode stepForward = KeyCode.D;
    public KeyCode stepBackwards = KeyCode.F;
    [Space(10)]
    public KeyCode stepper1 = KeyCode.Alpha1;
    public KeyCode stepper2 = KeyCode.Alpha2;
    public KeyCode stepper3 = KeyCode.Alpha3;
}

/// <summary>
/// Handels the communication with Shiftly
/// </summary>
public class StepperControllerClient : MonoBehaviour
{
    public bool wirelessConnection = false;
    public string ipAdress = "1111";
    public string usbPort = ""; 

    [Range(0.0f, 360.0f)]
    public int stepper1Degree = 0;
    public StepperMotorSpeeds stepper1Speed = StepperMotorSpeeds.Sixtheenth;
    [Space(10)]
    [Range(0.0f, 360.0f)]
    public int stepper2Degree = 0;
    public StepperMotorSpeeds stepper2Speed = StepperMotorSpeeds.Sixtheenth;
    [Space(10)]
    [Range(0.0f, 360.0f)]
    public int stepper3Degree = 0;
    public StepperMotorSpeeds stepper3Speed = StepperMotorSpeeds.Sixtheenth;

    [Space(10)]
    public ShiftlyKeyboardInstructions keyboardInstructions; 
    [Space(10)]
    
    public int fullyExtendedDegree = 180;
    public int fullyContractedDegree = 0;

    public int empthyStepDegree = 10; 


    private SerialPort serialPort;

    void Start()
    {
        if (!wirelessConnection)
        {
            serialPort = new SerialPort();
            serialPort.PortName = usbPort;
            serialPort.BaudRate = 115200;
        }
    }

    void Update()
    {
        HandleKeyboardControlls(); 
    }

    private void HandleKeyboardControlls()
    {
        // Debug On Keyboard updates
        // Update Values Manually 
        if (Input.GetKeyDown(keyboardInstructions.updateAll))
        {
            if (wirelessConnection)
            {
                StopAllCoroutines();
                StartCoroutine(Upload("1", stepper1Degree, stepper1Speed));
                StartCoroutine(Upload("2", stepper2Degree, stepper2Speed));
                StartCoroutine(Upload("3", stepper3Degree, stepper3Speed));
            } else
            {
                UploadWired("1", stepper1Degree, stepper1Speed);
                UploadWired("2", stepper2Degree, stepper2Speed);
                UploadWired("3", stepper3Degree, stepper3Speed);

            }
               
            
        }
        else if (Input.GetKey(keyboardInstructions.contractAll))
        {
            FullyContracted();
        }
        else if(Input.GetKeyDown(keyboardInstructions.extendAll))
        {
            Prisma();
        }
        else if(Input.GetKeyUp(keyboardInstructions.stepForward))
        {
            if (Input.GetKey(keyboardInstructions.stepper1))
            {
                StartCoroutine(UploadStep("1", empthyStepDegree, stepper1Speed));
            } 
            else if (Input.GetKey(keyboardInstructions.stepper2)) 
            {
                StartCoroutine(UploadStep("2", empthyStepDegree, stepper2Speed));
            }
            else if (Input.GetKey(keyboardInstructions.stepper3))
            {
                StartCoroutine(UploadStep("3", empthyStepDegree, stepper2Speed));
            }
        }
        else if (Input.GetKeyUp(keyboardInstructions.stepBackwards))
        {
            if (Input.GetKey(keyboardInstructions.stepper1))
            {
                int backwards = -10;
                StartCoroutine(UploadStep("1", backwards, stepper1Speed));
            }
            else if (Input.GetKey(keyboardInstructions.stepper2))
            {
                StartCoroutine(UploadStep("2", empthyStepDegree * -1, stepper2Speed));
            }
            else if (Input.GetKey(keyboardInstructions.stepper3))
            {
                StartCoroutine(UploadStep("3", empthyStepDegree * -1, stepper2Speed));
            }
        } 
        else if (Input.GetKey(keyboardInstructions.set))
        {
            if (Input.GetKey(keyboardInstructions.stepper1))
            {
                StartCoroutine(UploadSet("1", stepper1Degree));
            } 
            else if (Input.GetKey(keyboardInstructions.stepper2))
            {
                StartCoroutine(UploadSet("2", stepper2Degree)); 
            } 
            else if (Input.GetKey(keyboardInstructions.stepper3))
            {
                StartCoroutine(UploadSet("3", stepper3Degree));
            }
        }
    }

    #region Transform Shiftly Basic Shapes
    void Circle()
    {
        stepper1Degree = 30;
        stepper2Degree = 30;
        stepper3Degree = 30;

        StartCoroutine(Upload("1", stepper1Degree, stepper1Speed));
        StartCoroutine(Upload("2", stepper2Degree, stepper2Speed));
        StartCoroutine(Upload("3", stepper3Degree, stepper3Speed));
    }

    void Prisma()
    {
        stepper1Degree = fullyExtendedDegree;
        stepper2Degree = fullyExtendedDegree;
        stepper3Degree = fullyExtendedDegree;

        StartCoroutine(Upload("1", stepper1Degree, stepper1Speed));
        StartCoroutine(Upload("2", stepper2Degree, stepper2Speed));
        StartCoroutine(Upload("3", stepper3Degree, stepper3Speed));

    }
    void RightWing()
    {
        stepper1Degree = fullyContractedDegree;
        stepper2Degree = fullyExtendedDegree;
        stepper3Degree = fullyExtendedDegree;

        StartCoroutine(Upload("1", stepper1Degree, stepper1Speed));
        StartCoroutine(Upload("2", stepper2Degree, stepper2Speed));
        StartCoroutine(Upload("3", stepper3Degree, stepper3Speed));
    }

    void PointyEdge()
    {
        stepper1Degree = fullyExtendedDegree;
        stepper2Degree = fullyExtendedDegree;
        stepper3Degree = fullyContractedDegree;

        StartCoroutine(Upload("1", stepper1Degree, stepper1Speed));
        StartCoroutine(Upload("2", stepper2Degree, stepper2Speed));
        StartCoroutine(Upload("3", stepper3Degree, stepper3Speed));
    }

    public void FullyContracted()
    {
        stepper1Degree = fullyContractedDegree;
        stepper2Degree = fullyContractedDegree;
        stepper3Degree = fullyContractedDegree;

        StartCoroutine(Upload("1", stepper1Degree, stepper1Speed));
        StartCoroutine(Upload("2", stepper2Degree, stepper2Speed));
        StartCoroutine(Upload("3", stepper3Degree, stepper3Speed));
    }
    #endregion

    #region Upload to Shiftly 
    IEnumerator Upload(string name, float degree, StepperMotorSpeeds speed)
    {
        if (wirelessConnection)
        {
            byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
            string url = ipAdress + "/?" + "stepper=" + name + "&degree=" + degree + "&microsteps=" + (int)speed;
            // Debug.Log(url);
            UnityWebRequest www = UnityWebRequest.Put(url, System.Text.Encoding.UTF8.GetBytes("This is some test data"));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
                // Debug.Log(www.downloadHandler.text); 
            }
        }
        else
        {
            try
            {
                string line = string.Format("stepper={0} degree={1} microsteps={2}", name, (int)degree, (int)speed);
                Debug.Log(line);
                serialPort.Open();
                serialPort.WriteLine(line);
                serialPort.Close();
            } catch
            {
                Debug.Log("Could not Update shiftly via serial command"); 
            }
        }

    }


    public void UploadWired(string name, int degree, StepperMotorSpeeds speed) {
        try
        {
            string line = string.Format("stepper={0} degree={1} microsteps={2}", name, (int)degree, (int)speed);
            Debug.Log(line);
            serialPort.Open();
            serialPort.WriteLine(line);
            serialPort.Close();
        }
        catch
        {
            Debug.Log("Could not Update shiftly via serial command");
        }
    }

    IEnumerator UploadStep(string name, int degree, StepperMotorSpeeds speed)
    {
        Debug.Log(string.Format("Uploading Step {0} {1} {2}", name, degree, speed)); 
        if (wirelessConnection)
        {
            string url = ipAdress + "/step?" + "stepper=" + name + "&degree=" + degree + "&microsteps=" + (int)speed;
            Debug.Log(url);
            UnityWebRequest www = UnityWebRequest.Put(url, "some data");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        } else
        {
            try
            {
                string line = string.Format("step stepper={0} degree={1} microsteps={2}", name, degree, (int)speed);
                Debug.Log(line);
                serialPort.Open();
                serialPort.WriteLine(line);
                serialPort.Close();
            }
            catch
            {
                Debug.Log("Could not upload step command to shiftly via serial command");
            }
        }
    }

    IEnumerator UploadSet(string name, float degree)
    {
        Debug.Log(string.Format("Uploading Set {0} {1}", name, degree));
        if (wirelessConnection)
        {
            string url = ipAdress + "/set?" + "stepper=" + name + "&degree=" + degree;
            Debug.Log(url);
            UnityWebRequest www = UnityWebRequest.Put(url, "some data");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
        else
        {
            try
            {
                string line = string.Format("set stepper={0} degree={1}", name, degree);
                serialPort.Open();
                serialPort.WriteLine(line);
                serialPort.Close();
            }
            catch
            {
                Debug.Log("Could not upload set command to shiftly via serial command");
            }
        }
           
    }

    public void setAndUploadextensionValues(string module, float _extension, StepperMotorSpeeds speed)
    {
        float extension = _extension;
        if (extension < 0.0f || extension > 1.0f)
        {
            Debug.LogWarning("Extension value not between 0 and 1");
            extension = Mathf.Max(extension, 0.0f);
            extension = Mathf.Min(extension, 1.0f);
        }

        int degree =(int)(fullyExtendedDegree * extension + (1.0f - extension) * fullyContractedDegree);
        if (module == "1" || module == "2" || module == "3")
        {
            if (wirelessConnection)
            {
                StartCoroutine(Upload(module, degree, speed));
            } else
            {
                UploadWired(module, degree, speed); 
            }
            
            // ensure client has up to date data
            if (module == "1") stepper1Degree = (int)degree;
            else if (module == "2") stepper2Degree = (int)degree;
            else if (module == "3") stepper3Degree = (int)degree;

        }
        else
        {
            Debug.LogWarning("No valid module number");
        }
    }

    public void setAndUploadDegree(string module, float degree, StepperMotorSpeeds speed) {
        if (module == "1") stepper1Degree = (int)degree;
        else if (module == "2") stepper2Degree = (int)degree;
        else if (module == "3") stepper3Degree = (int)degree;
        if(!wirelessConnection)
        {
            UploadWired(module, (int)degree, speed);
        }
        else
        {
            StartCoroutine(Upload(module, (int)degree, speed));
        }
    }
    #endregion
}


