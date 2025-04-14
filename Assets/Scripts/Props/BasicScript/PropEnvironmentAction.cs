using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEnvironmentAction : MonoBehaviour
{
    // Public parameters for customization
    [Header("Setting")]
    [SerializeField] private float triggerInterval = 5f; // Fixed time interval between triggers
    [SerializeField] private float minTriggerInterval = 2f; // Minimum allowed trigger interval
    [SerializeField] private float maxTriggerInterval = 10f; // Maximum allowed trigger interval
    [SerializeField] private GameObject[] effects; // Array to store effects
    [SerializeField] private float triggerProbability = 0.5f; // Probability of triggering an effect
    [SerializeField] private float effectRadius = 10f; // Radius of effect generation
    [SerializeField] private float verticalAngle = 45f; // Angle from vertical direction for effects

    private float timer = 0f; // Timer to track time since last trigger
    [SerializeField] private float currentInterval; // Variable to store the dynamic interval

    void Start()
    {
        // Initialize the current interval to the fixed trigger interval
        currentInterval = triggerInterval;
    }

    void Update()
    {
        // Increment timer
        timer += Time.deltaTime;

        // Check if it's time to trigger
        if (timer >= currentInterval)
        {
            // Randomly decide whether to trigger based on probability
            if (Random.value <= triggerProbability)
            {
                TriggerEffect();
            }

            // Reset timer with random adjustment
            timer = 0f;
            currentInterval = Random.Range(minTriggerInterval, maxTriggerInterval); // Generate a new random interval within bounds
        }
    }

    void TriggerEffect()
    {
        // Generate a random position within the defined radius
        Vector3 randomPosition = transform.position +
            new Vector3(Random.Range(-effectRadius, effectRadius), 0, Random.Range(-effectRadius, effectRadius));

        // Check if the position is on the ground
        if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit))
        {
            randomPosition = hit.point;

            // Choose a random effect from the array
            if (effects.Length > 0)
            {
                GameObject randomEffect = effects[Random.Range(0, effects.Length)];

                // Generate rotation with vertical angle adjustment
                Quaternion adjustedRotation = Quaternion.Euler(verticalAngle, Random.Range(0f, 360f), 0);

                // Instantiate the effect at the position with adjusted rotation
                GameObject instantiatedEffect = Instantiate(randomEffect, randomPosition, adjustedRotation);

                // Destroy the effect after the specified lifetime
                Destroy(instantiatedEffect, triggerInterval);
            }
        }
    }
}
