using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float speed = 30f;
    public float sensitivity = 1f;

    void Update()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        transform.Translate(movement * speed * Time.deltaTime);

        // Camera rotation with mouse
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotate the player horizontally (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (pitch)
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float currentRotation = mainCamera.transform.rotation.eulerAngles.x;
            float newRotation = Mathf.Clamp(currentRotation - mouseY, 0f, 80f); // Adjust the clamp values as needed
            mainCamera.transform.rotation = Quaternion.Euler(newRotation, mainCamera.transform.rotation.eulerAngles.y, 0f);
        }

        // Custom action on 'U' key press
        if (Input.GetKeyDown(KeyCode.U))
        {
            GameObject gameObject = GameObject.Find("Button");
            ModifyShaders ms = gameObject.GetComponent<ModifyShaders>();
            ms.OnButtonClick();
        }

        // Custom action on 'U' key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject gameObject = GameObject.Find("Button TakeScreenshot");
            TakeScreenshot ts = gameObject.GetComponent<TakeScreenshot>();
            ts.OnButtonClick();
        }

    }
}
