using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
    public static ScreenCapture Instance;
    
    [SerializeField] private Camera m_Camera;
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private LayerMask defaultMask;
    private int captureWidth = 480;
    private int captureHeight = 720;
    
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    private string format = "png";
    private void Awake()
    {
        Instance = this;
    }

    private string CreateFileName(int width, int height)
    {
        //timestamp to append to the screenshot filename
        string timestamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
        // use width, height, and timestamp for unique file 
        var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", Application.persistentDataPath, width, height, timestamp, format.ToLower());
        // return filename
        return filename;
    }

    public byte[] CaptureScreenshot()
    {
        if (renderTexture == null)
        {
            // creates off-screen render texture to be rendered into
            rect = new Rect(0, 0, captureWidth, captureHeight);
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }

        m_Camera.cullingMask = cullingMask;
        m_Camera.targetTexture = renderTexture;
        m_Camera.Render();
        
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        
        // reset the textures and remove the render texture from the Camera since were done reading the screen data
        m_Camera.targetTexture = null;

        m_Camera.cullingMask = defaultMask;
        
        RenderTexture.active = null;
        
        string filename = CreateFileName((int)rect.width, (int)rect.height);
        
        byte[] fileData = screenShot.EncodeToJPG();

        /*
        new System.Threading.Thread(() =>
        {
            var file = System.IO.File.Create(filename);
            file.Write(fileData, 0, fileData.Length);
            file.Close();
            Debug.Log(string.Format("Screenshot Saved {0}, size {1}", filename, fileData.Length));
        }).Start();
        */

        Destroy(renderTexture);
        renderTexture = null;
        screenShot = null;
        
        return fileData;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
