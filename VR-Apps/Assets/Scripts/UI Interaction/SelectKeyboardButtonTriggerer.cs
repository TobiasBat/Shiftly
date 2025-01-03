using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class SelectKeyboardButtonTriggerer : MonoBehaviour
{
    [SerializeReference] private List<KeyCode> enterKeys = new List<KeyCode> { KeyCode.Return, KeyCode.Period, KeyCode.F5, KeyCode.Escape};
    [SerializeReference] private List<KeyCode> nextKeys = new List<KeyCode> {KeyCode.RightArrow, KeyCode.PageDown};
    [SerializeReference] private List<KeyCode> prevKeys = new List<KeyCode> { KeyCode.LeftArrow, KeyCode.PageUp }; 
    [SerializeReference] private List<Button> buttons;
    [SerializeReference] private int buttonIndexToSelectAfterOneSelected = -1; 

    private int currentlySelectedIndex = 0; 

    void Update()
    {

        if (gameObject.activeSelf)
        {
            foreach (var nextKey in nextKeys)
            {
                if (Input.GetKeyUp(nextKey))
                {
                    currentlySelectedIndex++;
                    FixCurrentlySelectedIndex();
                    SelectIndex();
                }
            }

            foreach (var prevKey in prevKeys)
            {
                if (Input.GetKeyUp(prevKey))
                {
                    currentlySelectedIndex--;
                    FixCurrentlySelectedIndex();
                    SelectIndex();
                }
            }

            foreach (var enterKey in enterKeys)
            {
                if (Input.GetKeyUp(enterKey))
                {
                    if (currentlySelectedIndex > -1 && currentlySelectedIndex < buttons.Count)
                    {
                        if (buttons[currentlySelectedIndex].gameObject.activeSelf)
                        {
                            if (IsCurrentlySelectedIndexSlected())
                            {
                                InvokeSelectedIndex();
                            } 
                            else
                            {
                                SelectIndex();
                            }
                        }
                    }
                    
                }
            }
        }
    }

    private void FixCurrentlySelectedIndex()
    {
        if (currentlySelectedIndex >= buttons.Count)
        {
            currentlySelectedIndex = 0;
        } else if (currentlySelectedIndex < 0)
        {
            currentlySelectedIndex = buttons.Count - 1; 
        }
    }

    private bool IsCurrentlySelectedIndexSlected()
    {
        if (currentlySelectedIndex > -1 && currentlySelectedIndex < buttons.Count)
        {
            if (EventSystem.current.currentSelectedGameObject == buttons[currentlySelectedIndex].gameObject)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void InvokeSelectedIndex()
    {
        buttons[currentlySelectedIndex].onClick.Invoke();

        if (buttonIndexToSelectAfterOneSelected > -1 && buttonIndexToSelectAfterOneSelected < buttons.Count)
        {
            if (buttons[buttonIndexToSelectAfterOneSelected].gameObject.activeSelf)
            {
                if (buttons[buttonIndexToSelectAfterOneSelected].gameObject.GetInstanceID() != buttons[currentlySelectedIndex].gameObject.GetInstanceID())
                {
                    buttons[buttonIndexToSelectAfterOneSelected].Select();
                    currentlySelectedIndex = buttonIndexToSelectAfterOneSelected;
                    Debug.Log("Next button is selected");
                }
                else
                {
                    Debug.Log("Its the same button");
                }
            }
            else
            {
                Debug.Log("Button is not Aktive");
            }
        }
    }
    private void SelectIndex()
    {
        if (buttons[currentlySelectedIndex])
        {
            if (buttons[currentlySelectedIndex].gameObject.activeSelf)
            {
                buttons[currentlySelectedIndex].Select();
            }
            else if (NumberOfSelfAktiveButtons() > 0)
            {
                currentlySelectedIndex++;
                FixCurrentlySelectedIndex();
                SelectIndex(); 
            }
        }
    }

    private int NumberOfSelfAktiveButtons()
    {
        int count = 0; 
        foreach(var button in buttons)
        {
            if (button.gameObject.activeSelf)
            {
                count++; 
            }
        }

        return count; 
    }
}
