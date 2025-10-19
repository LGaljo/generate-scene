using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class CaptureSystem_SmallScene : MonoBehaviour
{
    private Camera orthoCamera;
    public float orthographicSize = 50f;

    float cameraPosY = 300f;
    public float CameraMoveX = 10f;
    public float CameraMoveZ = 10f;

    private float centerX = 0;
    private float centerZ = 0;

    int idx = 100;
    public int loopLimit = 20;
    public int tileMultiplier = 10;

    private ArrayList pos;

    private GameObject sun;
    private SunDisableShadows sds;
    private Terrain terrain;
    private TerrainScript terrainScript;
    private PlaceObjects placeObjects;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("OrthoCamera");
        // Add a Camera component to the GameObject
        this.orthoCamera = gameObject.GetComponent<Camera>();

        this.sun = GameObject.Find("Sun");
        this.sds = this.sun.GetComponent<SunDisableShadows>();

        GameObject go = GameObject.Find("Terrain");
        this.terrain = go.GetComponent<Terrain>();
        this.terrainScript = this.terrain.GetComponent<TerrainScript>();

        // Set the camera to orthographic projection
        this.orthoCamera.orthographic = true;

        // Adjust the orthographic size (height of the view)
        this.orthoCamera.orthographicSize = this.orthographicSize; // Adjust this value based on your scene requirements

        this.placeObjects = GetComponent<PlaceObjects>();

        this.MoveCamera(new Vector3(this.centerX, this.cameraPosY, this.centerZ), 0f);

        this.CalculateTerrainCenter();
    }

    void CalculateTerrainCenter()
    {
        if (this.terrain != null)
        {
            TerrainData terrainData = this.terrain.terrainData;

            // Get the size of the terrain
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;

            // Calculate the center of the terrain
            //this.centerX = terrainWidth / 2f + UnityEngine.Random.Range(-25, 25);
            //this.centerZ = terrainLength / 2f + UnityEngine.Random.Range(-25, 25);
            this.centerX = terrainWidth / 2f;
            this.centerZ = terrainLength / 2f;
        }
        else
        {
            Debug.LogError("Terrain component not found!");
        }
    }

    void MoveCamera(Vector3 magnitude, float rotation)
    {
        Vector3 tmp = new(orthoCamera.transform.position.x + magnitude.x, orthoCamera.transform.position.y, orthoCamera.transform.position.z + magnitude.z);
        // Set the camera position and rotation as needed
        this.orthoCamera.transform.SetPositionAndRotation(tmp, Quaternion.Euler(90f, rotation, 0f));
        Debug.Log("Move to " + tmp);
        //this.orthoCamera.transform.SetPositionAndRotation(tmp, Quaternion.Euler(90f, 0f, 0f));
    }

    void MoveCameraAbsolute(Vector3 magnitude, float rotation)
    {
        Vector3 tmp = new(this.centerX + magnitude.x, orthoCamera.transform.position.y, this.centerZ + magnitude.z);
        // Set the camera position and rotation as needed
        this.orthoCamera.transform.SetPositionAndRotation(tmp, Quaternion.Euler(90f, rotation, 0f));
        Debug.Log("Move to " + tmp);
        //this.orthoCamera.transform.SetPositionAndRotation(tmp, Quaternion.Euler(90f, 0f, 0f));
    }

    void CaptureAndSave(string shortHash)
    {
        string folderPath = Application.persistentDataPath;

        string layerName = this.terrainScript.GetTerrainLayerName(0);

        Light sunLight = this.sun.GetComponent<Light>();
        string shadowType = sunLight.shadows.HumanName();

        //this.orthoCamera.orthographicSize = UnityEngine.Random.Range(40, 100);

        float x = orthoCamera.transform.position.x;
        float z = orthoCamera.transform.position.z;
        string savePath = System.IO.Path.Combine(folderPath, $"{layerName}_{shortHash}-x{x}-z{z}-{shadowType}.png");

        // Create a RenderTexture to temporarily store the camera's view
        RenderTexture renderTexture = new(256 * this.tileMultiplier, 256 * this.tileMultiplier, 24);
        //RenderTexture renderTexture = new(Screen.width, Screen.height, 24);
        orthoCamera.targetTexture = renderTexture;

        // Render the camera's view to the RenderTexture
        orthoCamera.Render();

        // Create a new Texture2D and read the pixels from the RenderTexture
        //Texture2D screenshot = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //RenderTexture.active = renderTexture;
        //screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //screenshot.Apply();
        Texture2D screenshot = new(256 * this.tileMultiplier, 256 * this.tileMultiplier, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, 256 * this.tileMultiplier, 256 * this.tileMultiplier), 0, 0);
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

    void MoveModifyAndCapture()
    {
        // 5 frames to render
        if (Time.frameCount % 5 == 0)
        {
            if (this.idx < this.loopLimit)
            {
                // Different arrangements of objects on map
                this.placeObjects.PlaceAssetsInCartesian("trees", true);
                this.placeObjects.PlaceAssetsInCartesian("houses", true);

                for (int i = 0; i < 4; i++)
                {
                    string shortHash = CalculateShortHash();
                    this.orthoCamera.orthographicSize = UnityEngine.Random.Range(this.orthographicSize - 20f, this.orthographicSize + 10f);
                    float offset = 30f;
                    if (i ==  0)
                    {
                        this.MoveCameraAbsolute(new Vector3(offset, 0, offset), UnityEngine.Random.Range(0f, 360f));
                    }
                    else if (i == 1)
                    {
                        this.MoveCameraAbsolute(new Vector3(offset, 0, -offset), UnityEngine.Random.Range(0f, 360f));
                    }
                    else if (i == 2)
                    {
                        this.MoveCameraAbsolute(new Vector3(-offset, 0f, offset), UnityEngine.Random.Range(0f, 360f));
                    }
                    else
                    {
                        this.MoveCameraAbsolute(new Vector3(-offset, 0, -offset), UnityEngine.Random.Range(0f, 360f));
                    }

                    // Different shadow locations
                    // Capture w/ and w/o shadows
                    this.sds.EnableShadows();
                    this.sds.RandomSunRotate();
                    this.CaptureAndSave(shortHash);
                    this.sds.DisableShadows();
                    this.CaptureAndSave(shortHash);
                }

                this.placeObjects.DestroyAllChildren("trees");
                this.placeObjects.DestroyAllChildren("houses");

                this.idx += 1;
            }
            else if (this.idx == this.loopLimit)
            {
                int tidx = this.terrainScript.ChangeTerrainMaterial();

                // Reached loop limit per terrain layer, change layer material
                if (tidx == this.terrainScript.textures.Length)
                {
                    this.idx += 1;
                    Debug.Log("Finished procedure");
                }
                else
                // Next terrain layer is available, repeat capture
                {
                    this.idx = 0;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.MoveModifyAndCapture();
        if (Input.GetKeyDown(KeyCode.U))
        {
            string shortHash = CalculateShortHash();
            this.CaptureAndSave(shortHash);
        }
        // This starts automatic capture
        if (Input.GetKeyDown(KeyCode.X))
        {
            this.idx = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.MoveCamera(new Vector3(this.CameraMoveX, 0f, 0f), 0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.MoveCamera(new Vector3(0f, 0f, -this.CameraMoveZ), 0f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.MoveCamera(new Vector3(-this.CameraMoveX, 0f, 0f), 0f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.MoveCamera(new Vector3(0f, 0f, this.CameraMoveZ), 0f);
        }
    }
}
