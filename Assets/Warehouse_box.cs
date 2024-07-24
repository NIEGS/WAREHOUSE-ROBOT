using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a box in the warehouse that can be picked up and moved by robots.
public class Warehouse_box : MonoBehaviour
{
    // Indicates whether the box is currently picked up by a robot.
    public bool picked = false;

    // The transform of the actuator picking up the box.
    public Transform act = null;

    // The Rigidbody component of the box.
    public Rigidbody rig;

    // The BoxCollider component of the box.
    public BoxCollider bc;

    // The transform of the shelf the box belongs to.
    public Transform bshelf;

    // Coroutine to disable gravity and make the box kinematic after a delay.
    IEnumerator disableGravity()
    {
        yield return new WaitForSeconds(2.0f); // Wait for 2 seconds.

        rig.isKinematic = true; // Make the box kinematic.
        rig.useGravity = false; // Disable gravity.
        bc.enabled = false; // Disable the box collider.
    }

    // Method to enable gravity and disable kinematic mode.
    public void enableGravity()
    {
        rig.isKinematic = false; // Disable kinematic mode.
        rig.useGravity = true; // Enable gravity.
        bc.enabled = true; // Enable the box collider.

        if (bshelf != null)
        {
            transform.SetParent(bshelf); // Set the box's parent to its original shelf.
        }
    }

    // Method to unpick the box, re-enabling gravity and starting the disableGravity coroutine.
    public void unpick()
    {
        picked = false; // Mark the box as not picked.
        rig.isKinematic = false; // Disable kinematic mode.
        rig.useGravity = true; // Enable gravity.

        StartCoroutine(disableGravity()); // Start the coroutine to disable gravity after a delay.
    }

    // Update is called once per frame.
    void Update()
    {
        // If the box is picked and an actuator is present, move the box to the actuator's position.
        if (picked && act != null)
        {
            transform.position = act.transform.position;
        }        
    }
}
