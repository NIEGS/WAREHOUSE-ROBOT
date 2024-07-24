using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages the orders within the warehouse, including assigning robots to tasks and handling shelves.
public class Warehouse_orders : MonoBehaviour
{
    // Reference to the warehouse.
    public Warehouse warehouse;

    // Array of shelves in the warehouse.
    public Warehouse_shelf[] shelves;

    // Flag indicating if robots are ready for assignments.
    public bool robotsReady = false;

    // Reference to the warehouse training system.
    public Warehouse_training warehouse_Training;

    // Nested class representing information about an order.
    public class OrderInfo
    {
        public Warehouse_node node; // The node where the order is located.
        public int quantity = 0; // The quantity of products to be picked.

        public OrderInfo(Warehouse_node node, int quantity)
        {
            this.node = node;
            this.quantity = quantity;
        }
    }

    // List of global orders in the warehouse.
    public List<OrderInfo> globalOrders = new List<OrderInfo>();

    // Method to find all shelves in the warehouse and store them in the shelves array.
    void getAllShelves()
    {
        shelves = FindObjectsOfType(typeof(Warehouse_shelf)) as Warehouse_shelf[];
        Debug.Log(shelves.Length + " estanterias encontradas en el almacen");
    }

    // Method to initialize all shelves by assigning them an ID and setting their warehouse orders reference.
    void initializeShelves()
    {
        int i = 0;
        foreach (Warehouse_shelf r in shelves)
        {
            r.id = i;
            i++;
            r.warehouse_orders = this;
        }

        // Start training if the training mode is enabled.
        if (Warehouse_training.trainingMode)
        {
            warehouse_Training.startTrainingIteration();
        }
    }

    // Method for basic assignment of orders to robots.
    void basicAssignation(Warehouse_shelf n, Warehouse_node nodo)
    {
        int order_quantity = n.products_to_pick;
        foreach (Robot r in warehouse.robots)
        {
            if (r.RobotState == Robot.RobotStates.Available && r.containerFilled < r.containerCapacity)
            {
                order_quantity = n.products_to_pick;
                if (order_quantity > 0)
                {
                    int canPickup = r.containerCapacity - r.containerFilled;
                    int quantityToPickup = 0;

                    if (order_quantity > canPickup)
                    {
                        quantityToPickup = canPickup;
                        n.products_to_pick = order_quantity - canPickup;
                    }
                    else
                    {
                        quantityToPickup = order_quantity;
                        n.products_to_pick = 0;
                    }

                    n.products -= quantityToPickup;
                    r.shelf_target = n;
                    r.warehousenodeTarget = nodo;
                    warehouse.setRobotRoute(r.robotID, -1, nodo.nodeID);
                    r.RobotState = Robot.RobotStates.OnWayToPick;
                    r.containerFilled += quantityToPickup;
                }
            }
        }
    }

    // Structure to hold the weights for the assignment formula.
    public struct formulaWeights
    {
        public float distance;
        public float quantityToPickup;
        public float filled;
    }

    public formulaWeights formula_weights;

    // Method called before the first frame update.
    void Start()
    {
        // Load formula weights from disk if available, otherwise use default values.
        if (PlayerPrefs.HasKey("formula_weight_distance"))
        {
            formula_weights.distance = PlayerPrefs.GetFloat("formula_weight_distance");
        }
        else
        {
            formula_weights.distance = 0.4151846f;
        }

        if (PlayerPrefs.HasKey("formula_weight_quantityToPickup"))
        {
            formula_weights.quantityToPickup = PlayerPrefs.GetFloat("formula_weight_quantityToPickup");
        }
        else
        {
            formula_weights.quantityToPickup = 0.1354322f;
        }

        if (PlayerPrefs.HasKey("formula_weight_filled"))
        {
            formula_weights.filled = PlayerPrefs.GetFloat("formula_weight_filled");
        }
        else
        {
            formula_weights.filled = 0.1040449f;
        }

        getAllShelves(); // Find and store all shelves.
        initializeShelves(); // Initialize shelves by assigning IDs and setting references.
    }

