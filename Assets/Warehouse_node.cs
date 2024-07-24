using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a node in the warehouse, which can be used for navigation.
public class Warehouse_node : MonoBehaviour
{
    // Unique identifier for the node.
    public int nodeID = -1;

    // Directional flags indicating the node's alignment.
    public bool directionX = true;
    public bool directionZ = false;

    // Identifiers for the neighboring nodes (edges).
    public int Edge1 = -1;
    public int Edge2 = -1;
    public int Edge3 = -1;
    public int Edge4 = -1;

    // Reference to the row this node belongs to.
    NodesRow nodeRow;

    // Start is called before the first frame update.
    void Start()
    {
        // Get the parent row component.
        nodeRow = this.gameObject.GetComponentInParent<NodesRow>();
        if (nodeRow != null)
        {
            // Assign the row ID to Edge2 if the node row is found.
            Edge2 = nodeRow.RowID;
        }
    }

    // Update is called once per frame.
    void Update()
    {
        // This method is intentionally left empty.
    }
}
