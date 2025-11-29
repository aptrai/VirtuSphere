using UnityEngine;
using System.IO; // Required for file operations (optional, but good practice for path info)
using System; // Required for DateTime

public class ScreenshotTaker : MonoBehaviour
{
    /// <summary>
    /// Captures a screenshot and saves it as a PNG file.
    /// </summary>
    public void TakeScreenshot()
    {
        // 1. Generate a unique filename using a timestamp
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = "Screenshot_" + timestamp + ".png";
        
        // 2. Use ScreenCapture.CaptureScreenshot to take the picture
        // On mobile (iOS/Android), the file is saved to Application.persistentDataPath
        // On other platforms, it's relative to the executable/project directory.
        ScreenCapture.CaptureScreenshot(filename);

        Debug.Log("Screenshot saved: " + filename);

        // Optional: If you want to log the full path on a mobile device for debugging:
        // string persistentPath = Application.persistentDataPath;
        // Debug.Log("Persistent Data Path: " + persistentPath);
    }

    // Optional: Use a public method for a 4x super-sized (higher resolution) screenshot
    public void TakeHighResScreenshot()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = "HighResScreenshot_" + timestamp + ".png";
        
        // superSize parameter: 4 will make the resolution 4x4 (16 times) larger.
        ScreenCapture.CaptureScreenshot(filename, 4); 
        
        Debug.Log("High-Resolution Screenshot saved: " + filename);
    }
}