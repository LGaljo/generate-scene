using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.InputManagerEntry;

public class SunDisableShadows : MonoBehaviour
{
    // Reference to the light component
    Light sunLight;
    public float maxSunIntensity = 90000f;
    Vector3 prevPosition = Vector3.up;

    public Volume volume;
    HDRISky hdriSky;
    public float initialExposureComp = 12f;

    public void Update()
    {
        // Custom action on 'Z' key press
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.ToggleShadows();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.RandomSunRotate();
        }
    }

    public void Start()
    {
        sunLight = GetComponent<Light>();
        Debug.Log("start intensity " + this.sunLight.intensity + " sun at " + sunLight.transform.eulerAngles);
        float emissivity = Mathf.Cos((90f - sunLight.transform.eulerAngles.x) * Mathf.Deg2Rad);
        this.sunLight.intensity = this.maxSunIntensity * (2 - emissivity);
        Debug.Log("start intensity " + this.sunLight.intensity + " sun at " + sunLight.transform.eulerAngles);

        Debug.Log(volume);
        if (volume != null)
        {
            volume.profile.TryGet<HDRISky>(out this.hdriSky);
            if (this.hdriSky != null)
            {
                Debug.Log(this.hdriSky);
                this.hdriSky.exposure.value = initialExposureComp;
            }
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
            //sunLight.transform.eulerAngles = this.prevPosition;
            //float emissivity = Mathf.Cos((90f - sunLight.transform.eulerAngles.x) * Mathf.Deg2Rad);
            //this.sunLight.intensity = this.maxSunIntensity * (2 - emissivity);

            Debug.Log("Enable shadows");
            //Debug.Log("current intensity " + this.sunLight.intensity + " sun at " + sunLight.transform.eulerAngles);
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
            //float emissivity = Mathf.Cos((90f - sunLight.transform.eulerAngles.x) * Mathf.Deg2Rad);
            //this.sunLight.intensity = this.maxSunIntensity * (2 - emissivity);

            //this.prevPosition = sunLight.transform.eulerAngles;

            sunLight.shadows = LightShadows.None;
            //sunLight.transform.eulerAngles = new Vector3(90f, 0f, 0f);

            //float emissivity = Mathf.Cos((90f - sunLight.transform.eulerAngles.x) * Mathf.Deg2Rad);
            //this.sunLight.intensity = this.maxSunIntensity * (2 - emissivity);

            Debug.Log("Disable shadows");
            //Debug.Log("current intensity " + this.sunLight.intensity + " sun at " + sunLight.transform.eulerAngles);
        }
        else
        {
            Debug.LogWarning("Light component not found on the GameObject.");
        }
    }

    public void RandomSunRotate()
    {
        // Generate a random angle within the specified range
        float randomElevation = Random.Range(20f, 75f);
        float randomAsimuth = Random.Range(0f, 360f);

        // Convert the angle to a rotation
        Vector3 sunRotation = new(randomElevation, randomAsimuth, 0);

        // Apply the rotation to the directional light
        this.sunLight.transform.eulerAngles = sunRotation;

        //Debug.Log("Set sun position to " + sunRotation.x);

        float emissivity = Mathf.Cos((90f - randomElevation) * Mathf.Deg2Rad);
        //Debug.Log("Emissivity proportionality " + emissivity);

        this.sunLight.intensity = this.maxSunIntensity * (2 - emissivity);
        //Debug.Log("current intensity " + this.sunLight.intensity + " sun at " + sunLight.transform.eulerAngles);

        HDAdditionalLightData hdLightData = GetComponent<HDAdditionalLightData>();
        if (hdLightData != null)
        {
            hdLightData.angularDiameter = Random.Range(0.5f, 3f);
            //Debug.Log("current angular dimension " + hdLightData.angularDiameter);
        }

        this.hdriSky.exposure.value = Random.Range(11f, 14f);
    }
}
