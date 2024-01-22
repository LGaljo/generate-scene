using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
    public void OnButtonClick()
    {
        string folderPath = Application.persistentDataPath;

        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        var screenshot = System.IO.Path.Combine(folderPath, "Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png");
        ScreenCapture.CaptureScreenshot(screenshot, 1);
        Debug.Log(screenshot);
    }

    public void Update()
    {
        // Custom action on 'U' key presspress
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject gameObject = GameObject.Find("Button TakeScreenshot");
            TakeScreenshot ts = gameObject.GetComponent<TakeScreenshot>();
            ts.OnButtonClick();
        }

    }
}
