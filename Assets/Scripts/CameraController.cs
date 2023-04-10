using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Clamping")]
    public float minY;
    public float maxY;

    [Header("Mouse Look Sensitivity")]
    public float xSensitivity;
    public float ySensitivity;

    [Header("Spectator Settings")]
    public float spectatorMovementSpeed;

    private float rotX;
    private float rotY;
    private bool isSpectating;

    void Start ()
    {
                                                                                // Locks the cursor to the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate ()
    {
                                                                                // Gets the mouse movement inputs
        rotX += Input.GetAxis("Mouse X") * xSensitivity;
        rotY += Input.GetAxis("Mouse Y") * ySensitivity;

                                                                                // Clamps the vertical rotation
        rotY = Mathf.Clamp(rotY, minY, maxY);

                                                                                // Cehcks if we are spectating
        if(isSpectating)
        {
                                                                                // Rotates the cam vertically
            transform.rotation = Quaternion.Euler(-rotY, 90, 0);

                                                                                // Camera Movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0;

            if (Input.GetKey(KeyCode.E))
            {
                y = 1;
            }
             
            else if (Input.GetKey(KeyCode.Q))
            {
                y = -1;
            }

            Vector3 direction = transform.right * x  + transform.forward * z;
            transform.position += direction * spectatorMovementSpeed * Time.deltaTime;
        }

        else
        {
                                                                                // Rotate the camera vertically
            transform.localRotation = Quaternion.Euler(-rotY, 0, 0);

                                                                                // Rotates the player horizontally
            transform.parent.rotation = Quaternion.Euler(0, rotX, 0);
        }
    }

    public void SetAsSpectator ()
    {
        isSpectating = true;
      // transform.parent = null;                                                // Disconnects the camera from the player object
    }
}