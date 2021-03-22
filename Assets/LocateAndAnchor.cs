using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class LocateAndAnchor : MonoBehaviour
{
    public static bool canDraw = false;
    private bool placed = false;
    public GameObject model;
    [SerializeField]
    private ARAnchorManager anchorManager;
    [SerializeField]
    private ARPlaneManager plane;
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private PatternDetector patternDetector;
    private ARAnchor anchor;
    [SerializeField]
    private GameObject ArrowPrefab;

    public Text planesDetected;
    // Start is called before the first frame update
    void Start()
    {
        planesDetected.text = "No planes detected. Can't anchor";
        ArrowPrefab.transform.parent = gameObject.transform;
    }


    // Update is called once per frame
    void Update()
    {
        if (plane.trackables.count > 0)
        {
            canDraw = true;
            //planesDetected.text = "Planes detected";
        }

        if (GPSService.Instance.IsInVisibleRadius() && GPSService.Instance.IsLookingAtCoords())
        {
            planesDetected.text = "In area range";
            // Planes detected and not placed already and Is in Range and is Looking at it
            if (canDraw && !placed && Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                var hits = new List<ARRaycastHit>();

                //if (touch.phase == TouchPhase.Began)
                {
                    if (anchor != null)
                    {
                        Debug.Log("Touched. Anchor removed");
                        anchorManager.RemoveAnchor(anchor);
                    }

                    if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                    {
                        patternDetector.SearchForObject();
                        var pose = hits[0].pose;
                        // Instantiate(model, pose.position, pose.rotation);

                        Debug.Log("Touch sensed in the anchor");

                        anchor = anchorManager.AddAnchor(new Pose(pose.position, Quaternion.identity));

                        Debug.Log("Anchor set on " + anchor.transform.position.ToString());
                        if (anchor == null)
                        {
                            Debug.Log("Unable to create anchor!!!");
                        }
                        else
                        {
                            placed = true;
                            //GameObject go = Instantiate(model, anchor.transform);
                            //planesDetected.text = go.transform.position.ToString();                        
                            patternDetector.placedObject.transform.transform.position = new Vector3(anchor.transform.position.x, anchor.transform.position.y, (float)GPSService.Instance.distance);
                            patternDetector.placedObject.transform.parent = anchor.transform;
                            Debug.Log("Object set on " + patternDetector.placedObject.transform.position.ToString());
                        }
                    }
                }
            }
        }
        else
        {
            planesDetected.text = "";
            if (!GPSService.Instance.IsInVisibleRadius())
            {
                planesDetected.text += " Out of area. ";
            }
            if (!GPSService.Instance.IsLookingAtCoords())
            {
                planesDetected.text += " Wrong direction";
            }
        }
    }
}
