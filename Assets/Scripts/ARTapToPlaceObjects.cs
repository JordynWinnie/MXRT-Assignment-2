using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObjects : MonoBehaviour
{
    
    public GameObject TranslationPrefab;
    
    private GameObject spawnedObj;
    private ARRaycastManager m_ARRaycastManager;
    private Vector2 touchPosition;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    // Start is called before the first frame update
    void Start()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPos(out Vector2 pos)
    {
        if (Input.touchCount > 0)
        {
            pos = Input.GetTouch(0).position;
            return true;
        }

        pos = default;
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPos(out var touchPosition)) return;

        if (m_ARRaycastManager.Raycast(touchPosition, hits))
        {
            
        }
    }
}
