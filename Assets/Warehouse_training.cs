using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages the training mode of the warehouse, optimizing robot behavior through iterative training.
public class Warehouse_training : MonoBehaviour
{
    // Reference to the warehouse system.
    public Warehouse warehouse;

    // Reference to the warehouse orders system.
    public Warehouse_orders warehouse_Orders;

    // Flag to indicate if training mode is active.
    public static bool trainingMode = false;

    // Counter for the number of robots that have finished their tasks in an iteration.
    public static int robotsFinished = 0;

    // Struct to store the best formula weights found during training.
    public Warehouse_orders.formulaWeights bestWeights;

    // Variable to store the minimum loss (time) achieved in training.
    public float minLoss = -1;

    // Start is called before the first frame update.
    void Start()
    {
        if (trainingMode)
        {
            // Load the minimum loss from player preferences if it exists.
            if (PlayerPrefs.HasKey("formula_weight_loss"))
            {
                minLoss = PlayerPrefs.GetFloat("formula_weight_loss");
            }
        }
    }

    // Variables to track the iteration start time and elapsed time.
    float iteration_start_time = 0;
    float iteration_elapsed_time = 0;

    // Variable to store the learning rate for weight adjustments.
    float variable_learning_rate;

    // Method to generate new weights by applying a learning rate based on the elapsed iteration time.
    private void newWeights()
    {
        variable_learning_rate = Random.Range(-10.0f, 10.0f);
        warehouse_Orders.formula_weights.distance -= variable_learning_rate * (1 / iteration_elapsed_time);

        variable_learning_rate = Random.Range(-10.0f, 10.0f);
        warehouse_Orders.formula_weights.quantityToPickup -= variable_learning_rate * (1 / iteration_elapsed_time);

        variable_learning_rate = Random.Range(-10.0f, 10.0f);
        warehouse_Orders.formula_weights.filled -= variable_learning_rate * (1 / iteration_elapsed_time);

        //Debug.Log("New weights: " + warehouse_Orders.formula_weights.distance + ", " + warehouse_Orders.formula_weights.quantityToPickup + ", " + warehouse_Orders.formula_weights.filled);
    }

    // Flag to indicate if this is the first iteration of training.
    public bool firstIteration = true;

    // Method to start a new training iteration.
    public void startTrainingIteration()
    {
        Debug.Log("startTrainingIteration");

        // If it's not the first iteration, generate new weights.
        if (firstIteration)
        {
            firstIteration = false;
        }
        else
        {
            newWeights();
        }

        // Record the start time of the iteration.
        iteration_start_time = Time.time;

        // Randomly assign products to be picked from shelves for this iteration.
        foreach (Warehouse_shelf r in warehouse_Orders.shelves)
        {
            if(Random.Range(0.0f, 1.0f) > 0.6f)
            {
                r.products_to_pick = 1;
            }
        }
    }

    // Number of orders to be completed per iteration.
    int ordersPerIteration = 10;

    // Update is called once per frame.
    void Update()
    {
        if (trainingMode)
        {
            // If the required number of robots have finished their tasks, end the current iteration.
            if (robotsFinished >= ordersPerIteration)
            {
                robotsFinished = 0;

                // Calculate the elapsed time for the iteration.
                iteration_elapsed_time = Time.time - iteration_start_time;

                // Calculate the current loss (time taken).
                float cLoss = iteration_elapsed_time;

                // If the current loss is less than the minimum loss, or if no minimum loss has been set, update the minimum loss and best weights.
                if (cLoss < minLoss || minLoss <= 0)
                {
                    minLoss = cLoss;
                    bestWeights = warehouse_Orders.formula_weights;

                    PlayerPrefs.SetFloat("formula_weight_loss", cLoss);
                    PlayerPrefs.SetFloat("formula_weight_distance", bestWeights.distance);
                    PlayerPrefs.SetFloat("formula_weight_quantityToPickup", bestWeights.quantityToPickup);
                    PlayerPrefs.SetFloat("formula_weight_filled", bestWeights.filled);
                }

                Debug.Log("Iteration elapsed time in seconds: " + iteration_elapsed_time);

                // Start a new training iteration.
                startTrainingIteration();
            }
        }
    }
}
