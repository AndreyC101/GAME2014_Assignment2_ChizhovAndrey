using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AxisButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float axisValue = 0.0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        axisValue = 1.0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        axisValue = 0.0f;
    }

    public float GetAxisValue() { return axisValue; }
}
