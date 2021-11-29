using System;
using Unity.Collections;
using UnityEngine;

public class TileMovement : MonoBehaviour
{
    private Vector3 _screenClickPos;

    private MovementDirections _currentDirection;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    void OnClick()
    {
        float circleAngle;
        
        Vector3 normalizedClick = new Vector3();
        
        _screenClickPos = Input.mousePosition;
        normalizedClick.x = (_screenClickPos.x / Screen.width - 0.5f) * 2;
        normalizedClick.y = (_screenClickPos.y / Screen.height - 0.5f) * 2;

        circleAngle = (Mathf.Atan2(normalizedClick.y, normalizedClick.x) * 180 / Mathf.PI) + 180;
        _currentDirection = getDirection(circleAngle);
    }

    MovementDirections getDirection(float angle)
    {
        if (angle < 270 && angle > 210)
        {
            return MovementDirections.TopRight;
        }
        if (angle < 210 && angle > 150)
        {
            return MovementDirections.Right;
        }
        if (angle < 150 && angle > 90)
        {
            return MovementDirections.BottomRight;
        }
        if (angle < 90 && angle > 30)
        {
            return MovementDirections.BottomLeft;
        }
        if (angle < 30 || angle > 330)
        {
            return MovementDirections.Left;
        }
        if (angle < 330 && angle > 270)
        {
            return MovementDirections.TopLeft;
        }
        return MovementDirections.None;
    }
}
