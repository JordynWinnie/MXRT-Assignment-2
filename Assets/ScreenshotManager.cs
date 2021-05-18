using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//https://www.youtube.com/watch?v=lT-SRLKUe5k
public class ScreenshotManager : MonoBehaviour
{
    public static ScreenshotManager Instance;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private LayerMask defaultMask;
    private bool takeScreenshotOnNextFrame;
    private void Awake()
    {
        Instance = this;
    }

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            
            takeScreenshotOnNextFrame = false;
            var renderTexture = m_Camera.targetTexture;

            var renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

            var rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            
            renderResult.ReadPixels(rect, 0,0);

            var byteArray = renderResult.EncodeToJPG();
            
            System.IO.File.WriteAllBytes(Application.persistentDataPath + $"/Screenshot-{Random.Range(0, 1000)}.jpg", byteArray);
            
            RenderTexture.ReleaseTemporary(renderTexture);
            m_Camera.targetTexture = null;
            m_Camera.cullingMask = defaultMask.value;
        }
        
    }

    public void TakeScreenshot(int width, int height)
    {
        m_Camera.cullingMask = cullingMask.value;
        m_Camera.targetTexture = RenderTexture.GetTemporary(480, 720, 16);
        takeScreenshotOnNextFrame = true;
    }
}
