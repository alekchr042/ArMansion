using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceOnPlane : MonoBehaviour
{
    private ARSessionOrigin sessionOrigin;

    private ARRaycastManager raycastManager;

    List<ARRaycastHit> hits;

    public GameObject PrefabToPlace;

    // Start is called before the first frame update
    void Start()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();

        raycastManager = GetComponent<ARRaycastManager>();

        hits = new List<ARRaycastHit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    var pose = hits[0].pose;

                    Instantiate(PrefabToPlace, pose.position, pose.rotation);
                }
            }
        }
    }
}
