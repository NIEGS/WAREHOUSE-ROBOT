using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class defines the behavior of the robot's arm, which consists of multiple joints that can rotate on different axes.
public class Robot_arm : MonoBehaviour
{
    // Enum representing the possible rotation axes for the joints.
    public enum JointAxis
    {
        X, Y, Z
    }

    // Array of joint GameObjects that make up the robot arm.
    public GameObject[] joints;
    // Array of axes corresponding to each joint.
    public JointAxis[] joints_axis;
    // Arrays for the minimum and maximum angles that each joint can rotate to.
    public int[] minAngles;
    public int[] maxAngles;
    // Array of rotation speeds for each joint.
    public int[] rotateSpeed;

    // Array of target angles for each joint.
    public int[] targetAngles;

    // Flags indicating whether the arm is currently rotating and the number of rotations completed.
    public bool rotating = false;
    public int rotated = 0;

    // Reference to the robot's actuator.
    public Robot_actuator actuator;

    // List of quaternions representing the target rotations for each joint.
    List<Quaternion> quaternions = new List<Quaternion>();

    // Function to get the current rotation of a joint on its designated axis.
    float getJointRotation(int id)
    {
        if(joints_axis[id] == JointAxis.X)
        {
            return joints[id].transform.localRotation.eulerAngles.x;
        }
        else if (joints_axis[id] == JointAxis.Y)
        {
            return joints[id].transform.localRotation.eulerAngles.y;
        }
        else
        {
            return joints[id].transform.localRotation.eulerAngles.z;
        }
    }

    // Function to create a quaternion for a joint's rotation on its designated axis.
    Quaternion JointQuaternion(int id, float angle)
    {
        Quaternion newRotation = Quaternion.Euler(0, 0, 0);

        if (id >= 0 && id < joints.Length)
        {
            if (joints_axis[id] == JointAxis.X)
            {
                newRotation = Quaternion.Euler(angle, 0, 0);
            }
            else if (joints_axis[id] == JointAxis.Y)
            {
                newRotation = Quaternion.Euler(0, angle, 0);
            }
            else
            {
                newRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        return newRotation;
    }

    // Function to set the rotation speed for a specific joint.
    public void setJointSpeed(int id, int speed)
    {
        if (id >= 0 && id < rotateSpeed.Length)
        {
            rotateSpeed[id] = speed;
        }
    }

    // Function to set the target rotation for a specific joint.
    public void rotateJoint(int id, float angle)
    {
        if (angle < 0)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }

        Quaternion newRotation = JointQuaternion(id, angle);
        quaternions[id] = newRotation;
        targetAngles[id] = (int)angle;
    }

    // Enable the actuator.
    public void enableActuator()
    {
        actuator.enableActuator();
    }

    // Disable the actuator.
    public void disableActuator()
    {
        actuator.disableActuator();
    }

    // Initialization function.
    void Start()
    {
        foreach(GameObject g in joints)
        {
            quaternions.Add(Quaternion.Euler(0, 0, 0));
        }
    }

    // Public variables for tracking the current joint and the number of joints rotated.
    public int id = 0;
    public int rotatedN = 0;

    // Update function called once per frame.
    void Update()
    {
        if (rotating)
        {
            id = 0;
            rotatedN = 0;
            foreach (GameObject g in joints)
            {
                int targetRotation = targetAngles[id];
                int rotation = (int)getJointRotation(id);
                Transform jointTransform = g.transform;
                Quaternion newRotation = quaternions[id];
                int speed = rotateSpeed[id];

                if(rotation < 0)
                {
                    rotation += 360;
                }
                if (rotation > 360)
                {
                    rotation -= 360;
                }

                // Check if the joint has reached its target rotation.
                if (rotation > targetRotation - 2 && rotation < targetRotation + 2)
                {
                    rotatedN++;
                }
                else
                {
                    jointTransform.localRotation = Quaternion.RotateTowards(jointTransform.localRotation, newRotation, speed * Time.deltaTime);
                    rotation = (int)getJointRotation(id);

                    // Debug log to check joint rotation.
                    // Debug.Log("ID: " + id + " rot:" + rotation + " target: " + targetRotation + " quat: (x: " + newRotation.x + ",y: " + newRotation.y + ",z: " + newRotation.z + ")");
                }

                id++;
            }

            // If all joints have reached their target rotations, update the rotation status.
            if(rotatedN == id)
            {
                rotated++;
                rotating = false;
            }
        }
    }
}
