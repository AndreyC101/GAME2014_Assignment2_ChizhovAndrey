using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerAxis
{
    Horizontal = 0,
    Jump = 1,
    Attack = 2,
    AXIS_COUNT
}

public class MobileController : MonoBehaviour
{
    [SerializeField]
    private GameObject onScreenControls;

    [SerializeField]
    private FixedJoystick horizontalAxisJoystick;

    [SerializeField]
    private AxisButton jumpAxisJoystick;

    [SerializeField]
    private AxisButton attackAxisJoystick;

    private void Start()
    {
        if (onScreenControls)
        {
            onScreenControls.gameObject.SetActive(Application.isMobilePlatform);
        }
    }

    public float GetAxis(ControllerAxis axis)
    {
        if (!Application.isMobilePlatform) return 0.0f;
        switch (axis)
        {
            case ControllerAxis.Horizontal:
                return horizontalAxisJoystick.Horizontal;
            case ControllerAxis.Jump:
                return jumpAxisJoystick.GetAxisValue();
            case ControllerAxis.Attack:
                return attackAxisJoystick.GetAxisValue();
            default:
                Debug.Log("Requested controller axis not found");
                return 0.0f;
        }
    }
}
