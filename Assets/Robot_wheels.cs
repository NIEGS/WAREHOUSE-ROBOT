using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class defines the behavior of the robot's wheels, controlling movement and rotation.
public class Robot_wheels : MonoBehaviour
{
    // Transforms representing the four wheels of the robot.
    public Transform wheel1;
    public Transform wheel2;
    public Transform wheel3;
    public Transform wheel4;

    // Reference to the main robot object.
    public Robot robot;

    // Speed variables for each wheel.
    public float speed1 = 20;
    public float speed2 = 20;
    public float speed3 = 20;
    public float speed4 = 20;

    // Flags to control the movement state of the robot.
    public bool stop = false;
    public bool forward_rotate = false; // true for forward, false for rotate

    // Initialization function.
    void Start()
    {
        // This method is intentionally left empty.
    }

    // Method to move the robot forward at a given speed.
    public void goForward(float speed)
    {
        speed1 = -20 * speed;
        speed2 = -20 * speed;
        speed3 = -20 * speed;
        speed4 = -20 * speed;

        forward_rotate = true;
        stop = false;
    }

    // Method to rotate the robot at a given speed.
    public void rotate(float speed)
    {
        speed1 = -3 * speed;
        speed2 = -3 * speed;
        speed3 = 3 * speed;
        speed4 = 3 * speed;

        forward_rotate = false;
        stop = false;
    }

    // Update function called once per frame.
    void Update()
    {
        // Check if the robot is not picking up an object and is not stopped, and if the right sensor is not colliding or the robot is rotating.
        if (robot.RobotState != Robot.RobotStates.PickingUp && !stop && ((!robot.rightSensorCollision && forward_rotate) | !forward_rotate))
        {
            // Rotate each wheel according to its speed.
            wheel1.Rotate(new Vector3(0, 0, speed1 * Time.deltaTime));
            wheel2.Rotate(new Vector3(0, 0, speed2 * Time.deltaTime));
            wheel3.Rotate(new Vector3(0, 0, speed3 * Time.deltaTime));
            wheel4.Rotate(new Vector3(0, 0, speed4 * Time.deltaTime));
        }
    }
}
