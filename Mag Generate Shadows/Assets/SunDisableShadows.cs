using UnityEngine;

public class SunDisableShadows : MonoBehaviour
{
    // Reference to the light component
    public Light sunLight;
    float nonShadowIntensity = 78000f;

    public void Update()
    {
        // Custom action on 'U' key press
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.ToggleShadows();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            this.nonShadowIntensity += 1000;
            sunLight.intensity = nonShadowIntensity;
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            this.nonShadowIntensity -= 1000;
            sunLight.intensity = nonShadowIntensity;
        }
    }

    // Start is called before the first frame update
    public void ToggleShadows()
    {
            //Debug.Log("Sun has currently " + sunLight.shadows);
            if (sunLight.shadows == LightShadows.None) {
                this.EnableShadows();
            }
            else
            {
                this.DisableShadows();
            }
    }

    public void EnableShadows()
    {
        if (sunLight != null)
        {
            sunLight.shadows = LightShadows.Hard;
            sunLight.transform.rotation = Quaternion.Euler(33f, 45f, 0f);
            sunLight.intensity = 130000f;
        }
        else
        {
            Debug.LogWarning("Light component not found on the GameObject.");
        }
    }

    public void DisableShadows()
    {
        if (sunLight != null)
        {
            sunLight.shadows = LightShadows.None;
            sunLight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            sunLight.intensity = this.nonShadowIntensity;
        }
        else
        {
            Debug.LogWarning("Light component not found on the GameObject.");
        }
    }
}
