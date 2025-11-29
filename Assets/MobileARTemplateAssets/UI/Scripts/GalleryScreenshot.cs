using UnityEngine;
using System.Threading.Tasks;
// NOTE: We do NOT use 'using NativeGallery;' to avoid the CS0138 error.

public class GalleryScreenshot : MonoBehaviour
{
    private const NativeGallery.PermissionType WRITE_PERMISSION = NativeGallery.PermissionType.Write;
    private const NativeGallery.MediaType IMAGE_TYPE = NativeGallery.MediaType.Image;
    private const string ALBUM_NAME = "MyGameAlbum";
    private const string FILE_NAME_FORMAT = "Screenshot_{0}.png";

    /// <summary>
    /// Checks/requests permissions and initiates the screenshot capture and save process.
    /// </summary>
    public async void TakeScreenshotAndSave()
    {
        // Check the current permission status (v1.9.0+ returns bool)
        // If it returns true, permission is granted.
        bool isPermissionGranted = NativeGallery.CheckPermission(WRITE_PERMISSION, IMAGE_TYPE);
        
        NativeGallery.Permission permissionResult = NativeGallery.Permission.Denied;

        if (isPermissionGranted)
        {
            permissionResult = NativeGallery.Permission.Granted;
        }
        else
        {
            // Use the new asynchronous method RequestPermissionAsync
            permissionResult = await NativeGallery.RequestPermissionAsync(WRITE_PERMISSION, IMAGE_TYPE);
        }

        // Handle Permission Result
        if (permissionResult == NativeGallery.Permission.Granted)
        {
            Debug.Log("Permission granted. Capturing screenshot...");
            await CaptureAndSaveScreenshotAsync();
        }
        else
        {
            Debug.LogError($"Permission status: {permissionResult}. Cannot save screenshot to gallery.");
        }
    }

    /// <summary>
    /// Captures screen texture and saves it using Native Gallery.
    /// </summary>
    private async Task CaptureAndSaveScreenshotAsync()
    {
        // Wait until the end of the frame before capturing
        await Task.Yield();
        
        // Use the cleaner, built-in Unity method for screen capture
        Texture2D screenImage = ScreenCapture.CaptureScreenshotAsTexture();

        // Check if capture was successful
        if (screenImage == null)
        {
            Debug.LogError("Failed to capture screenshot texture.");
            return;
        }

        // 3. Save to Gallery using the plugin
        NativeGallery.SaveImageToGallery(
            screenImage, 
            ALBUM_NAME, 
            FILE_NAME_FORMAT, 
            (success, path) => 
            {
                Debug.Log($"Save result: {success}. Path: {path}");
                // Clean up the Texture2D after saving
                Destroy(screenImage);
            }
        );
    }
}