    // Method to calculate the metric for assigning a robot to a task.
    float calculateMetric(Robot r, Warehouse_shelf n, Warehouse_node nodo, int quantityToPickup)
    {
        float dist = Vector2.Distance(r.get2dvectortransform(r.transform.position), r.get2dvectortransform(nodo.transform.position));
        float filled = r.containerFilled;
        float m = formula_weights.distance * dist + formula_weights.quantityToPickup * (1 / quantityToPickup) + formula_weights.filled * filled;
        return m;
    }

    // Method for improved assignment of orders to robots based on a metric.
    void improvedAssignation(Warehouse_shelf n, Warehouse_node nodo)
    {
        Robot bestRobotForTask = null;
        float bestMetric = float.MaxValue; // Initialize with maximum value.
        int bestRobotquantityToPickup = 0;
        int bestProductstoPick = 0;

        int order_quantity = n.products_to_pick;
        foreach (Robot r in warehouse.robots)
        {
            if (r.RobotState == Robot.RobotStates.Available && r.containerFilled < r.containerCapacity)
            {
                order_quantity = n.products_to_pick;
                if (order_quantity > 0)
                {
                    int canPickup = r.containerCapacity - r.containerFilled;
                    int quantityToPickup = 0;
                    int ProductstoPick = 0;

                    if (order_quantity > canPickup)
                    {
                        quantityToPickup = canPickup;
                        ProductstoPick = order_quantity - canPickup;
                    }
                    else
                    {
                        quantityToPickup = order_quantity;
                        ProductstoPick = 0;
                    }

                    float metric = calculateMetric(r, n, nodo, quantityToPickup);
                    if (metric < bestMetric)
                    {
                        bestRobotquantityToPickup = quantityToPickup;
                        bestMetric = metric;
                        bestRobotForTask = r;
                        bestProductstoPick = ProductstoPick;
                    }
                }
            }
        }

        if (bestRobotForTask != null)
        {
            n.products -= bestRobotquantityToPickup;
            n.products_to_pick = bestProductstoPick;
            bestRobotForTask.shelf_target = n;
            bestRobotForTask.warehousenodeTarget = nodo;
            warehouse.setRobotRoute(bestRobotForTask.robotID, -1, nodo.nodeID);
            bestRobotForTask.RobotState = Robot.RobotStates.OnWayToPick;
            bestRobotForTask.containerFilled += bestRobotquantityToPickup;
            Debug.Log("Robot with best metric for task: " + bestRobotForTask.robotID + " with metric: " + bestMetric);
        }
    }

    // Method to choose the assignment strategy for a shelf and node.
    void assignationStrategy(int strategy, Warehouse_shelf n, Warehouse_node nodo)
    {
        switch (strategy)
        {
            case 0:
                basicAssignation(n, nodo);
                break;
            case 1:
                improvedAssignation(n, nodo);
                break;
        }
    }

    // Method to assign robots to tasks based on the chosen strategy.
    void assignRobot()
    {
        foreach (Warehouse_shelf n in shelves)
        {
            Warehouse_node nodo = n.node1;

            // Randomly choose between the two nodes of the shelf.
            if (Random.Range(0, 100) > 50)
            {
                nodo = n.node2;
            }

            int order_quantity = n.products_to_pick;

            if (order_quantity > 0)
            {
                assignationStrategy(1, n, nodo);
            }
        }
    }

    // Method to add a new order to the global orders list.
    public void addOrder(Warehouse_node node, int quantity)
    {
        OrderInfo kv = new OrderInfo(node, quantity);
        globalOrders.Add(kv);
    }

    // Update is called once per frame.
    void Update()
    {
        // If robots are ready, assign them to tasks.
        if (robotsReady)
        {
            assignRobot();
        }
    }
}
