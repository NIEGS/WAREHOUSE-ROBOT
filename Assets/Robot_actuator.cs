using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class defines the behavior of the robot's actuator for picking up and dropping boxes.
public class Robot_actuator : MonoBehaviour
{
    // Positions for the actuator in enabled and disabled states.
    float p0 = -0.85f;
    float p1 = -1.15f;

    // Reference to the container where picked-up boxes will be stored.
    public GameObject boxesContainer;

    // Transform representing the suction point of the actuator.
    public Transform suctionTransform;

    // Reference to the Robot class for adding picked-up boxes to the robot's container.
    public Robot robot;

    // Reference to the box currently picked up by the actuator.
    Warehouse_box picked_box = null;

    // Enable the actuator, moving it to the position to pick up a box.
    public void enableActuator()
    {
        transform.localPosition = new Vector3(0, p1, 0);
    }

    // Disable the actuator, releasing any picked-up box and storing it in the container.
    public void disableActuator()
    {
        if (picked_box != null)
        {
            // Unpick the box and set its parent to the container.
            picked_box.unpick();
            picked_box.transform.SetParent(boxesContainer.transform);

            // Add the box to the robot's container.
            robot.addBox(picked_box);
        }

        // Move the actuator to the disabled position.
        transform.localPosition = new Vector3(0, p0, 0);

        // Clear the reference to the picked-up box.
        picked_box = null;
    }

    // Handle collisions with other objects.
    void OnTriggerEnter(Collider collider)
    {
        // Check if the collided object is a warehouse box.
        Warehouse_box r = collider.gameObject.GetComponent<Warehouse_box>();
        if (r != null)
        {
            // If the box is not already picked up and there is no other picked-up box, pick it up.
            if (!r.picked)
            {
                if (picked_box == null)
                {
                    picked_box = r;
                    r.act = suctionTransform;
                    r.picked = true;
                }
            }
        }
    }
}
