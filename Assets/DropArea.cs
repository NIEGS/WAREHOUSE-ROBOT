using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class defines a drop area where specific interactions with the Robot objects are handled.
public class DropArea : MonoBehaviour
{
    // Called when the script instance is being loaded.
    void Start()
    {
        // Initialization logic can be added here if needed.
    }

    // Update is called once per frame.
    void Update()
    {
        // Update logic can be added here if needed.
    }

    // Called when another collider enters the trigger collider attached to this object.
    void OnTriggerEnter(Collider collider)
    {
        // Check if the collider belongs to a GameObject with a Robot component.
        Robot r = collider.gameObject.GetComponent<Robot>();
        if (r != null)
        {
            // Log an error message indicating that a Robot has entered the drop area.
            Debug.LogError("Robot drop");
        }
    }
}
