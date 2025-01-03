using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIButtonKeyboardControll : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Button> buttonList = new List<Button>();
    private int currentlySelectedButton = -1;
    public KeyCode enter = KeyCode.Return;
    public KeyCode nextButton = KeyCode.DownArrow;
    public KeyCode prevButton = KeyCode.UpArrow;

    private int framesToWaitTillKeyboardAgain = 10; 
    private int framesTillCheckingButtonAgain = 0; 
 

    void Start()
    {
        if (buttonList.Count > 0)
        {
            currentlySelectedButton = 0; 
        }
    }

    // Update is called once per frame
    void Update()
    {
       if (framesTillCheckingButtonAgain == 0)
        {
            HandleKeyBoardInput(); 
        } else
        {
            framesTillCheckingButtonAgain -= 1; 
        }
    }

    private void HandleKeyBoardInput()
    {
        if (Input.GetKey(enter))
        {
            if (buttonList.Count > currentlySelectedButton)
            {
                buttonList[currentlySelectedButton].onClick.Invoke();
            }
           
        }
        else if (Input.GetKey(nextButton))
        {
            
            if (currentlySelectedButton == buttonList.Count - 1 && currentlySelectedButton < 0)
            {
                currentlySelectedButton = 0;
            }
            else
            {
                currentlySelectedButton += 1;
            }

            buttonList[currentlySelectedButton].Select();
            framesTillCheckingButtonAgain = framesToWaitTillKeyboardAgain; 

        } else if (Input.GetKey(prevButton))
        {
            if (currentlySelectedButton <= 0)
            {
                currentlySelectedButton = buttonList.Count - 1; 
            } else
            {
                currentlySelectedButton -= 1; 
            }
            buttonList[currentlySelectedButton].Select();
            framesTillCheckingButtonAgain = framesToWaitTillKeyboardAgain; 
        }
    }
}
