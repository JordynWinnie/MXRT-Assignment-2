using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Vector2Extensions;

/// <summary>
/// This Script is responsible for creating all the AR objects in the world
/// It detects two kinds of AR Trackables, the FeaturePoints and Planes,
/// This is so that an object can be placed virtually anywhere in space, which
/// will help as not all objects are facing a flat wall.
///
/// Some of this code references the AR Foundation Samples provided by Unity (refer to 2) 
///
/// (1) https://www.youtube.com/watch?v=NdrvihZhVqs
/// (2) https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scripts/AnchorCreator.cs 
/// </summary>
public class TranslationAnchorCreator : MonoBehaviour
{
    //Create a Constant of the two Tracking types we would lik to use:
    private const TrackableType trackableTypes =
        TrackableType.FeaturePoint | TrackableType.Planes;

    //Create a list which stores all the hits that the Raycast Manager returns:
    private readonly List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    //Stores a list of AR objects which represent all the current items marked in the world:
    private readonly List<ARAnchor> m_Anchors = new List<ARAnchor>();

    //Links to the Prefab that can translate real world objects:
    [SerializeField] private GameObject m_AnchorObj;

    //Create Private references that will all reference back to the Singleton
    //object created, and also allow these managers to be used throughout the code file
    private ARRaycastManager m_RaycastManager;
    private ARAnchorManager m_AnchorManager;
    private ARPlaneManager m_PlaneManager;

    //Sets if the trackables, like the Rendering of Plane Surfaces should be active:
    private bool isTrackablesActive = true;

    private void Start()
    {
        m_AnchorManager = TranslationManager.Instance.AnchorManager;
        m_RaycastManager = TranslationManager.Instance.RaycastManager;
        m_PlaneManager = TranslationManager.Instance.PlaneManager;
    }
    private void Update()
    {
        //Checks the PlaneManager for whether anything is tracked
        //If there is, show all the available points that the user can tap
        //However, this will disappear the moment the user places an anchor:
        if (m_PlaneManager.trackables.count > 0) m_PlaneManager.SetTrackablesActive(isTrackablesActive);

        //Check if there are any touches on the screen:
        if (Input.touchCount == 0) return;

        //Get the First Touch:
        var touch = Input.GetTouch(0);
        //For the user, the touchphase could be Moving or Failed,
        //In cases like this where a mistouch is registered, we would like to ignore it:
        if (touch.phase != TouchPhase.Began) return;

        //Code Referenced From (1),
        //Gets the current position of the touch, and checks if it over an object:
        //More information in Vector2Extensions.cs:
        var touchPos = touch.position;

        var isOverUI = touchPos.IsPointOverUIObject();

        if (isOverUI) return;
        

        //Uses the AR Raycast Manager to know where in the real world the touch position hit
        //Store it all in the list of ARHits
        if (m_RaycastManager.Raycast(touch.position, m_Hits, trackableTypes))
        {
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            var hit = m_Hits[0];

            // Create a new anchor
            var anchor = CreateAnchor(hit);
            if (anchor)
                // Remember the anchor so we can remove it later.
                m_Anchors.Add(anchor);
        }
    }

    //Helper method to remove Anchors by looping through all the anchors stored and Destroying them:
    //Then clearing the list to offload any empty references:
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_Anchors) Destroy(anchor.gameObject);
        m_Anchors.Clear();
    }

    //Helper method to Refresh translations, by looping through all the anchors,
    //Getting the ImageResult componenet then calling refresh:
    public void RefreshAllTranslations()
    {
        foreach (var anchor in m_Anchors) anchor.GetComponent<ImageResultFromAPI>().RefreshTranslations();
    }

    //Takes a reference anchor and sets the Appropritate Byte[] Image data
    //That allows the code to send a Request to the ImageAPI:
    private void SetAnchorInfo(ARAnchor anchor, byte[] data)
    {
        anchor.GetComponent<ImageResultFromAPI>().currentByteArray = data;
    }

    //This code References (2)
    private ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        //Check if the number of Planes is more than one, if an anchor is created,
        //The Plane Visualisation should be disabled to not hinder the user's
        //view of the world:
        if (m_PlaneManager.trackables.count > 0) isTrackablesActive = false;
        //Capture what the user is currently looking at:
        var imageByteArray = ScreenCapture.Instance.CaptureScreenshot();

        //Create a new ARAnchor Instance:
        ARAnchor anchor;
        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane)
        {
            //get a reference to our current Plane Manager:
            var planeManager = m_PlaneManager;
            if (planeManager)
            {
                //Set the Instance Anchor to be an Anchor that is attached to the plane
                //we detected, and have the same pos+rot as the area we tapped:
                //the default anchor prefab will instantiate with a Call to AttachAnchor:
                anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                //Set up the Anchor with the Appropriate image data:
                SetAnchorInfo(anchor, imageByteArray);
                //Return this attached anchor:
                return anchor;
            }
        }

        // Otherwise, just create a regular anchor at the hit pose
        
        var freeSpaceAnchor = Instantiate(m_AnchorObj, hit.pose.position, hit.pose.rotation);

        // Make sure the new GameObject has an ARAnchor component
        anchor = freeSpaceAnchor.GetComponent<ARAnchor>();
        if (anchor == null) anchor = freeSpaceAnchor.AddComponent<ARAnchor>();
        //Do the same set up
        SetAnchorInfo(anchor, imageByteArray);
        //Return the freespace anchor:
        return anchor;
    }
}