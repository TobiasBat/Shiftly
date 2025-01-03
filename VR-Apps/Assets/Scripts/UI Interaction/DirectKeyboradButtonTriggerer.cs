using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[Serializable]
public struct KeyboardButtonTriggers
{
    public List<KeyCode> keyCodes;
    public Button button;
}
public class DirectKeyboradButtonTriggerer : MonoBehaviour
{
    [SerializeField] private List<KeyboardButtonTriggers> keyBoardButtonTriggers;
    [SerializeField] private bool requiresDoubleClick = false;  

    void Update()
    {
        // check if one of the buttons is pressed
        // If yes execture
        if (gameObject.activeSelf)
        {
            foreach (var aktion in keyBoardButtonTriggers)
            {
                foreach (var key in aktion.keyCodes)
                {
                    if (Input.GetKeyUp(key))
                    {
                        if (aktion.button.gameObject.activeSelf)
                        {
                            if (!requiresDoubleClick)
                            {
                                aktion.button.onClick.Invoke();
                            } else
                            {
                                HandleDoubleClickRequired(aktion); 
                            }
                            
                        }
                    }
                }
            }
        }
       
    }

    void HandleDoubleClickRequired(KeyboardButtonTriggers aktion)
    {
        if (EventSystem.current.currentSelectedGameObject == aktion.button.gameObject)
        {
            aktion.button.onClick.Invoke();
        } else
        {
            aktion.button.Select(); 
        }
    } 
}
