using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjects : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeedX = 0f;
    [SerializeField] private float rotationSpeedY = 0f; 
    [SerializeField] private float rotationSpeedZ = 0f;

    [Header("Control Rotation")]
    [SerializeField] private bool rotateAroundX = true;
    [SerializeField] private bool rotateAroundY = true; 
    [SerializeField] private bool rotateAroundZ = true;

    private bool isTimeStoped = false;

    private void Update()
    {
        RotateObject();
    }

    private void RotateObject()
    {
        Vector3 rotation = Vector3.zero;

        if (rotateAroundX)
        {
            rotation.x = rotationSpeedX * Time.deltaTime;
        }
        if (rotateAroundY)
        {
            rotation.y = rotationSpeedY * Time.deltaTime;
        }
        if (rotateAroundZ)
        {
            rotation.z = isTimeStoped ? rotationSpeedZ * Time.unscaledDeltaTime :  rotationSpeedZ* Time.deltaTime;
        }

        transform.Rotate(rotation);
    }

    public void SetRotationSpeed(string axis, float newSpeed, bool isStoped = false)
    {
        isTimeStoped = isStoped;
        switch (axis.ToLower())
        {
            case "x":
                rotationSpeedX = newSpeed;
                break;
            case "y":
                rotationSpeedY = newSpeed;
                break;
            case "z":
                rotationSpeedZ = newSpeed;
                break;
            default:
                Debug.LogWarning("Невірна вісь");
                break;
        }
    }
}
