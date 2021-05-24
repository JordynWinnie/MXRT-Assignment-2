using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;

public class AnchorCreator : MonoBehaviour
{
    [SerializeField] private GameObject m_AnchorObj;
    List<ARAnchor> m_Anchors = new List<ARAnchor>();
    
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    [SerializeField] ARRaycastManager m_RaycastManager;

    [SerializeField] ARAnchorManager m_AnchorManager;

    [SerializeField] private ARPlaneManager m_PlaneManager;

    [SerializeField] private bool isTrackablesActive = true;
    
    private const TrackableType trackableTypes =
        TrackableType.FeaturePoint | TrackableType.Planes;
    public void RemoveAllAnchors()
    {
        //Logger.Log($"Removing all anchors ({m_Anchors.Count})");
        foreach (var anchor in m_Anchors)
        {
            Destroy(anchor.gameObject);
        }
        m_Anchors.Clear();
    }

    void SetAnchorInfo(ARAnchor anchor, byte[] data)
    {
        anchor.GetComponent<ImageResultFromAPI>().currentByteArray = data;
        //anchor.GetComponentInChildren<TextMesh>().text = text;
    }
    
    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        if (m_PlaneManager.trackables.count > 0)
        {
            isTrackablesActive = false;
        }
        
        ARAnchor anchor = null;
        var byteArray = ScreenCapture.Instance.CaptureScreenshot();
        
        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane)
        {
            var planeManager = m_PlaneManager;
            if (planeManager)
            {
                var oldPrefab = m_AnchorManager.anchorPrefab;
                m_AnchorManager.anchorPrefab = m_AnchorObj;
                anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                m_AnchorManager.anchorPrefab = oldPrefab;
                SetAnchorInfo(anchor, byteArray);
                return anchor;
            }
        }

        // Otherwise, just create a regular anchor at the hit pose
        //Logger.Log("Creating regular anchor.");

        // Note: the anchor can be anywhere in the scene hierarchy
        var gameObject = Instantiate(m_AnchorObj, hit.pose.position, hit.pose.rotation);

        // Make sure the new GameObject has an ARAnchor component
        anchor = gameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = gameObject.AddComponent<ARAnchor>();
        }

        SetAnchorInfo(anchor, byteArray);
        
        return anchor;
    }

    private void Update()
    {

        if (m_PlaneManager.trackables.count > 0)
        {
            m_PlaneManager.SetTrackablesActive(isTrackablesActive);
        }

        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
        {
            return;
        }
        //Code Referenced From: 
        //https://www.youtube.com/watch?v=NdrvihZhVqs
        var touchPos = touch.position;

        bool isOverUI = touchPos.IsPointOverUIObject();

        if (isOverUI)
        {
            return;
        }
        
        if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes))
        {
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            var hit = s_Hits[0];
            
            // Create a new anchor
            var anchor = CreateAnchor(hit);
            if (anchor)
            {
                // Remember the anchor so we can remove it later.
                m_Anchors.Add(anchor);
            }
        }
    }
}
