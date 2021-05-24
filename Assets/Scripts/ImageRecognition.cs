using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageRecognition : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private XRReferenceImageLibrary m_ImageLibrary;
    private ARTrackedImageManager mTrackedManager;

    private Guid s_FirstImg;
    private Guid s_SecondImg;

    private GameObject firstPrefab;
    private GameObject secondPrefab;
    private void Awake()
    {
        mTrackedManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        s_FirstImg = m_ImageLibrary[1].guid;
        s_SecondImg = m_ImageLibrary[0].guid;
        mTrackedManager.trackedImagesChanged += MTrackedManagerOnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        mTrackedManager.trackedImagesChanged -= MTrackedManagerOnTrackedImagesChanged;
    }

    private void MTrackedManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs image)
    {
        foreach (var trackedImg in image.added)
        {
            if (trackedImg.referenceImage.guid == s_FirstImg)
            {
                firstPrefab = Instantiate(cubePrefab, trackedImg.transform.position, trackedImg.transform.rotation);
                firstPrefab.GetComponent<Renderer>().material.color = Color.red;
            }
            else if (trackedImg.referenceImage.guid == s_SecondImg)
            {
                secondPrefab = Instantiate(cubePrefab, trackedImg.transform.position, trackedImg.transform.rotation);
                secondPrefab.GetComponent<Renderer>().material.color = Color.blue;
            }
        }
        
        foreach(var trackedImg in image.updated)
        {
            // image is tracking or tracking with limited state, show visuals and update it's position and rotation
            if (trackedImg.trackingState == TrackingState.Tracking)
            {
                if (trackedImg.referenceImage.guid == s_FirstImg)
                {
                    firstPrefab.SetActive(true);
                    firstPrefab.transform.SetPositionAndRotation(trackedImg.transform.position, trackedImg.transform.rotation);
                }
                else if (trackedImg.referenceImage.guid == s_SecondImg)
                {
                    secondPrefab.SetActive(true);
                    secondPrefab.transform.SetPositionAndRotation(trackedImg.transform.position, trackedImg.transform.rotation);
                }
            }
            // image is no longer tracking, disable visuals TrackingState.Limited TrackingState.None
            else
            {
                if (trackedImg.referenceImage.guid == s_FirstImg)
                {
                    firstPrefab.SetActive(false);
                }
                else if (trackedImg.referenceImage.guid == s_SecondImg)
                {
                    secondPrefab.SetActive(false);
                }
            }
        }
        
        foreach(var trackedImg in image.removed)
        {
            if (trackedImg.referenceImage.guid == s_FirstImg)
            {
                Destroy(firstPrefab);
            }
            else if (trackedImg.referenceImage.guid == s_SecondImg)
            {
                Destroy(secondPrefab);
            }
        }
        
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
