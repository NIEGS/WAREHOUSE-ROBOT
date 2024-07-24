using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the camera movement and rotation in the game.
public class CameraControl : MonoBehaviour
{
    // Variables to control rotation and movement speeds.
    public float rotateSpeed = 150f;
    public float moveSpeed = 50f;
    public float verticalSpeed = 25f;

    // Variables to store the current rotation of the camera.
    float rotationX = 0;
    float rotationY = 0;

    // Variable to toggle fast mode for movement.
    public bool fastMode = true;

    // Initialization of the camera control script.
    void Start()
    {
        // Set initial rotation based on the local rotation of the transform.
        rotationX = transform.localRotation.x;
        rotationY = transform.localRotation.y;

        // Lock the cursor and make it invisible.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame to handle input and camera movement.
    void Update()
    {
        // Check if the cursor is not visible to allow camera rotation.
        if (!Cursor.visible)
        {
            // Get mouse input for rotation and adjust the rotation variables.
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

            rotationX += mouseX;
            rotationY += mouseY;

            // Clamp the vertical rotation to prevent flipping.
            rotationY = Mathf.Clamp(rotationY, -85.0f, 85.0f);

            // Apply the rotation to the transform.
            transform.localRotation = Quaternion.Euler(-rotationY, rotationX, 0);
        }

        // Lock the cursor and make it invisible when the left mouse button is clicked.
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Unlock the cursor and make it visible when the Escape key is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Handle forward movement when W or UpArrow is pressed.
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 position2 = transform.position + transform.forward * moveSpeed * Time.deltaTime;
            transform.position = new Vector3(position2.x, transform.position.y, position2.z);
        }

        // Handle left movement when A or LeftArrow is pressed.
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self);
        }

        // Handle right movement when D or RightArrow is pressed.
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
        }

        // Handle backward movement when S or DownArrow is pressed.
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position2 = transform.position - transform.forward * moveSpeed * Time.deltaTime;
            transform.position = new Vector3(position2.x, transform.position.y, position2.z);
        }

        // Handle upward movement when Space is pressed.
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
        }
        // Handle downward movement when LeftShift is pressed, ensuring the camera doesn't go below a certain level.
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (transform.position.y > 2 * verticalSpeed * Time.deltaTime)
            {
                transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime, Space.World);
            }
        }

        // Toggle fast mode when V is pressed, adjusting the movement and rotation speeds accordingly.
        if (Input.GetKey(KeyCode.V))
        {
            fastMode = !fastMode;

            if (fastMode)
            {
                rotateSpeed = 150f;
                moveSpeed = 50f;
                verticalSpeed = 25f;
            }
            else
            {
                rotateSpeed = 60f;
                moveSpeed = 10f;
                verticalSpeed = 10f;
            }
        }
    }
}
