using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class defines the behavior and properties of a robot within the warehouse environment.
public class Robot : MonoBehaviour
{
    // Identifier for the robot.
    public int robotID = -1;
    
    // Position vector for various purposes.
    Vector3 positionB;
    
    // Speed properties for movement and rotation.
    public int robotSpeed = 5;
    public float rotateSpeed = 30;

    // Reference to the OptimalRoute script for calculating routes.
    public OptimalRoute optimalRoute;

    // Rotation properties.
    public float Yrotation = 0;
    public float targetYrotation = 50;

    // Reference to the warehouse object the robot is in.
    public Warehouse warehouse;

    // IDs for the closest and target nodes the robot is navigating to.
    public int closestNodeArrived = -1;
    public int TargetNodeID = -1;

    // Container capacity and current fill level.
    public int containerCapacity = 2; // Total number of boxes the robot can store in its container.
    public int containerFilled = 0; // Number of boxes currently in the robot's container.

    // Collision detection flags.
    public bool rightSensorCollision = false;

    // Reference to the robot's arm component.
    public Robot_arm robot_Arm;

    // Reference to the target shelf and a flag indicating which shelf node to target.
    public Warehouse_shelf shelf_target;
    public bool shelf_node_p = true; // true for 1, false for 2.

    // Z-axis increment flag.
    public bool incZ = false;

    // Reference to the target warehouse node.
    public Warehouse_node warehousenodeTarget;

    // Enumeration defining the possible states of the robot.
    public enum RobotStates
    {
        Available, OnWayToPick, PickingUp, OnWayToDrop, PrepareUnloading, RampGoingDown, Unloading, RampGoingUp, NotReady
    }

    // Current state of the robot.
    public RobotStates RobotState = RobotStates.NotReady;

    // Reference to the robot's wheels component.
    public Robot_wheels robot_Wheels;

    // Reference to the drop area and its node.
    public DropArea dropArea;
    public Warehouse_node dropNode;

    // Reference to the ramp transform.
    public Transform ramp;

    // Reference to the robot's collider.
    public Collider colliderRobot;

    // Called when the script instance is being loaded.
    void Start()
    {
        // Initialize rotation property.
        Yrotation = transform.eulerAngles.y;

        // Find and assign the drop area and its corresponding node.
        dropArea = FindObjectOfType(typeof(DropArea)) as DropArea;
        if (dropArea != null)
        {
            dropNode = dropArea.GetComponentInParent<Warehouse_node>();
        }

        // Ignore collisions between specific layers.
        Physics.IgnoreLayerCollision(9, 10);
        Physics.IgnoreLayerCollision(10, 10);
    }

    // Converts a 3D vector to a 2D vector by dropping the y-component.
    public Vector2 get2dvectortransform(Vector3 v3)
    {
        Vector2 v;
        v.x = v3.x;
        v.y = v3.z;

        return v;
    }

    // Navigate to the closest node in the warehouse.
    public void gotoClosestNode()
    {
        float mindist = -1;
        Warehouse_node closestNode = null;

        // Loop through all nodes in the warehouse to find the closest one.
        foreach (Warehouse_node node2 in warehouse.nodos)
        {
            float dist = Vector2.Distance(get2dvectortransform(transform.position), get2dvectortransform(node2.transform.position));

            if (closestNode == null || dist < mindist)
            {
                mindist = dist;
                closestNode = node2;
            }
        }

        // Update position and node ID properties for the closest node.
        positionB = closestNode.transform.position;
        closestNodeArrived = closestNode.nodeID;

        Debug.Log("En camino al nodo mas cercano: " + closestNode.transform.position);
    }

    // Flag indicating if the robot is on its way to the closest node.
    bool wayToClosestNode = false;

    // Last destination node the robot was heading to.
    Warehouse_node lastDestination;

    // Navigate to a specific node in the warehouse.
    void gotoNode(Warehouse_node node)
    {
        if (lastDestination == null || lastDestination != node)
        {
            if (!wayToClosestNode)
            {
                wayToClosestNode = true;
                gotoClosestNode();
            }
            else
            {
                lastDestination = node;
                positionB = node.transform.position;
                incZ = (positionB.z > transform.position.z) ? true : false;
                TargetNodeID = node.nodeID;
            }
        }
    }

    // List to store boxes that the robot is carrying.
    List<Warehouse_box> boxes = new List<Warehouse_box>();

    // Add a box to the robot's container.
    public void addBox(Warehouse_box b)
    {
        boxes.Add(b);
    }

    // Flag indicating if the boxes have been unloaded.
    bool unloadedBoxes = false;

    // Unload all boxes from the robot's container.
    public void unloadBoxes()
    {
        foreach (Warehouse_box box in boxes)
        {
            box.enableGravity();
        }

        boxes.Clear();
    }

