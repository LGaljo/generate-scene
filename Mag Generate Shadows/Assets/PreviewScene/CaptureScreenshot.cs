using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
{
    private string savePath;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            string shortHash = CalculateShortHash();
            this.CaptureAndSave(shortHash);
        }
    }

    void CaptureAndSave(string shortHash)
    {
        string folderPath = Application.persistentDataPath;

        GameObject terrain = GameObject.Find("Terrain");

        GameObject gameObject = GameObject.Find("OrthoCamera");
        // Add a Camera component to the GameObject
        Camera camera = gameObject.GetComponent<Camera>();

        float x = camera.transform.position.x;
        float z = camera.transform.position.z;
        string savePath = System.IO.Path.Combine(folderPath, $"x{x}-z{z}-{shortHash}.png");

        // Create a RenderTexture to temporarily store the camera's view
        RenderTexture renderTexture = new(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;

        // Render the camera's view to the RenderTexture
        camera.Render();

        // Create a new Texture2D and read the pixels from the RenderTexture
        Texture2D screenshot = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Reset the active RenderTexture and release the temporary RenderTexture
        RenderTexture.active = null;
        camera.targetTexture = null;
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
