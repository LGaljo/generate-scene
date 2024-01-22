using UnityEngine;

public class SunDisableShadows : MonoBehaviour
{
    // Reference to the light component
    public Light sunLight;

    public void Update()
    {
        // Custom action on 'U' key press
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.OnButtonClick();
        }
    }

    // Start is called before the first frame update
    public void OnButtonClick()
    {
        if (sunLight != null)
        {
            //Debug.Log("Sun has currently " + sunLight.shadows);

            if (sunLight.shadows == LightShadows.None) {
                sunLight.shadows = LightShadows.Hard;
            }
            else
            {
                sunLight.shadows = LightShadows.None;
            }

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
