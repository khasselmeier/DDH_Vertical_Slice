using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("Clamping")]
    public float minY;
    public float maxY;

    private float rotX;
    private float rotY;

    private bool canMove = true;

    void Start()
    {
        //lock the cursor to the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (!canMove) return;

        //get the mouse movement imputs
        rotX += Input.GetAxis("Mouse X") * sensX;
        rotY += Input.GetAxis("Mouse Y") * sensY;

        //clamp the vertical rotation
        rotY = Mathf.Clamp(rotY, minY, maxY);

        //rotate the camera vertically
        transform.localRotation = Quaternion.Euler(-rotY, 0, 0);

        //rotate the player horizontally
        transform.parent.rotation = Quaternion.Euler(0, rotX, 0);
    }

    public void ToggleCameraMovement(bool isActive)
    {
        canMove = isActive;
    }
}
