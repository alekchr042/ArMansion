using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceOnPlane : MonoBehaviour
{
    private ARSessionOrigin sessionOrigin;

    private ARRaycastManager raycastManager;

    private ARCameraBackground cameraBackground;

    private PatternDetector patternDetector;

    public GameObject PrefabToPlace;

    List<ARRaycastHit> hits;

    // Start is called before the first frame update
    void Start()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();

        raycastManager = GetComponent<ARRaycastManager>();

        cameraBackground = GetComponent<ARCameraBackground>();

        patternDetector = GetComponent<PatternDetector>();

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
                //if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                //{
                //    var pose = hits[0].pose;

                //    Instantiate(PrefabToPlace, pose.position, pose.rotation);
                //}
                var cameraTexture = CaptureCameraTexture();

                patternDetector.SearchForPattern(cameraTexture);
            }
        }
    }

    //private byte[] GetCameraImage()
    //{
    //    CameraImage image;
    //    if (!cameraManager.TryGetLatestImage(out image))
    //        return;

    //    // Consider each image plane
    //    for (int planeIndex = 0; planeIndex < image.planeCount; ++planeIndex)
    //    {
    //        // Log information about the image plane
    //        CameraImagePlane plane = image.GetPlane(planeIndex);
    //        Debug.LogFormat("Plane {0}:\n\tsize: {1}\n\trowStride: {2}\n\tpixelStride: {3}",
    //            planeIndex, plane.data.Length, plane.rowStride, plane.pixelStride);

    //        // Do something with the data:
    //        MyComputerVisionAlgorithm(plane.data);
    //    }

    //    // You must dispose the CameraImage to avoid resource leaks.
    //    image.Dispose();
    //}

    private Texture2D CaptureCameraTexture()
    {
        //var activeRenderTexture = RenderTexture.active;

        //RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

        var cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);

        //Graphics.Blit(null, renderTexture, cameraBackground.material);

        //RenderTexture.active = renderTexture;

        cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        //cameraTexture.Resize(Screen.width / 3, Screen.height / 3);

        //cameraTexture.Apply();

        //RenderTexture.active = activeRenderTexture;

        return cameraTexture;
    }
}
