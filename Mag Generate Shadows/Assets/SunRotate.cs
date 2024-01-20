using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotate : MonoBehaviour
{
    public float rotationSpeed = 10f; // Adjust the speed of rotation as needed

    void Update()
    {
        // Rotate the directional light (sun) based on user input or time of day logic
        RotateSun();
    }

    void RotateSun()
    {
        // Get the current rotation of the sun
        Vector3 currentRotation = transform.eulerAngles;

        // Update the rotation based on user input, time of day logic, or any other desired factor
        float rotationAmount = rotationSpeed * Time.deltaTime;
        currentRotation.x += rotationAmount; // Adjust the axis (x, y, or z) based on your scene setup


        // Apply the new rotation to the sun
        transform.eulerAngles = currentRotation;
    }
}