    // Navigate to the drop area for unloading.
    void goToDrop()
    {
        if (dropNode != null)
        {
            ArrayList r = optimalRoute.calculateOptimalRoute(warehouse.nodos[closestNodeArrived], dropNode);
            createNewRoute(r);
        }
        else
        {
            RobotState = RobotStates.Available;
        }
    }

    // Index of the last route position visited by the robot.
    int lastRoutePositionVisited = -1;

    // Follow the specified route of nodes.
    void goToRoute(ArrayList nodesToVisit)
    {
        if (lastRoutePositionVisited == -1)
        {
            //wayToClosestNode = true;
            //gotoClosestNode();
        }
        else if (lastRoutePositionVisited < nodesToVisit.Count)
        {
            int i = 0;
            foreach (int n in nodesToVisit)
            {
                if (i >= lastRoutePositionVisited)
                {
                    gotoNode(warehouse.nodos[n]);
                    Debug.LogWarning("Camino al nodo " + n + " en la posicion de la ruta num " + i);
                    break;
                }
                i++;
            }
        }
    }

    // ArrayList to store the route of nodes.
    ArrayList nodeRoute = new ArrayList();

    // Create a new route for the robot to follow.
    public void createNewRoute(ArrayList r)
    {
        lastRoutePositionVisited = -1;
        nodeRoute = r;
    }

    // Calculate the target rotation angle based on the destination position.
    void calculateTargetYrotation(Vector3 tow)
    {
        if (Vector2.Distance(get2dvectortransform(transform.position), get2dvectortransform(positionB)) <= Time.deltaTime * robotSpeed * 5)
        {
            return;
        }

        Vector2 v2 = get2dvectortransform(transform.position);
        float a = Vector2.Angle(Vector2.right, get2dvectortransform(positionB) - v2) + 90;

        targetYrotation = (int)a;

        if (targetYrotation > 360)
        {
            targetYrotation -= 360;
        }

        if (targetYrotation < 0)
        {
            targetYrotation += 360;
        }

        
        if (incZ)//positionB.z > transform.position.z - 2)
        {
            if (targetYrotation < -177 && targetYrotation > -183)
            {
                targetYrotation += 180;
            }
            else if (targetYrotation > 177 && targetYrotation < 183)
            {
                targetYrotation -= 180;
            }
        }
    }

    // Coroutine to wait for a specific time before unloading.
    IEnumerator waitToUnload()
    {
        yield return new WaitForSeconds(0.5f);

        RobotState = RobotStates.RampGoingUp;

        startedWaitingUnload = false;
    }

    // Constants and properties for ramp rotation.
    const float rampUpRotation = 0.0f;
    const float rampDownRotation = 320.0f;
    Quaternion rampUpQuaternion = Quaternion.Euler(0, 90, rampUpRotation);
    Quaternion rampDownQuaternion = Quaternion.Euler(0, 90, rampDownRotation);
    float rampRotation = 0;
    float rampTargetRotation = 0;
    Quaternion rampTargetQuaternion;
    bool startedWaitingUnload = false;

