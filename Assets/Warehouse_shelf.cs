using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a shelf within the warehouse, managing its products and orders.
public class Warehouse_shelf : MonoBehaviour
{
    // The ID of the shelf.
    public int id = -1;

    // The total number of products on the shelf.
    public int products = 2;
    
    // The number of products to be picked from the shelf.
    public int products_to_pick = 0;

    // References to the nodes associated with this shelf.
    public Warehouse_node node1;
    public Warehouse_node node2;

    // Reference to the warehouse orders system.
    public Warehouse_orders warehouse_orders;

    // Array of boxes on the shelf.
    public Warehouse_box[] boxes;

    // Method called before the first frame update.
    void Start()
    {
        initializeShelf(); // Initialize the shelf by finding the warehouse orders system.
    }

    // Coroutine to generate an order for picking products from the shelf.
    public IEnumerator generateOrder()
    {
        int tiempo = Random.Range(1, 20); // Randomly wait between 1 and 20 seconds.
        yield return new WaitForSeconds(tiempo);

        // 20% chance to generate a new order immediately.
        if(Random.Range(0, 100) > 80)
        {
            products_to_pick++;
            Debug.Log("New order generated in node " + node1.nodeID);
        }
        else
        {
            tiempo = Random.Range(10, 80); // Randomly wait between 10 and 80 seconds.
            yield return new WaitForSeconds(tiempo);

            // 50% chance to generate a new order after the wait.
            if (Random.Range(0, 100) > 50)
            {
                products_to_pick++;
                Debug.Log("New order generated in node " + node1.nodeID);
            }
        }
    }

    // Method to initialize the shelf by finding the warehouse orders system.
    void initializeShelf()
    {
        warehouse_orders = FindObjectOfType(typeof(Warehouse_orders)) as Warehouse_orders;
    }

    // Flag to control order generation.
    bool generateOrderB = true;

    // Update is called once per frame.
    void Update()
    {
        // If order generation is enabled and training mode is not active, start generating orders.
        if (generateOrderB && !Warehouse_training.trainingMode)
        {
            if (warehouse_orders != null)
            {
                generateOrderB = false;
                StartCoroutine(generateOrder());
            }
        }
    }
}
