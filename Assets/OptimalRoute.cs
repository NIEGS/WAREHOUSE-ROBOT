using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for calculating the optimal route for a robot within a warehouse,
// taking into account node directions to avoid collisions.
public class OptimalRoute : MonoBehaviour
{
    // Reference to the robot that will follow the route.
    public Robot robot;

    // Called when the script instance is being loaded.
    void Start()
    {
        // Initialization logic can be added here if needed.
    }

    /* Each node has a direction of X and a direction of Z depending on the
     * row and column where it is located, to have lane directions in the warehouse
     * and avoid frontal collisions between robots.
     */
    bool nodeDirection(Warehouse_node n1, Warehouse_node n3)
    {
        bool r = false;

        // Check if the nodes share the same Edge1.
        if (n1.Edge1 == n3.Edge1)
        {
            int z1 = (int) n1.transform.position.z;
            int z3 = (int) n3.transform.position.z;

            // Ensure direction of movement along the Z-axis.
            if (!n1.directionZ && z3 <= z1)
            {
                r = true;
            }
            else if (n1.directionZ && z3 >= z1)
            {
                r = true;
            }
        }
        // Check if the nodes share the same Edge2.
        else if (n1.Edge2 == n3.Edge2)
        {
            int x1 = (int) n1.transform.position.x;
            int x3 = (int) n3.transform.position.x;

            // Ensure direction of movement along the X-axis.
            if (n1.directionX && x3 >= x1)
            {
                r = true;
            }
            else if (!n1.directionX && x3 <= x1)
            {
                r = true;
            }
        }

        return r;
    }

    // Variable to store the shortest distance found.
    float minimaDistanciaTotal = -1;

    // List to store the best route found.
    public List<Warehouse_node> bestRoute = new List<Warehouse_node>();

    // Recursive function to find the optimal route between two nodes.
    void routeRec(Warehouse_node n1, Warehouse_node n2, List<Warehouse_node> routeR, float distanciaActual)
    {
        foreach (Warehouse_node n3 in robot.warehouse.nodos)
        {
            if (n3 != n1 && routeR.Count < 6)
            {
                if (bestRoute.Count == 0 || distanciaActual <= minimaDistanciaTotal)
                {
                    if ((n1.Edge1 == n3.Edge1 && n1.Edge1 != -1) || (n1.Edge2 == n3.Edge2 && n1.Edge2 != -1))
                    {
                        if (nodeDirection(n1, n3))
                        {
                            if (!routeR.Contains(n3))
                            {
                                // Create a new route and add the current node.
                                List<Warehouse_node> newRoute = new List<Warehouse_node>(routeR);
                                newRoute.Add(n3);

                                // Calculate the new distance.
                                float newDistance = distanciaActual + Vector2.Distance(robot.get2dvectortransform(n1.transform.position), robot.get2dvectortransform(n3.transform.position));

                                // If the destination node is reached, update the best route if this one is shorter.
                                if (n3 == n2)
                                {
                                    if (bestRoute.Count == 0 || newDistance < minimaDistanciaTotal)
                                    {
                                        minimaDistanciaTotal = newDistance;
                                        bestRoute = newRoute;
                                    }
                                }
                                else
                                {
                                    // Continue searching recursively.
                                    routeRec(n3, n2, newRoute, newDistance);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Function to calculate the optimal route from node n1 to node n2.
    public ArrayList calculateOptimalRoute(Warehouse_node n1, Warehouse_node n2)
    {
        Debug.LogWarning("Begin route from " + n1.nodeID + " to " + n2.nodeID);

        // Reset the best route.
        bestRoute = new List<Warehouse_node>();

        // Start the route with the initial node.
        List<Warehouse_node> route = new List<Warehouse_node>();
        route.Add(n1);
        routeRec(n1, n2, route, 0);

        // Convert the best route nodes to an array of node IDs.
        ArrayList rutaIDs = new ArrayList();
        foreach (Warehouse_node n3 in bestRoute)
        {
            rutaIDs.Add(n3.nodeID);
            // Debug.LogWarning("Route node: " + n3.nodeID);
        }

        // Debug.Log("Min distance: " + minimaDistanciaTotal);

        return rutaIDs;
    }
}
