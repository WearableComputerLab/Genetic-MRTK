using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAreaInteractable : MonoBehaviour, IMixedRealityPointerHandler
{
    private bool isSelected = false;
    public Color originalColor;
    public bool IsSelected()
    {
        return isSelected;
    }

    void Start()
    {
        originalColor = transform.Find("Ground").GetComponent<SpriteRenderer>().color;
        print(originalColor);
    }
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        isSelected = !isSelected;
        print("Clicked");

        var groundObject = transform.Find("Ground");
        if (isSelected)
        {
            groundObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            groundObject.GetComponent<SpriteRenderer>().color = originalColor;
        }
        

        print(isSelected);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        print("Down");
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        print("Dragged");
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        print("Up");
    }

}
