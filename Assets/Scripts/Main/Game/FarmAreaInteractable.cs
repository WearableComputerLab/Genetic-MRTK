using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAreaInteractable : MonoBehaviour, IMixedRealityPointerHandler
{
    private bool isSelected = false;
    public bool IsSelected()
    {
        return isSelected;
    }
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        isSelected = !isSelected;
        print("Clicked");

        var groundObject = transform.Find("Ground");
        groundObject.GetComponent<SpriteRenderer>().color = Color.blue;

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
