using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDisableShadows : MonoBehaviour
{
    // Reference to the light component
    private Light sunLight;

    // Start is called before the first frame update
    void Start()
    {
        // Assuming the script is attached to the GameObject with the light component
        sunLight = GetComponent<Light>();

        if (sunLight != null)
        {

            Debug.Log("Sun has currently " + sunLight.shadows);
            // Disable shadows
            //sunLight.shadows = LightShadows.None;

            // You can also adjust other shadow-related properties if needed
            // For example:
            // sunLight.shadowStrength = 0f; // Set shadow strength to 0 for no shadows
            // sunLight.shadowBias = 0f; // Set shadow bias to 0 for no bias
            // sunLight.shadowNormalBias = 0f; // Set shadow normal bias to 0 for no normal bias
        }
        else
        {
            Debug.LogWarning("Light component not found on the GameObject.");
        }
    }
}
