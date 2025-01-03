using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RadionButtonLogic : MonoBehaviour
{
    public int value = -1; // to which value does this button coresponds. Not an indicator for selection
    public SevenPointScaleUIElement sevenPointUIElement;
    public Button button;
    public Image image; 
    public Color notSelectColor = new Color(255, 255, 255); 
    public Color selectColor = new Color(0, 0, 255);

    public Sprite buttonAktiveSprite;
    public Sprite buttonNotAktiveSprite; 

    public void SelectOnlyThis()
    {
        sevenPointUIElement.SelectByIndex(value);
        button.Select();
        image.color = selectColor;
        image.sprite = buttonAktiveSprite; 
        
    }

    public void Deselect()
    {
        image.color = notSelectColor;
        image.sprite = buttonNotAktiveSprite; 
    }
}
