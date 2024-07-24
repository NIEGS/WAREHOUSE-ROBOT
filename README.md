# Autonomous Warehouse Robot
## Introduction
- In this project, it aims to develop an efficient and autonomous warehouse management system using a swarm of robots. Each robot is designed to perform tasks such as moving and organizing inventory within a warehouse. To achieve this, it must be implemented using simulations such as Unity together with the programming language C#. 
## Project Overview
- Each robot in this system consists of a base equipped with wheels, allowing it to navigate the warehouse. Attached to the base is a structure with an arm designed to pick up packages and place them into a basket structure located near the arm. This enables the robot to carry multiple packages simultaneously. The robots are equipped with GPS sensors for precise location tracking, and remote Wi-Fi connections for task reception.This simulation ensures that each robot can perform fundamental operations accurately. 
- The Unity simulation consists of multiple robots working collaboratively to collect boxes from a warehouse and transport them to the unloading area. To efficiently collect the boxes, scripts have been implemented that allow the robots to be controlled as a swarm. This swarm control assigns the optimal robot to each task based on specific metrics and employs a training system that adjusts the weights in the assignment formula to achieve optimal results. Additionally, each robot is completely autonomous, with functions for calculating and following optimal routes.
- By using Unity and C#, it emphasizes the efficiency of task distribution within the swarm, ensuring that tasks are allocated dynamically among the robots to maximize overall efficiency.
## Importance of Skills and Concepts Learned
- Through this project, it can help to apply various skills and concepts, including robotic kinematics, and swarm intelligence. The use of Unity and C# help to create a scalable and efficient simulation environment. This project not only demonstrates the practical application of these skills but also highlights their importance in developing advanced autonomous systems for real-world scenarios. And lastly, this module enhances skills of students in unity and C# programming language.
  
## Features and Functionalities
### Nodes with a single direction of circulation
- The warehouse is mapped as a directed graph using a system summarized by four variables for each node: two integers and two booleans. The integers represent the set of nodes to which each node belongs and is connected, while the booleans indicate the positive or negative directions in the X and Z axes
### Autonomous robots
- Each robot in the warehouse has scripts to calculate and follow optimal routes between nodes, allowing them to navigate efficiently from any point to another. If a robot is outside the warehouse, such as when charging its battery, it automatically locates and moves to the nearest node.
### Proximity sensors
- Simulated using triggers and box colliders, enabling robots to stop when they detect another robot in front of them. Robots also give priority to those detected on their right. Head-on collisions are prevented by the single-direction driving design of all nodes.
### Servometers & wheels
- Are simulated to rotate based on the robot's speed variable, ensuring that the wheels rotate faster as the robot moves faster
### Scalable robot arm script
- A fully scalable robotic arm control script allows easy addition of joints and axes by editing game object settings in the Unity inspector. Each axis can be configured by adjusting its rotation axes, minimum and maximum angles, rotation speed, and associated Transform Object. Additionally, a simulated pneumatic actuator is included to pick up boxes and store them in the robot's basket.
### Unloading boxes
- A fully scalable robotic arm control script allows easy addition of joints and axes by editing game object settings in the Unity inspector. Each axis can be configured by adjusting its rotation axes, minimum and maximum angles, rotation speed, and associated Transform Object.
### Unloading zone
- When robots reach their maximum box load capacity and are in the Available state, they automatically change to OnWayToDrop and head to the unloading zone. There, they smoothly unload the boxes onto a conveyor belt using a rotatable ramp, which then transports the boxes to a large container.
### Software architecture of the simulation
- The robot control script manages all basic behaviors autonomously, allowing it to operate independently. It includes functions for moving to nodes and following paths, as well as automatically rotating towards target locations. The script incorporates features such as a GPS sensor for position detection, a configurable robotic arm for picking up boxes, and simulated proximity sensors for collision avoidance. State management ensures the robot transitions smoothly between tasks like picking up, delivering, and unloading boxes. The script is highly customizable via the Unity inspector, facilitating adjustments to movement speed, rotation speed, and maximum load capacity. Multiple robots can be seamlessly integrated into the system without modifying code, making scalability straightforward.
### Each robot in the system operates with a defined state that indicates its current task readiness and activity status
- NotReady:The robot is outside the warehouse and hasn't reached the closest node yet.
- Available: The robot is ready and can be assigned a task.
- OnWayToPick: En route to pick up an assigned order from a shelf location.
- PickingUp: At the shelf, actively using the robotic arm to pick up the order.
- OnWayToDrop: Heading towards the unloading area to deliver picked-up orders stored in its basket.
- PrepareUnloading: Positioned at the unloading area, rotating for optimal box unloading.
- RampGoingDown: Lowering the ramp to unload boxes into the cargo container.
- Unloading: Engaged in the process of unloading boxes.
- RampGoingUp: Elevating the ramp after completing box unloading.

## Step by Step Guide 
  [View Project Documentation](https://docs.google.com/document/d/1z00WWJby2H_2fDr9AnieJ0wVluwxYejQ/editusp=sharing&ouid=101734205070446539416&rtpof=true&sd=true)

## Hands on Activities
  [View Project Documentation](https://docs.google.com/document/d/1QbCU5kkR3be_CvvwBwkss8JpcggZEdoY/edit?usp=sharing&ouid=101734205070446539416&rtpof=true&sd=true).








## Table of Contents
- [Introduction](#Introduction)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgements](#acknowledgements)

## Installation
### Prerequisites
- [Robot Operating System (ROS)](http://www.ros.org/)
- Python 3.x
- [Gazebo Simulator](http://gazebosim.org/)
- Additional dependencies listed in `requirements.txt`

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/autonomous-warehouse-robot.git
   cd autonomous-warehouse-robot