    // Update is called once per frame.
    void Update()
    {
        // Handle ramp rotation and state transitions.
        if (RobotState == RobotStates.RampGoingDown || RobotState == RobotStates.RampGoingUp)
        {
            robot_Wheels.stop = true;

            rampRotation = ramp.localRotation.eulerAngles.z;

            if (rampRotation < 0)
            {
                rampRotation += 360;
            }
            if (rampRotation >= 360)
            {
                rampRotation -= 360;
            }

            if(RobotState == RobotStates.RampGoingDown)
            {
                rampTargetRotation = rampDownRotation;
                rampTargetQuaternion = rampDownQuaternion;

                if (!unloadedBoxes)
                {
                    unloadedBoxes = true;
                    unloadBoxes();
                }
            }
            else
            {
                rampTargetRotation = rampUpRotation;
                rampTargetQuaternion = rampUpQuaternion;
            }

            if (rampRotation > rampTargetRotation - 2 && rampRotation < rampTargetRotation + 2)
            {
                RobotState = (RobotState == RobotStates.RampGoingDown) ? RobotStates.Unloading : RobotStates.Available;
            }
            else
            {
                ramp.localRotation = Quaternion.RotateTowards(ramp.localRotation, rampTargetQuaternion, 9.0f * Time.deltaTime);
            }
        }
        else if (RobotState == RobotStates.Unloading)
        {
            robot_Wheels.stop = true;

            containerFilled = 0;

            if (!startedWaitingUnload)
            {
                startedWaitingUnload = true;
                unloadedBoxes = false;
                StartCoroutine(waitToUnload());
            }
        }
        else
        {
            // Handle movement and state transitions when the robot is not interacting with the ramp.
            if (RobotState != RobotStates.PrepareUnloading && Vector2.Distance(get2dvectortransform(transform.position), get2dvectortransform(positionB)) <= Time.deltaTime * robotSpeed * 2)
            {
                if (TargetNodeID != -1)
                {
                    closestNodeArrived = TargetNodeID;
                    TargetNodeID = -1;
                }

                if (lastRoutePositionVisited > nodeRoute.Count)
                {
                    robot_Wheels.stop = true;

                    if (RobotState == RobotStates.OnWayToPick)
                    {
                        if (warehousenodeTarget.nodeID == closestNodeArrived)
                        {
                            RobotState = RobotStates.PickingUp;
                            robot_Arm.rotated = 0;

                            if (Warehouse_training.trainingMode)
                            {
                                Warehouse_training.robotsFinished++;
                            }
                        }
                        else
                        {
                            if (optimalRoute.bestRoute.Count == 0)
                            {
                                RobotState = RobotStates.Available;
                            }
                        }
                    }
                    else if (RobotState == RobotStates.OnWayToDrop)
                    {
                        RobotState = RobotStates.PrepareUnloading;
                    }
                }

                if (RobotState == RobotStates.NotReady && closestNodeArrived != -1)
                {
                    RobotState = RobotStates.Available;
                }

                goToRoute(nodeRoute);
                lastRoutePositionVisited++;
            }
            else
            {
                // Move towards the target position.
                Vector3 tow = Vector3.MoveTowards(transform.localPosition, positionB, Time.deltaTime * robotSpeed);

                if (RobotState == RobotStates.PrepareUnloading)
                {
                    targetYrotation = 0;
                }
                else
                {
                    calculateTargetYrotation(tow);
                }

                if (targetYrotation > 359)
                {
                    targetYrotation -= 360;
                }
                else if (targetYrotation < 0)
                {
                    targetYrotation += 360;
                }

                if (Yrotation < targetYrotation - 2 || Yrotation > targetYrotation + 2)
                {
                    if (RobotState != RobotStates.PickingUp)
                    {
                        Quaternion newRotation = Quaternion.Euler(0, targetYrotation, 0);

                        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);

                        float ad = Yrotation - targetYrotation;
                        if (ad < 0)
                        {
                            ad *= -1;
                        }
                        if (ad > 15)
                        {
                            robot_Wheels.rotate(rotateSpeed);
                        }
                    }
                }
                else
                {
                    if (RobotState == RobotStates.PrepareUnloading)
                    {
                        RobotState = RobotStates.RampGoingDown;
                    }
                    else
                    {
                        robot_Wheels.forward_rotate = true;

                        if (!rightSensorCollision)
                        {
                            transform.localPosition = tow;

                            robot_Wheels.goForward(robotSpeed);
                        }
                    }
                }

                Yrotation = transform.rotation.eulerAngles.y;

                if (Yrotation > 359)
                {
                    Yrotation -= 360;
                }
                else if (Yrotation < 0)
                {
                    Yrotation += 360;
                }
            }

            // Handle the picking up state.
            if (RobotState == RobotStates.PickingUp)
            {
                if (robot_Arm.rotated == 6)
                {
                    RobotState = RobotStates.Available;
                }
                else
                {
                    if (!robot_Arm.rotating)
                    {
                        robot_Arm.rotating = true;

                        if (robot_Arm.rotated == 0)
                        {
                            robot_Arm.setJointSpeed(2, 20);
                            robot_Arm.rotateJoint(0, 0);
                            robot_Arm.rotateJoint(1, -40);
                            robot_Arm.rotateJoint(2, -95);
                        }
                        else if (robot_Arm.rotated == 1)
                        {
                            robot_Arm.enableActuator();
                        }
                        else if (robot_Arm.rotated == 2)
                        {
                            robot_Arm.rotateJoint(0, 0);
                            robot_Arm.rotateJoint(1, 0);
                            robot_Arm.rotateJoint(2, -20);
                        }
                        else if (robot_Arm.rotated == 3)
                        {
                            robot_Arm.rotateJoint(0, 90);
                            robot_Arm.rotateJoint(1, 20);
                            robot_Arm.rotateJoint(2, -140);
                        }
                        else if (robot_Arm.rotated == 4)
                        {
                            robot_Arm.disableActuator();
                        }
                        else if (robot_Arm.rotated == 5)
                        {
                            robot_Arm.setJointSpeed(2, 60);
                            robot_Arm.rotateJoint(0, 90);
                            robot_Arm.rotateJoint(1, 0);
                            robot_Arm.rotateJoint(2, 0);
                        }
                    }
                }
            }
            // Handle the available state.
            else if (RobotState == RobotStates.Available)
            {
                if (containerCapacity == containerFilled)
                {
                    RobotState = RobotStates.OnWayToDrop;
                    goToDrop();
                }
            }
        }
    }

    // Handle collisions with warehouse boxes.
    void OnCollisionEnter(Collision collision)
    {
        Warehouse_box r = collision.gameObject.GetComponent<Warehouse_box>();

        if (r != null)
        {
            Physics.IgnoreCollision(collision.collider, colliderRobot);
        }
    }
}
