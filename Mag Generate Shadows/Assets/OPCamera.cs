using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
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

    int idx = 100;
    public int loopLimit = 20;

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
        this.MoveModifyAndCapture();
        if (Input.GetKeyDown(KeyCode.U))
        {
              this.CaptureAndSave("");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            this.idx = 0;
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

    void MoveModifyAndCapture()
    {
        if (this.idx < this.loopLimit)
        {
            GameObject sun = GameObject.Find("Sun");
            SunDisableShadows sds = sun.GetComponent<SunDisableShadows>();

            PlaceTree placeTrees = GetComponent<PlaceTree>();
            placeTrees.PlaceAssetsInPolar();

            string shortHash = CalculateShortHash();

            sds.EnableShadows();
            orthoCamera.transform.position = new(500f, 100f, 425f);
            this.CaptureAndSave(shortHash);
            orthoCamera.transform.position = new(500f, 100f, 575f);
            this.CaptureAndSave(shortHash);
            sds.DisableShadows();
            orthoCamera.transform.position = new(500f, 100f, 425f);
            this.CaptureAndSave(shortHash);
            orthoCamera.transform.position = new(500f, 100f, 575f);
            this.CaptureAndSave(shortHash);

            placeTrees.DestroyAllChildren();
            this.idx += 1;
        } else if (this.idx == this.loopLimit)
        {
            this.idx += 1;
            Debug.Log("Finished procedure");
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

            // Get the size of the terrain
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;

            // Calculate the center of the terrain
            this.centerX = terrainWidth / 2f;
            this.centerZ = terrainLength / 2f;
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


    void CaptureAndSave(string shortHash)
    {
        string folderPath = Application.persistentDataPath;

        GameObject terrain = GameObject.Find("Terrain");
        TerrainScript ts = terrain.GetComponent<TerrainScript>();
        string layerName = ts.GetTerrainLayerName(0);

        GameObject sun = GameObject.Find("Sun");
        Light sunLight = sun.GetComponent<Light>();
        string shadowType = sunLight.shadows.HumanName();

        float x = orthoCamera.transform.position.x;
        float z = orthoCamera.transform.position.z;
        string savePath = System.IO.Path.Combine(folderPath, $"{layerName}_x{x}-z{z}-{shadowType}-{shortHash}.png");

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
        Destroy(screenshot);

        Debug.Log("Screenshot saved to: " + savePath);
    }

    static string CalculateShortHash()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new System.Random();
        string randomString = new(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(randomString);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Take the first 8 bytes of the hash to create a short hash
            byte[] shortHashBytes = new byte[8];
            Array.Copy(hashBytes, shortHashBytes, 8);

            // Convert the bytes to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in shortHashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

}
