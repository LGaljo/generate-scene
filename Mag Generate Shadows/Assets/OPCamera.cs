using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPCamera : MonoBehaviour
{
    public string savePath;
    public Camera orthoCamera;
    public float orthographicSize = 50f;

    public float CameraMoveX = 10f;
    public float CameraMoveZ = 10f;

    public float centerX = 0;
    public float centerZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("OrthoCamera");
        // Add a Camera component to the GameObject
        orthoCamera = gameObject.GetComponent<Camera>();

        // Set the camera to orthographic projection
        orthoCamera.orthographic = true;

        // Adjust the orthographic size (height of the view)
        orthoCamera.orthographicSize = this.orthographicSize; // Adjust this value based on your scene requirements

        this.CalculateTerrainCenter();

        // Set the camera position and rotation as needed
        orthoCamera.transform.SetPositionAndRotation(new Vector3(this.centerX, 100f, this.centerZ), Quaternion.Euler(90f, 0f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            this.CaptureAndSave();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.MoveCamera(new Vector3(this.CameraMoveX, 0f, 0f));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.MoveCamera(new Vector3(0f, 0f, -this.CameraMoveZ));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.MoveCamera(new Vector3(-this.CameraMoveX, 0f, 0f));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.MoveCamera(new Vector3(0f, 0f, this.CameraMoveZ));
        }
    }

    void CalculateTerrainCenter()
    {
        // Assuming the script is attached to the GameObject with the Terrain component
        GameObject go = GameObject.Find("Terrain");
        Terrain terrain = go.GetComponent<Terrain>();

        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;

            Debug.Log(terrainData);
            Debug.Log(terrainData.size);

            // Get the size of the terrain
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;

            // Calculate the center of the terrain
            this.centerX = terrainWidth / 2f;
            this.centerZ = terrainLength / 2f;

            // Log the center coordinates (you can use these values as needed)
            Debug.Log("Terrain Center: (" + this.centerX + ", " + this.centerZ + ")");
        }
        else
        {
            Debug.LogError("Terrain component not found!");
        }
    }

    void MoveCamera(Vector3 magnitude)
    {
        Vector3 tmp = new(orthoCamera.transform.position.x + magnitude.x, orthoCamera.transform.position.y, orthoCamera.transform.position.z + magnitude.z);
        // Set the camera position and rotation as needed
        orthoCamera.transform.SetPositionAndRotation(tmp, Quaternion.Euler(90f, 0f, 0f));
    }

    void MoveCamera()
    {
        Vector3 tmp = new(orthoCamera.transform.position.x + CameraMoveX, orthoCamera.transform.position.y, orthoCamera.transform.position.z + CameraMoveZ);
        // Set the camera position and rotation as needed
        orthoCamera.transform.SetPositionAndRotation(tmp, Quaternion.Euler(90f, 0f, 0f));
    }

    void CaptureAndSave()
    {
        string folderPath = Application.persistentDataPath;
        string savePath = System.IO.Path.Combine(folderPath, "Screenshot_" + "x-" + orthoCamera.transform.position.x + "-z-" + orthoCamera.transform.position.z + ".png");

        // Create a RenderTexture to temporarily store the camera's view
        RenderTexture renderTexture = new(Screen.width, Screen.height, 24);
        orthoCamera.targetTexture = renderTexture;

        // Render the camera's view to the RenderTexture
        orthoCamera.Render();

        // Create a new Texture2D and read the pixels from the RenderTexture
        Texture2D screenshot = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Reset the active RenderTexture and release the temporary RenderTexture
        RenderTexture.active = null;
        orthoCamera.targetTexture = null;
        Destroy(renderTexture);

        // Convert the Texture2D to a byte array and save it to a PNG file
        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);

        Debug.Log("Screenshot saved to: " + savePath);
    }

}
