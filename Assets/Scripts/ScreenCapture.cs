using UnityEngine;
/// <summary>
/// This script file captures a temporary image that is 480x720 to upload for the ImageRecognition API
/// to come up with a prediction of what that object is. Higher resolutions were experiemented with
/// but the performance would suffer significantly on Android with not much payout, so a lower 480p
/// resolution was choosen.
/// The Majority of the code was modified from CodeMonkey's and from a Forum Tutorial, refer to 1 and 2
///
/// References:
/// (1) How To Take Screenshot in Unity by CodeMonkey: https://www.youtube.com/watch?v=lT-SRLKUe5k
/// (2) Tutorials For AR: How to Capture Screenshots https://tutorialsforar.com/how-to-take-screenshots-within-an-app-using-unity/
/// (3) Unity Tutorial: Using the Camera Culling Mask to not Render Specific Layers https://www.youtube.com/watch?v=iYaOkbC58W4
/// </summary>
public class ScreenCapture : MonoBehaviour
{
    //Creates a static instance that can be accessed from any code file to take a screenshot
    public static ScreenCapture Instance;

    //Gets a reference to the MainCamera instead of using Camera.main as it is inefficient
    [SerializeField] private Camera m_Camera;
    //Sets two culling masks, the first culling mask ignores rendering the UI and the Plane Objects
    //Second one is the default mask that allows the player to see everything
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private LayerMask defaultMask;

    //These two are fields that can be changed if the resolution needs to be increased
    private readonly int captureHeight = 720;
    private readonly int captureWidth = 480;

    //These 3 elements will be used to construct a screenshot afterwards:
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    //A Simple Singleton Initialiser that destroys any other instances of this class:
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    public byte[] CaptureScreenshot()
    {
        //Checks if the Current RenderTexture is null, meaning the previous operation is complete
        if (renderTexture == null)
        {
            //Creates a Unity Rectangle, Render Texture and 2D Texture with our Image's resolution
            rect = new Rect(0, 0, captureWidth, captureHeight);
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            //Use the RGB24 to support the entire RGB specturm, without Alpha:
            screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }

        //Changes the Culling mask to prevent the camera from capturing the unncessary elements
        //like UI and plane prefabs. Refer to (3)
        m_Camera.cullingMask = cullingMask;

        //set the current target texture of the camera to the new RenderTexture, so that the
        //camera's view can be rendered into the RenderTexture:
        m_Camera.targetTexture = renderTexture;
        m_Camera.Render();

        //Set the Active Render texture to whatever was rendered by the Camera:
        RenderTexture.active = renderTexture;

        //Read the Pixels into a Texture2D with the resolution specified by a Rect we generated above:
        screenShot.ReadPixels(rect, 0, 0);

        //Reset the textures and remove the render texture from the Camera since were done reading the screen data
        m_Camera.targetTexture = null;

        //Reset the Culling mask as we would like the user to be able to see all UI elements:
        m_Camera.cullingMask = defaultMask;

        //Reset the Active render texture for the next screenshotL
        RenderTexture.active = null;

        //Encode the Texture2D into a JPG, this is the most resource intensive part of the process:
        var fileData = screenShot.EncodeToJPG();
        //Once everything is complete, destroy the renderTexture to release the memory        
        Destroy(renderTexture);
        renderTexture = null;
        screenShot = null;

        //Returns the JPG screenshot encoded as a byte Array:
        return fileData;
    }
}