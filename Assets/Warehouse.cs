using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages the overall behavior of the warehouse, including the robots and nodes within it.
public class Warehouse : MonoBehaviour
{
    // Array of robots in the warehouse.
    public Robot[] robots;

    // Array of nodes in the warehouse.
    public Warehouse_node[] nodos;

    // Reference to the warehouse orders system.
    public Warehouse_orders warehouse_Orders;

    // Method to find all robots in the warehouse and store them in the robots array.
    void getAllRobots()
    {
        robots = FindObjectsOfType(typeof(Robot)) as Robot[];
        Debug.Log(robots.Length + " robots encontrados en el almacen");
    }

    // Method to initialize all robots by assigning them an ID and setting their warehouse reference.
    void initializeRobots()
    {
        int i = 0;
        foreach (Robot r in robots)
        {
            r.robotID = i;
            i++;

            r.warehouse = this;
        }
    }

    // Method to find all nodes in the warehouse and store them in the nodos array.
    void getAllNodes()
    {
        nodos = FindObjectsOfType(typeof(Warehouse_node)) as Warehouse_node[];
        Debug.Log(nodos.Length + " nodos encontrados en el almacen");
    }

    // Method to initialize all nodes by assigning them an ID.
    void initializeNodes()
    {
        int i = 0;
        foreach (Warehouse_node node2 in nodos)
        {
            node2.nodeID = i;
            i++;
        }
    }

    // Method to set a route for a specific robot from origin to destination.
    public void setRobotRoute(int n, int origin, int destination)
    {
        Robot rob = robots[n];

        // If the origin is -1, use the robot's closest arrived node as the origin.
        if (origin == -1)
        {
            origin = rob.closestNodeArrived;
        }

        // Calculate the optimal route and create the new route for the robot.
        ArrayList r = rob.optimalRoute.calculateOptimalRoute(nodos[origin], nodos[destination]);
        rob.createNewRoute(r);
    }

    // Method to set the first route for a robot, sending it to the closest node and then to the destination.
    void firstRobotRoute(int robotid, int destination)
    {
        robots[robotid].gotoClosestNode();
        setRobotRoute(robotid, -1, destination);
    }

    // Method to send all robots to their closest nodes.
    void robotsToClosestNodes()
    {
        foreach (Robot robot in robots)
        {
            robot.RobotState = Robot.RobotStates.NotReady;
            robot.gotoClosestNode();
        }
    }

    // Start is called before the first frame update.
    void Start()
    {
        getAllNodes(); // Find and store all nodes.

        initializeNodes(); // Initialize nodes by assigning IDs.

        getAllRobots(); // Find and store all robots.

        initializeRobots(); // Initialize robots by assigning IDs and setting warehouse reference.

        robotsToClosestNodes(); // Send all robots to their closest nodes.

        warehouse_Orders.robotsReady = true; // Indicate that robots are ready.

        // Uncomment the following lines to set initial routes for robots.
        /*firstRobotRoute(0, 82);
        firstRobotRoute(1, 138);
        firstRobotRoute(2, 191);
        firstRobotRoute(3, 43);*/
    }

    // Update is called once per frame.
    void Update()
    {
        // This method is intentionally left empty.
    }
}
